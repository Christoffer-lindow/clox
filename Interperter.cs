using System;
namespace App
{
    class Interperter : Visitor<object>
    {
        public void Interpret(Expr expression)
        {
            try {
                object value = Evaluate(expression);
                Console.WriteLine(Stringify(value));
            } catch (RuntimeException ex)
            {
            }
        }

        private string Stringify(object obj)
        {
            if (obj is null) return "nil";

            if (typeof(Double).IsInstanceOfType(obj))
            {
                var text = obj.ToString();
                if (text.EndsWith(".0"))
                {
                    text.Substring(0, text.Length - 2);
                }
                return text;
            }

            return obj.ToString();
        }
        public object visitBinaryExpr(Binary expr)
        {
            object left = Evaluate(expr.Left);
            object right = Evaluate(expr.Right);

            switch (expr.op.Type)
            {
                case TokenType.MINUS:
                    CheckNumberOperands(expr.op, left, right);
                    return (double)left - (double)right;
                case TokenType.PLUS:
                    if (typeof(Double).IsInstanceOfType(left) && typeof(Double).IsInstanceOfType(right))
                    {
                        return (double)left + (double)right;
                    }

                    if (typeof(String).IsInstanceOfType(left) && typeof(String).IsInstanceOfType(right))
                    {
                        return (String)left + (String)right;
                    }
                throw new RuntimeException(expr.op, "Operand must be a number");
                case TokenType.SLASH:
                    CheckNumberOperands(expr.op, left, right);
                    return (double)left / (double)right;
                case TokenType.STAR:
                    CheckNumberOperands(expr.op, left, right);
                    return (double)left * (double)right;
                case TokenType.GREATER:
                    CheckNumberOperands(expr.op, left, right);
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL:
                    CheckNumberOperands(expr.op, left, right);
                    return (double)left >= (double)right;
                case TokenType.LESS:
                    CheckNumberOperands(expr.op, left, right);
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL:
                    CheckNumberOperands(expr.op, left, right);
                    return (double)left <= (double)right;
                case TokenType.BANG_EQUAL: return !IsEqual(left, right);
                case TokenType.EQUAL_EQUAL: return IsEqual(left, right);


            }

            // Unreachable
            return null;
        }

        public object visitGroupingExpr(Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        public object visitLiteralExpr(Literal expr)
        {
            return expr.Value;
        }

        public object visitUnaryExpr(Unary expr)
        {
            object right = Evaluate(expr.Right);

            switch (expr.Op.Type)
            {
                case TokenType.BANG:
                    return !IsTruthy(right);
                case TokenType.MINUS:
                    CheckNumberOperand(expr.Op, right);
                    return -(double)right;
            }

            // unreachable
            return null;
        }

        private object Evaluate(Expr expr)
        {
            return expr.accept(this);
        }

        private bool IsTruthy(object obj)
        {
            if (obj == null) return true;
            if (typeof(Boolean).IsInstanceOfType(obj)) return (bool)obj;
            return true;
        }

        private bool IsEqual(object a, object b)
        {
            if (a == null && b == null) return true;
            if (a ==null) return false;

            return a.Equals(b);
        }
        
        private void CheckNumberOperand(Token op, object operand)
        {
            if (typeof(Double).IsInstanceOfType(operand)) return;
            throw new RuntimeException(op, "Operand must be a number");
        }

        private void CheckNumberOperands(Token op, object left, object right)
        {
            if (typeof(Double).IsInstanceOfType(left) && typeof(Double).IsInstanceOfType(right)) return;
            throw new RuntimeException(op, "Operand must be a number");
        }
    }
}