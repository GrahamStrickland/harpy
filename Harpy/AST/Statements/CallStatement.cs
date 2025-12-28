using Harpy.AST.Expressions;

namespace Harpy.AST.Statements;

/// <summary>
///     Represents a function call, e.g. <c>a(b, c)</c>.
/// </summary>
public class CallStatement : Statement
{
    private readonly CallExpression _callExpression;

    /// <summary>
    ///     Represents a function call, e.g. <c>a(b, c)</c>.
    /// </summary>
    public CallStatement(CallExpression callExpression) : base([])
    {
        _callExpression = callExpression;

        _callExpression.Parent = this;
        Children.Add(_callExpression);
    }

    public override string PrettyPrint(int indent = 0)
    {
        return NodeLine(indent) + "CallStatement(\n" + BlankLine(indent + 1) + "callExpression\n" +
               ChildNodeLine(indent + 1) + _callExpression.PrettyPrint(indent + 2) + "\n" + BlankLine(indent) + ")";
    }
}