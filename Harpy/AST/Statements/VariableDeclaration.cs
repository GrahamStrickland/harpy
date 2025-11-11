using Harpy.AST.Expressions;
using Harpy.Lexer;

namespace Harpy.AST.Statements;

/// <summary>
///     Represents a local or static variable declaration, e.g. <c>local a</c> or <c>static b := 1</c>.
/// </summary>
/// <param name="scope">String representing variable scope, e.g., <c>local</c>, <c>static</c>, etc.</param>
public abstract class VariableDeclaration(string scope, HarbourSyntaxToken name, Expression? assignment)
    : Statement
{
    public override IHarbourAstNode? Parent { get; set; }

    public override string PrettyPrint()
    {
        return assignment == null ? $"{scope} {name.Text}" : $"{scope} {assignment.PrettyPrint()}";
    }

    public override void Walk()
    {
        throw new NotImplementedException();
    }
}