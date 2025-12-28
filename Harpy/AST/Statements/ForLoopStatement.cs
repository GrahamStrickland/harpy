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

    public override string PrettyPrint(int indent = 0)
    {
        var output = NodeLine(indent) + "ForLoop(\n" + BlankLine(indent + 1) + "initializer\n" +
                     ChildNodeLine(indent + 1) +
                     _initializer.PrettyPrint(indent + 2) + "\n" + BlankLine(indent + 1) + "bound\n" +
                     ChildNodeLine(indent + 1) + _bound.PrettyPrint(indent + 2);

        if (_step != null)
            output += "\n" + BlankLine(indent + 1) + "step\n" + ChildNodeLine(indent + 1) +
                      _step.PrettyPrint(indent + 2);

        if (_body == null) return output + "\n" + BlankLine(indent) + ")";

        output += "\n" + BlankLine(indent + 1) + "body\n";
        output = _body.Aggregate(output,
            (current, statement) => current + ChildNodeLine(indent + 1) + statement.PrettyPrint(indent + 2) + "\n");

        return output + BlankLine(indent) + ")";
    }
}