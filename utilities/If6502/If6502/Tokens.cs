using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace If6502
{
    public struct Token
    {
        public TokenType TokenType;
        public string Value;

        public Token(string type, string value)
        {
            Value = value;
            switch (type)
            {
                case "operator":
                    TokenType = TokenType.Operator;
                    break;
                case "indexedvalue":
                    TokenType = TokenType.Indexed;
                    break;
                case "controlflow":
                    TokenType = TokenType.ControlFlow;
                    break;
                case "register":
                    TokenType = TokenType.Register;
                    break;
                case "bracket":
                    TokenType = TokenType.Bracket;
                    break;
                case "value":
                    TokenType = TokenType.Variable;
                    break;
                case "variable":
                    TokenType = TokenType.Variable;
                    break;
                default:
                    TokenType = TokenType.Unknown;
                    break;
            }
        }
    }

    public enum TokenType
    {
        WhiteSpace,
        ControlFlow,
        Register,
        Operator,
        Variable,
        Indexed,
        Bracket,
        Unknown
    }
}
