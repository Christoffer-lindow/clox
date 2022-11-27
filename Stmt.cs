using System.Collections.Generic;
namespace App {
	public abstract class Stmt {
		public abstract T accept<T>(Visitor<T> visitor);
	public interface Visitor<T> {
		T visitBlockStmt (Block stmt );
		T visitExpressionStmt (Expression stmt );
		T visitIfStmt (If stmt );
		T visitPrintStmt (Print stmt );
		T visitVarStmt (Var stmt );
	}
	public class Block: Stmt{
		public List<Stmt> Statements;
		public Block(List<Stmt> Statements) {
			this.Statements = Statements;
		}
		public override T accept<T>(Visitor<T> visitor) {
			 return visitor.visitBlockStmt(this);
		}
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

	public class If: Stmt{
		public Expr Condition;
		public Stmt ThenBrach;
		public Stmt ElseBranch;
		public If(Expr Condition, Stmt ThenBrach, Stmt ElseBranch) {
			this.Condition = Condition;
			this.ThenBrach = ThenBrach;
			this.ElseBranch = ElseBranch;
		}
		public override T accept<T>(Visitor<T> visitor) {
			 return visitor.visitIfStmt(this);
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

	public class Var: Stmt{
		public Token Name;
		public Expr Initializer;
		public Var(Token Name, Expr Initializer) {
			this.Name = Name;
			this.Initializer = Initializer;
		}
		public override T accept<T>(Visitor<T> visitor) {
			 return visitor.visitVarStmt(this);
		}
	}

	}
}
