using Harpy.AST.Expressions;
using Harpy.Lexer;

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

        _parameterNodes = new List<HarbourSyntaxTokenNode>();
        foreach (var parameter in parameters)
        {
            var parameterNode = new HarbourSyntaxTokenNode(parameter, [])
            {
                Parent = this
            };
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
            foreach (var pNode in _parameterNodes)
                result += ChildNodeLine(indent + 1) + pNode.PrettyPrint(indent + 2) + "\n";
        }

        if (_body.Count > 0)
        {
            result += BlankLine(indent + 1) + "body\n";
            foreach (var stmt in _body)
                result += ChildNodeLine(indent + 1) + stmt.PrettyPrint(indent + 2) + "\n";
        }

        result += BlankLine(indent + 1) + "returnValue\n" + ChildNodeLine(indent + 1) +
                  _returnValue.PrettyPrint(indent + 2) + "\n";
        result += BlankLine(indent) + ")";
        return result;
    }
}