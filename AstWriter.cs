using System.Collections.Generic;
using System.Text;

namespace App
{

    public class AstPrinter : Expr.Visitor<string>
    {
        public string visitBinaryExpr(Expr.Binary expr)
        {
            return Parenthesize(expr.op.Lexeme,
                                new List<Expr> { expr.Left, expr.Right });
        }

        public string visitGroupingExpr(Expr.Grouping expr)
        {
            return Parenthesize("group", new List<Expr> { expr.Expression });
        }

        public string visitLiteralExpr(Expr.Literal expr)
        {
            if (expr.Value == null) return "nil";
            return expr.Value.ToString();
        }

        public string visitUnaryExpr(Expr.Unary expr)
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