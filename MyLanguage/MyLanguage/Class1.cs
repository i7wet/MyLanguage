using System.Data;
using MyLanguage.CodeAnalysis;
using MyLanguage.CodeAnalysis.Syntax;

namespace MyLanguage;

public class Class1
{
    public void Start()
    {
        var showTree = false;
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
                return;

            if (line == "#showTree")
            {
                showTree = !showTree;
                Console.WriteLine(showTree ? "Showing parse trees" : "Not showing parse trees");
                continue;
            }
            else if (line == "#cls")
            {
                Console.Clear();
                continue;
            }
            
            var syntaxTree = SyntaxTree.Parse(line);
            if (showTree)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                PrettyPrint(syntaxTree.Root);
                Console.ResetColor();
            }

            if (syntaxTree.Diagnostics.Any())
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkRed;
                foreach (var diagnostic in syntaxTree.Diagnostics)
                    Console.WriteLine(diagnostic);
                Console.ResetColor();
            }
            else
            {
                var evaluator = new Evaluator(syntaxTree.Root);
                var result = evaluator.Evaluate();
                Console.WriteLine(result);
            }
        }
    }

    static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = true)
    {//"├─ └ │"
        if (isLast)
            Console.Write($"{indent}└─{node.Kind}");
        else
            Console.Write($"{indent}├─{node.Kind}");
        
        if(node is SyntaxToken { Value: { } } token)
            Console.Write($" {token.Value}");
        Console.WriteLine();
        
        if (indent != "" && !isLast)
            indent += "│  ";
        else
            indent += "   ";
        
        foreach (var childNode in node.GetChildren())
        {
            
            if(childNode == node.GetChildren().Last())
                PrettyPrint(childNode, indent);
            else 
                PrettyPrint(childNode, indent, false);
        }
    }
}