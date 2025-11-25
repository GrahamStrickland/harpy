using Harpy.Lexer;

namespace Harpy.AST.Expressions;

/// <summary>
///     A postfix unary arithmetic expression like <c>a!</c>.
/// </summary>
public class PostfixExpression : Expression
{
    private readonly Expression _left;
    private readonly HarbourSyntaxToken _operator;

    /// <summary>
    ///     A postfix unary arithmetic expression like <c>a!</c>.
    /// </summary>
    public PostfixExpression(Expression left, HarbourSyntaxToken @operator) : base(false, [])
    {
        _left = left;
        _operator = @operator;

        _left.Parent = this;
        Children.Add(_left);

        var operatorNode = new HarbourSyntaxTokenNode(_operator, []);
        {
            Parent = this;
        }
        Children.Add(operatorNode);
    }

    public override string PrettyPrint()
    {
        return
            $"({_left.PrettyPrint()}{HarbourSyntaxToken.SimpleOperator(_operator.Text) ?? HarbourSyntaxToken.CompoundOperator(_operator.Text)})";
    }
}