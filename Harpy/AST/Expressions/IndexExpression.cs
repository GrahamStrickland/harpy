namespace Harpy.AST.Expressions;

/// <summary>
///     An index into a hash or array like <c>a[b]</c>.
/// </summary>
public class IndexExpression : Expression
{
    private readonly Expression _left;
    private readonly List<Expression> _right;

    /// <summary>
    ///     An index into a hash or array like <c>a[b]</c>.
    /// </summary>
    public IndexExpression(Expression left, List<Expression> right) : base(false, [])
    {
        _left = left;
        _right = right;

        _left.Parent = this;
        Children.Add(_left);

        foreach (var e in right)
        {
            e.Parent = this;
            Children.Add(e);
        }
    }

    public override string PrettyPrint()
    {
        var indexExpressions = "";
        var i = 0;

        foreach (var e in _right)
        {
            indexExpressions += e.PrettyPrint();
            if (i < _right.Count - 1)
                indexExpressions += ", ";
            i++;
        }

        return $"{_left.PrettyPrint()}[{indexExpressions}]";
    }
}