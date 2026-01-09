using Harpy.CodeGen;
using Harpy.Lexer;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Harpy.AST.Expressions;

/// <summary>
///     A postfix unary arithmetic expression like <c>a!</c>.
/// </summary>
public class PostfixExpression : Expression
{
    private readonly Expression _left;
    private readonly HarbourSyntaxTokenNode _operatorNode;

    /// <summary>
    ///     A postfix unary arithmetic expression like <c>a!</c>.
    /// </summary>
    public PostfixExpression(Expression left, HarbourSyntaxToken @operator) : base(false, [])
    {
        _left = left;

        _left.Parent = this;
        Children.Add(_left);

        _operatorNode = new HarbourSyntaxTokenNode(@operator, []);
        _operatorNode.Parent = this;
        Children.Add(_operatorNode);
    }

    public override string PrettyPrint(int indent = 0)
    {
        return NodeLine(indent) +
               "PostfixExpression(\n" +
               BlankLine(indent + 1) + "left\n" +
               ChildNodeLine(indent + 1) +
               $"{_left.PrettyPrint(indent + 2)}\n" +
               BlankLine(indent + 1) + "operator\n" +
               ChildNodeLine(indent + 1) +
               $"{_operatorNode.PrettyPrint(indent + 2)}\n" +
               BlankLine(indent) + ")";
    }

    public override ExpressionSyntax WalkExpression(CodeGenContext context)
    {
        // TODO: Implement postfix expression code generation
        throw new NotImplementedException("PostfixExpression.WalkExpression not yet implemented");
    }
}