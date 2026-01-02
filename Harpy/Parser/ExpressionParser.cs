using Harpy.AST.Expressions;
using Harpy.Lexer;
using Harpy.Parser.SubParsers;

namespace Harpy.Parser;

/// <summary>
///     Parser class with support for parsing the Harbour expression grammar.
/// </summary>
public class ExpressionParser
{
    private readonly Dictionary<HarbourSyntaxKind, IInfixSubParser> _infixParsers;
    private readonly Dictionary<HarbourSyntaxKind, IPrefixSubParser> _prefixParsers;
    private readonly SourceReader _reader;

    /// <summary>
    ///     Parser class with support for parsing the Harbour expression grammar.
    /// </summary>
    public ExpressionParser(SourceReader reader)
    {
        _reader = reader;
        _infixParsers = [];
        _prefixParsers = [];

        Register(HarbourSyntaxKind.NAME, new NameParser());
        Register(HarbourSyntaxKind.ASSIGN, new AssignmentParser());
        Register(HarbourSyntaxKind.PLUSEQ, new AssignmentParser());
        Register(HarbourSyntaxKind.MINUSEQ, new AssignmentParser());
        Register(HarbourSyntaxKind.MULTEQ, new AssignmentParser());
        Register(HarbourSyntaxKind.DIVEQ, new AssignmentParser());
        Register(HarbourSyntaxKind.MODEQ, new AssignmentParser());
        Register(HarbourSyntaxKind.EXPEQ, new AssignmentParser());
        Register(HarbourSyntaxKind.LEFT_PAREN, new GroupParser());
        Register(HarbourSyntaxKind.LEFT_PAREN, new CallParser());
        Register(HarbourSyntaxKind.LEFT_BRACKET, new IndexParser());
        Register(HarbourSyntaxKind.LEFT_BRACE, new ContainerDeclarationParser());
        Register(HarbourSyntaxKind.LEFT_BRACE, new CodeblockParser());
        Register(HarbourSyntaxKind.COLON, new ObjectAccessParser());
        Register(HarbourSyntaxKind.IIF, new ConditionalParser());

        Prefix(HarbourSyntaxKind.PLUS, Precedence.PREFIX);
        Prefix(HarbourSyntaxKind.MINUS, Precedence.PREFIX);
        Prefix(HarbourSyntaxKind.NOT, Precedence.PREFIX);
        Prefix(HarbourSyntaxKind.AT, Precedence.PREFIX);

        Prefix(HarbourSyntaxKind.PLUSPLUS, Precedence.PREFIX);
        Prefix(HarbourSyntaxKind.MINUSMINUS, Precedence.PREFIX);

        Postfix(HarbourSyntaxKind.PLUSPLUS, Precedence.POSTFIX);
        Postfix(HarbourSyntaxKind.MINUSMINUS, Precedence.POSTFIX);

        InfixRight(HarbourSyntaxKind.OR, Precedence.OR);
        InfixRight(HarbourSyntaxKind.AND, Precedence.AND);

        InfixLeft(HarbourSyntaxKind.EQ1, Precedence.EQRELATION);
        InfixLeft(HarbourSyntaxKind.EQ2, Precedence.EQRELATION);
        InfixLeft(HarbourSyntaxKind.NE1, Precedence.EQRELATION);
        InfixLeft(HarbourSyntaxKind.NE2, Precedence.EQRELATION);
        InfixLeft(HarbourSyntaxKind.LE, Precedence.ORDRELATION);
        InfixLeft(HarbourSyntaxKind.GE, Precedence.ORDRELATION);
        InfixLeft(HarbourSyntaxKind.LT, Precedence.ORDRELATION);
        InfixLeft(HarbourSyntaxKind.GT, Precedence.ORDRELATION);
        InfixLeft(HarbourSyntaxKind.DOLLAR, Precedence.ORDRELATION);

        InfixLeft(HarbourSyntaxKind.PLUS, Precedence.SUM);
        InfixLeft(HarbourSyntaxKind.MINUS, Precedence.SUM);
        InfixLeft(HarbourSyntaxKind.ASTERISK, Precedence.PRODUCT);
        InfixLeft(HarbourSyntaxKind.SLASH, Precedence.PRODUCT);
        InfixLeft(HarbourSyntaxKind.PERCENT, Precedence.PRODUCT);
        InfixRight(HarbourSyntaxKind.CARET, Precedence.EXPONENT);
    }

    public Expression? Parse(Precedence precedence = Precedence.NONE, bool optional = false, bool undo = false)
    {
        if (undo)
            _reader.SetUndoPoint();
        var token = _reader.LookAhead();

        if (token.Keyword() != null && token.Kind != HarbourSyntaxKind.NIL &&
            token.Kind != HarbourSyntaxKind.IIF) return null;

        token = _reader.Consume();

        Expression left;
        if (token.Literal() != null)
        {
            left = new LiteralExpression(token);
        }
        else
        {
            IPrefixSubParser? prefix;
            if (token.Kind == HarbourSyntaxKind.LEFT_BRACE)
            {
                var nextToken = _reader.LookAhead();

                if (nextToken.Kind == HarbourSyntaxKind.PIPE)
                    prefix = new CodeblockParser();
                else
                    prefix = new ContainerDeclarationParser();
            }
            else
            {
                prefix = _prefixParsers.GetValueOrDefault(token.Kind);
            }

            if (prefix is null)
                return !optional
                    ? throw new InvalidSyntaxException(
                        $"Could not parse token '{token.Text}' of type '{token.Kind}' on line {token.Line}, column {token.Start}."
                    )
                    : null;

            left = prefix.Parse(this, token);
        }

        while (precedence < GetPrecedence())
        {
            token = _reader.Consume();

            if (token.Literal() != null)
            {
                left = new LiteralExpression(_reader.Consume());
            }
            else
            {
                var infix = _infixParsers[token.Kind];
                left = infix.Parse(this, left, token);
            }
        }

        return left;
    }

    public bool Match(HarbourSyntaxKind expected)
    {
        return _reader.Match(expected);
    }

    public HarbourSyntaxToken Consume(HarbourSyntaxKind? expected = null)
    {
        return _reader.Consume(expected);
    }

    public void Unparse()
    {
        _reader.Undo();
    }

    private Precedence GetPrecedence()
    {
        var kind = _reader.LookAhead().Kind;

        var parser = _infixParsers.GetValueOrDefault(kind);

        return parser?.GetPrecedence() ?? Precedence.NONE;
    }

    /// <summary>
    ///     Registers a postfix unary operator parser for the given token and precedence.
    /// </summary>
    private void Postfix(HarbourSyntaxKind kind, Precedence precedence)
    {
        Register(kind, new PostfixOperatorParser(precedence));
    }

    /// <summary>
    ///     Registers a prefix unary operator parser for the given token and precedence.
    /// </summary>
    private void Prefix(HarbourSyntaxKind kind, Precedence precedence)
    {
        Register(kind, new PrefixOperatorParser(precedence));
    }

    /// <summary>
    ///     Registers a left-associative binary operator parser for the given token and precedence.
    /// </summary>
    private void InfixLeft(HarbourSyntaxKind kind, Precedence precedence)
    {
        Register(kind, new BinaryOperatorParser(precedence, false));
    }

    /// <summary>
    ///     Registers a right-associative binary operator parser for the given token and precedence.
    /// </summary>
    private void InfixRight(HarbourSyntaxKind kind, Precedence precedence)
    {
        Register(kind, new BinaryOperatorParser(precedence, true));
    }

    private void Register(HarbourSyntaxKind kind, ISubParser parser)
    {
        switch (parser)
        {
            case IInfixSubParser operatorParser:
                _infixParsers[kind] = operatorParser;
                break;
            case IPrefixSubParser operatorParser:
                _prefixParsers[kind] = operatorParser;
                break;
            default:
                throw new InvalidSyntaxException($"Attempt to register invalid parser {parser}.");
        }
    }
}
