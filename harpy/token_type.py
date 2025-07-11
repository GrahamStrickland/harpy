from enum import Enum


class TokenType(Enum):
    # Punctuation and grouping
    LEFT_PAREN = 0
    RIGHT_PAREN = 1
    COMMA = 2
    ASSIGN = 3
    EQ1 = 4
    EQ2 = 5
    NE1 = 6
    NE2 = 7
    LE = 8
    GE = 9
    LT = 10
    GT = 11
    PLUS = 12
    MINUS = 13
    ASTERISK = 14
    SLASH = 15
    PERCENT = 16
    CARET = 17
    BANG = 18
    QUESTION = 19
    COLON = 20

    # Identifiers
    NAME = 21

    # Comments
    BLOCK_COMMENT = 22
    LINE_COMMENT = 23

    # Spacing
    EOF = 24

    def punctuator(self) -> str | None:
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
