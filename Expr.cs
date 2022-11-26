using System.Collections.Generic;
namespace App {
	public abstract class Expr {
		public abstract T accept<T>(Visitor<T> visitor);
	public interface Visitor<T> {
		T visitBinaryExpr (Binary expr );
		T visitGroupingExpr (Grouping expr );
		T visitLiteralExpr (Literal expr );
		T visitUnaryExpr (Unary expr );
	}
	public class Binary: Expr{
		public Expr Left;
		public Token op;
		public Expr Right;
		public Binary(Expr Left, Token op, Expr Right) {
			this.Left = Left;
			this.op = op;
			this.Right = Right;
		}
		public override T accept<T>(Visitor<T> visitor) {
			 return visitor.visitBinaryExpr(this);
		}
	}

	public class Grouping: Expr{
		public Expr Expression;
		public Grouping(Expr Expression) {
			this.Expression = Expression;
		}
		public override T accept<T>(Visitor<T> visitor) {
			 return visitor.visitGroupingExpr(this);
		}
	}

	public class Literal: Expr{
		public object Value;
		public Literal(object Value) {
			this.Value = Value;
		}
		public override T accept<T>(Visitor<T> visitor) {
			 return visitor.visitLiteralExpr(this);
		}
	}

	public class Unary: Expr{
		public Token Op;
		public Expr Right;
		public Unary(Token Op, Expr Right) {
			this.Op = Op;
			this.Right = Right;
		}
		public override T accept<T>(Visitor<T> visitor) {
			 return visitor.visitUnaryExpr(this);
		}
	}

	}
}
