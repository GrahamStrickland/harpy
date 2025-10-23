using Harpy.Lexer;

namespace Harpy.AST.Expressions;

/// <summary>
///     A binary arithmetic expression like <c>a + b</c> or <c>c ^ d</c>.
/// </summary>
public class OperatorExpression(Expression left, HarbourSyntaxToken @operator, Expression right) : Expression(false)
{
    public override IHarbourAstNode? Parent { get; set; }

    public override string PrettyPrint()
    {
        if (@operator.CompoundOperator() != null)
            return $"({left.PrettyPrint()} {@operator.CompoundOperator()} {right.PrettyPrint()})";

        return @operator.SimpleOperator() != null
            ? $"({left.PrettyPrint()} {@operator.SimpleOperator()} {right.PrettyPrint()})"
            : throw new InvalidSyntaxException($"Invalid operator '{@operator.Text}'");
    }

    public override void Walk()
    {
        Console.WriteLine(PrettyPrint());
    }
}