namespace Harpy.Lexer;

/// <summary>
///     Base class for syntax elements.
/// </summary>
/// <param name="kind">A <see cref="HarbourSyntaxKind" /> for the syntax element.</param>
/// <param name="text">The actual text used for the syntax element.</param>
/// <param name="line">The line on which the syntax element occurred in the source file.</param>
/// <param name="position">
///     The position of the last character in the syntax element.
///     Start and end positions will be calculated in the constructor.
/// </param>
public class HarbourSyntaxElement(HarbourSyntaxKind kind, string text, int line, int position)
{
    public HarbourSyntaxKind Kind { get; } = kind;

    public string Text { get; } = text;

    public int Start { get; } = position - text.Length >= 1 ? position - text.Length : 1;

    public int End { get; } = position + 1;

    public int Line { get; } = line;
}
