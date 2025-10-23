using Harpy.AST.Expressions;
using Harpy.Lexer;

namespace Harpy.Parser.SubParsers;

/// <summary>
///     Parses assignment expressions like <c>a := b</c>. The left side of an assignment
///     expression must be an expression where <c>left.left_expr() == True</c> and expressions
///     are right-associative. (In other words, <c>a := b := c</c> is parsed as <c>a := (b := c)</c>).
/// </summary>
public class AssignmentParser : IInfixSubParser
{
    public Expression Parse(ExpressionParser parser, Expression left, HarbourSyntaxToken token)
    {
        var right = parser.Parse(GetPrecedence() - 1);

        return right == null
            ? throw new InvalidSyntaxException(
                $"Expected expression after assignment expression '{left.PrettyPrint()}' after token '{token.Text}' on line {token.Line}, column {token.Start}, found null.")
            : new AssignmentExpression(left, right);
    }

    public Precedence GetPrecedence()
    {
        return Precedence.ASSIGNMENT;
    }
}