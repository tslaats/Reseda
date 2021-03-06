﻿using System;
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
        static RootEvent term;
        static ResedaParser parser;
        static bool autoCompute = false;

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
                try
                {
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
                        case "term": Console.WriteLine(term.ToSource()); break;
                        case "auto": autoCompute = !autoCompute;  Console.WriteLine(autoCompute); break;
                        case "tree": Console.WriteLine(term.PrintTree(true)); break;
                        case "live": Console.WriteLine(term.ProcessIsLive()); break;
                        case "inseq": term = term.MakeInSeq(); break;
                        case "glitchfree": term = term.MakeGlitchFree(); break;
                        case "list":
                            foreach (var pe in term.GetAllEnabledEvents())
                                Console.WriteLine(pe.ToSource());
                            break;
                        case "execute":
                            if (commandArgs.Count > 1)
                                Execute(commandArgs[0], commandArgs[1]);
                            else
                                Execute(commandArgs[0]);
                            break;
                        default:
                            if (commandArgs.Count > 0)
                                Execute(commandName, commandArgs[0]);
                            else
                                Execute(commandName);

                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Command " + commandName + "failed, because:");
                    Console.WriteLine(e.Message);
                }
            }
        }

        private static void Execute(string v)
        {
            var path = parser.GeneratePath(v);
            var es = path.Eval(term);
            if (es.Count == 0)
                Console.WriteLine("Not a valid event.");
            else if (es.Count > 1)
                Console.WriteLine("Path selects more than one event, consider adding [0].");
            else
            {
                try
                {
                    es.ElementAt(0).Execute();
                    Console.WriteLine(term.ToSource());
                    Console.WriteLine(term.PrintTree(true));
                    if (autoCompute)
                    {
                        Console.WriteLine("Auto computing:");
                        Console.WriteLine(term.AutoComputeToString());
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Execution of " + v + " failed because: " + e.Message);
                }
            }
        }

        private static void Execute(string v, string value)
        {
            var path = parser.GeneratePath(v);
            var es = path.Eval(term);
            if (es.Count == 0)
                Console.WriteLine("Not a valid event.");
            else if (es.Count > 1)
                Console.WriteLine("Path selects more than one event, consider adding [0].");
            else
            {
                try
                { 
                    InputEvent e = (InputEvent)es.ElementAt(0);
                    bool b;
                    int i;
                    if (Boolean.TryParse(value, out b))
                    {
                        e.Execute(b);
                    }
                    else if (int.TryParse(value, out i))
                    {
                        e.Execute(i);
                    }
                    else
                        e.Execute(value);
                    Console.WriteLine(term.ToSource());
                    Console.WriteLine(term.PrintTree(true));
                    if (autoCompute)
                    {
                        Console.WriteLine("Auto computing:");
                        Console.WriteLine(term.AutoComputeToString());
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Execution of " + v + " failed because: " + e.Message);
                }
            }
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
                term = (RootEvent)parser.Generate(s);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Parsing Error:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
