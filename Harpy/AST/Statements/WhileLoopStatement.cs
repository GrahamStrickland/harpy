using Harpy.AST.Expressions;

namespace Harpy.AST.Statements;

/// <summary>
///     Represents a <c>while</c> loop.
/// </summary>
public class WhileLoopStatement(Expression condition, List<Statement> body) : Statement
{
    public override IHarbourAstNode? Parent { get; set; }

    public override string PrettyPrint()
    {
        var output = $"while {condition.PrettyPrint()}\n";

        output = body.Aggregate(output, (current, statement) => current + statement.PrettyPrint() + "\n");

        return output + "end while";
    }

    public override void Walk()
    {
        throw new NotImplementedException();
    }
}