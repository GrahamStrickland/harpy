namespace Harpy.AST.Statements;

/// <summary>
///     Represents a loop statement in a <c>for</c> or <c>while</c> loop.
/// </summary>
public class LoopStatement : Statement
{
    public override IHarbourAstNode? Parent { get; set; }

    public override string PrettyPrint()
    {
        return "loop";
    }

    public override void Walk()
    {
        throw new NotImplementedException();
    }
}