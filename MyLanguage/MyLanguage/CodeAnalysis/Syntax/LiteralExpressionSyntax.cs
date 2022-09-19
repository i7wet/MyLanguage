namespace MyLanguage.CodeAnalysis.Syntax;

sealed class LiteralExpressionSyntax : ExpressionSyntax
{
    public LiteralExpressionSyntax(SyntaxToken numberToken)
    {
        NumberToken = numberToken;
    }

    public override SyntaxKind Kind => SyntaxKind.NumberExpression;
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return NumberToken;
    }

    public SyntaxToken NumberToken { get; }
}