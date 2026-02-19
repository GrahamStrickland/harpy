using Harpy.AST.Expressions;
using Harpy.CodeGen;
using Harpy.Lexer;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Harpy.AST.Statements;

/// <summary>
///     """Represents a function declaration, e.g. <c>function a(b, c) ; return c</c>."""
/// </summary>
public class FunctionStatement : Statement
{
    private readonly List<Statement> _body;
    private readonly bool _isStatic;
    private readonly HarbourSyntaxTokenNode _nameNode;
    private readonly List<HarbourSyntaxTokenNode> _parameterNodes;
    private readonly Expression _returnValue;

    /// <summary>
    ///     """Represents a function declaration, e.g. <c>function a(b, c) ; return c</c>."""
    /// </summary>
    public FunctionStatement(HarbourSyntaxToken name,
        List<HarbourSyntaxToken> parameters,
        List<Statement> body,
        Expression returnValue,
        bool isStatic) : base([])
    {
        _body = body;
        _returnValue = returnValue;
        _isStatic = isStatic;

        _nameNode = new HarbourSyntaxTokenNode(name, [])
        {
            Parent = this
        };
        Children.Add(_nameNode);

        _parameterNodes = [];
        foreach (var parameterNode in parameters.Select(parameter => new HarbourSyntaxTokenNode(parameter, [])
        {
            Parent = this
        }))
        {
            _parameterNodes.Add(parameterNode);
            Children.Add(parameterNode);
        }

        foreach (var bodyChild in body)
        {
            bodyChild.Parent = this;
            Children.Add(bodyChild);
        }

        returnValue.Parent = this;
        Children.Add(returnValue);
    }

    public override string PrettyPrint(int indent = 0)
    {
        var result = NodeLine(indent) + "FunctionStatement(" + (_isStatic ? "static" : "") + "\n";
        result += BlankLine(indent + 1) + "name\n" + ChildNodeLine(indent + 1) + _nameNode.PrettyPrint(indent + 2) +
                  "\n";
        if (_parameterNodes.Count > 0)
        {
            result += BlankLine(indent + 1) + "parameters\n";
            result = _parameterNodes.Aggregate(result,
                (current, pNode) => current + ChildNodeLine(indent + 1) + pNode.PrettyPrint(indent + 2) + "\n");
        }

        if (_body.Count > 0)
        {
            result += BlankLine(indent + 1) + "body\n";
            result = _body.Aggregate(result,
                (current, stmt) => current + ChildNodeLine(indent + 1) + stmt.PrettyPrint(indent + 2) + "\n");
        }

        result += BlankLine(indent + 1) + "returnValue\n" + ChildNodeLine(indent + 1) +
                  _returnValue.PrettyPrint(indent + 2) + "\n";
        result += BlankLine(indent) + ")";
        return result;
    }

    public override StatementSyntax WalkStatement(CodeGenContext context)
    {
        // TODO: Implement function statement code generation
        throw new NotImplementedException("FunctionStatement.WalkStatement not yet implemented");
    }
}