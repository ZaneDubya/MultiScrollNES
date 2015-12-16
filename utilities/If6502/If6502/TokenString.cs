using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace If6502
{
    class StringTokenized
    {
        private string m_String;
        private List<Token> m_Tokens;

        public string CompiledString = null;
        public string Line { get { return m_String; } }

        public List<Token> Tokens
        {
            get
            {
                return m_Tokens;
            }
        }

        public StringTokenized(string s)
        {
            if (m_RegexPattern == null)
            {
                m_RegexPattern = new Regex(pattern, RegexOptions.Singleline);
            }
            m_String = s;
            parse(m_String);
        }

        private void parse(string s)
        {
            int comment = s.IndexOf(';');
            if (comment != -1)
                s = s.Substring(0, comment);
            MatchCollection matches = m_RegexPattern.Matches(s);
            m_Tokens = new List<Token>();
            foreach (Match match in matches)
            {
                int i = 0, x;
                foreach (Group group in match.Groups)
                {
                    // ignore capture index 0 and 1 (general and WhiteSpace)
                    string type = m_RegexPattern.GroupNameFromNumber(i);
                    bool typeisnumber = int.TryParse(type, out x);
                    if (group.Success && i > 1 && !typeisnumber)
                    {
                        string value = group.Value;
                        m_Tokens.Add(new Token(type, value));
                    }
                    i++;
                }
            }
        }

        public override string ToString()
        {
            return m_String;
        }
        // http://regexhero.net/tester/
        private static string pattern = @"(?=(?<operator>(\<(?!=)|\<=|\>(?!=)|\>=|==)))|(?=(?<indexedvalue>(\#\$|\#)[^\s,$]*\,(x|y)))|(?=(?<controlflow>(\.if|\.else\b|\.elseif|\.while)))|(?=(?<register>\b(a|[^\,]x|[^\,]y)\b))|(?=(?<bracket>(\{|\})))|(?=(?<value>(\#\$[^\s,$]*|\#[^\s$]*|([^\#][\$])[^\s]*)))|(?=(?<variable>(\#\$[^\s,$]*|\#[^\s$]*|([^\#][\$])[^\s]*)))|(?=(?:,.|[,xy])|(?<variable>(?<![.$#])(\b[a-zA-Z][a-zA-Z0-9_,]*)))";
        private static Regex m_RegexPattern;
    }
}
