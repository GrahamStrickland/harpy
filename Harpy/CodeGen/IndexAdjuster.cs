using Harpy.AST.Expressions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Harpy.CodeGen;

/// <summary>
///     Helper class for adjusting 1-based Harbour array indices to 0-based C# indices.
/// </summary>
public static class IndexAdjuster
{
    /// <summary>
    ///     Adjusts a Harbour index expression (1-based) to a C# index expression (0-based).
    /// </summary>
    /// <param name="indexExpression">The index expression to adjust</param>
    /// <param name="context">The code generation context</param>
    /// <returns>An adjusted expression that subtracts 1 from the original index</returns>
    public static ExpressionSyntax AdjustIndex(ExpressionSyntax indexExpression, CodeGenContext context)
    {
        // Check if it's a literal numeric expression
        if (indexExpression is LiteralExpressionSyntax literal)
        {
            // If it's a numeric literal, we can adjust it at compile time
            if (literal.Token.Value is int intValue)
            {
                return SyntaxFactory.LiteralExpression(
                    SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal(intValue - 1));
            }
            if (literal.Token.Value is double doubleValue)
            {
                return SyntaxFactory.LiteralExpression(
                    SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal((int)doubleValue - 1));
            }
        }

        // For non-literal expressions, generate: (expr - 1)
        return SyntaxFactory.ParenthesizedExpression(
            SyntaxFactory.BinaryExpression(
                SyntaxKind.SubtractExpression,
                indexExpression,
                SyntaxFactory.LiteralExpression(
                    SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal(1))));
    }

    /// <summary>
    ///     Checks if an index needs adjustment (always true for Harbour->C# conversion).
    ///     This method is provided for future flexibility if we want to add heuristics
    ///     to detect already-adjusted indices.
    /// </summary>
    public static bool NeedsAdjustment(Expression expression)
    {
        // For now, always adjust. In the future, we might add heuristics to detect
        // cases where the code is already 0-based.
        return true;
    }
}
