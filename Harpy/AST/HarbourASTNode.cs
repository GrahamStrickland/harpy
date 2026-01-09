using Harpy.CodeGen;
using Microsoft.CodeAnalysis;

namespace Harpy.AST;

public abstract class HarbourAstNode(List<HarbourAstNode> children)
{
    public HarbourAstNode? Parent { get; set; }
    public List<HarbourAstNode> Children { get; init; } = children;

    /// <summary>
    ///     Pretty print the Harbour AST node to a string.
    /// </summary>
    /// <returns>A string representing the AST node in pseudocode.</returns>
    public virtual string PrettyPrint(int indent = 0)
    {
        return Children.Aggregate("\n", (current, child) => current + child.PrettyPrint(indent + 1) + "\n");
    }

    /// <summary>
    ///     Walk through the AST node and generate equivalent C# syntax.
    /// </summary>
    /// <param name="context">The code generation context</param>
    /// <returns>A Roslyn syntax node representing the equivalent C# code</returns>
    public abstract SyntaxNode Walk(CodeGenContext context);

    protected static string NodeLine(int indent = 0)
    {
        return indent == 0 
            ? "" 
            : new string(' ', 4 * (indent - 1)) + "└───";
    }

    protected static string BlankLine(int indent = 0)
    {
        return new string(' ', 4 * indent);
    }

    protected static string ChildNodeLine(int indent = 0)
    {
        return new string(' ', 4 * indent) + "│\n";
    }
}
