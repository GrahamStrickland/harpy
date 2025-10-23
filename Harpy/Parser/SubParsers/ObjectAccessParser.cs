using Harpy.AST.Expressions;
using Harpy.Lexer;
using Expression = Harpy.AST.Expressions.Expression;

namespace Harpy.Parser.SubParsers;

/// <summary>
///     Parser to parse an object method call or field access like <c>a:b(c)</c> or <c>a:b</c>.
/// </summary>
public class ObjectAccessParser : IInfixSubParser
{
    public Expression Parse(ExpressionParser parser, Expression left, HarbourSyntaxToken token)
    {
        var right = parser.Parse(GetPrecedence());

        return new ObjectAccessExpression(left,
            right ?? throw new InvalidSyntaxException(
                $"Expected expression after object access expression with left expression {left.PrettyPrint()} with first token '{token.Text}' on line {token.Line}, column {token.Start}, found null."));
    }

    public Precedence GetPrecedence()
    {
        return Precedence.ACCESS;
    }
}