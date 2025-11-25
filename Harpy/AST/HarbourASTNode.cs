namespace Harpy.AST;

public abstract class HarbourAstNode(List<HarbourAstNode> children)
{
    public HarbourAstNode? Parent { get; set; }
    public List<HarbourAstNode> Children { get; init; } = children;

    /// <summary>
    ///     Pretty print the Harbour AST node to a string.
    /// </summary>
    /// <returns>A string representing the AST node in pseudocode.</returns>
    public virtual string PrettyPrint()
    {
        return "";
    }

    /// <summary>
    ///     Walk through all children of the AST node in order.
    /// </summary>
    public void Walk()
    {
        foreach (var child in Children) child.Walk();
    }
}