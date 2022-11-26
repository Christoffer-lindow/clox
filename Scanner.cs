using System;
using System.Collections.Generic;

namespace App
{
    public class Scanner
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
            while (!IsAtEnd() && Peek() != '"')
            {
                if (Peek() == '\n') Line++;
                Advance();
            }

            if (IsAtEnd())
            {
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
            var text = Source[Start..Current];
            Tokens.Add(new Token(type, text, literal, Line));
        }

        private bool IsAtEnd() => Current >= Source.Length;

    }
}