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

        public List<Stmt> Parse()
        {
            var statements = new List<Stmt>();
            while (!IsAtEnd())
            {
                statements.Add(Declaration());
            }

            return statements;
        }

        private Stmt Declaration()
        {
            try 
            {
                if (Match(TokenType.VAR)) return VarDeclaration();

                return Statement();
            } catch (ParseException ex)
            {
                Synchronize();
                return null;
            }
        }

        private Stmt VarDeclaration()
        {
            Token name = Consume(TokenType.IDENTIFIER, "Expect variable name.");

            Expr initializer = null;

            if (Match(TokenType.EQUAL))
            {
                initializer = Expression();
            }

            Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration");
            return new Stmt.Var(name, initializer);
        }

        private Stmt Statement()
        {
            if (Match(TokenType.PRINT)) return PrintStatement();
            if (Match(TokenType.LEFT_BRACE)) return new Stmt.Block(Block());
            return ExpressionStatement();
        }

        private Stmt PrintStatement()
        {
            Expr value = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after value." );
            return new Stmt.Print(value);
        }

        private Stmt ExpressionStatement()
        {
            var expr = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after expression.");
            return new Stmt.Expression(expr);
        }

        private List<Stmt> Block()
        {
            List<Stmt> statements = new();

            while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd())
            {
                statements.Add(Declaration());
            }

            Consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
            return statements;
        }

        private Expr Expression()
        {
            return Assignment();
        }

        private Expr Assignment()
        {
            Expr expr = Equality();

            if (Match(TokenType.EQUAL))   
            {
                Token equals = Previous();
                Expr value = Assignment();

                if (typeof(Expr.Variable).IsInstanceOfType(value))
                {
                    Token name = ((Expr.Variable)expr).Name;
                    return new Expr.Assign(name, value);
                }

                Error(equals, "Invalid assignment target.");
            }
            return expr;
        }

        private Expr Equality()
        {
            Expr expr = Comparision();
            var tokenTypes = new TokenType[] { TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL};

            while (Match(tokenTypes))
            {
                Token op = Previous();
                Expr right = Comparision();
                expr = new Expr.Binary(expr, op, right);
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
                expr = new Expr.Binary(expr, op, right);
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
                expr = new Expr.Binary(expr, op, right);
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
                expr = new Expr.Binary(expr,op, right);
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
                return new Expr.Unary(op, right);
            }
            return Primary();
        }

        private Expr Primary()
        {
            if (Match(TokenType.FALSE)) return new Expr.Literal(false);
            if (Match(TokenType.TRUE)) return new Expr.Literal(true);
            if (Match(TokenType.NIL)) return new Expr.Literal(null);

            var tokenTypes = new TokenType[] { TokenType.NUMBER, TokenType.STRING};

            if (Match(tokenTypes))
            {
                return new Expr.Literal(Previous().Literal);
            }

            if (Match(TokenType.IDENTIFIER))
            {
                return new Expr.Variable(Previous());
            }

            if (Match(TokenType.LEFT_PAREN))
            {
                Expr expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
                return new Expr.Grouping(expr);
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