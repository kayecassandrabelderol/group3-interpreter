﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Reflection;
using System.Net;
using System.Linq.Expressions;
using System.Collections;

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
        //variablename and its [datatype and value]
        Dictionary<string, string[]> Globalvariables = new Dictionary<string, string[]>();
      //  string[] booleanOperators = new string[] { "Hello", "world", "!" };


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
        private string Parse_Expression(string value)
        {
            if (Parse_Bool_Value(value))
            {
                value = value.Replace("\"", "");

            }




            foreach (string key in Globalvariables.Keys)
            {

                string pattern123 = $@"\b{key}\b";
                string[] varInfo = Globalvariables[key];
                string replacement = varInfo[1];

                if (varInfo[0] == "INT" || varInfo[0] == "FLOAT" || varInfo[0] == "BOOL")
                {

                    value = Regex.Replace(value, pattern123, replacement);
                    value = value.Replace("\"", "");

                }
            }

            //check if value contains a variable
            string pattern1 = @"\b^[a-zA-Z]|[a-zA-Z_]\w*$\b"; // matches any number (whole or decimal)
            bool containsLetterAsOperand = Regex.IsMatch(value, pattern1);
            MatchCollection matchesVariableName = Regex.Matches(value, pattern1);
            foreach (Match match in matchesVariableName)
            {
                if (match.Value.Equals("TRUE") || match.Value.Equals("FALSE"))
                {

                    continue;
                }

                if (containsLetterAsOperand)
                {
                    Console.WriteLine("The expression contains a letter or a word as an operand.");
                    Environment.Exit(1);
                }

            }

            if (dt.Compute(value, null).ToString() == "True" || dt.Compute(value, null).ToString() == "False") 
            {
               value =  dt.Compute(value, null).ToString();
              
                string patternTRUE = $@"\bTRUE\b";
                string patternFALSE = $@"\bFALSE\b";
                string patternTrue = $@"\bTrue\b";
                string patternFalse = $@"\bFalse\b";
                value = Regex.Replace(value, patternTRUE, "\"TRUE\"");
                value = Regex.Replace(value, patternFALSE, "\"FALSE\"");
                value = Regex.Replace(value, patternTrue, "\"TRUE\"");
                value = Regex.Replace(value, patternFalse, "\"FALSE\"");
                return value;
            }
           
            return dt.Compute(value,null).ToString();

        }
        private Boolean Parse_Int_Value(string value)
        {

            // Regular expression to match numbers, including integers and floating-point numbers
            string pattern = @"[-+]?[0-9]*\.?[0-9]+";

            // Extract all operands from the expression using the regular expression
            MatchCollection matches = Regex.Matches(value, pattern);

            // Regular expression to match only integers
            string integerPattern = @"^[+-]?\d+$";

            // Check if all operands are whole numbers
            bool isValid = true;
            foreach (Match match in matches)
            {
                if (!Regex.IsMatch(match.Value, integerPattern))
                {
                    isValid = false;
                    break;
                }
            }

            // Output the result
            if (isValid)
            {
                return true;
            }
            else
            {
                return false;
            }


          

        }
        private Boolean Parse_Float_Value(string value)
        {
            string pattern = @"^\s*[+-]?\d+(\.\d+)?\s*([+\-*/]\s*[+-]?\d+(\.\d+)?\s*)*$";
            bool isMatch = Regex.IsMatch(value.Trim(), pattern);

            return isMatch;

        }

        private Boolean Parse_Char_Value(string value)
        {
            string pattern = @"^'([a-zA-Z])'$";
            bool isMatch = Regex.IsMatch(value.Trim(), pattern);

            return isMatch;

        }
        //fix the boolean value
        private Boolean Parse_Bool_Value(string value)
        {
            string pattern = @"^""(TRUE|FALSE)""$";
            string patternBoolean = @"(\b[a-zA-Z]\w*\b|\d+)(\s*[<>]=?|!=|[=]=?|\sAND\s|\sOR\s|\sNOT\s)(\s*(\b[a-zA-Z]\w*\b|\d+))+";

            //get rid of "" in true or false
          
            bool isMatch = Regex.IsMatch(value.Trim(), pattern);
          
            if (!isMatch) 
            {
               
                    string patternTRUE = $@"\b""TRUE""\b";
                    string patternFALSE = $@"\b""FALSE""\b";
                    value = Regex.Replace(value, patternTRUE, "TRUE");
                    value = Regex.Replace(value, patternFALSE, "FALSE");
                    isMatch = Regex.IsMatch(value.Trim(), patternBoolean);
            
            }

           
            return isMatch;
        }

        private Boolean ParseValue(string datatype,string VariableValue)
        {
            bool valid_value = false;
            if (datatype == "INT")
            {
                valid_value = Parse_Int_Value(VariableValue);
            }
            else if (datatype == "FLOAT")
            {
                valid_value = Parse_Float_Value(VariableValue);
            }
            else if (datatype == "CHAR")
            {
                valid_value = Parse_Char_Value(VariableValue);
            }
            else if (datatype == "BOOL")
            {
                valid_value = Parse_Bool_Value(VariableValue);
            }

            return valid_value;
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
                string arithmeticPattern = @"^\s*[+-]?(?:\b[a-zA-Z_]\w*\b|\d+(\.\d+)?)\s*([+\-*/]\s*[+-]?(?:\b[a-zA-Z_]\w*\b|\d+(\.\d+)?)\s*)*$";
                //check for double qoutation ^\s*"[^"]*"\s*$
                string stringPattern = @"^\s*""[^""]*""\s*$";
              
                string bracketPattern = @"^\[[^\s\]]+\]$";
                string dollarSignPattern = @"^\s*\$\s*$";
               // string variableNamePattern = @"^[a-zA-Z]|[a-zA-Z_]\w*$";


                foreach (string input in inputs)
                {
                    bool isMatch_arithmeticPattern = Regex.IsMatch(input, arithmeticPattern);
                    bool isMatch_stringPattern = Regex.IsMatch(input, stringPattern);
                    bool isMatch_bracketPattern = Regex.IsMatch(input, bracketPattern);
                    bool isMatch_dollarSign = Regex.IsMatch(input, dollarSignPattern);

                    inputError = input;
                  
                    if (isMatch_arithmeticPattern || isMatch_bracketPattern || isMatch_stringPattern || isMatch_dollarSign || Parse_Bool_Value(input))
                    {
                        //to do remove space
                        if (isMatch_arithmeticPattern) 
                        {
                            string value = "";
                            value = Parse_Expression(input);
                            display += value;
                        }

                        if (Parse_Bool_Value(input)) 
                        {
                            string value = "";
                            value = Parse_Expression(input);
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

                            int startIndex = input.IndexOf("[") + 1;
                            int endIndex = input.IndexOf("]", startIndex);
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
                if (ex.Message.Contains("Cannot find column"))
                {

                    int startIndex = ex.Message.IndexOf("[") + 1;
                    int endIndex = ex.Message.IndexOf("]", startIndex);
                    string variableName = ex.Message.Substring(startIndex, endIndex - startIndex);
                    Console.WriteLine("Variable name: "+variableName + " is undefine");
                }
                else 
                {
                    Console.WriteLine(ex.Message + inputError);
                }
                   
              
                Environment.Exit(1);
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

    

        private Token ParseReserveWord(string line)
        {
            string value = "";
            try 
            {
                string variableNamePattern = @"^[a-zA-Z]|[a-zA-Z_]\w*$";
                //INT a Variable declaration
                // ^(?:INT|FLOAT)\s+([a-zA-Z_]\w*)(?:,\s*([a-zA-Z_]\w*))*$ 
                //INT A = 10 Variable assignment
                //^(?:INT|FLOAT)\s+([a-zA-Z_]\w*)(?:,\s*([a-zA-Z_]\w*))*\s*,\s*([a-zA-Z_]\w*\s*=.*)|(?:INT|FLOAT)\s+([a-zA-Z_]\w*)\s*=\s*([0-9]+)$

                //INT A,B,C = 10 Multiple variable assignment
                // A=B=C=3

                //check the assinment/declaration
                string variablePattern = @"^(?:INT|FLOAT|BOOL|CHAR)\s+([a-zA-Z_]\w*)(?:,\s*([a-zA-Z_]\w*))*\s*(?:=\s*[^=]+)*$";
                bool isMatch_variablePattern = Regex.IsMatch(line, variablePattern);
                //for variable declaration and assignment
                if (isMatch_variablePattern)
                {
                    string datatype = "";
                    //remove datatype
                    if (line.StartsWith("INT"))
                    {
                        datatype = "INT";
                        line = line.Substring(datatype.Length).Trim();
                    }
                    else if (line.StartsWith("BOOL"))
                    {
                        datatype = "BOOL";
                        line = line.Substring(datatype.Length).Trim();
                    }
                    else if (line.StartsWith("CHAR"))
                    {
                        datatype = "CHAR";
                        line = line.Substring(datatype.Length).Trim();
                    }
                    else if (line.StartsWith("FLOAT"))
                    {
                        datatype = "FLOAT";
                        line = line.Substring(datatype.Length).Trim();
                    }
                    else 
                    {
                        throw new Exception("Data Type not Found");
                    }

                    //multiple variable assignment
                    //check variable name if it is valid or not or it is a keyword
                    string multipleVariablePattern = @"^[^,][^,]*(,[^,]+[^,]*)*$";
                    bool isMatch_multipleVariable = Regex.IsMatch(line, multipleVariablePattern);
                    if (isMatch_multipleVariable && line.Contains(','))
                    {
                        
                        string[] variables = line.Split(',');


                        //---------Get last variable and value then add to dictionary-----------------------------
                        string[] last_variable_info = variables[variables.Length - 1].Split('=');
                        string lastVariableName = last_variable_info[0];

                        bool isMatch_lastVariableName = Regex.IsMatch(lastVariableName, variableNamePattern);
                        string lastVariableValue = last_variable_info[1];


                      


                        //check if value is valid
                        bool valid_value = false;
                        valid_value = ParseValue(datatype, lastVariableValue);

                      
                        if (datatype == "INT" || datatype == "FLOAT" || datatype == "BOOL")
                        {
                          
                            lastVariableValue = Parse_Expression(lastVariableValue);
                           
                            valid_value = ParseValue(datatype, lastVariableValue);//to recheck after evaluation if it is still whole number
                          
                        }

                       
                        //----------------------------------------------- INT x,y,z=10
                        //add data to dictionary
                        if (isMatch_lastVariableName && valid_value)
                        {
                            value= lastVariableValue;
                            Globalvariables.Add(lastVariableName.Trim(), new string[] { datatype, lastVariableValue });
                        }
                        else 
                        {
                            throw new Exception("Invalid Value");
                        }
                        //-----------------------------------------------------------------------------------------   


                        //check the variable
                        for (int i = 0; i < variables.Length - 1; i++)
                        {
                            bool isMatch_variableName = Regex.IsMatch(variables[i].Trim(), variableNamePattern);
                            if (isMatch_variableName)
                            {
                                if (!keyWords.ContainsKey(variables[i]))
                                {
                                    if (variables[0].Contains("="))
                                    {
                                        if (!variables[i].Contains("=")) 
                                        {
                                            throw new Exception("Variable assignment does not have =");
                                        }

                                        string[] variable_info = variables[i].Split('=');
                                        string Variable_Name = variable_info[0];
                                        string Variable_Value = variable_info[1];
                                       
                                        if (datatype == "INT" || datatype == "FLOAT" || datatype == "BOOL")
                                        {

                                            Variable_Value = Parse_Expression(Variable_Value);

                                            valid_value = ParseValue(datatype, Variable_Value);//to recheck after evaluation if it is still whole number
                                           
                                        }
                                        value = Variable_Value;
                                       
                                        Globalvariables.Add(Variable_Name.Trim(), new string[] { datatype, Variable_Value });
                                    
                                    }
                                    else //last variable assignment INT x,y=3
                                    {
                                        value = lastVariableValue;
                                        Globalvariables.Add(variables[i].Trim(), new string[] { datatype, lastVariableValue });
                                    }
                                  


                                }
                                else 
                                {
                                    throw new Exception("Variable name is a keyword");
                                }
                            }
                            else
                            {
                              
                                throw new Exception("Invalid Variable name");
                            }

                        }
                    }  //----------------------------------------------- INT x,y,z=10
                    else //single variable assignment/declaration
                    {   
                        //to do if variable declaration only??


                        string[] last_variable_info = line.Split('=');
                        string VariableName = last_variable_info[0].Trim();
                        bool isMatch_VariableName = Regex.IsMatch(VariableName, variableNamePattern);
                        string VariableValue = last_variable_info[1];

                        bool valid_value = false;
                       
                        valid_value = ParseValue(datatype, VariableValue);
                      
                        if (datatype == "INT" || datatype == "FLOAT" || datatype == "BOOL")
                        {
                            VariableValue = Parse_Expression(VariableValue);
                            valid_value = ParseValue(datatype, VariableValue);//to recheck after evaluation 
                          
                        }

                        if (isMatch_VariableName && valid_value)
                        {
                            value = VariableValue;
                            Globalvariables.Add(VariableName.Trim(), new string[] { datatype, VariableValue });
                            
                        }
                        else
                        {
                            throw new Exception("Invalid Value");
                        }



                    }
                 
                }
                else if (line.StartsWith("DISPLAY:"))
                {
                   
                    string startingWord = "DISPLAY:";
                    string message = line.Substring(startingWord.Length).Trim();
                    return ParseDisplay(message);
                }
                else if (false) //variable assignment ex: x=y=1
                {
                }
                else 
                {
                    throw new Exception("Invalid keyword");
                }
                      
                return new Token(TokenType.DataType,value);
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

