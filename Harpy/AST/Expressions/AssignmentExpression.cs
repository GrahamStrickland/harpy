namespace Harpy.AST.Expressions;

/// <summary>
///     An assignment expression like <c>a := b</c>.
/// </summary>
public class AssignmentExpression(Expression left, Expression right) : Expression(false)
{
    public override IHarbourAstNode? Parent { get; set; }

    public override string PrettyPrint()
    {
        return $"({left.PrettyPrint()} := {right.PrettyPrint()})";
    }

    public override void Walk()
    {
        Console.WriteLine(PrettyPrint());
    }
}