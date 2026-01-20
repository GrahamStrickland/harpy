using Harpy.CodeGen;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Harpy.AST.Expressions;

/// <summary>
///     An object method call or field access like <c>a:b(c)</c> or <c>a:b</c>.
/// </summary>
public class ObjectAccessExpression : Expression
{
    private readonly Expression _left;
    private readonly Expression _right;

    /// <summary>
    ///     An object method call or field access like <c>a:b(c)</c> or <c>a:b</c>.
    /// </summary>
    public ObjectAccessExpression(Expression left, Expression right) : base(false, [])
    {
        _left = left;
        _right = right;

        _left.Parent = this;
        Children.Add(_left);
        _right.Parent = this;
        Children.Add(_right);
    }

    public override string PrettyPrint(int indent = 0)
    {
        return NodeLine(indent) +
               "ObjectAccessExpression(\n" + BlankLine(indent + 1) + "left\n" + ChildNodeLine(indent + 1) +
               $"{_left.PrettyPrint(indent + 2)}\n" + BlankLine(indent + 1) + "right\n" + ChildNodeLine(indent + 1) +
               $"{_right.PrettyPrint(indent + 2)}\n" + BlankLine(indent) + ")";
    }

    protected override ExpressionSyntax WalkExpression(CodeGenContext context)
    {
        // TODO: Implement object access expression code generation
        throw new NotImplementedException("ObjectAccessExpression.WalkExpression not yet implemented");
    }
}