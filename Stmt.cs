using System.Collections.Generic;
namespace App {
	public abstract class Stmt {
		public abstract T accept<T>(Visitor<T> visitor);
	public interface Visitor<T> {
		T visitExpressionStmt (Expression stmt );
		T visitPrintStmt (Print stmt );
	}
	public class Expression: Stmt{
		public Expr expression;
		public Expression(Expr expression) {
			this.expression = expression;
		}
		public override T accept<T>(Visitor<T> visitor) {
			 return visitor.visitExpressionStmt(this);
		}
	}

	public class Print: Stmt{
		public Expr expression;
		public Print(Expr expression) {
			this.expression = expression;
		}
		public override T accept<T>(Visitor<T> visitor) {
			 return visitor.visitPrintStmt(this);
		}
	}

	}
}
