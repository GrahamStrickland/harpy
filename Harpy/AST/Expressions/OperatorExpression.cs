using Harpy.CodeGen;
using Harpy.Lexer;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Harpy.AST.Expressions;

/// <summary>
///     A binary arithmetic expression like <c>a + b</c> or <c>c ^ d</c>.
/// </summary>
public class OperatorExpression : Expression
{
    private readonly Expression _left;
    private readonly HarbourSyntaxTokenNode _operatorNode;
    private readonly Expression _right;

    /// <summary>
    ///     A binary arithmetic expression like <c>a + b</c> or <c>c ^ d</c>.
    /// </summary>
    public OperatorExpression(Expression left, HarbourSyntaxToken @operator, Expression right) : base(false, [])
    {
        _left = left;
        _right = right;

        _left.Parent = this;
        Children.Add(_left);

        _operatorNode = new HarbourSyntaxTokenNode(@operator, [])
        {
            Parent = this
        };
        Children.Add(_operatorNode);

        _right.Parent = this;
        Children.Add(_right);
    }

    public override string PrettyPrint(int indent = 0)
    {
        return NodeLine(indent) +
               "OperatorExpression(\n" +
               BlankLine(indent + 1) + "left\n" +
               ChildNodeLine(indent + 1) +
               $"{_left.PrettyPrint(indent + 2)}\n" +
               BlankLine(indent + 1) + "operator\n" +
               ChildNodeLine(indent + 1) +
               $"{_operatorNode.PrettyPrint(indent + 2)}\n" +
               BlankLine(indent + 1) + "right\n" +
               ChildNodeLine(indent + 1) +
               $"{_right.PrettyPrint(indent + 2)}\n" +
               BlankLine(indent) + ")";
    }

    protected override ExpressionSyntax WalkExpression(CodeGenContext context)
    {
        SyntaxKind operatorKind;
        switch (_operatorNode.Token.Kind)
        {
            case HarbourSyntaxKind.PLUS:
                operatorKind = SyntaxKind.AddExpression;
                break;
            case HarbourSyntaxKind.MINUS:
                operatorKind = SyntaxKind.SubtractExpression;
                break;
            case HarbourSyntaxKind.ASTERISK:
                operatorKind = SyntaxKind.MultiplyExpression;
                break;
            case HarbourSyntaxKind.SLASH:
                operatorKind = SyntaxKind.DivideExpression;
                break;
            case HarbourSyntaxKind.CARET:
                var arguments = SyntaxFactory.SeparatedList<ArgumentSyntax>();

                arguments = arguments.Add(SyntaxFactory.Argument((ExpressionSyntax)_left.Walk(context)));
                arguments = arguments.Add(SyntaxFactory.Argument((ExpressionSyntax)_right.Walk(context)));

                var mathPow = SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName("Math"),
                    SyntaxFactory.IdentifierName("Pow")
                );

                return SyntaxFactory.InvocationExpression(mathPow, SyntaxFactory.ArgumentList(arguments));
            default:
                throw new ArgumentException($"Invalid operator token passed to `OperatorExpression`: {PrettyPrint()}");
        }

        return SyntaxFactory.BinaryExpression(operatorKind, (ExpressionSyntax)_left.Walk(context), (ExpressionSyntax)_right.Walk(context));
    }
}
