namespace Harpy.AST.Expressions;

/// <summary>
///     A hash declaration like <c>{ => }</c> or <c>{ "a" => 1, "b" => 2 }</c>.
/// </summary>
public class HashDeclarationExpression : Expression
{
    private readonly Dictionary<Expression, Expression> _valuePairs;

    /// <summary>
    ///     A hash declaration like <c>{ => }</c> or <c>{ "a" => 1, "b" => 2 }</c>.
    /// </summary>
    public HashDeclarationExpression(Dictionary<Expression, Expression> valuePairs) : base(false, [])
    {
        _valuePairs = valuePairs;

        foreach (var (k, v) in valuePairs)
        {
            k.Parent = this;
            Children.Add(k);
            v.Parent = this;
            Children.Add(v);
        }
    }

    public override string PrettyPrint()
    {
        var keyValuePairs = "";
        var i = 0;

        if (_valuePairs.Count == 0) return "{ => }";

        foreach (var k in _valuePairs.Keys)
        {
            keyValuePairs += $"{k.PrettyPrint()} => {_valuePairs[k].PrettyPrint()}";
            if (i < _valuePairs.Count - 1)
                keyValuePairs += ", ";
            else
                keyValuePairs += " ";
            i++;
        }

        return "{ " + keyValuePairs + "}";
    }
}