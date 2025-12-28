using Harpy.AST.Expressions;
using Harpy.Lexer;

namespace Harpy.Parser.SubParsers;

/// <summary>
///     Simple parser for a named variable like <c>abc</c>.
/// </summary>
public class NameParser : IPrefixSubParser
{
    public Expression Parse(ExpressionParser parser, HarbourSyntaxToken token)
    {
        return new NameExpression(token);
    }

    public Precedence GetPrecedence()
    {
        return Precedence.NONE;
    }
}