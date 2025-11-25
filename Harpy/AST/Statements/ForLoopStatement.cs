using Harpy.AST.Expressions;

namespace Harpy.AST.Statements;

/// <summary>
///     Represents a <c>for</c> loop.
/// </summary>
public class ForLoopStatement : Statement
{
    private readonly List<Statement>? _body;
    private readonly Expression _bound;
    private readonly Expression _initializer;
    private readonly Expression? _step;

    /// <summary>
    ///     Represents a <c>for</c> loop.
    /// </summary>
    public ForLoopStatement(Expression initializer, Expression bound, Expression? step, List<Statement>? body) :
        base([])
    {
        _initializer = initializer;
        _bound = bound;
        _step = step;
        _body = body;

        _initializer.Parent = this;
        Children.Add(_initializer);
        _bound.Parent = this;
        Children.Add(_bound);

        if (_step != null)
        {
            _step.Parent = this;
            Children.Add(_step);
        }

        if (_body == null) return;
        foreach (var statement in _body)
        {
            statement.Parent = this;
            Children.Add(statement);
        }
    }

    public override string PrettyPrint()
    {
        var output = $"for {_initializer.PrettyPrint()} to {_bound.PrettyPrint()}";

        if (_step != null)
            output += $" step {_step.PrettyPrint()}\n";
        else
            output += "\n";

        if (_body != null)
            output = _body.Aggregate(output, (current, statement) => current + statement.PrettyPrint() + "\n");

        return output + "next";
    }
}