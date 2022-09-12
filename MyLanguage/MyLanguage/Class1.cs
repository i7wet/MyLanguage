using System.Data;

namespace MyLanguage;

public class Class1
{
    public void Start()
    {
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
                return;

            var parser = new Parser(line);
            var syntaxTree = parser.Parse();
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            PrettyPrint(syntaxTree.Root);
            Console.ForegroundColor = color;

            if (syntaxTree.Diagnostics.Any())
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                foreach (var diagnostic in parser.Diagnostics)
                    Console.WriteLine(diagnostic);
                Console.ForegroundColor = color;
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

enum SyntaxKind
{
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
    NumberExpression,
    BinaryExpression
}

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

class Lexer
{
    private readonly string _text;
    private int _position;
    private List<string> _diagnostics = new List<string>();

    public IEnumerable<string> Diagnostics => _diagnostics;
    private char Current
    {
        get
        {
            if (_position >= _text.Length)
                return '\0';
            return _text[_position];

        }
    }

    public Lexer(string text)
    {
        _text = text;
    }

    private void Next()
    {
        _position++;
    }
    public SyntaxToken NextToken()
    {
        if (_position >= _text.Length)
            return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0", null);
        
        if (char.IsDigit(Current))
        {
            var start = _position;
            
            while(char.IsDigit(Current))
                Next();

            var length = _position - start;
            var text = _text.Substring(start, length);
            if (!(int.TryParse(text, out var value)))
            {
                _diagnostics.Add($"The number {_text} isn't valid Int32");
            }
            return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
        }

        if (char.IsWhiteSpace(Current))
        {
            var start = _position;
            
            while(char.IsWhiteSpace(Current))
                Next();

            var length = _position - start;
            var text = _text.Substring(start, length);
            object? value = null;
            return new SyntaxToken(SyntaxKind.WhitespaceToken, start, text, value);
        }

        if (Current == '+')
            return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null);
        else if (Current == '-')
            return new SyntaxToken(SyntaxKind.MinusToken, _position++, "-", null);
        else if (Current == '*')
            return new SyntaxToken(SyntaxKind.StarToken, _position++, "*", null);
        else if (Current == '/')
            return new SyntaxToken(SyntaxKind.SlashToken, _position++, "/", null);
        else if (Current == '(')
            return new SyntaxToken(SyntaxKind.OpenParenthesisToken, _position++, "(", null);
        else if (Current == ')')
            return new SyntaxToken(SyntaxKind.CloseParenthesisToken, _position++, ")", null);

        _diagnostics.Add($"ERROR: bad character input: '{Current}'");
        return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
    }
}

abstract class SyntaxNode
{
    public abstract SyntaxKind Kind { get; }
    public abstract IEnumerable<SyntaxNode> GetChildren();
}

abstract class ExpressionSyntax : SyntaxNode
{
    
}

sealed class NumberExpressionSyntax : ExpressionSyntax
{
    public NumberExpressionSyntax(SyntaxToken numberToken)
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

sealed class BinaryExpressionSyntax : ExpressionSyntax
{
    public ExpressionSyntax Left { get; }
    public SyntaxToken OperationToken { get; }
    public ExpressionSyntax Right { get; }

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

sealed class SyntaxTree
{
    public IReadOnlyList<string> Diagnostics { get; }
    public ExpressionSyntax Root { get; }
    public SyntaxToken EndOfFileToken { get; }

    public SyntaxTree(IEnumerable<string> diagnostics, ExpressionSyntax root, SyntaxToken endOfFileToken)
    {
        Diagnostics = diagnostics.ToArray();
        Root = root;
        EndOfFileToken = endOfFileToken;
    }
}

class Parser
{
    private readonly SyntaxToken[] _tokens;
    private int _position;
    private List<string> _diagnostics = new List<string>();

    public Parser(string text)
    {
        var tokens = new List<SyntaxToken>();
        
        var lexer = new Lexer(text);
        SyntaxToken token;
        do
        {
            token = lexer.NextToken();
            if (token.Kind != SyntaxKind.WhitespaceToken &&
                token.Kind != SyntaxKind.BadToken)
            {
                tokens.Add(token);
            }
        } while (token.Kind != SyntaxKind.EndOfFileToken);

        _tokens = tokens.ToArray();
        _diagnostics.AddRange(lexer.Diagnostics);
    }

    public IEnumerable<string> Diagnostics => _diagnostics;
    private SyntaxToken Peek(int offset)
    {
        var index = _position + offset;
        if (index >= _tokens.Length)
            return _tokens[_tokens.Length - 1];
        
        return _tokens[index];
    }

    private SyntaxToken Current => Peek(0);

    private SyntaxToken NextToken()
    {
        var current = Current;
        _position++;
        return current;
    }
    
    private SyntaxToken Match(SyntaxKind kind)
    {
        if (Current.Kind == kind)
            return NextToken();
        
        _diagnostics.Add($"ERROR: Unexpected tokens <{Current.Kind}>, expected<{kind}>");
        return new SyntaxToken(kind, Current.Position, null, null);
    }

    public SyntaxTree Parse()
    {
        var expression = ParseExpression();
        var endOfFileToken = Match(SyntaxKind.EndOfFileToken);
        return new SyntaxTree(_diagnostics, expression, endOfFileToken);
    }
    
    public ExpressionSyntax ParseExpression()
    {
        var left = ParsePrimaryExpression();
        
        while (Current.Kind == SyntaxKind.PlusToken
               || Current.Kind == SyntaxKind.MinusToken
               || Current.Kind == SyntaxKind.StarToken)
        {
            var operatorToken = NextToken();
            var right = ParsePrimaryExpression();
            left = new BinaryExpressionSyntax(left, operatorToken, right);
        }

        return left;
    }

    private ExpressionSyntax ParsePrimaryExpression()
    {
        var numberToken = Match(SyntaxKind.NumberToken);
        return new NumberExpressionSyntax(numberToken);
    }
}

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

        if (node is NumberExpressionSyntax numberExpressionSyntax)
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
        throw new Exception($"Unexpected node: {node.Kind}");
    }
}