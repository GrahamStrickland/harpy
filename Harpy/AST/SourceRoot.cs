namespace Harpy.AST;

/// <summary>
///     The root AST node of a parsed Harbour source file.
/// </summary>
public class SourceRoot(List<HarbourAstNode> children) : HarbourAstNode(children)
{
    public override string PrettyPrint()
    {
        return Children.Aggregate("", (current, child) => current + child.PrettyPrint());
    }
}