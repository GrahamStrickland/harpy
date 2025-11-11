namespace Harpy.AST.Statements;

/// <summary>
///     Interface for all statement AST node classes.
/// </summary>
public abstract class Statement : IHarbourAstNode
{
    public abstract IHarbourAstNode? Parent { get; set; }
    public abstract string PrettyPrint();
    public abstract void Walk();
}