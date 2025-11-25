namespace Harpy.AST.Expressions;

/// <summary>
///     A codeblock like <c>{ |a| b(), c() }</c>.
/// </summary>
public class CodeblockExpression : Expression
{
    private readonly List<Expression> _expressions;
    private readonly List<NameExpression> _parameters;

    /// <summary>
    ///     A codeblock like <c>{ |a| b(), c() }</c>.
    /// </summary>
    public CodeblockExpression(List<NameExpression> parameters, List<Expression> expressions) : base(false, [])
    {
        _parameters = parameters;
        _expressions = expressions;

        foreach (var e in parameters)
        {
            e.Parent = this;
            Children.Add(e);
        }

        foreach (var e in expressions)
        {
            e.Parent = this;
            Children.Add(e);
        }
    }

    public override string PrettyPrint()
    {
        var parameters1 = "";
        var i = 0;

        foreach (var p in _parameters)
        {
            parameters1 += p.PrettyPrint();
            if (i < _parameters.Count - 1) parameters1 += ", ";
            i++;
        }

        var expressions1 = "";
        i = 0;

        foreach (var e in _expressions)
        {
            expressions1 += e.PrettyPrint();
            if (i < _expressions.Count - 1) expressions1 += ", ";
            i++;
        }

        return $"{{ |{parameters1}| {expressions1} }}";
    }
}