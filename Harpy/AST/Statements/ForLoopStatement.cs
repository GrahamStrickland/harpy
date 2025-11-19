using Harpy.AST.Expressions;

namespace Harpy.AST.Statements;

/// <summary>
///     Represents a <c>for</c> loop or a <c>foreach</c> loop.
/// </summary>
public class ForLoopStatement(Expression initializer, Expression bound, Expression? step, List<Statement>? body)
    : Statement
{
    public override IHarbourAstNode? Parent { get; set; }

    public override string PrettyPrint()
    {
        var output = $"for {initializer.PrettyPrint()} to {bound.PrettyPrint()}";

        if (step != null)
            output += $" step {step.PrettyPrint()}\n";
        else
            output += "\n";

        if (body != null)
            output = body.Aggregate(output, (current, statement) => current + statement.PrettyPrint() + "\n");

        return output + "next";
    }

    public override void Walk()
    {
        throw new NotImplementedException();
    }
}