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

    public override string PrettyPrint(int indent = 0)
    {
        var result = NodeLine(indent) + "ArrayDeclarationExpression(\n";
        for (var i = 0; i < _elements.Count; i++)
        {
            result += BlankLine(indent + 1) + $"element {i}\n" + ChildNodeLine(indent + 1);
            result += $"{_elements[i].PrettyPrint(indent + 2)}\n";
        }

        result += BlankLine(indent) + ")";
        return result;
    }
}