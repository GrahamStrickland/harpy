using Harpy.AST.Expressions;

namespace Harpy.AST.Statements;

/// <summary>
///     Represents a variable assignment, e.g. <c>b := 1</c>.
/// </summary>
public class AssignmentStatement(AssignmentExpression assignmentExpression) : Statement
{
    public override IHarbourAstNode? Parent { get; set; }

    public override string PrettyPrint()
    {
        return assignmentExpression.PrettyPrint();
    }

    public override void Walk()
    {
        throw new NotImplementedException();
    }
}