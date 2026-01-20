using Harpy.CodeGen;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Harpy.AST.Expressions;

public abstract class Expression(bool leftExpression, List<HarbourAstNode> children) : HarbourAstNode(children)
{
    public bool LeftExpression { get; } = leftExpression;

    /// <summary>
    ///     Walk through the expression and generate equivalent C# expression syntax.
    /// </summary>
    /// <param name="context">The code generation context</param>
    /// <returns>A Roslyn <c>ExpressionSyntax</c> node representing the equivalent C# expression</returns>
    public abstract ExpressionSyntax WalkExpression(CodeGenContext context);

    /// <summary>
    ///     Implementation of the base <c>Walk</c> method that calls the expression-specific Walk.
    /// </summary>
    public override SyntaxNode Walk(CodeGenContext context)
    {
        return WalkExpression(context);
    }
}