namespace Harpy.Lexer;

/// <summary>
///     Syntax element used to represent a Harbour token with optional leading and trailing trivia.
/// </summary>
public class HarbourSyntaxToken : HarbourSyntaxElement
{
    public static readonly Dictionary<HarbourSyntaxKind, string> Keywords = new()
    {
        { HarbourSyntaxKind.FUNCTION, "function" },
        { HarbourSyntaxKind.PROCEDURE, "procedure" },
        { HarbourSyntaxKind.RETURN, "return" },
        { HarbourSyntaxKind.NIL, "nil" },
        { HarbourSyntaxKind.LOCAL, "local" },
        { HarbourSyntaxKind.STATIC, "static" },
        { HarbourSyntaxKind.IIF, "iif" },
        { HarbourSyntaxKind.IF, "if" },
        { HarbourSyntaxKind.ELSE, "else" },
        { HarbourSyntaxKind.ELSEIF, "elseif" },
        { HarbourSyntaxKind.END, "end" },
        { HarbourSyntaxKind.ENDIF, "endif" },
        { HarbourSyntaxKind.ENDERR, "enderr" },
        { HarbourSyntaxKind.WHILE, "while" },
        { HarbourSyntaxKind.ENDWHILE, "endwhile" },
        { HarbourSyntaxKind.FOR, "for" },
        { HarbourSyntaxKind.TO, "to" },
        { HarbourSyntaxKind.STEP, "step" },
        { HarbourSyntaxKind.LOOP, "loop" },
        { HarbourSyntaxKind.EXIT, "exit" },
        { HarbourSyntaxKind.NEXT, "next" },
        { HarbourSyntaxKind.EACH, "each" },
        { HarbourSyntaxKind.IN, "in" },
        { HarbourSyntaxKind.BEGIN, "begin" },
        { HarbourSyntaxKind.SEQUENCE, "sequence" },
        { HarbourSyntaxKind.WITH, "with" },
        { HarbourSyntaxKind.RECOVER, "recover" },
        { HarbourSyntaxKind.USING, "using" },
        { HarbourSyntaxKind.ALWAYS, "always" },
        { HarbourSyntaxKind.ENDSEQUENCE, "endsequence" }
    };

    public static readonly Dictionary<HarbourSyntaxKind, string> CompoundOperators = new()
    {
        { HarbourSyntaxKind.PLUSPLUS, "++" },
        { HarbourSyntaxKind.MINUSMINUS, "--" },
        { HarbourSyntaxKind.ASSIGN, ":=" },
        { HarbourSyntaxKind.PLUSEQ, "+=" },
        { HarbourSyntaxKind.MINUSEQ, "-=" },
        { HarbourSyntaxKind.MULTEQ, "*=" },
        { HarbourSyntaxKind.DIVEQ, "/=" },
        { HarbourSyntaxKind.MODEQ, "%=" },
        { HarbourSyntaxKind.EXPEQ, "^=" },
        { HarbourSyntaxKind.AND, ".and." },
        { HarbourSyntaxKind.OR, ".or." },
        { HarbourSyntaxKind.EQ1, "==" },
        { HarbourSyntaxKind.NE2, "!=" },
        { HarbourSyntaxKind.LE, "<=" },
        { HarbourSyntaxKind.GE, ">=" },
        { HarbourSyntaxKind.HASHOP, "=>" }
    };

    public static readonly Dictionary<HarbourSyntaxKind, string> SimpleOperators = new()
    {
        { HarbourSyntaxKind.LEFT_PAREN, "(" },
        { HarbourSyntaxKind.RIGHT_PAREN, ")" },
        { HarbourSyntaxKind.LEFT_BRACKET, "[" },
        { HarbourSyntaxKind.RIGHT_BRACKET, "]" },
        { HarbourSyntaxKind.LEFT_BRACE, "{" },
        { HarbourSyntaxKind.RIGHT_BRACE, "}" },
        { HarbourSyntaxKind.PIPE, "|" },
        { HarbourSyntaxKind.COMMA, "," },
        { HarbourSyntaxKind.EQ2, "=" },
        { HarbourSyntaxKind.NE1, "#" },
        { HarbourSyntaxKind.LT, "<" },
        { HarbourSyntaxKind.GT, ">" },
        { HarbourSyntaxKind.DOLLAR, "$" },
        { HarbourSyntaxKind.PLUS, "+" },
        { HarbourSyntaxKind.MINUS, "-" },
        { HarbourSyntaxKind.ASTERISK, "*" },
        { HarbourSyntaxKind.SLASH, "/" },
        { HarbourSyntaxKind.PERCENT, "%" },
        { HarbourSyntaxKind.CARET, "^" },
        { HarbourSyntaxKind.NOT, "!" },
        { HarbourSyntaxKind.QUESTION, "?" },
        { HarbourSyntaxKind.COLON, ":" },
        { HarbourSyntaxKind.AT, "@" }
    };

    /// <param name="kind">A <see cref="HarbourSyntaxKind" /> for the token.</param>
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
        LeadingTrivia = [];
        TrailingTrivia = [];
    }

    public List<HarbourSyntaxTrivia> LeadingTrivia { get; set; }

    public List<HarbourSyntaxTrivia> TrailingTrivia { get; set; }

    public static string? Keyword(string text)
    {
        return Keywords.ContainsValue(text) ? text : null;
    }

    public string? Keyword()
    {
        return Keywords.GetValueOrDefault(Kind);
    }

    public static HarbourSyntaxKind KeywordFromText(string text)
    {
        return Keywords.Keys.FirstOrDefault(key => Keywords[key] == text);
    }

    public static string? SimpleOperator(string text)
    {
        return SimpleOperators.ContainsValue(text) ? text : null;
    }

    public string? SimpleOperator()
    {
        return SimpleOperators.GetValueOrDefault(Kind);
    }

    public static HarbourSyntaxKind SimpleOperatorFromText(string text)
    {
        return SimpleOperators.Keys.FirstOrDefault(key => SimpleOperators[key] == text);
    }

    public static string? CompoundOperator(string text)
    {
        return CompoundOperators.ContainsValue(text) ? text : null;
    }

    public string? CompoundOperator()
    {
        return CompoundOperators.GetValueOrDefault(Kind);
    }

    public static HarbourSyntaxKind CompoundOperatorFromText(string text)
    {
        return CompoundOperators.Keys.FirstOrDefault(key => CompoundOperators[key] == text);
    }

    public string? Literal()
    {
        return Kind switch
        {
            HarbourSyntaxKind.BOOL_LITERAL => "boolean",
            HarbourSyntaxKind.NUM_LITERAL => "number",
            HarbourSyntaxKind.STR_LITERAL => "string",
            HarbourSyntaxKind.NIL => "nil",
            _ => null
        };
    }

    public string PrettyPrint(int indent = 0)
    {
        return new string(' ', (indent > 0 ? indent - 1 : indent) * 4) + $"+---Token({(Text[0] == '\'' ? "\"" + Text + "\"" : "'" + Text + "'")},{Line},[{Start}:{End}))";
    }
}
