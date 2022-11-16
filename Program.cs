using System;

class Program
{
    public static void Main()
    {
        var args = GetCommandLineArgs();
        if (args.Length > 1)
        {
            PrintUsage();
            Environment.Exit(1);
        }
        else if (args.Length is 1)
        {
            RunFile(args[0]);
        }
        else
        {
            RunPrompt();
        }

    }

    private static string[] GetCommandLineArgs()
    {
        var argsLength = Environment.GetCommandLineArgs().Length;
        return Environment.GetCommandLineArgs()[1..argsLength];
    }

    private static void PrintUsage()
    {
        Console.WriteLine("Usage: lox [script]");
    }

    private static void RunFile(string file)
    {

    }

    private static void RunPrompt()
    {
        
    }

}
