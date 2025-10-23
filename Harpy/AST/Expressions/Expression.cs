namespace Harpy.AST.Expressions;

public abstract class Expression(bool leftExpression) : IHarbourAstNode
{
    public bool LeftExpression { get; } = leftExpression;
    public abstract IHarbourAstNode? Parent { get; set; }
    public abstract string PrettyPrint();
    public abstract void Walk();
}