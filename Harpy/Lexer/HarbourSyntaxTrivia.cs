namespace Harpy.Lexer;

/// <summary>
///     Syntax element used to represent comments, whitespace, and preprocessor directives.
/// </summary>
/// <param name="kind">A <see cref="HarbourSyntaxKind" /> for the trivia.</param>
/// <param name="text">The actual text used for the trivia.</param>
/// <param name="line">The line on which the trivia occurred in the source file.</param>
/// <param name="position">
///     The position of the last character in the trivia.
///     Start and end positions will be calculated in the parent class.
/// </param>
public class HarbourSyntaxTrivia(HarbourSyntaxKind kind, string text, int line, int position)
    : HarbourSyntaxElement(kind, text, line, position)
{
    public static readonly Dictionary<HarbourSyntaxKind, string> Directives = new()
    {
        { HarbourSyntaxKind.INCLUDE_DIRECTIVE, "include" },
        { HarbourSyntaxKind.DEFINE_DIRECTIVE, "define" },
        { HarbourSyntaxKind.IFDEF_DIRECTIVE, "ifdef" },
        { HarbourSyntaxKind.IFNDEF_DIRECTIVE, "ifndef" },
        { HarbourSyntaxKind.ELIF_DIRECTIVE, "elif" },
        { HarbourSyntaxKind.ELSE_DIRECTIVE, "else" },
        { HarbourSyntaxKind.ENDIF_DIRECTIVE, "endif" },
        { HarbourSyntaxKind.UNDEF_DIRECTIVE, "undef" },
        { HarbourSyntaxKind.PRAGMA_DIRECTIVE, "pragma" },
        { HarbourSyntaxKind.COMMAND_DIRECTIVE, "command" },
        { HarbourSyntaxKind.XCOMMAND_DIRECTIVE, "xcommand" },
        { HarbourSyntaxKind.TRANSLATE_DIRECTIVE, "translate" },
        { HarbourSyntaxKind.XTRANSLATE_DIRECTIVE, "xtranslate" },
        { HarbourSyntaxKind.ERROR_DIRECTIVE, "error" },
        { HarbourSyntaxKind.STDOUT_DIRECTIVE, "stdout" }
    };

    public static string? Directive(string text)
    {
        return Directives.ContainsValue(text) ? text : null;
    }

    public string? Directive()
    {
        return Directives.GetValueOrDefault(Kind);
    }

    public static HarbourSyntaxKind DirectiveFromText(string text)
    {
        return Directives.Keys.FirstOrDefault(key => Directives[key] == text);
    }
}