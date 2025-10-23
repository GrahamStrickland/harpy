namespace Harpy.AST.Expressions;

/// <summary>
///     An index into a hash or array like <c>a[b]</c>.
/// </summary>
public class IndexExpression(Expression left, List<Expression> right) : Expression(false)
{
    public override IHarbourAstNode? Parent { get; set; }

    public override string PrettyPrint()
    {
        var indexExpressions = "";
        var i = 0;

        foreach (var e in right)
        {
            indexExpressions += e.PrettyPrint();
            if (i < right.Count - 1)
                indexExpressions += ", ";
            i++;
        }

        return $"{left.PrettyPrint()}[{indexExpressions}]";
    }

    public override void Walk()
    {
        Console.WriteLine(PrettyPrint());
    }
}