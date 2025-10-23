namespace Harpy.AST.Expressions;

/// <summary>
///     A hash declaration like <c>{ => }</c> or <c>{ "a" => 1, "b" => 2 }</c>.
/// </summary>
public class HashDeclarationExpression(Dictionary<Expression, Expression> valuePairs) : Expression(false)
{
    public override IHarbourAstNode? Parent { get; set; }

    public override string PrettyPrint()
    {
        var keyValuePairs = "";
        var i = 0;

        if (valuePairs.Count == 0) return "{ => }";

        foreach (var k in valuePairs.Keys)
        {
            keyValuePairs += $"{k.PrettyPrint()} => {valuePairs[k].PrettyPrint()}";
            if (i < valuePairs.Count - 1)
                keyValuePairs += ", ";
            else
                keyValuePairs += " ";
            i++;
        }

        return "{ " + keyValuePairs + "}";
    }

    public override void Walk()
    {
        PrettyPrint();
    }
}