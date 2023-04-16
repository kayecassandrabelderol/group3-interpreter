using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group3_Interpreter
{
    class Token
    {
        public TokenType Type { get; }
        public string Value { get; }

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return $"[{Type}, {Value}]";
        }
        public string getValue() 
        {
            return $"{Value}"+"";
        }
        public string getType()
        {
            return $"{Type}"+"";
        }
    }
    enum TokenType
    {
        Identifier,
        Operator,
        Constant,
        DataType,
        Comment,
        Print
    }

}
