namespace Harpy.Lexer
{
    /// <summary>
    /// Base class for syntax elements.
    /// </summary>
    /// <param name="kind">A <c>HarbourSyntaxKind</c> for the syntax element.</param>
    /// <param name="text">The actual text used for the syntax element.</param>
    /// <param name="line">The line on which the syntax element occurred in the source file.</param>
    /// <param name="position">
    ///     The position of the last character in the syntax element. 
    ///     Start and end positions will be calculated in the constructor.
    /// </param>
    internal class HarbourSyntaxElement(HarbourSyntaxKind kind, string text, int line, int position)
    {
        private readonly HarbourSyntaxKind _kind = kind;
        private readonly string _text = text;
        private readonly int _start = position - text.Length >= 1 ? position - text.Length : 1;
        private readonly int _end = position + 1;
        private readonly int _line = line;

        public HarbourSyntaxKind Kind { get { return _kind; } }
        public string Text { get { return _text; } }
        public int Start { get { return _start; } }
        public int End { get { return _end; } }
        public int Line { get { return _line; } }
    }
}
