using Harpy.AST.Expressions;

namespace Harpy.AST.Statements;

/// <summary>
///     Represents a <c>while</c> loop.
/// </summary>
public class WhileLoopStatement : Statement
{
    private readonly List<Statement> _body;
    private readonly Expression _condition;

    /// <summary>
    ///     Represents a <c>while</c> loop.
    /// </summary>
    public WhileLoopStatement(Expression condition, List<Statement> body) : base([])
    {
        _condition = condition;
        _body = body;

        _condition.Parent = this;
        Children.Add(_condition);

        foreach (var statement in _body)
        {
            statement.Parent = this;
            Children.Add(statement);
        }
    }

    public override string PrettyPrint()
    {
        var output = $"while {_condition.PrettyPrint()}\n";

        output = _body.Aggregate(output, (current, statement) => current + statement.PrettyPrint() + "\n");

        return output + "end while";
    }
}