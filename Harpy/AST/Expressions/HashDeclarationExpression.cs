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

    public override string PrettyPrint(int indent = 0)
    {
        var result = NodeLine(indent) + "HashDeclarationExpression(\n";
        foreach (var (key, value) in _valuePairs)
        {
            result += BlankLine(indent + 1) + "pair\n";
            result += ChildNodeLine(indent + 1) + BlankLine(indent + 2) + "key\n" + ChildNodeLine(indent + 2) +
                      key.PrettyPrint(indent + 3) + "\n";
            result += ChildNodeLine(indent + 1) + BlankLine(indent + 2) + "value\n" + ChildNodeLine(indent + 2) +
                      value.PrettyPrint(indent + 3) + "\n";
        }

        result += BlankLine(indent) + ")";
        return result;
    }
}