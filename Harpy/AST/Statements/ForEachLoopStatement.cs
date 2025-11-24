using Harpy.AST.Expressions;
using Harpy.Lexer;

namespace Harpy.AST.Statements;

/// <summary>
///     Represents a <c>for each</c> loop.
/// </summary>
public class ForEachLoopStatement(HarbourSyntaxToken variable, Expression collection, List<Statement>? body)
    : Statement
{
    public override IHarbourAstNode? Parent { get; set; }

    public override string PrettyPrint()
    {
        var output = $"for each {variable.Text} in {collection.PrettyPrint()}\n";

        if (body != null)
            output = body.Aggregate(output, (current, statement) => current + statement.PrettyPrint() + "\n");

        return output + "next";
    }

    public override void Walk()
    {
        throw new NotImplementedException();
    }
}