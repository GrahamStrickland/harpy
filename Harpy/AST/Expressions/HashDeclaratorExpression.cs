using Harpy.CodeGen;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Harpy.AST.Expressions;

/// <summary>
///     A hash declarator like <c>{ => }</c> or <c>{ "a" => 1, "b" => 2 }</c>.
/// </summary>
public class HashDeclaratorExpression : Expression
{
    private readonly Dictionary<Expression, Expression> _valuePairs;

    /// <summary>
    ///     A hash declaration like <c>{ => }</c> or <c>{ "a" => 1, "b" => 2 }</c>.
    /// </summary>
    public HashDeclaratorExpression(Dictionary<Expression, Expression> valuePairs) : base(false, [])
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
        var result = NodeLine(indent) + "HashDeclaratorExpression(";

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

    protected override ExpressionSyntax WalkExpression(CodeGenContext context)
    {
        var typeArgumentList = SyntaxFactory.SeparatedList<TypeSyntax>();
        typeArgumentList = typeArgumentList.Add(SyntaxFactory.IdentifierName("string"));
        typeArgumentList = typeArgumentList.Add(SyntaxFactory.IdentifierName("dynamic"));

        var argumentList = SyntaxFactory.SeparatedList<ArgumentSyntax>();

        if (_valuePairs.Count == 0)
        {
            return SyntaxFactory.ObjectCreationExpression(
                        SyntaxFactory.GenericName(SyntaxFactory.Identifier("Dictionary"))
                                     .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(typeArgumentList)))
                                .WithArgumentList(SyntaxFactory.ArgumentList(argumentList));
        }

        if (_valuePairs.Count == 1)
        {
            var initializerList = SyntaxFactory.SeparatedList<ExpressionSyntax>();
            foreach (var (k, v) in _valuePairs)
            {
                initializerList = initializerList.Add((ExpressionSyntax)k.Walk(context));
                initializerList = initializerList.Add((ExpressionSyntax)v.Walk(context));
            }

            var valuePair = SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(SyntaxFactory.InitializerExpression(SyntaxKind.ComplexElementInitializerExpression, initializerList));

            return SyntaxFactory.ObjectCreationExpression(
                        SyntaxFactory.GenericName(SyntaxFactory.Identifier("Dictionary"))
                                     .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(typeArgumentList)))
                                .WithInitializer(SyntaxFactory.InitializerExpression(SyntaxKind.CollectionInitializerExpression, valuePair));
        }
        else
        {
            var valuePairs = SyntaxFactory.SeparatedList<ExpressionSyntax>();

            foreach (var (k, v) in _valuePairs)
            {
                var initializerList = SyntaxFactory.SeparatedList<ExpressionSyntax>();

                initializerList = initializerList.Add((ExpressionSyntax)k.Walk(context));
                initializerList = initializerList.Add((ExpressionSyntax)v.Walk(context));

                valuePairs = valuePairs.Add(SyntaxFactory.InitializerExpression(SyntaxKind.ComplexElementInitializerExpression, initializerList));
            }

            return SyntaxFactory.ObjectCreationExpression(
                        SyntaxFactory.GenericName(SyntaxFactory.Identifier("Dictionary"))
                                     .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(typeArgumentList)))
                                .WithInitializer(SyntaxFactory.InitializerExpression(SyntaxKind.CollectionInitializerExpression, valuePairs));
        }
    }
}
