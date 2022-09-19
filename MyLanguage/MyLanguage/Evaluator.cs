using MyLanguage.CodeAnalysis;
using MyLanguage.CodeAnalysis.Syntax;

namespace MyLanguage;

class Evaluator
{
    private readonly ExpressionSyntax _root;

    public Evaluator(ExpressionSyntax root)
    {
        _root = root;
    }

    public int Evaluate()
    {
        return EvaluateExpression(_root);
    }

    private int EvaluateExpression(ExpressionSyntax node)
    {
        //BinaryExpression
        //NumberExpression

        if (node is LiteralExpressionSyntax numberExpressionSyntax)
            return (int) numberExpressionSyntax.NumberToken.Value;

        if (node is BinaryExpressionSyntax binaryExpressionSyntax)
        {
            var left = EvaluateExpression(binaryExpressionSyntax.Left);
            var right = EvaluateExpression(binaryExpressionSyntax.Right);

            if (binaryExpressionSyntax.OperationToken.Kind == SyntaxKind.PlusToken)
                return left + right;
            else if (binaryExpressionSyntax.OperationToken.Kind == SyntaxKind.MinusToken)
                return left - right;
            else if (binaryExpressionSyntax.OperationToken.Kind == SyntaxKind.StarToken)
                return left * right;
            else if (binaryExpressionSyntax.OperationToken.Kind == SyntaxKind.SlashToken)
                return left / right;
            else
                throw new Exception("Unexpected binary operator");
        }

        if (node is ParenthesizedExpressionSyntax parenthesized)
            return EvaluateExpression(parenthesized.Expression);
        
        throw new Exception($"Unexpected node: {node.Kind}");
    }
}