using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group3_Interpreter
{
    class Lexer
    {
        private readonly string code;
        private readonly Dictionary<string, TokenType> keyWords = new Dictionary<string, TokenType>() {
        { "int", TokenType.DataType },
        { "double", TokenType.DataType },
        { "string", TokenType.DataType }
         };

        public Lexer(string code)
        {
            this.code = code;
        }

        //this one is for recording each token
        public IEnumerable<Token> Tokenize()
        {
            int pos = 0;

            //iterate to scan for tokens
            while (pos < code.Length)
            {
                if (code[pos] == '/' && code[pos + 1] == '/' && pos + 1 < code.Length)
                {
                    yield return ParseSingleLineComment(ref pos);
                }
                else if (code[pos] == '/' && pos + 1 < code.Length && code[pos + 1] == '*')
                {
                    yield return ParseMultiLineComment(ref pos);
                }
                else if (char.IsLetter(code[pos]))
                {
                    yield return ParseDataType(ref pos);
                }
                else if (char.IsWhiteSpace(code[pos]))
                {
                    pos++;
                }
            }
        }

        private Token ParseSingleLineComment(ref int pos)
        {
            int start = pos;
            pos += 2;

            while (pos < code.Length && code[pos] != '\n')
            {
                pos++;
            }

            return new Token(TokenType.Comment, code.Substring(start, pos - 1 - start));
        }
        private Token ParseMultiLineComment(ref int pos)
        {
            int start = pos;
            pos += 2;

            while (pos + 1 < code.Length && !(code[pos] == '*' && code[pos + 1] == '/'))
            {
                pos++;
            }

            pos += 2;

            return new Token(TokenType.Comment, code.Substring(start, pos - start));
        }

        private Token ParseDataType(ref int pos)
        {

            int start = pos;

            while (pos < code.Length && char.IsLetter(code[pos]))
            {
                pos++;
            }

            string keyword = code.Substring(start, pos - start);
            try
            {
                if (!keyWords.ContainsKey(keyword))
                {

                    throw new Exception("Invalid keyword");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(keyword + ":" + ex.Message);
                Environment.Exit(1);

            }

            //next of data type is space
            if (char.IsWhiteSpace(code[pos]))
            {
                pos++;
            }

            //parse the variable name
            string variableName = "";

            while (pos < code.Length && char.IsLetterOrDigit(code[pos]))
            {
                variableName += code[pos];
                pos++;
            }
            try
            {
                if (variableName == "")
                {
                    throw new Exception("Invalid variable name");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }
            //next of variable name is space
            if (char.IsWhiteSpace(code[pos]))
            {
                pos++;
            }
            // Parse the equals sign
            //in this language no variable declaration ONLY variable assignment
            //that is why we look for assignment operator for every data type variable
            try
            {
                if (pos < code.Length && code[pos] == '=')
                {
                    pos++; // move past the equals sign
                }
                else
                {
                    throw new Exception("Invalid variable declaration");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }
            //after equal is space
            if (char.IsWhiteSpace(code[pos]))
            {
                pos++;
            }
            // Parse the variable value
            int valueStart = pos;

            while (pos < code.Length && char.IsDigit(code[pos]))
            {
                pos++;
            }
            try
            {
                if (valueStart == pos)
                {
                    throw new Exception("Invalid variable value");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }

            int value = int.Parse(code.Substring(valueStart, pos - valueStart));
            return new Token(TokenType.DataType, value.ToString());
        }



    }

}

