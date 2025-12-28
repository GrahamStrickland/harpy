using Harpy.Lexer;

namespace Harpy.AST.Expressions;

/// <summary>
///     A boolean, numeric, string, or nil literal, e.g., <c>.t.</c>, <c>123</c>, <c>'hello'</c>, or <c>NIL</c>.
/// </summary>
public class LiteralExpression : Expression
{
    private readonly HarbourSyntaxTokenNode _literalNode;

    /// <summary>
    ///     A boolean, numeric, string, or nil literal, e.g., <c>.t.</c>, <c>123</c>, <c>'hello'</c>, or <c>NIL</c>.
    /// </summary>
    public LiteralExpression(HarbourSyntaxToken literal) : base(false, [])
    {
        _literalNode = new HarbourSyntaxTokenNode(literal, [])
        {
            Parent = this
        };
        Children.Add(_literalNode);
    }

    public override string PrettyPrint(int indent = 0)
    {
        return NodeLine(indent) + "LiteralExpression(\n" + BlankLine(indent + 1) + $"literal(type={_literalNode.token.Literal()})\n" +
               ChildNodeLine(indent + 1) + _literalNode.PrettyPrint(indent + 2) + "\n" + BlankLine(indent) + ")";
    }
}
