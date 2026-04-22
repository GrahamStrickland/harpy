using Harpy.CodeGen;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Harpy.AST.Expressions;

/// <summary>
///     A function call like <c>a(b, c, d)</c>.
/// </summary>
public class CallExpression : Expression
{
    private readonly List<Expression?> _arguments;
    private readonly Expression _function;

    /// <summary>
    ///     A function call like <c>a(b, c, d)</c>.
    /// </summary>
    public CallExpression(Expression function, List<Expression?> arguments) : base(true, [])
    {
        _function = function;
        _arguments = arguments;

        _function.Parent = this;
        Children.Add(_function);

        foreach (var e in arguments.OfType<Expression>())
        {
            e.Parent = this;
            Children.Add(e);
        }
    }

    public override string PrettyPrint(int indent = 0)
    {
        var argumentString = _arguments.Aggregate("",
            (current, e) => current + ChildNodeLine(indent + 1) +
                            (e is not null ? e.PrettyPrint(indent + 2) : NodeLine(indent + 2) + "nil") + "\n");

        return NodeLine(indent) + "CallExpression(\n" + BlankLine(indent + 1) + "function\n" +
               ChildNodeLine(indent + 1) +
               $"{_function.PrettyPrint(indent + 2)}" +
               (_arguments.Count > 0
                   ? "\n" + BlankLine(indent + 1) + "arguments\n" + argumentString
                   : "\n") + BlankLine(indent) + ")";
    }

    protected override ExpressionSyntax WalkExpression(CodeGenContext context)
    {
        var expressionSyntax = _function.Walk(context);
        var arguments = SyntaxFactory.SeparatedList<ArgumentSyntax>();

        if (_arguments != null)
        {
            foreach (var argument in _arguments)
            {
                if (argument is not null)
                {
                    var argumentExpression = argument.Walk(context);
                    arguments = arguments.Add(SyntaxFactory.Argument((ExpressionSyntax)argumentExpression));
                }
            }
        }

        var argumentList = SyntaxFactory.ArgumentList(arguments);

        return SyntaxFactory.InvocationExpression((ExpressionSyntax)expressionSyntax, argumentList);
    }
}
