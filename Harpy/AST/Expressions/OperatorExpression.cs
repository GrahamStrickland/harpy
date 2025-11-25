using Harpy.Lexer;

namespace Harpy.AST.Expressions;

/// <summary>
///     A binary arithmetic expression like <c>a + b</c> or <c>c ^ d</c>.
/// </summary>
public class OperatorExpression : Expression
{
    private readonly Expression _left;
    private readonly HarbourSyntaxToken _operator;
    private readonly Expression _right;

    /// <summary>
    ///     A binary arithmetic expression like <c>a + b</c> or <c>c ^ d</c>.
    /// </summary>
    public OperatorExpression(Expression left, HarbourSyntaxToken @operator, Expression right) : base(false, [])
    {
        _left = left;
        _operator = @operator;
        _right = right;

        _left.Parent = this;
        Children.Add(_left);

        var operatorNode = new HarbourSyntaxTokenNode(_operator, []);
        {
            Parent = this;
        }
        Children.Add(operatorNode);

        _right.Parent = this;
        Children.Add(_right);
    }

    public override string PrettyPrint()
    {
        if (_operator.CompoundOperator() != null)
            return $"({_left.PrettyPrint()} {_operator.CompoundOperator()} {_right.PrettyPrint()})";

        return _operator.SimpleOperator() != null
            ? $"({_left.PrettyPrint()} {_operator.SimpleOperator()} {_right.PrettyPrint()})"
            : throw new InvalidSyntaxException($"Invalid operator '{_operator.Text}'");
    }
}