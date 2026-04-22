using Harpy.CodeGen;
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
    public abstract override StatementSyntax Walk(CodeGenContext context);
}
