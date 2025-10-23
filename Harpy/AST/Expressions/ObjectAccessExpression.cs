namespace Harpy.AST.Expressions;

/// <summary>
///     An object method call or field access like <c>a:b(c)</c> or <c>a:b</c>.
/// </summary>
public class ObjectAccessExpression(Expression left, Expression right) : Expression(false)
{
    public override IHarbourAstNode? Parent { get; set; }

    public override string PrettyPrint()
    {
        return $"{left.PrettyPrint()}:{right.PrettyPrint()}";
    }

    public override void Walk()
    {
        Console.WriteLine(PrettyPrint());
    }
}