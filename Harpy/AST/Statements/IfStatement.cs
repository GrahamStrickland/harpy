using Harpy.AST.Expressions;

namespace Harpy.AST.Statements;

public class IfStatement(
    Expression ifCondition,
    List<Tuple<Expression, List<Statement>>> elseIfConditions,
    List<Statement> ifBody,
    List<Statement> elseBody) : Statement
{
    public override IHarbourAstNode? Parent { get; set; }

    public override string PrettyPrint()
    {
        var output = $"if {ifCondition.PrettyPrint()}\n";

        output = ifBody.Aggregate(output, (current, statement) => current + statement.PrettyPrint() + "\n");

        foreach (var item in elseIfConditions)
        {
            var elseIfBody = item.Item2.Aggregate("", (current, statement) => current + statement.PrettyPrint());

            output += $"elseif {item.Item1.PrettyPrint()}\n{elseIfBody}\n";
        }

        if (elseBody.Count <= 0) return output + "endif";

        output += "else\n";
        output = elseBody.Aggregate(output, (current, statement) => current + $"{statement.PrettyPrint()}\n");

        return output + "endif";
    }

    public override void Walk()
    {
        throw new NotImplementedException();
    }
}