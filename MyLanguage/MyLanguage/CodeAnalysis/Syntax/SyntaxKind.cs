namespace MyLanguage.CodeAnalysis.Syntax;

enum SyntaxKind
{
    //Tokens
    NumberToken,
    WhitespaceToken,
    PlusToken,
    MinusToken,
    StarToken,
    SlashToken,
    OpenParenthesisToken,
    CloseParenthesisToken,
    BadToken,
    EndOfFileToken,
    
    // Expressions
    LiteralExpression,
    BinaryExpression,
    ParenthesizedExpression
}