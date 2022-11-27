using System.Collections.Generic;
namespace App {
	public abstract class Expr {
		public abstract T accept<T>(Visitor<T> visitor);
	public interface Visitor<T> {
		T visitAssignExpr (Assign expr );
		T visitBinaryExpr (Binary expr );
		T visitGroupingExpr (Grouping expr );
		T visitLiteralExpr (Literal expr );
		T visitUnaryExpr (Unary expr );
		T visitVariableExpr (Variable expr );
	}
	public class Assign: Expr{
		public Token Name;
		public Expr value;
		public Assign(Token Name, Expr value) {
			this.Name = Name;
			this.value = value;
		}
		public override T accept<T>(Visitor<T> visitor) {
			 return visitor.visitAssignExpr(this);
		}
	}

	public class Binary: Expr{
		public Expr Left;
		public Token Op;
		public Expr Right;
		public Binary(Expr Left, Token Op, Expr Right) {
			this.Left = Left;
			this.Op = Op;
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

	public class Variable: Expr{
		public Token Name;
		public Variable(Token Name) {
			this.Name = Name;
		}
		public override T accept<T>(Visitor<T> visitor) {
			 return visitor.visitVariableExpr(this);
		}
	}

	}
}
