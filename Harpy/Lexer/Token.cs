namespace Harpy.Lexer
{
    /// <summary>
    /// Syntax element used to represent a Harbour token with optional leading and trailing trivia.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="text"></param>
    /// <param name="end"></param>
    /// <param name="line"></param>
    internal class Token(SyntaxType type, string text, int line, int position) : SyntaxElement(type, text, line, position)
    {
        private List<Trivia> _leadingTrivia = [];
        private List<Trivia> _trailingTrivia = [];

        public List<Trivia> LeadingTrivia 
        { 
            get { return _leadingTrivia; } 
            set { _leadingTrivia = value; } 
        }
        public List<Trivia> TrailingTrivia
        {
            get { return _trailingTrivia; }
            set { _trailingTrivia = value; }
        }
    }
}
