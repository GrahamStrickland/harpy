using Harpy.CodeGen;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Harpy.AST.Expressions;

/// <summary>
///     A codeblock like <c>{ |a| b(), c() }</c>.
/// </summary>
public class CodeblockExpression : Expression
{
    private readonly List<Expression> _expressions;
    private readonly List<NameExpression> _parameters;

    /// <summary>
    ///     A codeblock like <c>{ |a| b(), c() }</c>.
    /// </summary>
    public CodeblockExpression(List<NameExpression> parameters, List<Expression> expressions) : base(false, [])
    {
        _parameters = parameters;
        _expressions = expressions;

        foreach (var e in parameters)
        {
            e.Parent = this;
            Children.Add(e);
        }

        foreach (var e in expressions)
        {
            e.Parent = this;
            Children.Add(e);
        }
    }

    public override string PrettyPrint(int indent = 0)
    {
        var result = NodeLine(indent) + "CodeblockExpression(\n";
        if (_parameters.Count > 0)
        {
            result += BlankLine(indent + 1) + "parameters\n";
            result = _parameters.Aggregate(result,
                (current, p) => current + ChildNodeLine(indent + 1) + p.PrettyPrint(indent + 2) + "\n");
        }

        if (_expressions.Count > 0)
        {
            result += BlankLine(indent + 1) + "expressions\n";
            result = _expressions.Aggregate(result,
                (current, e) => current + ChildNodeLine(indent + 1) + e.PrettyPrint(indent + 2) + "\n");
        }

        result += BlankLine(indent) + ")";
        return result;
    }

    protected override ExpressionSyntax WalkExpression(CodeGenContext context)
    {
        var statements = SyntaxFactory.SeparatedList<StatementSyntax>();

        foreach (var expression in _expressions)
        {
            statements = statements.Add(SyntaxFactory.ExpressionStatement((ExpressionSyntax)expression.Walk(context)));
        }

        if (_parameters.Count == 0)
        {
            return SyntaxFactory.ParenthesizedLambdaExpression().WithBlock(SyntaxFactory.Block(statements));
        }
        else if (_parameters.Count == 1)
        {
            return SyntaxFactory.SimpleLambdaExpression(SyntaxFactory.Parameter(_parameters[0].Walk(context).GetFirstToken()))
                                .WithBlock(SyntaxFactory.Block(statements));
        }
        else
        {
            var parameters = SyntaxFactory.SeparatedList<ParameterSyntax>();

            foreach (var parameter in _parameters)
            {
                parameters = parameters.Add(SyntaxFactory.Parameter(parameter.Walk(context).GetFirstToken()));
            }

            return SyntaxFactory.ParenthesizedLambdaExpression()
                                .WithParameterList(SyntaxFactory.ParameterList(parameters))
                                .WithBlock(SyntaxFactory.Block(statements));
        }
    }
}
