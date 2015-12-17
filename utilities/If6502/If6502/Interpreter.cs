using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace If6502
{

    class Interpreter
    {
        private int iLine = 0;
        private List<BracketPair> m_Brackets;
        private List<ControlLine> m_ControlLines;

        public bool Success = false;

        string msg_error_failure = "Failure ", msg_error_atline = " at line {0}.";

        public string Interpret(List<StringTokenized> lines)
        {
            Success = false;
            List<PassWithError> passes = new List<PassWithError>();
            passes.Add(new PassWithError(passMatchBrackets, msg_error_failure + "matching Brackets" + msg_error_atline));
            passes.Add(new PassWithError(passGetControlFlow, msg_error_failure + "interpreting ControlFlow" + msg_error_atline));
            passes.Add(new PassWithError(passControlFlowToBrackets, msg_error_failure + "combining ControlFlow and Brackets" + msg_error_atline));
            passes.Add(new PassWithError(passMatchControlFlow, msg_error_failure + "matching ControlFlow" + msg_error_atline));
            passes.Add(new PassWithError(passCompileControlFlow, msg_error_failure + "compiling ControlFlow" + msg_error_atline));

            Success = true;

            foreach (PassWithError pass in passes)
                if (!pass.Pass.Invoke(lines))
                    return pass.ErrorMessage(iLine);
            return "Success!";
        }

        private bool passCompileControlFlow(List<StringTokenized> lines)
        {
            foreach (ControlLine cl in m_ControlLines)
            {
                if (cl.Line.Tokens[0].Value == ".else")
                {
                    lines[cl.LineIndex].CompiledString = "; .else";
                    string compiled = "_L" + cl.LineIndex + ":" + "; } ";
                    lines[cl.BracketPair.LineEnd].CompiledString = compiled;
                }
                else if ((cl.Line.Tokens[0].Value == ".if") || (cl.Line.Tokens[0].Value == ".elseif"))
                {
                    string assembled = assembleControlLine(cl);
                    if (assembled == null)
                        return false;
                    lines[cl.LineIndex].CompiledString = assembled;
                    assembled = string.Empty;
                    if (cl.HasNextControlLine)
                    {
                        ControlLine cl2 = cl.NextControlLine;
                        while (cl2.HasNextControlLine)
                            cl2 = cl2.NextControlLine;
                        assembled = "jmp _L" + cl2.LineIndex + "; } " + "\n";
                        assembled += "_L" + cl.LineIndex + ":";
                    }
                    else
                    {
                        assembled = "_L" + cl.LineIndex + ":" + "; }";
                    }
                    lines[cl.BracketPair.LineEnd].CompiledString = assembled;
                }
                else if (cl.Line.Tokens[0].Value == ".while")
                {
                    string assembled = string.Format("_LW{0}:\n",cl.LineIndex) + assembleControlLine(cl);
                    if (assembled == null)
                        return false;
                    lines[cl.LineIndex].CompiledString = assembled;
                    lines[cl.BracketPair.LineEnd].CompiledString = string.Format("jmp _LW{0}\n_L{0}:", cl.LineIndex) + "; }";
                }
            }

            foreach (BracketPair bp in m_Brackets)
            {
                if (lines[bp.LineBegin].CompiledString == null)
                    lines[bp.LineBegin].CompiledString = "; {";
                if (lines[bp.LineEnd].CompiledString == null)
                    lines[bp.LineEnd].CompiledString = "; }";
            }
            return true;
        }

        private string assembleControlLine(ControlLine cl)
        {
            string assembled = string.Empty;
            if (cl.Line.Tokens[1].TokenType == TokenType.Register)
            {
                switch (cl.Line.Tokens[1].Value.Trim())
                {
                    case "a":
                        assembled += "cmp " + cl.Line.Tokens[3].Value + "; " + cl.Line.Line.Replace("\t", string.Empty) + "\n";
                        break;
                    case "x":
                        assembled += "cpx " + cl.Line.Tokens[3].Value + " ; " + cl.Line.Line.Replace("\t", string.Empty) + "\n";
                        break;
                    case "y":
                        assembled += "cpy " + cl.Line.Tokens[3].Value + "; " + cl.Line.Line.Replace("\t", string.Empty) + "\n";
                        break;
                    default:
                        iLine = cl.LineIndex;
                        return null;
                }
            }
            else if (cl.Line.Tokens[1].TokenType == TokenType.Variable)
            {
                assembled += "lda " + cl.Line.Tokens[1].Value + "; " + cl.Line.Line.Replace("\t", string.Empty) + "\ncmp " + cl.Line.Tokens[3].Value + "\n";
            }
            switch (cl.Line.Tokens[2].Value.Trim())
            {
                case "<":
                    assembled += "bcs _L" + cl.LineIndex;
                    break;
                case ">":
                    assembled += "bcc _L" + cl.LineIndex + "\n";
                    assembled += "beq _L" + cl.LineIndex;
                    break;
                case "<=":
                    assembled += "bcs _L" + cl.LineIndex + "\n";
                    assembled += "bne _L" + cl.LineIndex;
                    break;
                case ">=":
                    assembled += "bcc _L" + cl.LineIndex;
                    break;
                case "==":
                    assembled += "bne _L" + cl.LineIndex;
                    break;
                default:
                    iLine = cl.LineIndex;
                    return null;
            }
            return assembled;
        }

        private bool passMatchControlFlow(List<StringTokenized> lines)
        {
            foreach (ControlLine cl in m_ControlLines)
            {
                if (cl.Line.Tokens[0].Value == ".else")
                {
                    if (!cl.HasLastControlLine)
                    {
                        iLine = cl.LineIndex;
                        return false;
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (cl.Line.Tokens[0].Value == ".elseif")
                {
                    if (!cl.HasLastControlLine)
                    {
                        iLine = cl.LineIndex;
                        return false;
                    }
                }

                foreach (ControlLine cl2 in m_ControlLines)
                {
                    if ((cl2.LineIndex > cl.BracketPair.LineEnd) && (!cl2.HasLastControlLine))
                    {
                        if ((cl2.Line.Tokens[0].Value == ".else") || (cl2.Line.Tokens[0].Value == ".elseif"))
                        {
                            cl2.LastControlLine = cl;
                            cl.NextControlLine = cl2;
                        }
                        break;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Requires that all control lines (if, elseif, etc) have brackets.
        /// Allow brackets unmatched to control lines; replace these with .scope and .scend.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private bool passControlFlowToBrackets(List<StringTokenized> lines)
        {
            foreach (ControlLine cl in m_ControlLines)
            {
                foreach (BracketPair bp in m_Brackets)
                {
                    if (!bp.Matched && bp.LineBegin == cl.LineIndex + 1)
                    {
                        bp.Matched = true;
                        cl.BracketPair = bp;
                    }
                }
            }

            foreach (ControlLine cl in m_ControlLines)
                if (!cl.HasBracketPair)
                {
                    iLine = cl.LineIndex;
                    return false;
                }

            foreach (BracketPair bp in m_Brackets)
            {
                if (!bp.Matched)
                {
                    lines[bp.LineBegin].CompiledString = ".scope";
                    lines[bp.LineEnd].CompiledString = ".scend";
                    // iLine = bp.LineBegin;
                    // return false;
                }
            }

            return true;
        }

        private bool passGetControlFlow(List<StringTokenized> lines)
        {
            m_ControlLines = new List<ControlLine>();
            for (iLine = 0; iLine < lines.Count; iLine++)
            {
                StringTokenized line = lines[iLine];
                if (line.Tokens.Count > 0 && line.Tokens[0].TokenType == TokenType.ControlFlow)
                {
                    if (line.Tokens.Count == 1 && line.Tokens[0].Value == ".else")
                    {
                        // automatically included
                    }
                    else
                    {
                        if (line.Tokens.Count != 4)
                            return false;
                        if ((line.Tokens[1].TokenType != TokenType.Register) &&
                            (line.Tokens[1].TokenType != TokenType.Variable))
                            return false;
                        if (line.Tokens[2].TokenType != TokenType.Operator)
                            return false;
                        if ((line.Tokens[3].TokenType != TokenType.Variable) &&
                            (line.Tokens[3].TokenType != TokenType.Indexed))
                            return false;
                    }
                    m_ControlLines.Add(new ControlLine(iLine, line));
                }
            }
            return true;
        }

        private bool passMatchBrackets(List<StringTokenized> lines)
        {
            m_Brackets = new List<BracketPair>();
            for (iLine = 0; iLine < lines.Count; iLine++)
            {
                bool line_has_brackets = false;
                StringTokenized line = lines[iLine];
                for (int iToken = 0; iToken < line.Tokens.Count; iToken++)
                {
                    Token token = line.Tokens[iToken];
                    if (token.TokenType == TokenType.Bracket)
                    {
                        if (line_has_brackets)
                            return false;
                        line_has_brackets = true;
                        if (token.Value == "{")
                            m_Brackets.Add(new BracketPair(iLine, -1));
                        else if (token.Value == "}")
                        {
                            bool found_match = false;
                            for (int j = m_Brackets.Count - 1; j >= 0; j--)
                            {
                                if (m_Brackets[j].LineEnd == -1)
                                {
                                    m_Brackets[j].Index = Interpreter.NextIndex;
                                    m_Brackets[j].LineEnd = iLine;
                                    found_match = true;
                                    break;
                                }
                            }
                            if (!found_match)
                                return false;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        class BracketPair
        {
            public int Index = -1;
            public int LineBegin, LineEnd;
            public bool Matched = false;
            public BracketPair(int begin, int end)
            {
                LineBegin = begin;
                LineEnd = end;
            }

            public override string ToString()
            {
                return LineBegin + "-" + LineEnd;
            }
        }

        static int lastLabel = 0;
        public static int NextIndex
        {
            get { return lastLabel++; }
        }

        public static string GetLabelFromIndex(int index)
        {
            return string.Format("_L{0:D4}",index);
        }

        class ControlLine
        {
            public int LineIndex;
            public StringTokenized Line;
            public ControlLine LastControlLine = null;
            public ControlLine NextControlLine = null;
            public BracketPair BracketPair = null;
            public ControlLine(int index, StringTokenized line)
            {
                LineIndex = index;
                Line = line;
            }

            public bool HasLastControlLine
            {
                get { return (LastControlLine != null); }
            }

            public bool HasNextControlLine
            {
                get { return (NextControlLine != null); }
            }

            public bool HasBracketPair
            {
                get { return (BracketPair != null); }
            }

            public override string ToString()
            {
                return LineIndex + ": " + Line;
            }
        }

        delegate bool PassFunction(List<StringTokenized> lines);
        struct PassWithError
        {
            private string _Error;
            public PassFunction Pass;
            public PassWithError(PassFunction pass, string error)
            {
                Pass = pass;
                _Error = error;
            }
            public string ErrorMessage(int line)
            {
                return string.Format(_Error, line);
            }
        }
    }
}
