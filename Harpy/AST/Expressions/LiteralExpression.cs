using System.Globalization;
using Harpy.CodeGen;
using Harpy.Lexer;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Harpy.AST.Expressions;

/// <summary>
///     A boolean, numeric, string, or nil literal, e.g., <c>.t.</c>, <c>123</c>, <c>'hello'</c>, or <c>NIL</c>.
/// </summary>
public class LiteralExpression : Expression
{
    private readonly HarbourSyntaxTokenNode _literalNode;

    /// <summary>
    ///     A boolean, numeric, string, or nil literal, e.g., <c>.t.</c>, <c>123</c>, <c>'hello'</c>, or <c>NIL</c>.
    /// </summary>
    public LiteralExpression(HarbourSyntaxToken literal) : base(false, [])
    {
        _literalNode = new HarbourSyntaxTokenNode(literal, [])
        {
            Parent = this
        };
        Children.Add(_literalNode);
    }

    public override string PrettyPrint(int indent = 0)
    {
        return NodeLine(indent) + "LiteralExpression(\n" + BlankLine(indent + 1) + $"literal(type={_literalNode.token.Literal()})\n" +
               ChildNodeLine(indent + 1) + _literalNode.PrettyPrint(indent + 2) + "\n" + BlankLine(indent) + ")";
    }

    public override ExpressionSyntax WalkExpression(CodeGenContext context)
    {
        var token = _literalNode.token;
        var literalType = token.Literal();

        return literalType switch
        {
            "boolean" => token.Text.ToLower() switch
            {
                ".t." => SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression),
                ".f." => SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression),
                _ => throw new InvalidOperationException($"Unknown boolean literal: {token.Text}")
            },
            "number" => ParseNumericLiteral(token.Text),
            "string" => SyntaxFactory.LiteralExpression(
                SyntaxKind.StringLiteralExpression,
                SyntaxFactory.Literal(UnescapeString(token.Text))),
            "nil" => SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression),
            _ => throw new NotImplementedException($"Literal type {literalType} not implemented")
        };
    }

    private static ExpressionSyntax ParseNumericLiteral(string text)
    {
        if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intValue))
        {
            return SyntaxFactory.LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                SyntaxFactory.Literal(intValue));
        }

        if ((text.StartsWith("0x") || text.StartsWith("0X")) && int.TryParse(text[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var hexValue))
        {
            return SyntaxFactory.LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                SyntaxFactory.Literal(text, hexValue));
        }

        if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var doubleValue))
        {
            return SyntaxFactory.LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                SyntaxFactory.Literal(doubleValue));
        }

        throw new InvalidOperationException($"Unable to parse numeric literal: {text}");
    }

    private static string UnescapeString(string str)
    {
        if (str.Length >= 2 && (str[0] == '"' || str[0] == '\'' || str[0] == '['))
        {
            str = str[1..^1];
        }

        return str
            .Replace("\\n", "\n")
            .Replace("\\r", "\r")
            .Replace("\\t", "\t")
            .Replace("\\\\", "\\")
            .Replace("\\\"", "\"")
            .Replace("\\'", "'");
    }
}
