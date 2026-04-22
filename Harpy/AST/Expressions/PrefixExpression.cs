using Harpy.CodeGen;
using Harpy.Lexer;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Harpy.AST.Expressions;

/// <summary>
///     A prefix unary arithmetic expression like <c>!a</c> or <c>-b</c>.
/// </summary>
public class PrefixExpression : Expression
{
    private readonly HarbourSyntaxTokenNode _operatorNode;
    private readonly Expression _right;

    /// <summary>
    ///     A prefix unary arithmetic expression like <c>!a</c> or <c>-b</c>.
    /// </summary>
    public PrefixExpression(HarbourSyntaxToken @operator, Expression right) : base(false, [])
    {
        _right = right;

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
               "PrefixExpression(\n" +
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
                operatorKind = SyntaxKind.UnaryPlusExpression;
                break;
            case HarbourSyntaxKind.MINUS:
                operatorKind = SyntaxKind.UnaryMinusExpression;
                break;
            case HarbourSyntaxKind.NOT:
                operatorKind = SyntaxKind.LogicalNotExpression;
                break;
            // case HarbourSyntaxKind.AT:
            //     return SyntaxFactory.Argument((ExpressionSyntax)_right.Walk(context)).WithRefOrOutKeyword(SyntaxFactory.Token(SyntaxKind.RefKeyword));
            case HarbourSyntaxKind.PLUSPLUS:
                operatorKind = SyntaxKind.PreIncrementExpression;
                break;
            case HarbourSyntaxKind.MINUSMINUS:
                operatorKind = SyntaxKind.PreDecrementExpression;
                break;
            default:
                throw new ArgumentException($"Invalid operator token passed to `PrefixExpression`: {PrettyPrint()}");
        }

        return SyntaxFactory.PrefixUnaryExpression(operatorKind, (ExpressionSyntax)_right.Walk(context));
    }
}
