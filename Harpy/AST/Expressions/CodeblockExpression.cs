namespace Harpy.AST.Expressions;

/// <summary>
///     A codeblock like <c>{ |a| b(), c() }</c>.
/// </summary>
public class CodeblockExpression(List<NameExpression> parameters, List<Expression> expressions) : Expression(false)
{
    public override IHarbourAstNode? Parent { get; set; }

    public override string PrettyPrint()
    {
        var parameters1 = "";
        var i = 0;

        foreach (var p in parameters)
        {
            parameters1 += p.PrettyPrint();
            if (i < parameters.Count - 1) parameters1 += ", ";
            i++;
        }

        var expressions1 = "";
        i = 0;

        foreach (var e in expressions)
        {
            expressions1 += e.PrettyPrint();
            if (i < expressions.Count - 1) expressions1 += ", ";
            i++;
        }

        return $"{{ |{parameters1}| {expressions1} }}";
    }

    public override void Walk()
    {
        Console.WriteLine(PrettyPrint());
    }
}