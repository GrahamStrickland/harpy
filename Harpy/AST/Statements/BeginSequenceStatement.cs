using Harpy.AST.Expressions;
using Harpy.Lexer;

namespace Harpy.AST.Statements;

public class BeginSequenceStatement(
    CodeblockExpression? errorHandler,
    HarbourSyntaxToken? exception,
    List<Statement> beginSequenceBody,
    List<Statement> recoverBody,
    List<Statement> alwaysBody) : Statement
{
    public override IHarbourAstNode? Parent { get; set; }

    public override string PrettyPrint()
    {
        var output = "begin sequence";

        if (errorHandler != null) output += $" with {errorHandler?.PrettyPrint()}";

        output += "\n";
        output = beginSequenceBody.Aggregate(output, (current, statement) => current + statement.PrettyPrint() + "\n");

        if (recoverBody.Count > 0)
        {
            output += "recover";
            if (exception != null)
                output += $" using {exception.Text}";
            output += "\n";
        }

        output = recoverBody.Aggregate(output, (current, statement) => current + $"{statement.PrettyPrint()}\n");

        if (alwaysBody.Count > 0) output += "always\n";

        output = alwaysBody.Aggregate(output, (current, statement) => current + $"{statement.PrettyPrint()}\n");

        return output + "end sequence";
    }

    public override void Walk()
    {
        throw new NotImplementedException();
    }
}