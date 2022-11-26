using System;
namespace App
{
    public class RuntimeException: Exception
    {
        public Token Token;

        public RuntimeException(Token token, String message) : base(message)
        {
            Token = token;
        }
    }
}