using Harpy.AST.Expressions;
using Harpy.Lexer;

namespace Harpy.AST.Statements;

/// <summary>
///     Represents a static variable declaration, e.g. <c>static a</c> or <c>static b := 1</c>.
/// </summary>
public class StaticVariableDeclaration(HarbourSyntaxToken name, Expression? assignment)
    : VariableDeclaration("static", name, assignment)
{
    public override IHarbourAstNode? Parent { get; set; }

    public override void Walk()
    {
        throw new NotImplementedException();
    }
}