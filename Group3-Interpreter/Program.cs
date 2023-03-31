using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            // Input code string to be parsed
            Console.WriteLine("\n\nSource Code:");
            string code = @"
            ints xen = 10 //single line 
            int x = 11 
            /* This is a
               multiline
               comment */
            
        ";
            Console.WriteLine(code);
            Console.WriteLine("Output\n\n");
            // Create a new lexical analyzer
            Lexer lexer = new Lexer(code);

            // Parse the code and print out the tokens
            foreach (Token token in lexer.Tokenize())
            {
                Console.WriteLine(token);
            }

        }
    }
}
