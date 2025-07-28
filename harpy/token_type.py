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
    DOLLAR = 25
    PLUS = 26
    MINUS = 27
    ASTERISK = 28
    SLASH = 29
    PERCENT = 30
    CARET = 31
    QUESTION = 32
    COLON = 33

    # Keywords
    FUNCTION = 34
    PROCEDURE = 35
    RETURN = 36
    NIL = 37
    LOCAL = 38
    STATIC = 39
    IIF = 40
    IF = 41
    ELSE = 42
    ELSEIF = 43
    END = 44
    ENDIF = 45
    ENDERR = 46

    # Literals
    STR_LITERAL = 47
    NUM_LITERAL = 48
    BOOL_LITERAL = 49

    # Identifiers
    NAME = 50

    # Comments
    BLOCK_COMMENT = 51
    LINE_COMMENT = 52

    # Spacing
    EOF = 53

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
            case _:
                return None
