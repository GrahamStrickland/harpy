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
        // Parse the comma-separated arguments until we hit ")".
        List<Expression> arguments = [];

        if (!parser.Match(HarbourSyntaxKind.RIGHT_PAREN))
        {
            arguments.Add(parser.Parse() ??
                          throw new InvalidSyntaxException(
                              $"Expected expression after left expression '{left.PrettyPrint()}' after token '{token.Text}' on line {token.Line}, column {token.Start}, found null."));
            while (parser.Match(HarbourSyntaxKind.COMMA))
            {
                parser.Consume(HarbourSyntaxKind.COMMA);
                arguments.Add(parser.Parse() ??
                              throw new InvalidSyntaxException(
                                  $"Expected expression after left expression '{left.PrettyPrint()}' after token '{token.Text}' on line {token.Line}, column {token.Start}, found null."));
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