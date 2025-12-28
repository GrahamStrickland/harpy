using Harpy.AST.Expressions;

namespace Harpy.AST.Statements;

/// <summary>
///     Represents a variable assignment, e.g. <c>b := 1</c>.
/// </summary>
public class AssignmentStatement : Statement
{
    private readonly AssignmentExpression _assignmentExpression;

    /// <summary>
    ///     Represents a variable assignment, e.g. <c>b := 1</c>.
    /// </summary>
    public AssignmentStatement(AssignmentExpression assignmentExpression) : base([])
    {
        _assignmentExpression = assignmentExpression;

        _assignmentExpression.Parent = this;
        Children.Add(_assignmentExpression);
    }

    public override string PrettyPrint(int indent = 0)
    {
        return NodeLine(indent) + "AssignmentStatement(\n" + BlankLine(indent + 1) + "assignmentExpression\n" +
               ChildNodeLine(indent + 1) +
               _assignmentExpression.PrettyPrint(indent + 2) + "\n" + BlankLine(indent) + ")";
    }
}