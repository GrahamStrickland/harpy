namespace Harpy.AST.Expressions;

public abstract class Expression(bool leftExpression, List<HarbourAstNode> children) : HarbourAstNode(children)
{
    public bool LeftExpression { get; } = leftExpression;
}