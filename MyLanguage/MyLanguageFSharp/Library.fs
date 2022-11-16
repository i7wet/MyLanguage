namespace MyLanguageFSharp

open System.Collections.Generic


module Lexer =
    
    let lex (inputString : string) : IEnumerable<SyntaxToken> =
        let stringWithEnd = inputString + "\0"
        let result : List<SyntaxToken> = new List<SyntaxToken>
        let current = stringWithEnd.ToCharArray ()
        while current is not '0' do
            