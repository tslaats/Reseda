using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reseda.Core;
using Reseda.Parser;
using System.IO;

namespace Reseda.ConsoleApp
{
    class Program
    {
        static Event term;
        static ResedaParser parser;

        static void Main(string[] args)
        {
            parser = new ResedaParser();
            var exit = false;
            while (exit == false)
            {
                Console.WriteLine();
                Console.WriteLine("Enter command (help to display help): ");
                var commandParts = Console.ReadLine().Split(' ').ToList();
                var commandName = commandParts[0];
                var commandArgs = commandParts.Skip(1).ToList(); // the arguments is after the command                
                switch (commandName)
                {
                    // Create command based on CommandName (and maybe arguments)
                    case "exit": exit = true; break;
                    case "parse":
                        String s = "";
                        foreach (var i in commandArgs)
                            s += i + " ";
                        Parse(s);
                        break;
                    case "load":
                        Load(commandArgs[0]);
                        break;
                    case "print": Console.WriteLine(term.ToSource()); break;
                    case "execute":
                        Execute(commandArgs[0]);
                        break;
                }
            }
        }

        private static void Execute(string v)
        {
            throw new NotImplementedException();
        }

        private static void Load(string v)
        {
            try
            { 
                using (StreamReader sr = new StreamReader("D:\\reseda\\" + v + ".reseda"))
                {                    
                    Parse(sr.ReadToEnd());                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        private static void Parse(string s)
        {
            try
            {
                Console.WriteLine("Parsing: " + s);
                Console.WriteLine("AST: " + parser.stringTree(s));
                term = parser.Generate(s);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Parsing Error:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
