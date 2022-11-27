using System;
using System.Collections.Generic;
using System.IO;

namespace App
{
    public class GenerateAst
    {
        public static void Run()
        {
            var args = System.Environment.GetCommandLineArgs();
            if (args.Length != 2)
            {
                Console.Error.WriteLine("Usage: generate_ast <output directory>");
                System.Environment.Exit(3);
            }
            DefineAst(Directory.GetCurrentDirectory(), "Expr", new List<string>{
                "Assign   : Token Name, Expr value",
                "Binary   : Expr Left, Token Op, Expr Right",
                "Grouping : Expr Expression",
                "Literal  : object Value",
                "Logical  : Expr Left, Token Op, Expr Right",
                "Unary    : Token Op, Expr Right",
                "Variable : Token Name"
            });
            DefineAst(Directory.GetCurrentDirectory(), "Stmt", new List<string>{
                "Block      : List<Stmt> Statements",
                "Expression : Expr expression",
                "If         : Expr Condition, Stmt ThenBrach, Stmt ElseBranch",
                "Print      : Expr expression",
                "Var        : Token Name, Expr Initializer",
                "While      : Expr Condition, Stmt Body"
            });
        }

        private static void DefineAst(string outputDir, string baseName, List<string> types)
        {
            var path = Path.Join(outputDir, $"{baseName}.cs");
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine("using System.Collections.Generic;");
                sw.WriteLine("namespace App {");
                sw.WriteLine("\tpublic abstract class " + baseName + " {");
                sw.WriteLine("\t\tpublic abstract T accept<T>(Visitor<T> visitor);");
                DefineVisitor(sw, baseName, types);

                types.ForEach(type =>
                {
                    var typeArr = type.Split(':');
                    var className = typeArr[0].Trim();
                    var fields = typeArr[1].Trim();
                    DefineType(sw, baseName, className, fields);
                    sw.WriteLine();
                });
                sw.WriteLine("\t}");
                sw.WriteLine("}");
            }
        }

        private static void DefineType(StreamWriter sw, string baseName, string className, string fields)
        {
            var stringFields = fields.Split(", ");
            sw.WriteLine($"\tpublic class {className}: {baseName}" + "{");
            foreach (var field in stringFields)
            {
                sw.WriteLine($"\t\tpublic {field};");
            }
            sw.WriteLine($"\t\tpublic {className}({fields}) " + "{");
            foreach (var field in stringFields)
            {
                var name = field.Split(" ")[1];
                sw.WriteLine($"\t\t\tthis.{name} = {name};");
            }
            sw.WriteLine("\t\t}");
            sw.WriteLine("\t\tpublic override T accept<T>(Visitor<T> visitor) {");
            sw.WriteLine($"\t\t\t return visitor.visit{className}{baseName}(this);");
            sw.WriteLine("\t\t}");
            sw.WriteLine("\t}");
        }

        private static void DefineVisitor(StreamWriter sw, string baseName, List<string> types)
        {
            sw.WriteLine("\tpublic interface Visitor<T> {");
            foreach (var type in types)
            {
                var typeArr = type.Split(':');
                var className = typeArr[0].Trim();
                sw.WriteLine($"\t\tT visit{className}{baseName} ({className} {baseName.ToLower()} );");
            }
            sw.WriteLine("\t}");
        }

    }
}