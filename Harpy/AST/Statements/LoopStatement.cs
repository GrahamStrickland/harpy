namespace Harpy.AST.Statements;

/// <summary>
///     Represents a loop statement in a <c>for</c> or <c>while</c> loop.
/// </summary>
public class LoopStatement() : Statement([])
{
    public override string PrettyPrint()
    {
        return "loop";
    }
}