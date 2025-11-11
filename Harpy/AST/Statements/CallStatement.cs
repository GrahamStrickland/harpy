using Harpy.AST.Expressions;

namespace Harpy.AST.Statements;

/// <summary>
///     Represents a function call, e.g. <c>a(b, c)</c>.
/// </summary>
public class CallStatement(CallExpression callExpression) : Statement
{
    public override IHarbourAstNode? Parent { get; set; }

    public override string PrettyPrint()
    {
        return callExpression.PrettyPrint();
    }

    public override void Walk()
    {
        throw new NotImplementedException();
    }
}