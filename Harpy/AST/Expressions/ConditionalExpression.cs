namespace Harpy.AST.Expressions;

/// <summary>
///     A conditional or 'ternary' operator, like <c>iif(a, b, c)</c>.
/// </summary>
public class ConditionalExpression(Expression ifArm, Expression thenArm, Expression elseArm) : Expression(false)
{
    public override IHarbourAstNode? Parent { get; set; }

    public override string PrettyPrint()
    {
        return $"iif({ifArm.PrettyPrint()}, {thenArm.PrettyPrint()}, {elseArm.PrettyPrint()})";
    }

    public override void Walk()
    {
        Console.WriteLine(PrettyPrint());
    }
}