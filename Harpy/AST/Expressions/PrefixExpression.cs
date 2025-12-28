using Harpy.Lexer;

namespace Harpy.AST.Expressions;

/// <summary>
///     A prefix unary arithmetic expression like <c>!a</c> or <c>-b</c>.
/// </summary>
public class PrefixExpression : Expression
{
    private readonly HarbourSyntaxTokenNode _operatorNode;
    private readonly Expression _right;

    /// <summary>
    ///     A prefix unary arithmetic expression like <c>!a</c> or <c>-b</c>.
    /// </summary>
    public PrefixExpression(HarbourSyntaxToken @operator, Expression right) : base(false, [])
    {
        _right = right;

        _operatorNode = new HarbourSyntaxTokenNode(@operator, []);
        _operatorNode.Parent = this;
        Children.Add(_operatorNode);

        _right.Parent = this;
        Children.Add(_right);
    }

    public override string PrettyPrint(int indent = 0)
    {
        return NodeLine(indent) +
               "PrefixExpression(\n" +
               BlankLine(indent + 1) + "operator\n" +
               ChildNodeLine(indent + 1) +
               $"{_operatorNode.PrettyPrint(indent + 2)}\n" +
               BlankLine(indent + 1) + "right\n" +
               ChildNodeLine(indent + 1) +
               $"{_right.PrettyPrint(indent + 2)}\n" +
               BlankLine(indent) + ")";
    }
}