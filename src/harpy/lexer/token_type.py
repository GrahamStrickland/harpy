from enum import Enum


class TokenType(Enum):
    # Punctuation and grouping
    LEFT_PAREN = 0
    RIGHT_PAREN = 1
    LEFT_BRACKET = 2
    RIGHT_BRACKET = 3
    LEFT_BRACE = 4
    RIGHT_BRACE = 5
    COMMA = 6
    PIPE = 7

    # Assignment operators
    ASSIGN = 8
    PLUSEQ = 9
    MINUSEQ = 10
    MULTEQ = 11
    DIVEQ = 12
    MODEQ = 13
    EXPEQ = 14

    # Logical operators
    OR = 15
    AND = 16
    NOT = 17

    # Relational operators
    EQ1 = 18
    EQ2 = 19
    NE1 = 20
    NE2 = 21
    LE = 22
    GE = 23
    LT = 24
    GT = 25

    # Arithmetic operators
    PLUS = 26
    MINUS = 27
    ASTERISK = 28
    SLASH = 29
    PERCENT = 30
    CARET = 31

    # Miscellaneous operators
    DOLLAR = 32
    QUESTION = 33
    COLON = 34
    AT = 35
    HASHOP = 36

    # Keywords
    FUNCTION = 37
    PROCEDURE = 38
    RETURN = 39
    NIL = 40
    LOCAL = 41
    STATIC = 42
    IIF = 43
    IF = 44
    ELSE = 45
    ELSEIF = 46
    END = 47
    ENDIF = 48
    ENDERR = 49
    WHILE = 50
    ENDWHILE = 51

    # Literals
    STR_LITERAL = 52
    NUM_LITERAL = 53
    BOOL_LITERAL = 54

    # Identifiers
    NAME = 55

    # Preprocessor directives
    INCLUDE_DIRECTIVE = 56
    DEFINE_DIRECTIVE = 57
    IFDEF_DIRECTIVE = 58
    IFNDEF_DIRECTIVE = 59
    ELIF_DIRECTIVE = 60
    ELSE_DIRECTIVE = 61
    ENDIF_DIRECTIVE = 62
    UNDEF_DIRECTIVE = 63
    PRAGMA_DIRECTIVE = 64
    COMMAND_DIRECTIVE = 65
    XCOMMAND_DIRECTIVE = 66
    TRANSLATE_DIRECTIVE = 67
    XTRANSLATE_DIRECTIVE = 68
    ERROR_DIRECTIVE = 69
    STDOUT_DIRECTIVE = 70

    # Comments
    BLOCK_COMMENT = 71
    LINE_COMMENT = 72

    # Spacing
    EOF = 73

    def preprocessor_directive(self) -> str | None:
        match self:
            case TokenType.INCLUDE_DIRECTIVE:
                return "<include>"
            case TokenType.DEFINE_DIRECTIVE:
                return "<define>"
            case TokenType.IFDEF_DIRECTIVE:
                return "<ifdef>"
            case TokenType.IFNDEF_DIRECTIVE:
                return "<ifndef>"
            case TokenType.ELIF_DIRECTIVE:
                return "<elif>"
            case TokenType.ELSE_DIRECTIVE:
                return "<else>"
            case TokenType.ENDIF_DIRECTIVE:
                return "<endif>"
            case TokenType.UNDEF_DIRECTIVE:
                return "<undef>"
            case TokenType.PRAGMA_DIRECTIVE:
                return "<pragma>"
            case TokenType.COMMAND_DIRECTIVE:
                return "<command>"
            case TokenType.XCOMMAND_DIRECTIVE:
                return "<xcommand>"
            case TokenType.TRANSLATE_DIRECTIVE:
                return "<translate>"
            case TokenType.XTRANSLATE_DIRECTIVE:
                return "<xtranslate>"
            case TokenType.ERROR_DIRECTIVE:
                return "<error>"
            case TokenType.STDOUT_DIRECTIVE:
                return "<stdout>"
            case _:
                return None

    def literal(self) -> str | None:
        match self:
            case TokenType.BOOL_LITERAL:
                return "<bool>"
            case TokenType.NUM_LITERAL:
                return "<num>"
            case TokenType.STR_LITERAL:
                return "<str>"
            case TokenType.NIL:
                return "<nil>"
            case _:
                return None

    def keyword(self) -> str | None:
        match self:
            case TokenType.FUNCTION:
                return "function"
            case TokenType.PROCEDURE:
                return "procedure"
            case TokenType.RETURN:
                return "return"
            case TokenType.NIL:
                return "nil"
            case TokenType.LOCAL:
                return "local"
            case TokenType.STATIC:
                return "static"
            case TokenType.IIF:
                return "iif"
            case TokenType.IF:
                return "if"
            case TokenType.ELSE:
                return "else"
            case TokenType.ELSEIF:
                return "elseif"
            case TokenType.END:
                return "end"
            case TokenType.ENDIF:
                return "endif"
            case TokenType.ENDERR:
                return "enderr"
            case TokenType.WHILE:
                return "while"
            case TokenType.ENDWHILE:
                return "endwhile"
            case _:
                return None

    def compound_operator(self) -> str | None:
        match self:
            case TokenType.ASSIGN:
                return ":="
            case TokenType.PLUSEQ:
                return "+="
            case TokenType.MINUSEQ:
                return "-="
            case TokenType.MULTEQ:
                return "*="
            case TokenType.DIVEQ:
                return "/="
            case TokenType.MODEQ:
                return "%="
            case TokenType.EXPEQ:
                return "^="
            case TokenType.AND:
                return ".and."
            case TokenType.OR:
                return ".or."
            case TokenType.EQ1:
                return "=="
            case TokenType.NE2:
                return "!="
            case TokenType.LE:
                return "<="
            case TokenType.GE:
                return ">="
            case TokenType.HASHOP:
                return "=>"
            case _:
                return None

    def simple_operator(self) -> str | None:
        match self:
            case TokenType.LEFT_PAREN:
                return "("
            case TokenType.RIGHT_PAREN:
                return ")"
            case TokenType.LEFT_BRACKET:
                return "["
            case TokenType.RIGHT_BRACKET:
                return "]"
            case TokenType.LEFT_BRACE:
                return "{"
            case TokenType.RIGHT_BRACE:
                return "}"
            case TokenType.PIPE:
                return "|"
            case TokenType.COMMA:
                return ","
            case TokenType.EQ2:
                return "="
            case TokenType.NE1:
                return "#"
            case TokenType.LT:
                return "<"
            case TokenType.GT:
                return ">"
            case TokenType.DOLLAR:
                return "$"
            case TokenType.PLUS:
                return "+"
            case TokenType.MINUS:
                return "-"
            case TokenType.ASTERISK:
                return "*"
            case TokenType.SLASH:
                return "/"
            case TokenType.PERCENT:
                return "%"
            case TokenType.CARET:
                return "^"
            case TokenType.NOT:
                return "!"
            case TokenType.QUESTION:
                return "?"
            case TokenType.COLON:
                return ":"
            case TokenType.AT:
                return "@"
            case _:
                return None

    def comment(self) -> str | None:
        match self:
            case TokenType.BLOCK_COMMENT:
                return "<block_comment>"
            case TokenType.LINE_COMMENT:
                return "<line_comment>"
            case _:
                return None
