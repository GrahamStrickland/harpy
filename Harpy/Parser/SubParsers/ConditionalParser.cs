using Harpy.Lexer;
using Expression = Harpy.AST.Expressions.Expression;
using ConditionalExpression = Harpy.AST.Expressions.ConditionalExpression;

namespace Harpy.Parser.SubParsers;

/// <summary>
///     Parser for the conditional or 'ternary' operator, like <c>iif(a, b, c)</c>.
/// </summary>
public class ConditionalParser : IPrefixSubParser
{
    public Expression Parse(ExpressionParser parser, HarbourSyntaxToken token)
    {
        parser.Consume(HarbourSyntaxKind.LEFT_PAREN);

        var ifArm = parser.Parse() ?? throw new InvalidSyntaxException(
            $"Expected expression after conditional expression with first token '{token.Text}' on line {token.Line}, column {token.Start}, found null.");
        parser.Consume(HarbourSyntaxKind.COMMA);

        var thenArm = parser.Match(HarbourSyntaxKind.COMMA) ? null : parser.Parse();
        parser.Consume(HarbourSyntaxKind.COMMA);

        var elseArm = parser.Match(HarbourSyntaxKind.RIGHT_PAREN) ? null : parser.Parse();
        parser.Consume(HarbourSyntaxKind.RIGHT_PAREN);

        return new ConditionalExpression(ifArm, thenArm, elseArm);
    }

    public Precedence GetPrecedence()
    {
        return Precedence.NONE;
    }
}