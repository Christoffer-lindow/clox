using System;
using System.Collections.Generic;
using System.IO;

namespace App
{
    class Program
    {
        private static bool HadError = false;
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
                if (args[0] == "gen")
                {
                    GenerateAst.Run();
                    Environment.Exit(0);
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
            var argsLength = Environment.GetCommandLineArgs().Length;
            return Environment.GetCommandLineArgs()[1..argsLength];
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
                Environment.Exit(2);
            }
        }

        private static void Run(string source)
        {
            var scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();
            var errors = scanner.GetErrors();
            tokens.ForEach(token => Console.WriteLine(token.ToString()));
            errors.ForEach(error => Error(error.Item1, error.Item2));
        }

        public static void Error(int line, String message)
        {
            Report(line, "", message);
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

    enum TokenType
    {
        IDENTIFIER,
        // Single-character tokens.
        LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE,
        COMMA, DOT, MINUS, PLUS, SEMICOLON, SLASH, STAR,

        // One or two character tokens.
        BANG, BANG_EQUAL,
        EQUAL, EQUAL_EQUAL,
        GREATER, GREATER_EQUAL,
        LESS, LESS_EQUAL,

        // Literals.
        STRING, NUMBER,

        // Keywords.
        AND, CLASS, ELSE, FALSE, FUN, FOR, IF, NIL, OR,
        PRINT, RETURN, SUPER, THIS, TRUE, VAR, WHILE,

        EOF
    }

    class Token
    {
        private TokenType Type;
        private String Lexeme;
        private Object Literal;
        private int Line;

        public Token(TokenType type, String lexeme, Object literal, int Line)
        {
            Type = type;
            Lexeme = lexeme;
            Literal = literal;
            this.Line = Line;
        }

        public override string ToString()
        {
            return $"{Type} {Lexeme} {Literal}";
        }
    }

    class Scanner
    {
        private Dictionary<string, TokenType> Keywords;
        private String Source;
        private List<Token> Tokens;
        private int Start = 0;
        private int Current = 0;
        private int Line = 1;
        private List<(int, string)> Errors;

        public Scanner(string source)
        {
            Source = source;
            Tokens = new();
            Errors = new();
            Keywords = new();

            Keywords.Add("and", TokenType.AND);
            Keywords.Add("class", TokenType.CLASS);
            Keywords.Add("else", TokenType.ELSE);
            Keywords.Add("false", TokenType.FALSE);
            Keywords.Add("for", TokenType.FOR);
            Keywords.Add("fun", TokenType.FUN);
            Keywords.Add("if", TokenType.IF);
            Keywords.Add("nil", TokenType.NIL);
            Keywords.Add("or", TokenType.OR);
            Keywords.Add("print", TokenType.PRINT);
            Keywords.Add("return", TokenType.RETURN);
            Keywords.Add("super", TokenType.SUPER);
            Keywords.Add("this", TokenType.THIS);
            Keywords.Add("true", TokenType.TRUE);
            Keywords.Add("var", TokenType.VAR);
            Keywords.Add("while", TokenType.WHILE);
        }

        public List<Token> ScanTokens()
        {
            Console.WriteLine("running");
            while (!IsAtEnd())
            {
                Start = Current;
                ScanToken();
            }
            Tokens.Add(new Token(TokenType.EOF, "", null, Line));
            return Tokens;
        }

        private void ScanToken()
        {
            var c = Advance();
            switch (c)
            {
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '{': AddToken(TokenType.LEFT_BRACE); break;
                case '}': AddToken(TokenType.RIGHT_BRACE); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(TokenType.MINUS); break;
                case '+': AddToken(TokenType.PLUS); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '*': AddToken(TokenType.STAR); break;

                case '!':
                    AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                case '=':
                    AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                case '<':
                    AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;
                case '>':
                    AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;

                case '/':
                    if (Match('/'))
                    {
                        while (Peek() != '\n' && !IsAtEnd())
                        {
                            Advance();
                        }
                    }
                    else
                    {
                        AddToken(TokenType.SLASH);
                    }
                    break;

                case ' ':
                case '\r':
                case '\t':
                    break;

                case '\n':
                    Line++;
                    break;

                case '"': HandleString(); break;


                default:
                    if (char.IsDigit(c))
                    {
                        HandleNumber();
                        break;
                    }
                    else if (char.IsAsciiLetterLower(c))
                    {
                        HandleIdentifier();
                        break;
                    }
                    Console.WriteLine("we are here");
                    AddError(Line, $"Unexpected character {c}");
                    break;
            };
        }

        // Recheeck this
        private char Advance()
        {
            var c = Source[Current];
            Current++;
            return c;
        }

        private char Peek()
        {
            if (IsAtEnd()) return '\0';
            return Source[Current];
        }

        private char PeekNext()
        {
            if (Current + 1 >= Source.Length) return '\0';
            return Source[Current + 1];
        }

        private void HandleString()
        {
            Console.WriteLine("handling string");
            while (!IsAtEnd() && Peek() != '"')
            {
                if (Peek() == '\n') Line++;
                Advance();
            }

            if (IsAtEnd())
            {
                Console.WriteLine("Here");
                AddError(Line, "Unterminated string.");
                return;
            }
            Advance();

            var value = Source[(Start + 1)..(Current - 1)];
            AddToken(TokenType.STRING, value);
        }

        private void HandleNumber()
        {
            while (char.IsDigit(Peek())) Advance();

            if (Peek() == '.' && char.IsDigit(PeekNext()))
            {
                Advance();

                while (char.IsDigit(Peek())) Advance();
            }

            AddToken(TokenType.NUMBER, Double.Parse(Source[Start..Current]));
        }

        private bool IsAlphaNumeric(char c)
        {
            return char.IsAsciiLetterLower(c) || char.IsAsciiDigit(c);
        }

        private void HandleIdentifier()
        {
            while (IsAlphaNumeric(Peek())) Advance();
            var text = Source[Start..Current];
            var type = Keywords.GetValueOrDefault(text);
            AddToken(type);
        }


        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (Source[Current] != expected) return false;

            Current++;
            return true;
        }
        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        public List<(int, string)> GetErrors()
        {
            return Errors;
        }

        private void AddError(int line, string reason)
        {
            Errors.Add((line, reason));
        }

        private void AddToken(TokenType type, Object literal)
        {
            Console.WriteLine($"Start: | {Start} | Current: {Current} | Length | {Source.Length} ");
            var text = Source[Start..Current];
            Console.WriteLine(literal);
            Tokens.Add(new Token(type, text, literal, Line));
        }

        private bool IsAtEnd() => Current >= Source.Length;

    }
}
