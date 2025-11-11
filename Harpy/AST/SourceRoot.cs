namespace Harpy.AST;

/// <summary>
///     The root AST node of a parsed Harbour source file.
/// </summary>
public class SourceRoot(List<IHarbourAstNode> children) : IHarbourAstNode
{
    public List<IHarbourAstNode> Children { get; set; } = children;
    public IHarbourAstNode? Parent { get; set; }

    public string PrettyPrint()
    {
        return Children.Aggregate("", (current, child) => current + child.PrettyPrint());
    }

    public void Walk()
    {
        foreach (var child in Children) child.Walk();
    }
}