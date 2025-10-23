using Harpy.Lexer;
using Expression = Harpy.AST.Expressions.Expression;
using IndexExpression = Harpy.AST.Expressions.IndexExpression;

namespace Harpy.Parser.SubParsers;

/// <summary>
///     Parser to parse an array index expression like <c>a[b]</c>.
/// </summary>
public class IndexParser : IInfixSubParser
{
    public Expression Parse(ExpressionParser parser, Expression left, HarbourSyntaxToken token)
    {
        var indexExpressions = new List<Expression>();

        if (!parser.Match(HarbourSyntaxKind.RIGHT_BRACKET))
            while (parser.Match(HarbourSyntaxKind.COMMA))
            {
                parser.Consume(HarbourSyntaxKind.COMMA);
                indexExpressions.Add(parser.Parse() ?? throw new InvalidSyntaxException(
                    $"Expected expression after index expression with left expression '{left.PrettyPrint()}' with first token '{token.Text}' on line {token.Line}, column {token.Start}, found null."));
            }

        parser.Consume(HarbourSyntaxKind.RIGHT_BRACKET);

        return new IndexExpression(left, indexExpressions);
    }

    public Precedence GetPrecedence()
    {
        return Precedence.INDEX;
    }
}