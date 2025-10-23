using Harpy.AST.Expressions;
using Harpy.Lexer;

namespace Harpy.Parser.SubParsers;

/// <summary>
///     Parses parentheses used to group an expression, like <c>a * (b + c)</c>.
/// </summary>
public class GroupParser : IPrefixSubParser
{
    public Expression Parse(ExpressionParser parser, HarbourSyntaxToken token)
    {
        var expression = parser.Parse();
        parser.Consume(HarbourSyntaxKind.RIGHT_PAREN);

        return expression ?? throw new InvalidSyntaxException(
            $"Unable to parse group expression '{expression}' with first token '{token.Text}' on line {token.Line}, column {token.Start}, found null.");
    }

    public Precedence GetPrecedence()
    {
        return Precedence.NONE;
    }
}