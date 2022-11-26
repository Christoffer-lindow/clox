using System.Collections.Generic;
using System.Text;

namespace App
{

    public class AstPrinter : Visitor<string>
    {
        public string visitBinaryExpr(Binary expr)
        {
            return Parenthesize(expr.op.Lexeme,
                                new List<Expr> { expr.Left, expr.Right });
        }

        public string visitGroupingExpr(Grouping expr)
        {
            return Parenthesize("group", new List<Expr> { expr.Expression });
        }

        public string visitLiteralExpr(Literal expr)
        {
            if (expr.Value == null) return "nil";
            return expr.Value.ToString();
        }

        public string visitUnaryExpr(Unary expr)
        {
            return Parenthesize(expr.Op.Lexeme, new List<Expr> { expr.Right });
        }

        public string Print(Expr expr)
        {
            return expr.accept(this);
        }

        private string Parenthesize(string name, List<Expr> exprs)
        {
            StringBuilder builder = new();
            builder.Append("(").Append(name);
            exprs.ForEach(expr =>
            {
                builder.Append(" ");
                builder.Append(expr.accept(this));
            });
            builder.Append(")");
            return builder.ToString();
        }
    }
}