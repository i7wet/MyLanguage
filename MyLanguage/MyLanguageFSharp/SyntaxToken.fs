namespace MyLanguageFSharp

type SyntaxToken (value : string, kind : SyntaxKind) = 

    member val Value = value with get
    member val Kind = kind with get