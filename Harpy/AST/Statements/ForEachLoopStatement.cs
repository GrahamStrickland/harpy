using Harpy.AST.Expressions;
using Harpy.Lexer;

namespace Harpy.AST.Statements;

/// <summary>
///     Represents a <c>for each</c> loop.
/// </summary>
public class ForEachLoopStatement : Statement
{
    private readonly List<Statement>? _body;
    private readonly Expression _collection;
    private readonly HarbourSyntaxToken _variable;

    /// <summary>
    ///     Represents a <c>for each</c> loop.
    /// </summary>
    public ForEachLoopStatement(HarbourSyntaxToken variable, Expression collection, List<Statement>? body) : base([])
    {
        _variable = variable;
        _collection = collection;
        _body = body;

        var variableNode = new HarbourSyntaxTokenNode(_variable, [])
        {
            Parent = this
        };
        Children.Add(variableNode);

        _collection.Parent = this;
        Children.Add(_collection);

        if (_body == null) return;
        foreach (var statement in _body)
        {
            statement.Parent = this;
            Children.Add(statement);
        }
    }

    public override string PrettyPrint()
    {
        var output = $"for each {_variable.Text} in {_collection.PrettyPrint()}\n";

        if (_body != null)
            output = _body.Aggregate(output, (current, statement) => current + statement.PrettyPrint() + "\n");

        return output + "next";
    }
}