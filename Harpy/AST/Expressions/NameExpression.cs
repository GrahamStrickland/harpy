using Harpy.Lexer;

namespace Harpy.AST.Expressions;

/// <summary>
///     A simple variable name expression like <c>abc</c>.
/// </summary>
public class NameExpression : Expression
{
    private readonly HarbourSyntaxTokenNode _nameNode;

    /// <summary>
    ///     A simple variable name expression like <c>abc</c>.
    /// </summary>
    public NameExpression(HarbourSyntaxToken name) : base(false, [])
    {
        _nameNode = new HarbourSyntaxTokenNode(name, []);
        _nameNode.Parent = this;
        Children.Add(_nameNode);
    }

    public override string PrettyPrint(int indent = 0)
    {
        return NodeLine(indent) + "NameExpression(\n" + BlankLine(indent + 1) + "name\n" + ChildNodeLine(indent + 1) +
               _nameNode.PrettyPrint(indent + 2) + "\n" + BlankLine(indent) + ")";
    }
}