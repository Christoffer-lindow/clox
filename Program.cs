using System;
using System.Collections.Generic;
using System.IO;

namespace App
{
    public class Program
    {
        public static void RuntimeException(RuntimeException ex)
        {
            Console.Error.WriteLine($"{ex.Message} \n [line {ex.Token.Line}]");
            HadRuntimeError = true;

        }
        private static bool HadError = false;
        private static bool HadRuntimeError = false;
        private static Interperter Interperter = new Interperter();
        public static void Main()
        {
            var args = GetCommandLineArgs();
            if (args.Length > 1)
            {
                PrintUsage();
                System.Environment.Exit(1);
            }
            else if (args.Length is 1)
            {
                if (args[0] == "gen")
                {
                    GenerateAst.Run();
                    System.Environment.Exit(0);
                }
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }
        }

        private static string[] GetCommandLineArgs()
        {
            var argsLength = System.Environment.GetCommandLineArgs().Length;
            return System.Environment.GetCommandLineArgs()[1..argsLength];
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage: lox [script]");
        }

        private static void RunFile(string path)
        {
            var text = File.ReadAllText(path);
            Run(text);
            if (HadError)
            {
                System.Environment.Exit(65);
            }
            if (HadRuntimeError)
            {
                System.Environment.Exit(70);
            }
        }

        private static void Run(string source)
        {
            var scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();
            Parser parser = new Parser(tokens);
            var statements = parser.Parse();
            var errors = scanner.GetErrors();
            if (HadError) {
                HadError = false;
                return;
            }
            Interperter.Interpret(statements);
        }

        public static void Error(int line, String message)
        {
            Report(line, "", message);
        }

        public static void Error(Token token, String message)
        {
            if (token.Type == TokenType.EOF)
            {
                Report(token.Line, " at end", message);
                return;
            }
            Report(token.Line, $" at '{token.Lexeme}'",message);

        }

        private static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine($"[ line {line} ] Error {where}: {message}");
            HadError = true;
        }

        private static void RunPrompt()
        {
            while (true)
            {
                Console.Write("> ");
                String line = Console.ReadLine();
                if (String.IsNullOrEmpty(line))
                {
                    break;
                }
                Run(line);
            }
        }
    }
}
