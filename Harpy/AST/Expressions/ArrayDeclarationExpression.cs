using Harpy.CodeGen;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        var result = NodeLine(indent) + "ArrayDeclarationExpression(";

        if (_elements.Count > 0)
        {
            result += "\n";

            for (var i = 0; i < _elements.Count; i++)
            {
                result += BlankLine(indent + 1) + $"element {i}\n" + ChildNodeLine(indent + 1);
                result += $"{_elements[i].PrettyPrint(indent + 2)}\n";
            }

            result += BlankLine(indent);
        }

        result += ")";

        return result;
    }

    public override ExpressionSyntax WalkExpression(CodeGenContext context)
    {
        // TODO: Implement array declaration code generation
        throw new NotImplementedException("ArrayDeclarationExpression.WalkExpression not yet implemented");
    }
}