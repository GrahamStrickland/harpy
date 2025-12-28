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

    public override string PrettyPrint(int indent = 0)
    {
        var result = NodeLine(indent) + "CodeblockExpression(\n";
        if (_parameters.Count > 0)
        {
            result += BlankLine(indent + 1) + "parameters\n";
            foreach (var p in _parameters) result += ChildNodeLine(indent + 1) + p.PrettyPrint(indent + 2) + "\n";
        }

        if (_expressions.Count > 0)
        {
            result += BlankLine(indent + 1) + "expressions\n";
            foreach (var e in _expressions) result += ChildNodeLine(indent + 1) + e.PrettyPrint(indent + 2) + "\n";
        }

        result += BlankLine(indent) + ")";
        return result;
    }
}