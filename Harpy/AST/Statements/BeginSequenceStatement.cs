using Harpy.AST.Expressions;
using Harpy.Lexer;

namespace Harpy.AST.Statements;

public class BeginSequenceStatement : Statement
{
    private readonly List<Statement> _alwaysBody;
    private readonly List<Statement> _beginSequenceBody;
    private readonly CodeblockExpression? _errorHandler;
    private readonly HarbourSyntaxToken? _exception;
    private readonly List<Statement> _recoverBody;

    public BeginSequenceStatement(CodeblockExpression? errorHandler,
        HarbourSyntaxToken? exception,
        List<Statement> beginSequenceBody,
        List<Statement> recoverBody,
        List<Statement> alwaysBody) : base([])
    {
        _errorHandler = errorHandler;
        _exception = exception;
        _beginSequenceBody = beginSequenceBody;
        _recoverBody = recoverBody;
        _alwaysBody = alwaysBody;

        if (_errorHandler != null)
        {
            _errorHandler.Parent = this;
            Children.Add(_errorHandler);
        }

        if (_exception != null)
        {
            var exceptionNode = new HarbourSyntaxTokenNode(_exception, [])
            {
                Parent = this
            };
            Children.Add(exceptionNode);
        }

        foreach (var statement in _beginSequenceBody)
        {
            statement.Parent = this;
            Children.Add(statement);
        }

        foreach (var statement in _recoverBody)
        {
            statement.Parent = this;
            Children.Add(statement);
        }

        foreach (var statement in _alwaysBody)
        {
            statement.Parent = this;
            Children.Add(statement);
        }
    }

    public override string PrettyPrint()
    {
        var output = "begin sequence";

        if (_errorHandler != null) output += $" with {_errorHandler?.PrettyPrint()}";

        output += "\n";
        output = _beginSequenceBody.Aggregate(output, (current, statement) => current + statement.PrettyPrint() + "\n");

        if (_recoverBody.Count > 0)
        {
            output += "recover";
            if (_exception != null)
                output += $" using {_exception.Text}";
            output += "\n";
        }

        output = _recoverBody.Aggregate(output, (current, statement) => current + $"{statement.PrettyPrint()}\n");

        if (_alwaysBody.Count > 0) output += "always\n";

        output = _alwaysBody.Aggregate(output, (current, statement) => current + $"{statement.PrettyPrint()}\n");

        return output + "end sequence";
    }
}