using Harpy.AST.Expressions;
using Harpy.CodeGen;
using Harpy.Lexer;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        if (_body is { Count: > 0 })
        {
            result += BlankLine(indent + 1) + "body\n";
            result = _body.Aggregate(result,
                (current, stmt) => current + ChildNodeLine(indent + 1) + stmt.PrettyPrint(indent + 2) + "\n");
        }

        result += BlankLine(indent) + ")";
        return result;
    }

    public override StatementSyntax WalkStatement(CodeGenContext context)
    {
        // TODO: Implement for each loop statement code generation
        throw new NotImplementedException("ForEachLoopStatement.WalkStatement not yet implemented");
    }
}