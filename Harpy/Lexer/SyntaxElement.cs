namespace Harpy.Lexer
{
    internal class SyntaxElement(SyntaxType type, string text, int line, int position)
    {
        private readonly SyntaxType _type = type;
        private readonly string _text = text;
        private readonly int _start = position - text.Length >= 1 ? position - text.Length : 1;
        private readonly int _end = position + 1;
        private readonly int _line = line;

        public SyntaxType Type { get { return _type; } }
        public string Text { get { return _text; } }
        public int Start { get { return _start; } }
        public int End { get { return _end; } }
        public int Line { get { return _line; } }
    }
}
