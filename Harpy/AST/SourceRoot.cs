namespace Harpy.AST;

/// <summary>
///     The root AST node of a parsed Harbour source file.
/// </summary>
public class SourceRoot(string name, List<HarbourAstNode> children) : HarbourAstNode(children)
{
    public override string PrettyPrint(int indent = 0)
    {
        return $"SourceRoot(name='{name}')" + base.PrettyPrint(indent);
    }
}