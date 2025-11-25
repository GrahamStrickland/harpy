using Harpy.Lexer;

namespace Harpy.AST.Statements;

/// <summary>
///     Represents a procedure declaration, e.g. <c>procedure a(b, c) ; return</c>.
/// </summary>
public class ProcedureStatement : Statement
{
    private readonly List<Statement> _body;
    private readonly bool _isStatic;
    private readonly HarbourSyntaxToken _name;
    private readonly List<HarbourSyntaxToken> _parameters;

    /// <summary>
    ///     Represents a procedure declaration, e.g. <c>procedure a(b, c) ; return</c>.
    /// </summary>
    public ProcedureStatement(HarbourSyntaxToken name,
        List<HarbourSyntaxToken> parameters,
        List<Statement> body,
        bool isStatic) : base([])
    {
        _name = name;
        _parameters = parameters;
        _body = body;
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

        return output + $"procedure {_name.Text}({parametersString})\n{bodyString}return";
    }
}