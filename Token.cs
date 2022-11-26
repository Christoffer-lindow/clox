using System;

namespace App
{
        public enum TokenType
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

    public class Token
    {
        public TokenType Type;
        public String Lexeme;
        public Object Literal;
        public int Line;

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
}