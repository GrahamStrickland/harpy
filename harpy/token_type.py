from enum import Enum


class TokenType(Enum):
    # Punctuation and grouping
    LEFT_PAREN = 0
    RIGHT_PAREN = 1
    COMMA = 2
    ASSIGN = 3
    EQ = 4
    LE = 5
    GE = 6
    LT = 7
    GT = 8
    PLUS = 9
    MINUS = 10
    ASTERISK = 11
    SLASH = 12
    PERCENT = 13
    CARET = 14
    BANG = 15
    QUESTION = 16
    COLON = 17

    # Identifiers
    NAME = 18

    # Comments
    BLOCK_COMMENT = 19
    LINE_COMMENT = 20

    # Spacing
    EOF = 21

    def punctuator(self) -> str | None:
        match self:
            case TokenType.LEFT_PAREN:
                return "("
            case TokenType.RIGHT_PAREN:
                return ")"
            case TokenType.COMMA:
                return ","
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
