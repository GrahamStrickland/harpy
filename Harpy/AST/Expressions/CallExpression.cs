namespace Harpy.AST.Expressions;

/// <summary>
///     A function call like <c>a(b, c, d)</c>.
/// </summary>
public class CallExpression : Expression
{
    private readonly List<Expression> _arguments;
    private readonly Expression _function;

    /// <summary>
    ///     A function call like <c>a(b, c, d)</c>.
    /// </summary>
    public CallExpression(Expression function, List<Expression> arguments) : base(true, [])
    {
        _function = function;
        _arguments = arguments;

        _function.Parent = this;
        Children.Add(_function);

        foreach (var e in arguments)
        {
            e.Parent = this;
            Children.Add(e);
        }
    }

    public override string PrettyPrint()
    {
        var arguments1 = "";
        var i = 0;

        foreach (var e in _arguments)
        {
            arguments1 += e.PrettyPrint();
            if (i < _arguments.Count - 1)
                arguments1 += ", ";
            i++;
        }

        return $"{_function.PrettyPrint()}({arguments1})";
    }
}