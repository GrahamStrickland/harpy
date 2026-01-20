using Harpy.AST.Expressions;
using Harpy.CodeGen;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

    public override string PrettyPrint(int indent = 0)
    {
        var result = NodeLine(indent) + "IfStatement(\n";

        result += BlankLine(indent + 1) + "ifCondition\n" + ChildNodeLine(indent + 1) +
                  _ifCondition.PrettyPrint(indent + 2) + "\n";

        if (_ifBody.Count > 0)
        {
            result += BlankLine(indent + 1) + "ifBody\n";
            result = _ifBody.Aggregate(result,
                (current, stmt) => current + ChildNodeLine(indent + 1) + stmt.PrettyPrint(indent + 2) + "\n");
        }

        if (_elseIfConditions.Count > 0)
        {
            result += BlankLine(indent + 1) + "elseIfConditions\n";
            foreach (var (condition, statements) in _elseIfConditions)
            {
                result += BlankLine(indent + 2) + "condition\n" + ChildNodeLine(indent + 2) +
                          condition.PrettyPrint(indent + 3) + "\n";
                if (statements.Count <= 0) continue;
                result += BlankLine(indent + 2) + "body\n";
                result = statements.Aggregate(result,
                    (current, stmt) => current + ChildNodeLine(indent + 2) + stmt.PrettyPrint(indent + 3) + "\n");
            }
        }

        if (_elseBody.Count > 0)
        {
            result += BlankLine(indent + 1) + "elseBody\n";
            result = _elseBody.Aggregate(result,
                (current, stmt) => current + ChildNodeLine(indent + 1) + stmt.PrettyPrint(indent + 2) + "\n");
        }

        result += BlankLine(indent) + ")";
        return result;
    }

    public override StatementSyntax WalkStatement(CodeGenContext context)
    {
        // TODO: Implement if statement code generation
        throw new NotImplementedException("IfStatement.WalkStatement not yet implemented");
    }
}