using Harpy.AST.Expressions;
using Harpy.Lexer;

namespace Harpy.Parser.SubParsers;

/// <summary>
///     Parser to parse an array declaration like <c>{}</c> or <c>{a}</c> or a hash declaration like
///     <c>{ => }</c> or <c>{ "a" => 1 }</c>.
/// </summary>
public class ContainerDeclarationParser : IPrefixSubParser
{
    public Expression Parse(ExpressionParser parser, HarbourSyntaxToken token)
    {
        List<Expression> elements = [];
        Dictionary<Expression, Expression> keyValuePairs = [];

        if (parser.Match(HarbourSyntaxKind.RIGHT_BRACE))
        {
            parser.Consume(HarbourSyntaxKind.RIGHT_BRACE);
            return new ArrayDeclarationExpression(elements);
        }

        if (parser.Match(HarbourSyntaxKind.HASHOP))
        {
            parser.Consume(HarbourSyntaxKind.HASHOP);
            parser.Consume(HarbourSyntaxKind.RIGHT_BRACE);
            return new HashDeclarationExpression(keyValuePairs);
        }

        var firstExpression = parser.Parse() ?? throw new InvalidSyntaxException(
            $"Expected expression after declaration expression with first token '{token.Text}' on line {token.Line}, column {token.Start}, found null.");
        {
            if (parser.Match(HarbourSyntaxKind.HASHOP))
            {
                parser.Consume(HarbourSyntaxKind.HASHOP);
                var v = parser.Parse();
                keyValuePairs[firstExpression] = v ?? throw new InvalidSyntaxException(
                    $"Expected expression after hash declaration expression with left expression {firstExpression.PrettyPrint()}, found null.");
                while (parser.Match(HarbourSyntaxKind.COMMA))
                {
                    parser.Consume(HarbourSyntaxKind.COMMA);

                    var k = parser.Parse();
                    if (k == null)
                        throw new InvalidSyntaxException(
                            $"Expected expression after declaration expression with first token '{token.Text}' on line {token.Line}, column {token.Start}, found null.");
                    parser.Consume(HarbourSyntaxKind.HASHOP);
                    v = parser.Parse();

                    keyValuePairs[k] = v ?? throw new InvalidSyntaxException(
                        $"Expected expression after hash declaration expression with left expression {k.PrettyPrint()}, found null.");
                }
            }
            else
            {
                elements.Add(firstExpression);

                while (parser.Match(HarbourSyntaxKind.COMMA))
                {
                    parser.Consume(HarbourSyntaxKind.COMMA);

                    elements.Add(parser.Parse() ?? throw new InvalidSyntaxException(
                        $"Expected expression after declaration expression with first token '{token.Text}' on line {token.Line}, column {token.Start}, found null."));
                }
            }
        }

        parser.Consume(HarbourSyntaxKind.RIGHT_BRACE);

        if (elements.Count > 0) return new ArrayDeclarationExpression(elements);

        return keyValuePairs.Count > 0
            ? new HashDeclarationExpression(keyValuePairs)
            : throw new InvalidSyntaxException(
                $"Unable to parse container declaration with first token '{token.Text}' on line {token.Line}, column {token.Start}, found null.");
    }

    public Precedence GetPrecedence()
    {
        return Precedence.NONE;
    }
}