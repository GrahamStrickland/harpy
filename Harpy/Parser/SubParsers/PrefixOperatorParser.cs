using Harpy.AST.Expressions;
using Harpy.Lexer;
using Expression = Harpy.AST.Expressions.Expression;

namespace Harpy.Parser.SubParsers;

/// <summary>
///     Generic prefix parser for a unary arithmetic operator.
/// </summary>
public class PrefixOperatorParser(Precedence precedence) : IPrefixSubParser
{
    /// <summary>
    ///     To handle right-associative operators like <c>^</c>, we allow a slightly
    ///     lower precedence when parsing the right-hand side. This will let a
    ///     parser with the same precedence appear on the right, which will then
    ///     take *this* parser's result as its left-hand argument.
    /// </summary>
    /// <param name="parser">The parent parser for this expression.</param>
    /// <param name="token">Token before right expression.</param>
    /// <returns>An instance of <see cref="PrefixExpression" /></returns>
    public Expression Parse(ExpressionParser parser, HarbourSyntaxToken token)
    {
        var right = parser.Parse(precedence);

        return new PrefixExpression(token, right ?? throw new InvalidSyntaxException(
            $"Expected expression after token '{token.Text}' on line {token.Line}, column {token.Start}, found null."));
    }

    public Precedence GetPrecedence()
    {
        return precedence;
    }
}