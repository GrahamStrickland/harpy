using Harpy.AST.Expressions;
using Harpy.Lexer;
using Expression = Harpy.AST.Expressions.Expression;

namespace Harpy.Parser.SubParsers;

/// <summary>
///     Generic infix parser for a unary arithmetic operator.
/// </summary>
public class PostfixOperatorParser(Precedence precedence) : IInfixSubParser
{
    public Expression Parse(ExpressionParser parser, Expression left, HarbourSyntaxToken token)
    {
        return new PostfixExpression(left, token);
    }

    public Precedence GetPrecedence()
    {
        return precedence;
    }
}