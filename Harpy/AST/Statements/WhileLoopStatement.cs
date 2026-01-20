using Harpy.AST.Expressions;
using Harpy.CodeGen;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

    public override string PrettyPrint(int indent = 0)
    {
        var result = NodeLine(indent) + "WhileLoopStatement(\n";
        result += BlankLine(indent + 1) + "condition\n" + ChildNodeLine(indent + 1) +
                  _condition.PrettyPrint(indent + 2) + "\n";

        if (_body.Count > 0)
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
        // TODO: Implement while loop statement code generation
        throw new NotImplementedException("WhileLoopStatement.WalkStatement not yet implemented");
    }
}