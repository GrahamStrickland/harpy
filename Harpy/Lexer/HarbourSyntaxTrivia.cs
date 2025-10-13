namespace Harpy.Lexer;

/// <summary>
///     Syntax element used to represent comments, whitespace, and preprocessor directives.
/// </summary>
/// <param name="kind">A <c>HarbourSyntaxKind</c> for the trivia.</param>
/// <param name="text">The actual text used for the trivia.</param>
/// <param name="line">The line on which the trivia occurred in the source file.</param>
/// <param name="position">
///     The position of the last character in the trivia.
///     Start and end positions will be calculated in the parent class.
/// </param>
internal class HarbourSyntaxTrivia(HarbourSyntaxKind kind, string text, int line, int position)
    : HarbourSyntaxElement(kind, text, line, position)
{
}