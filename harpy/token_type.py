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

    # Keywords
    FUNCTION = 35
    PROCEDURE = 36
    RETURN = 37
    NIL = 38
    LOCAL = 39
    STATIC = 40
    IIF = 41
    IF = 42
    ELSE = 43
    ELSEIF = 44
    END = 45
    ENDIF = 46
    ENDERR = 47

    # Literals
    STR_LITERAL = 48
    NUM_LITERAL = 49
    BOOL_LITERAL = 50

    # Identifiers
    NAME = 51

    # Comments
    BLOCK_COMMENT = 52
    LINE_COMMENT = 53

    # Spacing
    EOF = 54

    def literal(self) -> str | None:
        match self:
            case TokenType.BOOL_LITERAL:
                return "bool"
            case TokenType.NUM_LITERAL:
                return "num"
            case TokenType.STR_LITERAL:
                return "str"
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
