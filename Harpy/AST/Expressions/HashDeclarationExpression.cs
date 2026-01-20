using Harpy.CodeGen;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Harpy.AST.Expressions;

/// <summary>
///     A hash declaration like <c>{ => }</c> or <c>{ "a" => 1, "b" => 2 }</c>.
/// </summary>
public class HashDeclarationExpression : Expression
{
    private readonly Dictionary<Expression, Expression> _valuePairs;

    /// <summary>
    ///     A hash declaration like <c>{ => }</c> or <c>{ "a" => 1, "b" => 2 }</c>.
    /// </summary>
    public HashDeclarationExpression(Dictionary<Expression, Expression> valuePairs) : base(false, [])
    {
        _valuePairs = valuePairs;

        foreach (var (k, v) in valuePairs)
        {
            k.Parent = this;
            Children.Add(k);
            v.Parent = this;
            Children.Add(v);
        }
    }

    public override string PrettyPrint(int indent = 0)
    {
        var result = NodeLine(indent) + "HashDeclarationExpression(";

        if (_valuePairs.Count > 0)
        {
            var i = 0;
            result += "\n";

            foreach (var (key, value) in _valuePairs)
            {
                result += BlankLine(indent + 1) + $"key {i}\n" + ChildNodeLine(indent + 1) +
                          key.PrettyPrint(indent + 2) + "\n";
                result += BlankLine(indent + 1) + $"value {i}\n" + ChildNodeLine(indent + 1) +
                          value.PrettyPrint(indent + 2) + "\n";
                i++;
            }

            result += BlankLine(indent);
        }

        result += ")";

        return result;
    }

    public override ExpressionSyntax WalkExpression(CodeGenContext context)
    {
        // TODO: Implement hash declaration code generation
        throw new NotImplementedException("HashDeclarationExpression.WalkExpression not yet implemented");
    }
}