namespace Harpy.Lexer;

public enum HarbourSyntaxKind
{
    // Punctuation and grouping
    LEFT_PAREN,
    RIGHT_PAREN,
    LEFT_BRACKET,
    RIGHT_BRACKET,
    LEFT_BRACE,
    RIGHT_BRACE,
    COMMA,
    PIPE,

    // Assignment operators
    ASSIGN,
    PLUSEQ,
    MINUSEQ,
    MULTEQ,
    DIVEQ,
    MODEQ,
    EXPEQ,

    // Logical operators
    OR,
    AND,
    NOT,

    // Relational operators
    EQ1,
    EQ2,
    NE1,
    NE2,
    LE,
    GE,
    LT,
    GT,

    // Arithmetic operators
    PLUS,
    MINUS,
    ASTERISK,
    SLASH,
    PERCENT,
    CARET,
    PLUSPLUS,
    MINUSMINUS,

    // Miscellaneous operators
    DOLLAR,
    QUESTION,
    COLON,
    AT,
    HASHOP,

    // Keywords
    FUNCTION,
    PROCEDURE,
    RETURN,
    NIL,
    LOCAL,
    STATIC,
    IIF,
    IF,
    ELSE,
    ELSEIF,
    END,
    ENDIF,
    ENDERR,
    WHILE,
    ENDWHILE,

    // Literals
    STR_LITERAL,
    NUM_LITERAL,
    BOOL_LITERAL,

    // Identifiers
    NAME,

    // Preprocessor directives
    INCLUDE_DIRECTIVE,
    DEFINE_DIRECTIVE,
    IFDEF_DIRECTIVE,
    IFNDEF_DIRECTIVE,
    ELIF_DIRECTIVE,
    ELSE_DIRECTIVE,
    ENDIF_DIRECTIVE,
    UNDEF_DIRECTIVE,
    PRAGMA_DIRECTIVE,
    COMMAND_DIRECTIVE,
    XCOMMAND_DIRECTIVE,
    TRANSLATE_DIRECTIVE,
    XTRANSLATE_DIRECTIVE,
    ERROR_DIRECTIVE,
    STDOUT_DIRECTIVE,

    // Comments
    BLOCK_COMMENT,
    LINE_COMMENT,

    // Spacing
    SPACE,
    TAB,
    CARRIAGE_RETURN,
    NEWLINE,
    LINE_CONTINUATION,
    EOF
}