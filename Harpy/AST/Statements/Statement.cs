using Harpy.CodeGen;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Harpy.AST.Statements;

/// <summary>
///     Interface for all statement AST node classes.
/// </summary>
public abstract class Statement(List<HarbourAstNode> children) : HarbourAstNode(children)
{
    /// <summary>
    ///     Walk through the statement and generate equivalent C# statement syntax.
    /// </summary>
    /// <param name="context">The code generation context</param>
    /// <returns>A Roslyn <c>StatementSyntax</c> node representing the equivalent C# statement</returns>
    public abstract StatementSyntax WalkStatement(CodeGenContext context);

    /// <summary>
    ///     Implementation of the base Walk method that calls the statement-specific <c>Walk</c>.
    /// </summary>
    public override SyntaxNode Walk(CodeGenContext context) => WalkStatement(context);
}
