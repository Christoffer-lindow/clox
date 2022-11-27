using System;
using System.Collections.Generic;

namespace App
{
    class Interperter : Expr.Visitor<object>, Stmt.Visitor<object>
    {
        private Environment environment = new Environment();
        public void Interpret(List<Stmt> statements)
        {
            try {
                foreach (Stmt statement in statements)
                {
                    Execute(statement);
                }
            } catch (RuntimeException ex)
            {
                Program.RuntimeException(ex);
            }
        }

        private void Execute(Stmt stmt)
        {
            stmt.accept(this);
        }

        private void ExecuteBlock(List<Stmt> statements, Environment env)
        {
            var previous = environment;
            try {
                this.environment = env;
                foreach(var statement in statements)
                {
                    Execute(statement);
                }
            } finally {
                this.environment = previous;
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
        public object visitBinaryExpr(Expr.Binary expr)
        {
            object left = Evaluate(expr.Left);
            object right = Evaluate(expr.Right);

            switch (expr.Op.Type)
            {
                case TokenType.MINUS:
                    CheckNumberOperands(expr.Op, left, right);
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
                throw new RuntimeException(expr.Op, "Operand must be a number");
                case TokenType.SLASH:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left / (double)right;
                case TokenType.STAR:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left * (double)right;
                case TokenType.GREATER:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left >= (double)right;
                case TokenType.LESS:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left <= (double)right;
                case TokenType.BANG_EQUAL: return !IsEqual(left, right);
                case TokenType.EQUAL_EQUAL: return IsEqual(left, right);


            }

            // Unreachable
            return null;
        }

        public object visitGroupingExpr(Expr.Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        public object visitLiteralExpr(Expr.Literal expr)
        {
            return expr.Value;
        }

        public object visitUnaryExpr(Expr.Unary expr)
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

        public object visitExpressionStmt(Stmt.Expression stmt)
        {
            Evaluate(stmt.expression);
            return null;
        }

        public object visitPrintStmt(Stmt.Print stmt)
        {
            object value = Evaluate(stmt.expression);
            Console.WriteLine(Stringify(value));
            return null;
        }

        public object visitVariableExpr(Expr.Variable expr)
        {
            return environment.Get(expr.Name);
        }

        public object visitVarStmt(Stmt.Var stmt)
        {
            object val = null;

            if (stmt.Initializer != null)
            {
                val = Evaluate(stmt.Initializer);
            }

            environment.Define(stmt.Name.Lexeme, val);
            return null;
        }

        public object visitAssignExpr(Expr.Assign expr)
        {
            object val = Evaluate(expr.value);
            environment.Assign(expr.Name, val);
            return val;
        }

        public object visitBlockStmt(Stmt.Block stmt)
        {
            ExecuteBlock(stmt.Statements, new Environment(environment));
            return null;
        }

        public object visitIfStmt(Stmt.If stmt)
        {
            if (IsTruthy(Evaluate(stmt.Condition)))
            {
                Execute(stmt.ThenBrach);
            }
            else if (stmt.ElseBranch != null)
            {
                Execute(stmt.ElseBranch);
            }
            return null;
        }

        //FIXME: nil is truthy when it should be falsey
        public object visitLogicalExpr(Expr.Logical expr)
        {
            object left = Evaluate(expr.Left);

            if (expr.Op.Type == TokenType.OR)
            {
                if (IsTruthy(left)) return left;
            } 
            else 
            {
                if (!IsTruthy(left)) return left;
            }

            return Evaluate(expr.Right);
        }

        public object visitWhileStmt(Stmt.While stmt)
        {
            while (IsTruthy(Evaluate(stmt.Condition)))
            {
                Execute(stmt.Body);
            }
            return null;
        }
    }
}