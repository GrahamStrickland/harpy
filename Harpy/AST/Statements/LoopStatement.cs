using Harpy.CodeGen;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Harpy.AST.Statements;

/// <summary>
///     Represents a loop statement in a <c>for</c> or <c>while</c> loop.
/// </summary>
public class LoopStatement() : Statement([])
{
    public override string PrettyPrint(int indent = 0)
    {
        return NodeLine(indent) + "LoopStatement";
    }

    public override StatementSyntax Walk(CodeGenContext context)
    {
        // TODO: Implement loop statement code generation
        throw new NotImplementedException("LoopStatement.Walk not yet implemented");
    }
}
