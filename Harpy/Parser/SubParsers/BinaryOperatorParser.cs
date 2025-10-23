using Harpy.AST.Expressions;
using Harpy.Lexer;
using Expression = Harpy.AST.Expressions.Expression;

namespace Harpy.Parser.SubParsers;

/// <summary>
///     Generic infix parser for a binary arithmetic operator. The only
///     difference when parsing, <c>+</c>, <c>-</c>, <c>*</c>, <c>/</c>, and
///     <c>^</c> is precedence and associativity, so we can use a single
///     parser class for all of those.
/// </summary>
public class BinaryOperatorParser(Precedence precedence, bool isRight) : IInfixSubParser
{
    /// <summary>
    ///     To handle right-associative operators like <c>^</c>, we allow a slightly
    ///     lower precedence when parsing the right-hand side. This will let a
    ///     parser with the same precedence appear on the right, which will then
    ///     take *this* parser's result as its left-hand argument.
    /// </summary>
    /// <param name="parser">The parent parser for this expression.</param>
    /// <param name="left">Left expression.</param>
    /// <param name="token">Token between left and right expressions.</param>
    /// <returns>An instance of <see cref="OperatorExpression" />.</returns>
    public Expression Parse(ExpressionParser parser, Expression left, HarbourSyntaxToken token)
    {
        var right = parser.Parse(GetPrecedence() - (isRight ? 1 : 0));

        return new OperatorExpression(left, token,
            right ?? throw new InvalidSyntaxException(
                $"Operator expression '{left.PrettyPrint()} {token.Text}' at line {token.Line}, column {token.Start} missing right-hand token."));
    }

    public Precedence GetPrecedence()
    {
        return precedence;
    }
}