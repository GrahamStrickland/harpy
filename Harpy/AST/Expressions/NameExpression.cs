namespace Harpy.AST.Expressions;

/// <summary>
///     A simple variable name expression like <c>abc</c>.
/// </summary>
public class NameExpression(string name) : Expression(false)
{
    public override IHarbourAstNode? Parent { get; set; }

    public override string PrettyPrint()
    {
        return name;
    }

    public override void Walk()
    {
        Console.WriteLine(PrettyPrint());
    }
}