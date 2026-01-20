using Harpy.CodeGen;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Harpy.AST.Expressions;

/// <summary>
///     An assignment expression like <c>a := b</c>.
/// </summary>
public class AssignmentExpression : Expression
{
    /// <summary>
    ///     An assignment expression like <c>a := b</c>.
    /// </summary>
    public AssignmentExpression(Expression left, Expression right) : base(false, [])
    {
        Left = left;
        Right = right;

        Left.Parent = this;
        Children.Add(Left);
        Right.Parent = this;
        Children.Add(Right);
    }

    private Expression Left { get; }
    public Expression Right { get; }

    public override string PrettyPrint(int indent = 0)
    {
        return NodeLine(indent) +
               "AssignmentExpression(\n" + BlankLine(indent + 1) + "left\n" + ChildNodeLine(indent + 1) +
               $"{Left.PrettyPrint(indent + 2)}\n" +
               BlankLine(indent + 1) + "right\n" + ChildNodeLine(indent + 1) + $"{Right.PrettyPrint(indent + 2)}\n" +
               BlankLine(indent) + ")";
    }

    protected override ExpressionSyntax WalkExpression(CodeGenContext context)
    {
        return SyntaxFactory.AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            (ExpressionSyntax)Left.Walk(context),
            (ExpressionSyntax)Right.Walk(context));
    }
}