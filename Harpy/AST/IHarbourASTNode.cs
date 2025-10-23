namespace Harpy.AST;

public interface IHarbourAstNode
{
    public IHarbourAstNode? Parent { get; set; }

    /// <summary>
    ///     Pretty print the Harbour AST node to a string.
    /// </summary>
    /// <returns>A string representing the AST node in pseudocode.</returns>
    public string PrettyPrint();

    /// <summary>
    ///     Walk through all children of the AST node in order.
    /// </summary>
    public void Walk();
}