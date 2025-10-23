namespace Harpy.AST.Expressions;

/// <summary>
///     An array declaration like <c>{ }</c> or <c>{ b, c, d }</c>.
/// </summary>
public class ArrayDeclarationExpression(List<Expression> elements) : Expression(false)
{
    public override IHarbourAstNode? Parent { get; set; }

    public override string PrettyPrint()
    {
        var elements1 = "";
        var i = 0;

        foreach (var e in elements)
        {
            elements1 += e.PrettyPrint();
            if (i < elements.Count - 1)
                elements1 += ", ";
            else
                elements1 += " ";
            i++;
        }

        return "{ " + elements1 + "}";
    }

    public override void Walk()
    {
        Console.WriteLine(PrettyPrint());
    }
}