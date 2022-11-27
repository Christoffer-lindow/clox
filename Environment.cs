using System.Collections.Generic;

namespace App
{

    public class Environment
    {
        public Environment Enclosing;
        private Dictionary<string, object> values = new();

        public Environment()
        {
            Enclosing = null;
        }

        public Environment(Environment enclosing)
        {
            Enclosing = enclosing;
        }

        public void Define(string name, object value)
        {
            values.Add(name, value);
        }

        public void Assign(Token name, object value)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                values.Add(name.Lexeme, value);
                return;
            }

            if (Enclosing is not null)
            {
                Enclosing.Assign(name, value);
                return;
            }

            throw new RuntimeException(name, $"Undefined variable '{name.Lexeme}'.");
        }

        public object Get(Token name)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                return values.GetValueOrDefault(name.Lexeme);
            }

            if (Enclosing is not null) return Enclosing.Get(name);

            throw new RuntimeException(name, $"Undefined variable '{name.Lexeme}'.");
        }
    }

}
