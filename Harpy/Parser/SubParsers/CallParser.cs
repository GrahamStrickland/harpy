using Harpy.AST.Expressions;
using Harpy.Lexer;
using Expression = Harpy.AST.Expressions.Expression;

namespace Harpy.Parser.SubParsers;

/// <summary>
///     Parser to parse a function call like <c>a(b, c, d)</c>.
/// </summary>
public class CallParser : IInfixSubParser
{
    public Expression Parse(ExpressionParser parser, Expression left, HarbourSyntaxToken token)
    {
        // Parse the comma-separated arguments (possibly implicitly nil) until we hit ")".
        List<Expression?> arguments = [];

        if (!parser.Match(HarbourSyntaxKind.RIGHT_PAREN))
        {
            arguments.Add(parser.Match(HarbourSyntaxKind.COMMA) ? null : parser.Parse());
            while (parser.Match(HarbourSyntaxKind.COMMA))
            {
                parser.Consume(HarbourSyntaxKind.COMMA);
                arguments.Add(parser.Match(HarbourSyntaxKind.COMMA) ? null : parser.Parse());
            }
        }

        parser.Consume(HarbourSyntaxKind.RIGHT_PAREN);

        return new CallExpression(left, arguments);
    }

    public Precedence GetPrecedence()
    {
        return Precedence.CALL;
    }
}
