using Harpy.Lexer;

namespace Harpy.AST.Expressions;

/// <summary>
///     A prefix unary arithmetic expression like <c>!a</c> or <c>-b</c>.
/// </summary>
public class PrefixExpression : Expression
{
    private readonly HarbourSyntaxToken _operator;
    private readonly Expression _right;

    /// <summary>
    ///     A prefix unary arithmetic expression like <c>!a</c> or <c>-b</c>.
    /// </summary>
    public PrefixExpression(HarbourSyntaxToken @operator, Expression right) : base(false, [])
    {
        _operator = @operator;
        _right = right;

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
        return
            $"({HarbourSyntaxToken.SimpleOperator(_operator.Text) ?? HarbourSyntaxToken.CompoundOperator(_operator.Text)}{_right.PrettyPrint()})";
    }
}