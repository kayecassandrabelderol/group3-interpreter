using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Group3_Interpreter
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Application.Run(new Form1());
            //thoughts? get all if statement/ while loop to be subjected to its own lexer
            Trace.Listeners.Clear(); // Clear existing listeners
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out)); // Add a listener for console output
            // Input code string to be parsed
            Console.WriteLine("\n\nSource Code:");
              string code = @"
                BEGIN CODE
	                INT x = -5
                    DISPLAY: x
                END CODE
                ";

            if (code.Trim().StartsWith("BEGIN CODE") && code.Trim().EndsWith("END CODE"))
            {
                Console.WriteLine(code);
                code = code.Replace("BEGIN CODE", "").Replace("END CODE", "");
                Console.WriteLine("Output\n\n");
                // Create a new lexical analyzer
                Lexer lexer = new Lexer(code);

                // Parse the code and print out the tokens
                foreach (Token token in lexer.Tokenize())
                {
                    TokenType tokenType = token.Type;
                    if (token.getType() == "Print") 
                    {
                        Console.WriteLine(token.getValue());
                    }
                }
            }
            else 
            {
                Console.WriteLine("CODE must start with BEGIN CODE and end with END CODE");
            }



            Trace.Close();
            /*

            string pattern = @"(?:^|\s)([a-zA-Z0-9]+(?:,[a-zA-Z0-9]+)*|[a-zA-Z0-9]+)\s*&\s*([a-zA-Z0-9]+(?:,[a-zA-Z0-9]+)*|[a-zA-Z0-9]+)(?=\s|$)";
            string input = "a1,2b & c_d,e & 3f & g & h,i & jk & l,m,n & opqrs & 5t,6u & 7v";
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(input);
            foreach (Match match in matches)
            {
                Console.WriteLine("Left side: " + match.Groups[1].Value);
                Console.WriteLine("Right side: " + match.Groups[2].Value);
                Console.WriteLine();
            }

            */

        }
    }
}
