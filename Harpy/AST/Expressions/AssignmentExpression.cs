namespace Harpy.AST.Expressions;

/// <summary>
///     An assignment expression like <c>a := b</c>.
/// </summary>
public class AssignmentExpression : Expression
{
    private readonly Expression _left;
    private readonly Expression _right;

    /// <summary>
    ///     An assignment expression like <c>a := b</c>.
    /// </summary>
    public AssignmentExpression(Expression left, Expression right) : base(false, [])
    {
        _left = left;
        _right = right;

        _left.Parent = this;
        Children.Add(_left);
        _right.Parent = this;
        Children.Add(_right);
    }

    public override string PrettyPrint(int indent = 0)
    {
        return NodeLine(indent) +
               "AssignmentExpression(\n" + BlankLine(indent + 1) + "left\n" + ChildNodeLine(indent + 1) +
               $"{_left.PrettyPrint(indent + 2)}\n" +
               BlankLine(indent + 1) + "right\n" + ChildNodeLine(indent + 1) + $"{_right.PrettyPrint(indent + 2)}\n" +
               BlankLine(indent) + ")";
    }
}
