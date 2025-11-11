using Harpy.AST.Expressions;
using Harpy.Lexer;

namespace Harpy.AST.Statements;

/// <summary>
///     """Represents a function declaration, e.g. <c>function a(b, c) ; return c</c>."""
/// </summary>
public class FunctionStatement(
    HarbourSyntaxToken name,
    List<HarbourSyntaxToken> parameters,
    List<Statement> body,
    Expression returnValue,
    bool isStatic) : Statement
{
    public override IHarbourAstNode? Parent { get; set; }

    public override string PrettyPrint()
    {
        var parametersString = "";

        for (var i = 0; i < parameters.Count; i++)
            if (i != parameters.Count - 1)
                parametersString += parameters[i].Text + ", ";
            else
                parametersString += parameters[i].Text;

        var bodyString = body.Aggregate("", (current, statement) => current + statement.PrettyPrint() + "\n");

        var output = isStatic ? "static " : "";

        return output + $"function {name.Text}({parametersString})\n{bodyString}return {returnValue.PrettyPrint()}";
    }

    public override void Walk()
    {
        throw new NotImplementedException();
    }
}