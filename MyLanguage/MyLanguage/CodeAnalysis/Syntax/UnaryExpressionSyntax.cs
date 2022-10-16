namespace MyLanguage.CodeAnalysis.Syntax;

sealed class UnaryExpressionSyntax : ExpressionSyntax
{
    public SyntaxToken OperationToken { get; }
    public ExpressionSyntax Operand { get; set; }

    public UnaryExpressionSyntax(SyntaxToken operationToken, ExpressionSyntax operand)
    {
        OperationToken = operationToken;
        Operand = operand;
    }

    public override SyntaxKind Kind => SyntaxKind.UnaryExpression;
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return OperationToken;
        yield return Operand;
    }
}