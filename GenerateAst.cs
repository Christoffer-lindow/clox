using System;
using System.Collections.Generic;
using System.IO;

namespace App
{
    public class GenerateAst
    {
        public static void Run()
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length != 2)
            {
                Console.Error.WriteLine("Usage: generate_ast <output directory>");
                Environment.Exit(3);
            }
            DefineAst(Directory.GetCurrentDirectory(), "Expr", new List<string>{
            "Binary   : Expr left, Token op, Expr right",
            "Grouping : Expr expression",
            "Literal  : object value",
            "Unary    : Token op, Expr right"
        });
        }

        private static void DefineAst(string outputDir, string baseName, List<string> types)
        {
            var path = Path.Join(outputDir, $"{baseName}.cs");
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine("using System.Collections.Generic;");
                sw.WriteLine("namespace App {");
                sw.WriteLine("\tabstract class " + baseName + " {}");
                types.ForEach(type =>
                {
                    var typeArr = type.Split(':');
                    var className = typeArr[0].Trim();
                    var fields = typeArr[1].Trim();
                    DefineType(sw, baseName, className, fields);
                    sw.WriteLine();
                });
                sw.WriteLine("}");
            }
        }

        private static void DefineType(StreamWriter sw, string baseName, string className, string fields)
        {
            var stringFields = fields.Split(", ");
            sw.WriteLine($"\tclass {className}: {baseName}" + "{");
            foreach (var field in stringFields)
            {
                sw.WriteLine($"\t\t{field};");
            }
            sw.WriteLine($"\t\t{className}({fields}) " + "{");
            foreach (var field in stringFields)
            {
                var name = field.Split(" ")[1];
                sw.WriteLine($"\t\t\tthis.{name} = {name};");
            }
            sw.WriteLine("\t\t}");
            sw.WriteLine("\t}");
        }

    }
}