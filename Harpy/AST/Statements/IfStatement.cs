using Harpy.AST.Expressions;

namespace Harpy.AST.Statements;

public class IfStatement : Statement
{
    private readonly List<Statement> _elseBody;
    private readonly List<Tuple<Expression, List<Statement>>> _elseIfConditions;
    private readonly List<Statement> _ifBody;
    private readonly Expression _ifCondition;

    public IfStatement(Expression ifCondition,
        List<Tuple<Expression, List<Statement>>> elseIfConditions,
        List<Statement> ifBody,
        List<Statement> elseBody) : base([])
    {
        _ifCondition = ifCondition;
        _elseIfConditions = elseIfConditions;
        _ifBody = ifBody;
        _elseBody = elseBody;

        _ifCondition.Parent = this;
        Children.Add(_ifCondition);

        foreach (var statement in _ifBody)
        {
            statement.Parent = this;
            Children.Add(statement);
        }

        foreach (var (condition, statements) in _elseIfConditions)
        {
            condition.Parent = this;
            Children.Add(condition);

            foreach (var statement in statements)
            {
                statement.Parent = this;
                Children.Add(statement);
            }
        }

        foreach (var statement in _elseBody)
        {
            statement.Parent = this;
            Children.Add(statement);
        }
    }

    public override string PrettyPrint()
    {
        var output = $"if {_ifCondition.PrettyPrint()}\n";

        output = _ifBody.Aggregate(output, (current, statement) => current + statement.PrettyPrint() + "\n");

        foreach (var item in _elseIfConditions)
        {
            var elseIfBody = item.Item2.Aggregate("", (current, statement) => current + statement.PrettyPrint());

            output += $"elseif {item.Item1.PrettyPrint()}\n{elseIfBody}\n";
        }

        if (_elseBody.Count <= 0) return output + "endif";

        output += "else\n";
        output = _elseBody.Aggregate(output, (current, statement) => current + $"{statement.PrettyPrint()}\n");

        return output + "endif";
    }
}