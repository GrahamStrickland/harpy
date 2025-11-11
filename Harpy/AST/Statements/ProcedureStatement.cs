using Harpy.Lexer;

namespace Harpy.AST.Statements;

/// <summary>
///     Represents a procedure declaration, e.g. <c>procedure a(b, c) ; return</c>.
/// </summary>
public class ProcedureStatement(
    HarbourSyntaxToken name,
    List<HarbourSyntaxToken> parameters,
    List<Statement> body,
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

        return output + $"procedure {name.Text}({parametersString})\n{bodyString}return";
    }

    public override void Walk()
    {
        throw new NotImplementedException();
    }
}