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
    private readonly HarbourSyntaxToken _name;
    private readonly List<HarbourSyntaxToken> _parameters;
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
        _name = name;
        _parameters = parameters;
        _body = body;
        _returnValue = returnValue;
        _isStatic = isStatic;

        var nameNode = new HarbourSyntaxTokenNode(name, [])
        {
            Parent = this
        };
        Children.Add(nameNode);
        foreach (var parameterChild in parameters.Select(parameter => new HarbourSyntaxTokenNode(parameter, [])
                 {
                     Parent = this
                 }))
            Children.Add(parameterChild);

        foreach (var bodyChild in body)
        {
            bodyChild.Parent = this;
            Children.Add(bodyChild);
        }

        returnValue.Parent = this;
        Children.Add(returnValue);
    }

    public override string PrettyPrint()
    {
        var parametersString = "";

        for (var i = 0; i < _parameters.Count; i++)
            if (i != _parameters.Count - 1)
                parametersString += _parameters[i].Text + ", ";
            else
                parametersString += _parameters[i].Text;

        var bodyString = _body.Aggregate("", (current, statement) => current + statement.PrettyPrint() + "\n");

        var output = _isStatic ? "static " : "";

        return output + $"function {_name.Text}({parametersString})\n{bodyString}return {_returnValue.PrettyPrint()}";
    }
}