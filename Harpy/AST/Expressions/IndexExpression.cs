using Harpy.CodeGen;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

    public override string PrettyPrint(int indent = 0)
    {
        var result = NodeLine(indent) + "IndexExpression(\n";
        result += BlankLine(indent + 1) + "left\n" + ChildNodeLine(indent + 1) + _left.PrettyPrint(indent + 2) + "\n";
        if (_right.Count > 0)
        {
            result += BlankLine(indent + 1) + "index\n";
            result = _right.Aggregate(result,
                (current, r) => current + ChildNodeLine(indent + 1) + r.PrettyPrint(indent + 2) + "\n");
        }

        result += BlankLine(indent) + ")";
        return result;
    }

    protected override ExpressionSyntax WalkExpression(CodeGenContext context)
    {
        var argumentList = SyntaxFactory.BracketedArgumentList();

        foreach (var e in _right)
        {
            var expression = (ExpressionSyntax)e.Walk(context);

            if (IndexAdjuster.NeedsAdjustment(e))
                expression = IndexAdjuster.AdjustIndex(expression, context);

            argumentList = argumentList.AddArguments(SyntaxFactory.Argument(expression));
        }

        return SyntaxFactory.ElementAccessExpression((ExpressionSyntax)_left.Walk(context))
                            .WithArgumentList(argumentList);
    }
}
