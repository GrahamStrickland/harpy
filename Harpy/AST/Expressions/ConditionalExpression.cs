namespace Harpy.AST.Expressions;

/// <summary>
///     A conditional or 'ternary' operator, like <c>iif(a, b, c)</c>.
/// </summary>
public class ConditionalExpression : Expression
{
    private readonly Expression _elseArm;
    private readonly Expression _ifArm;
    private readonly Expression _thenArm;

    /// <summary>
    ///     A conditional or 'ternary' operator, like <c>iif(a, b, c)</c>.
    /// </summary>
    public ConditionalExpression(Expression ifArm, Expression thenArm, Expression elseArm) : base(false, [])
    {
        _ifArm = ifArm;
        _thenArm = thenArm;
        _elseArm = elseArm;

        _ifArm.Parent = this;
        Children.Add(_ifArm);
        _thenArm.Parent = this;
        Children.Add(_thenArm);
        _elseArm.Parent = this;
        Children.Add(_elseArm);
    }

    public override string PrettyPrint()
    {
        return $"iif({_ifArm.PrettyPrint()}, {_thenArm.PrettyPrint()}, {_elseArm.PrettyPrint()})";
    }
}