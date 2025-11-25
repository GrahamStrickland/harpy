namespace Harpy.AST.Expressions;

/// <summary>
///     An object method call or field access like <c>a:b(c)</c> or <c>a:b</c>.
/// </summary>
public class ObjectAccessExpression : Expression
{
    private readonly Expression _left;
    private readonly Expression _right;

    /// <summary>
    ///     An object method call or field access like <c>a:b(c)</c> or <c>a:b</c>.
    /// </summary>
    public ObjectAccessExpression(Expression left, Expression right) : base(false, [])
    {
        _left = left;
        _right = right;

        _left.Parent = this;
        Children.Add(_left);
        _right.Parent = this;
        Children.Add(_right);
    }

    public override string PrettyPrint()
    {
        return $"{_left.PrettyPrint()}:{_right.PrettyPrint()}";
    }
}