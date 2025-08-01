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

    # Assignment operators
    ASSIGN = 7
    PLUSEQ = 8
    MINUSEQ = 9
    MULTEQ = 10
    DIVEQ = 11
    MODEQ = 12
    EXPEQ = 13

    # Logical operators
    OR = 14
    AND = 15
    NOT = 16

    # Relational operators
    EQ1 = 17
    EQ2 = 18
    NE1 = 19
    NE2 = 20
    LE = 21
    GE = 22
    LT = 23
    GT = 24

    # Arithmetic operators
    PLUS = 25
    MINUS = 26
    ASTERISK = 27
    SLASH = 28
    PERCENT = 29
    CARET = 30

    # Miscellaneous operators
    DOLLAR = 31
    QUESTION = 32
    COLON = 33
    AT = 34
    HASHOP = 35

    # Keywords
    FUNCTION = 36
    PROCEDURE = 37
    RETURN = 38
    NIL = 39
    LOCAL = 40
    STATIC = 41
    IIF = 42
    IF = 43
    ELSE = 44
    ELSEIF = 45
    END = 46
    ENDIF = 47
    ENDERR = 48

    # Literals
    STR_LITERAL = 49
    NUM_LITERAL = 50
    BOOL_LITERAL = 51

    # Identifiers
    NAME = 52

    # Preprocessor directives
    INCLUDE_DIRECTIVE = 53
    DEFINE_DIRECTIVE = 54
    IFDEF_DIRECTIVE = 55
    IFNDEF_DIRECTIVE = 56
    ELIF_DIRECTIVE = 57
    ELSE_DIRECTIVE = 58
    ENDIF_DIRECTIVE = 59
    UNDEF_DIRECTIVE = 60
    PRAGMA_DIRECTIVE = 61
    COMMAND_DIRECTIVE = 62
    XCOMMAND_DIRECTIVE = 63
    TRANSLATE_DIRECTIVE = 64
    XTRANSLATE_DIRECTIVE = 65
    ERROR_DIRECTIVE = 66
    STDOUT_DIRECTIVE = 67

    # Comments
    BLOCK_COMMENT = 68
    LINE_COMMENT = 69

    # Spacing
    EOF = 70

    def preprocessor_directive(self) -> str | None:
        match self:
            case TokenType.INCLUDE_DIRECTIVE:
                return "include"
            case TokenType.DEFINE_DIRECTIVE:
                return "define"
            case TokenType.IFDEF_DIRECTIVE:
                return "ifdef"
            case TokenType.IFNDEF_DIRECTIVE:
                return "ifndef"
            case TokenType.ELIF_DIRECTIVE:
                return "elif"
            case TokenType.ELSE_DIRECTIVE:
                return "else"
            case TokenType.ENDIF_DIRECTIVE:
                return "endif"
            case TokenType.UNDEF_DIRECTIVE:
                return "undef"
            case TokenType.PRAGMA_DIRECTIVE:
                return "pragma"
            case TokenType.COMMAND_DIRECTIVE:
                return "command"
            case TokenType.XCOMMAND_DIRECTIVE:
                return "xcommand"
            case TokenType.TRANSLATE_DIRECTIVE:
                return "translate"
            case TokenType.XTRANSLATE_DIRECTIVE:
                return "xtranslate"
            case TokenType.ERROR_DIRECTIVE:
                return "error"
            case TokenType.STDOUT_DIRECTIVE:
                return "stdout"
            case _:
                return None

    def literal(self) -> str | None:
        match self:
            case TokenType.BOOL_LITERAL:
                return "bool"
            case TokenType.NUM_LITERAL:
                return "num"
            case TokenType.STR_LITERAL:
                return "str"
            case TokenType.NIL:
                return "nil"
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
