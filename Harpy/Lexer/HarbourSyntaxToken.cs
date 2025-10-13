namespace Harpy.Lexer;

/// <summary>
///     Syntax element used to represent a Harbour token with optional leading and trailing trivia.
/// </summary>
internal class HarbourSyntaxToken : HarbourSyntaxElement
{
    /// <param name="kind">A <c>HarbourSyntaxKind</c> for the token.</param>
    /// <param name="text">The actual text used for the token.</param>
    /// <param name="line">The line on which the token occurred in the source file.</param>
    /// <param name="position">
    ///     The position of the last character in the token.
    ///     Start and end positions will be calculated in the parent class.
    /// </param>
    /// <param name="leadingTrivia">A list of trivia elements encountered before the token.</param>
    /// <param name="trailingTrivia">A list of trivia elements encountered after the token.</param>
    public HarbourSyntaxToken(HarbourSyntaxKind kind, string text, int line, int position,
        List<HarbourSyntaxTrivia> leadingTrivia, List<HarbourSyntaxTrivia> trailingTrivia) : base(kind, text, line,
        position)
    {
        LeadingTrivia = leadingTrivia;
        TrailingTrivia = trailingTrivia;
    }

    public HarbourSyntaxToken(HarbourSyntaxKind kind, string text, int line, int position) : base(kind, text, line,
        position)
    {
        LeadingTrivia = new List<HarbourSyntaxTrivia>();
        TrailingTrivia = new List<HarbourSyntaxTrivia>();
    }

    public List<HarbourSyntaxTrivia> LeadingTrivia { get; set; }

    public List<HarbourSyntaxTrivia> TrailingTrivia { get; set; }
}