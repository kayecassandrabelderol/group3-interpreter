using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace Group3_Interpreter
{
    class Lexer
    {
        DataTable dt = new DataTable();
        private readonly string code;
        private readonly Dictionary<string, TokenType> keyWords = new Dictionary<string, TokenType>() {
        { "INT", TokenType.DataType },
        { "CHAR", TokenType.DataType },
        { "BOOL", TokenType.DataType },
        { "FLOAT", TokenType.DataType },
         { "DISPLAY:", TokenType.Print }
         };

        Dictionary<string, string> variables = new Dictionary<string, string>();

        public Lexer(string code)
        {
            this.code = code;
        }

        //this one is for recording each token
        public IEnumerable<Token> Tokenize()
        {
            using (StringReader reader = new StringReader(code))
            {
                string line = code;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                    {
                        continue;
                    }
                    else
                    {

                        yield return ParseReserveWord(line);
                    }
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

        private Boolean Parse_Int_Value(string value)
        {


            string pattern = @"^[+-]?\d+(\s*[+\-*/]\s*[+-]?\d+)*\s*$";
            bool isMatch = Regex.IsMatch(value, pattern);
           
            return isMatch;

        }
        private Boolean Parse_Float_Value(string value)
        {
            string pattern = @"^\s*[+-]?\d+(\.\d+)?\s*([+\-*/]\s*[+-]?\d+(\.\d+)?\s*)*$";
            bool isMatch = Regex.IsMatch(value, pattern);

            return isMatch;

        }

        private Boolean Parse_Char_Value(string value)
        {
            int position = 0;
            Boolean result = false;

            //must start with single qoute
            if (value[position] == '\'')
            {
                position++;
                if (char.IsLetter(value[position]))
                {

                    position++;
                    if (value[position] == '\'')
                    {
                        result |= true;
                    }
                    else 
                    {
                        result = false;
                    }
                }
                else 
                {
                    result= false;
                }

            }
            else 
            {
                result = false;
            }
                    
            return result;

        }
        //fix the boolean value
        private Boolean Parse_Bool_Value(string value)
        {



            string pattern = @"^\""(TRUE|FALSE)\""$";
            bool isMatch = Regex.IsMatch(value, pattern);

            return isMatch;


        }

        private Token ParseDisplay(string message) 
        {
            
            string display = "";
            string inputError = "";

            try 
            {
                //check the message if the & grammar is correct
                //the & must not start at the start and end
                if (message.StartsWith("&") || message.EndsWith("&")) 
                {
                    throw new Exception("It must not be start or end with &");
                }

               
                string pattern_Consecutive_Ampersands = @"&\s*&";
                bool isMatch_Consecutive_Ampersands = Regex.IsMatch(message, pattern_Consecutive_Ampersands);

                if (isMatch_Consecutive_Ampersands)
                {
                    throw new Exception("The string contains two consecutive ampersands");
                }
                //split 
                string[] inputs = message.Split('&');


                //check for arithmetic
                string arithmeticPattern = @"(\b[a-zA-Z]\w*\b|\d+)(\s*[\+\-\*\/\%]\s*(\b[a-zA-Z]\w*\b|\d+))+";
                //check for double qoutation ^\s*"[^"]*"\s*$
                string stringPattern = @"^\s*""[^""]*""\s*$";
              
                string bracketPattern = @"^\[[^\s\]]+\]$";
                string dollarSignPattern = @"^\s*\$\s*$";
               

               
                foreach (string input in inputs)
                {
                   // Console.WriteLine(input[1]+"h");
                    bool isMatch_arithmeticPattern = Regex.IsMatch(input, arithmeticPattern);
                    bool isMatch_stringPattern = Regex.IsMatch(input, stringPattern);
                    bool isMatch_bracketPattern = Regex.IsMatch(input, bracketPattern);
                    bool isMatch_dollarSign = Regex.IsMatch(input, dollarSignPattern);

                    inputError = input;
                  
                    if (isMatch_arithmeticPattern || isMatch_bracketPattern || isMatch_stringPattern)
                    {
                        //to do remove space
                        if (isMatch_arithmeticPattern) 
                        {
                            string value = "";
                            value = dt.Compute(input,null).ToString();
                            display += value;
                        }

                        if (isMatch_stringPattern)
                        {
                           
                           
                            int startIndex = input.IndexOf("\"") + 1;
                            int endIndex = input.IndexOf("\"", startIndex);
                            string value = input.Substring(startIndex, endIndex - startIndex);

                            display += value;
                        }

                        if (isMatch_bracketPattern)
                        {

                            int startIndex = input.IndexOf("\"") + 1;
                            int endIndex = input.IndexOf("\"", startIndex);
                            string value = input.Substring(startIndex, endIndex - startIndex);

                            display += value;
                        }
                        if (input.Trim().StartsWith("$"))
                        {
                            display += "\n";
                        }
                 
                     


                   }
                    else 
                    {
                       
                        throw new Exception("Invalid Input");
                    }  

                }



                //Escape code
                //must start and ends with []
                //^\s *\[[^""\[\]]*\]\s *$
                //^\s*\[[^""\[\]]*\]\s*$
                //^\[[^\s\]]+\]$  must not contain any space inside 
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message + inputError);
                Environment.Exit(1);
               return new Token(TokenType.DataType, "Program Stop");
            }
            



            //if no & just get all the keywords w
            //if their are spaces
            //split the display by &
            //check the expression if expression only expression error if it encountered anything else same as the rest
            //(\b[a-zA-Z]\w*\b|\d+)(\s*[\+\-\*\/\%]\s*(\b[a-zA-Z]\w*\b|\d+))+ for arithmetic
            //^\"[^\"]*\"$ for enclosed double qoutation
            //check the variables and replaceW
            //else "Unexpected input"
            //evaluate each then concatenate each
            return new Token(TokenType.Print, display);
        }


        private Token ParseDataType(ref int pos, string data_type) 
        {
            int start = pos;
            string variableName = "";
           

            try
            {
                //skipe white space
                if (char.IsWhiteSpace(code[pos]))
                {
                    pos++;
                }

                //parse the variable name


                while (pos < code.Length && char.IsLetterOrDigit(code[pos]))
                {
                    variableName += code[pos];
                    pos++;
                }

                //check if it is empty or it is a keyword
                if (variableName == "" || keyWords.ContainsKey(variableName))
                {
                    throw new Exception("Invalid variable name");
                }

                //skip space
                if (char.IsWhiteSpace(code[pos]))
                {
                    pos++;
                }

                //find the assignment operator
                if (pos < code.Length && code[pos] == '=')
                {
                    pos++; // move past the equals sign
                }
                else
                {
                    throw new Exception("Invalid variable declaration");
                }
                //after equal is space
                if (char.IsWhiteSpace(code[pos]))
                {
                    pos++;
                }

                //Check what type of data type


                // Parse the variable value
                int valueStart = pos;

                while (pos < code.Length && code[pos] != '\n')
                {
                    pos++;
                }
                
                if (valueStart == pos)
                {
                    
                    throw new Exception("No variable value found");
                }

                string value = (code.Substring(valueStart, pos - valueStart)).Replace(" ", "");

              
                if (data_type.Equals("INT"))
                {
                    
                    //check if the value is acceptable
                    if (Parse_Int_Value(value)) 
                    {
                        //it will cause an exception if other than digit and operator is found
                        //if char it will look in database if none Evaluate exception

                        value = dt.Compute(value, null).ToString(); 
                        variables.Add(variableName, value);
                    }
                    else 
                    {
                        
                        throw new Exception("Invalid variable value");
                    }
                    
                }
                else if (data_type.Equals("CHAR"))
                {

                    if (Parse_Char_Value(value))
                    {
                        variables.Add(variableName, value);
                    }
                    else
                    {
                        throw new Exception("Invalid variable value");
                    }
                }
                else if (data_type.Equals("BOOL"))
                {
                    if (Parse_Bool_Value(value))
                    {
                        variables.Add(variableName, value);
                    }
                    else
                    {
                        throw new Exception("Invalid variable value");
                    }

                }
                else if (data_type.Equals("FLOAT"))
                {
                    //check if the value is acceptable
                    if (Parse_Float_Value(value))
                    {
                        value = dt.Compute(value, null).ToString();
                        variables.Add(variableName, value);
                        
                    }
                    else
                    {
                        throw new Exception("Invalid variable value");
                    }
                }
                else 
                {
                    throw new Exception("Invalid DataType");
                }

               
                return new Token(TokenType.DataType, value.ToString());

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
                return new Token(TokenType.DataType, "Program Stop");

            }

        }
        

        private Token ParseReserveWord(string line)
        {

       
           
            try 
            {
                //INT a Variable declaration
                //INT a,b,c Multiple Variable declaration
                //INT A = 10 Variable assignment
                //INT A,B,C = 10 Multiple variable assignment
                // A=B=C=3





                return new Token(TokenType.DataType, "Program Stop");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
                return new Token(TokenType.DataType, "Program Stop");

            } 

        }



    }

}

