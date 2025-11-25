using Harpy.Lexer;

namespace Harpy.AST.Expressions;

/// <summary>
///     A boolean, numeric, string, or nil literal, e.g., <c>.t.</c>, <c>123</c>, <c>'hello'</c>, or <c>NIL</c>.
/// </summary>
public class LiteralExpression : Expression
{
    private readonly HarbourSyntaxToken _literal;

    /// <summary>
    ///     A boolean, numeric, string, or nil literal, e.g., <c>.t.</c>, <c>123</c>, <c>'hello'</c>, or <c>NIL</c>.
    /// </summary>
    public LiteralExpression(HarbourSyntaxToken literal) : base(false, [])
    {
        _literal = literal;

        var literalNode = new HarbourSyntaxTokenNode(literal, []);
        {
            Parent = this;
        }
        Children.Add(literalNode);
    }

    public override string PrettyPrint()
    {
        return _literal.Text;
    }
}