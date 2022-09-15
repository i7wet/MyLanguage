namespace MyLanguage.CodeAnalysis.Syntax;

class SyntaxToken : SyntaxNode
{
    public override SyntaxKind Kind { get; }
    public int Position { get; }
    public string Text { get; }
    public object? Value { get; }

    public SyntaxToken(SyntaxKind kind, int position, string text, object? value)
    {
        Kind = kind;
        Text = text;
        Value = value;
        Position = position;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Enumerable.Empty<SyntaxNode>();
    }
}