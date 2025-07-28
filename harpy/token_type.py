from enum import Enum


class TokenType(Enum):
    # Punctuation and grouping
    LEFT_PAREN = 0
    RIGHT_PAREN = 1
    COMMA = 2

    # Assignment operators
    ASSIGN = 3
    PLUSEQ = 4
    MINUSEQ = 5
    MULTEQ = 6
    DIVEQ = 7
    MODEQ = 8
    EXPEQ = 9

    # Logical operators
    OR = 10
    AND = 11
    NOT = 12

    # Relational operators
    EQ1 = 13
    EQ2 = 14
    NE1 = 15
    NE2 = 16
    LE = 17
    GE = 18
    LT = 19
    GT = 20

    # Arithmetic operators
    DOLLAR = 21
    PLUS = 22
    MINUS = 23
    ASTERISK = 24
    SLASH = 25
    PERCENT = 26
    CARET = 27
    QUESTION = 28
    COLON = 29

    # Keywords
    FUNCTION = 30
    PROCEDURE = 31
    RETURN = 32
    NIL = 33
    LOCAL = 34
    STATIC = 35
    IIF = 36
    IF = 37
    ELSE = 38
    ELSEIF = 39
    END = 40
    ENDIF = 41
    ENDERR = 42
    
    # Literals
    STR_LITERAL = 43
    NUM_LITERAL = 44
    BOOL_LITERAL = 45

    # Identifiers
    NAME = 43

    # Comments
    BLOCK_COMMENT = 44
    LINE_COMMENT = 45

    # Spacing
    EOF = 46

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
            case TokenType.NOT:
                return "!"
            case TokenType.QUESTION:
                return "?"
            case TokenType.COLON:
                return ":"
            case _:
                return None
