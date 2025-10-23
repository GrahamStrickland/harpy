using Harpy.Lexer;

namespace Harpy.AST.Expressions;

/// <summary>
///     A boolean, numeric, string, or nil literal, e.g., <c>.t.</c>, <c>123</c>, <c>'hello'</c>, or <c>NIL</c>.
/// </summary>
public class LiteralExpression(HarbourSyntaxToken literal) : Expression(false)
{
    public override IHarbourAstNode? Parent { get; set; }

    public override string PrettyPrint()
    {
        return literal.Text;
    }

    public override void Walk()
    {
        Console.WriteLine(PrettyPrint());
    }
}