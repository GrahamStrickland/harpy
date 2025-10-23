namespace Harpy.AST.Expressions;

/// <summary>
///     A function call like <c>a(b, c, d)</c>.
/// </summary>
public class CallExpression(Expression function, List<Expression> arguments) : Expression(true)
{
    public override IHarbourAstNode? Parent { get; set; }

    public override string PrettyPrint()
    {
        var arguments1 = "";
        var i = 0;

        foreach (var e in arguments)
        {
            arguments1 += e.PrettyPrint();
            if (i < arguments.Count - 1)
                arguments1 += ", ";
            i++;
        }

        return $"{function.PrettyPrint()}({arguments1})";
    }

    public override void Walk()
    {
        Console.WriteLine(PrettyPrint());
    }
}