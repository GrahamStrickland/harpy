using Harpy.AST.Expressions;
using Harpy.CodeGen;
using Harpy.Lexer;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Harpy.AST.Statements;

public class BeginSequenceStatement : Statement
{
    private readonly List<Statement> _alwaysBody;
    private readonly List<Statement> _beginSequenceBody;
    private readonly CodeblockExpression? _errorHandler;
    private readonly HarbourSyntaxTokenNode? _exceptionNode;
    private readonly List<Statement> _recoverBody;

    public BeginSequenceStatement(CodeblockExpression? errorHandler,
        HarbourSyntaxToken? exception,
        List<Statement> beginSequenceBody,
        List<Statement> recoverBody,
        List<Statement> alwaysBody) : base([])
    {
        _errorHandler = errorHandler;
        _beginSequenceBody = beginSequenceBody;
        _recoverBody = recoverBody;
        _alwaysBody = alwaysBody;

        if (_errorHandler != null)
        {
            _errorHandler.Parent = this;
            Children.Add(_errorHandler);
        }

        if (exception != null)
        {
            _exceptionNode = new HarbourSyntaxTokenNode(exception, [])
            {
                Parent = this
            };
            Children.Add(_exceptionNode);
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

    public override string PrettyPrint(int indent = 0)
    {
        var result = NodeLine(indent) + "BeginSequenceStatement(\n";
        if (_errorHandler != null)
            result += BlankLine(indent + 1) + "errorHandler\n" + ChildNodeLine(indent + 1) +
                      _errorHandler.PrettyPrint(indent + 2) + "\n";

        if (_beginSequenceBody.Count > 0)
        {
            result += BlankLine(indent + 1) + "beginSequenceBody\n";
            foreach (var stmt in _beginSequenceBody)
                result += ChildNodeLine(indent + 1) + stmt.PrettyPrint(indent + 2) + "\n";
        }

        if (_recoverBody.Count > 0)
        {
            result += BlankLine(indent + 1) + "recoverBody\n";
            if (_exceptionNode != null)
                result += BlankLine(indent + 2) + "exception\n" + ChildNodeLine(indent + 2) +
                          _exceptionNode.PrettyPrint(indent + 3) + "\n";
            foreach (var stmt in _recoverBody)
                result += ChildNodeLine(indent + 1) + stmt.PrettyPrint(indent + 2) + "\n";
        }

        if (_alwaysBody.Count > 0)
        {
            result += BlankLine(indent + 1) + "alwaysBody\n";
            foreach (var stmt in _alwaysBody)
                result += ChildNodeLine(indent + 1) + stmt.PrettyPrint(indent + 2) + "\n";
        }

        result += BlankLine(indent) + ")";
        return result;
    }

    public override StatementSyntax WalkStatement(CodeGenContext context)
    {
        // TODO: Implement begin sequence statement code generation
        throw new NotImplementedException("BeginSequenceStatement.WalkStatement not yet implemented");
    }
}