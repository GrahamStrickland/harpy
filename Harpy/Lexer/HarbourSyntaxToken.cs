namespace Harpy.Lexer
{
    /// <summary>
    /// Syntax element used to represent a Harbour token with optional leading and trailing trivia.
    /// </summary>
    internal class HarbourSyntaxToken : HarbourSyntaxElement
    {
        private List<HarbourSyntaxTrivia> _leadingTrivia;
        private List<HarbourSyntaxTrivia> _trailingTrivia;

        /// <param name="kind">A <c>HarbourSyntaxKind</c> for the token.</param>
        /// <param name="text">The actual text used for the token.</param>
        /// <param name="line">The line on which the token occurred in the source file.</param>
        /// <param name="position">
        ///     The position of the last character in the token. 
        ///     Start and end positions will be calculated in the parent class.
        /// </param>
        /// <param name="leadingTrivia">A list of trivia elements encountered before the token.</param>
        /// <param name="trailingTrivia">A list of trivia elements encountered after the token.</param>
        public HarbourSyntaxToken(HarbourSyntaxKind kind, string text, int line, int position, List<HarbourSyntaxTrivia> leadingTrivia, List<HarbourSyntaxTrivia> trailingTrivia) : base(kind, text, line, position)
        {
            _leadingTrivia = leadingTrivia;
            _trailingTrivia = trailingTrivia;
        }

        public HarbourSyntaxToken(HarbourSyntaxKind kind, string text, int line, int position) : base(kind, text, line, position)
        {
            _leadingTrivia = new List<HarbourSyntaxTrivia>();
            _trailingTrivia = new List<HarbourSyntaxTrivia>();
        }

        public List<HarbourSyntaxTrivia> LeadingTrivia
        {
            get { return _leadingTrivia; }
            set { _leadingTrivia = value; }
        }
        public List<HarbourSyntaxTrivia> TrailingTrivia
        {
            get { return _trailingTrivia; }
            set { _trailingTrivia = value; }
        }
    }
}
