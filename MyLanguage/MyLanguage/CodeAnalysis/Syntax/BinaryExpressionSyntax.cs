namespace MyLanguage.CodeAnalysis.Syntax;

sealed class BinaryExpressionSyntax : ExpressionSyntax
{
    public ExpressionSyntax Left { get; }
    public SyntaxToken OperationToken { get; }
    public ExpressionSyntax Right { get; set; }

    public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operationToken, ExpressionSyntax right)
    {
        Left = left;
        OperationToken = operationToken;
        Right = right;
    }

    public override SyntaxKind Kind => SyntaxKind.BinaryExpression;
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Left;
        yield return OperationToken;
        yield return Right;
    }
}