using Harpy.CodeGen;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Harpy.AST.Expressions;

/// <summary>
///     An assignment expression like <c>a := b</c>.
/// </summary>
public class AssignmentExpression : Expression
{
    public Expression Left { get; }
    public Expression Right { get; }

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

    public override string PrettyPrint(int indent = 0)
    {
        return NodeLine(indent) +
               "AssignmentExpression(\n" + BlankLine(indent + 1) + "left\n" + ChildNodeLine(indent + 1) +
               $"{Left.PrettyPrint(indent + 2)}\n" +
               BlankLine(indent + 1) + "right\n" + ChildNodeLine(indent + 1) + $"{Right.PrettyPrint(indent + 2)}\n" +
               BlankLine(indent) + ")";
    }

    public override ExpressionSyntax WalkExpression(CodeGenContext context)
    {
        return Microsoft.CodeAnalysis.CSharp.SyntaxFactory.AssignmentExpression(
            Microsoft.CodeAnalysis.CSharp.SyntaxKind.SimpleAssignmentExpression,
            (ExpressionSyntax)Left.Walk(context),
            (ExpressionSyntax)Right.Walk(context));
    }
}
