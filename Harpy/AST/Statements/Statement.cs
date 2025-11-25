namespace Harpy.AST.Statements;

/// <summary>
///     Interface for all statement AST node classes.
/// </summary>
public abstract class Statement(List<HarbourAstNode> children) : HarbourAstNode(children);