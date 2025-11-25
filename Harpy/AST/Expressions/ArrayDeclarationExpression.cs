namespace Harpy.AST.Expressions;

/// <summary>
///     An array declaration like <c>{ }</c> or <c>{ b, c, d }</c>.
/// </summary>
public class ArrayDeclarationExpression : Expression
{
    private readonly List<Expression> _elements;

    /// <summary>
    ///     An array declaration like <c>{ }</c> or <c>{ b, c, d }</c>.
    /// </summary>
    public ArrayDeclarationExpression(List<Expression> elements) : base(false, [])
    {
        _elements = elements;

        foreach (var e in elements)
        {
            e.Parent = this;
            Children.Add(e);
        }
    }

    public override string PrettyPrint()
    {
        var elements1 = "";
        var i = 0;

        foreach (var e in _elements)
        {
            elements1 += e.PrettyPrint();
            if (i < _elements.Count - 1)
                elements1 += ", ";
            else
                elements1 += " ";
            i++;
        }

        return "{ " + elements1 + "}";
    }
}