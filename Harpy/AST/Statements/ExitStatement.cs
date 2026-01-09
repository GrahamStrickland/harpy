using Harpy.CodeGen;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Harpy.AST.Statements;

/// <summary>
///     Represents an exit statement in a <c>for</c> or <c>while</c> loop.
/// </summary>
public class ExitStatement() : Statement([])
{
    public override string PrettyPrint(int indent = 0)
    {
        return NodeLine(indent) + "ExitStatement";
    }

    public override StatementSyntax WalkStatement(CodeGenContext context)
    {
        // TODO: Implement exit statement code generation
        throw new NotImplementedException("ExitStatement.WalkStatement not yet implemented");
    }
}