using System.Collections.Generic;
using System;
namespace App
{
    public class Parser 
    {
        private List<Token> Tokens;
        private int Current = 0;

        public Parser(List<Token> tokens)
        {
            Tokens = tokens;
        }

        public Expr Parse()
        {
            try {
                return Expression();
            } catch (ParseException)
            {
                return null;
            }
        }

        private Expr Expression()
        {
            return Equality();
        }

        private Expr Equality()
        {
            Expr expr = Comparision();
            var tokenTypes = new TokenType[] { TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL};

            while (Match(tokenTypes))
            {
                Token op = Previous();
                Expr right = Comparision();
                expr = new Binary(expr, op, right);
            }
            return expr;
        }

        private bool Match(TokenType[] tokenTypes)
        {
            foreach (TokenType tokenType in tokenTypes)
            {
                if(Match(tokenType))
                {
                    return true;
                }
            }

            return false;
        }

        private bool Match(TokenType tokenType)
        {
            if (Check(tokenType))
                {
                    Advance();
                    return true;
                }
            return false;
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }

        private Token Advance()
        {
            if (!IsAtEnd()) Current = Current + 1;
            return Previous();
        }

        private bool IsAtEnd()
        {
            return Peek().Type == TokenType.EOF;
        }

        private Token Peek() 
        {
            return Tokens[Current];
        }

        private Token Previous()
        {
            return Tokens[Current - 1];
        }

        private Expr Comparision()
        {
            Expr expr = Term();
            var tokenTypes = new TokenType[] {TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL};
            while (Match(tokenTypes))
            {
                Token op = Previous();
                Expr right = Term();
                expr = new Binary(expr, op, right);
            }
            return expr;
        }

        private Expr Term()
        {
            Expr expr = Factor();

            var tokenTypes = new TokenType[] {TokenType.MINUS, TokenType.PLUS};

            while (Match(tokenTypes))
            {
                Token op = Previous();
                Expr right = Factor();
                expr = new Binary(expr, op, right);
            }

            return expr;
        }

        private Expr Factor()
        {
            Expr expr = this.Unary();
            var tokenTypes = new TokenType[] {TokenType.SLASH, TokenType.STAR};

            while (Match(tokenTypes))
            {
                Token op = Previous();
                Expr right = this.Unary();
                expr = new Binary(expr,op, right);
            }

            return expr;
        }

        private Expr Unary()
        {
            var tokenTypes = new TokenType[] {TokenType.BANG, TokenType.MINUS};

            if (Match(tokenTypes))
            {
                Token op = Previous();
                Expr right = this.Unary();
                return new Unary(op, right);
            }
            return Primary();
        }

        private Expr Primary()
        {
            if (Match(TokenType.FALSE)) return new Literal(false);
            if (Match(TokenType.TRUE)) return new Literal(true);
            if (Match(TokenType.NIL)) return new Literal(null);

            var tokenTypes = new TokenType[] { TokenType.NUMBER, TokenType.STRING};

            if (Match(tokenTypes))
            {
                return new Literal(Previous().Literal);
            }

            if (Match(TokenType.LEFT_PAREN))
            {
                Expr expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
                return new Grouping(expr);
            }

            throw Error(Peek(), "Expect expression.");
        }

        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();

            throw Error(Peek(), message);
        }

        private ParseException Error(Token token, string message)
        {
            Program.Error(token, message);
            return new ParseException();
        }

        private class ParseException : Exception
        {
            public ParseException()
            {

            }
        }

        private void Synchronize()
        {
            Advance();

            while(!IsAtEnd())
            {
                if (Previous().Type == TokenType.SEMICOLON) return;

                switch (Peek().Type)
                {
                    case TokenType.CLASS:
                    case TokenType.FUN:
                    case TokenType.VAR:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;
                }

                Advance();
            }
        }
    }
}