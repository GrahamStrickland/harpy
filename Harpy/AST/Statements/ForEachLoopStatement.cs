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
    private readonly HarbourSyntaxTokenNode _variableNode;

    /// <summary>
    ///     Represents a <c>for each</c> loop.
    /// </summary>
    public ForEachLoopStatement(HarbourSyntaxToken variable, Expression collection, List<Statement>? body) : base([])
    {
        _collection = collection;
        _body = body;

        _variableNode = new HarbourSyntaxTokenNode(variable, [])
        {
            Parent = this
        };
        Children.Add(_variableNode);

        _collection.Parent = this;
        Children.Add(_collection);

        if (_body == null) return;
        foreach (var statement in _body)
        {
            statement.Parent = this;
            Children.Add(statement);
        }
    }

    public override string PrettyPrint(int indent = 0)
    {
        var result = NodeLine(indent) + "ForEachLoopStatement(\n";
        result += BlankLine(indent + 1) + "variable\n" + ChildNodeLine(indent + 1) +
                  _variableNode.PrettyPrint(indent + 2) + "\n";
        result += BlankLine(indent + 1) + "collection\n" + ChildNodeLine(indent + 1) +
                  _collection.PrettyPrint(indent + 2) + "\n";
        if (_body != null && _body.Count > 0)
        {
            result += BlankLine(indent + 1) + "body\n";
            foreach (var stmt in _body)
                result += ChildNodeLine(indent + 1) + stmt.PrettyPrint(indent + 2) + "\n";
        }

        result += BlankLine(indent) + ")";
        return result;
    }
}
