namespace Harpy.AST.Expressions;

/// <summary>
///     A conditional or 'ternary' operator, like <c>iif(a, b, c)</c>.
/// </summary>
public class ConditionalExpression : Expression
{
    private readonly Expression? _elseArm;
    private readonly Expression _ifArm;
    private readonly Expression? _thenArm;

    /// <summary>
    ///     A conditional or 'ternary' operator, like <c>iif(a, b, c)</c>.
    /// </summary>
    public ConditionalExpression(Expression ifArm, Expression? thenArm, Expression? elseArm) : base(false, [])
    {
        _ifArm = ifArm;
        _thenArm = thenArm;
        _elseArm = elseArm;

        _ifArm.Parent = this;
        Children.Add(_ifArm);
        if (_thenArm is not null)
        {
            _thenArm.Parent = this;
            Children.Add(_thenArm);
        }
        if (_elseArm is not null)
        {
            _elseArm.Parent = this;
            Children.Add(_elseArm);
        }
    }

    public override string PrettyPrint(int indent = 0)
    {
        return NodeLine(indent) +
               "ConditionalExpression(\n" + BlankLine(indent + 1) + "if\n" + ChildNodeLine(indent + 1) +
               $"{_ifArm.PrettyPrint(indent + 2)}\n" +
               BlankLine(indent + 1) + "then\n" + ChildNodeLine(indent + 1) +
               $"{(_thenArm is not null ? _thenArm.PrettyPrint(indent + 2) : NodeLine(indent + 2) + "nil")}\n" +
               BlankLine(indent + 1) + "else\n" + ChildNodeLine(indent + 1) +
               $"{(_elseArm is not null ? _elseArm.PrettyPrint(indent + 2) : NodeLine(indent + 2) + "nil")}\n" +
               BlankLine(indent) + ")";
    }
}
