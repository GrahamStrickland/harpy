namespace Harpy.Lexer
{
    /// <summary>
    /// Syntax element used to represent comments, whitespace, and preprocessor directives.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="text"></param>
    /// <param name="end"></param>
    /// <param name="line"></param>
    internal class Trivia(SyntaxType type, string text, int line, int position) : SyntaxElement(type, text, line, position)
    { }
}
