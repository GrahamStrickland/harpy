using Harpy.AST.Expressions;
using Harpy.CodeGen;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Harpy.AST.Statements;

/// <summary>
///     Represents a conditional or 'ternary' operator as a statement, e.g., <c>iif(a, b, c)</c>.
/// </summary>
public class ConditionalStatement : Statement
{
    private readonly ConditionalExpression _conditionalExpression;

    /// <summary>
    ///     Represents a conditional or 'ternary' operator as a statement, e.g., <c>iif(a, b, c)</c>.
    /// </summary>
    public ConditionalStatement(ConditionalExpression conditionalExpression) : base([])
    {
        _conditionalExpression = conditionalExpression;

        _conditionalExpression.Parent = this;
        Children.Add(_conditionalExpression);
    }

    public override string PrettyPrint(int indent = 0)
    {
        return NodeLine(indent) + "ConditionalStatement(\n" + BlankLine(indent + 1) + "conditionalExpression\n" +
               ChildNodeLine(indent + 1) + _conditionalExpression.PrettyPrint(indent + 2) + "\n" + BlankLine(indent) +
               ")";
    }

    public override StatementSyntax WalkStatement(CodeGenContext context)
    {
        // TODO: Implement conditional statement code generation
        throw new NotImplementedException("ConditionalStatement.WalkStatement not yet implemented");
    }
}