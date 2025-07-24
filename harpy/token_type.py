from enum import Enum


class TokenType(Enum):
    # Punctuation and grouping
    LEFT_PAREN = 0
    RIGHT_PAREN = 1
    COMMA = 2

    ASSIGN = 3
    PLUSEQ = 4
    MINUSEQ = 5
    MULTEQ = 6
    DIVEQ = 7
    MODEQ = 8
    EXPEQ = 9

    EQ1 = 10
    EQ2 = 11
    NE1 = 12
    NE2 = 13
    LE = 14
    GE = 15
    LT = 16
    GT = 17

    DOLLAR = 18
    PLUS = 19
    MINUS = 20
    ASTERISK = 21
    SLASH = 22
    PERCENT = 23
    CARET = 24
    BANG = 25
    QUESTION = 26
    COLON = 27

    # Keywords
    FUNCTION = 28
    PROCEDURE = 29
    RETURN = 30
    NIL = 31
    LOCAL = 32
    STATIC = 33
    IIF = 34
    IF = 35
    ELSE = 36
    ELSEIF = 37
    END = 38
    ENDIF = 39
    ENDERR = 40

    # Identifiers
    NAME = 41

    # Comments
    BLOCK_COMMENT = 42
    LINE_COMMENT = 43

    # Spacing
    EOF = 44

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
            case TokenType.EQ1:
                return "=="
            case TokenType.NE2:
                return "!="
            case TokenType.LE:
                return "<="
            case TokenType.GE:
                return ">="

    def simple_operator(self) -> str | None:
        match self:
            case TokenType.LEFT_PAREN:
                return "("
            case TokenType.RIGHT_PAREN:
                return ")"
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
            case TokenType.BANG:
                return "!"
            case TokenType.QUESTION:
                return "?"
            case TokenType.COLON:
                return ":"
            case _:
                return None
