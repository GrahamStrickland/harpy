from enum import Enum


class TokenType(Enum):
    # Punctuation and grouping
    LEFT_PAREN = 0
    RIGHT_PAREN = 1
    COMMA = 2
    ASSIGN = 3
    PLUS = 4
    MINUS = 5
    ASTERISK = 6
    SLASH = 7
    CARET = 8
    BANG = 9
    QUESTION = 10
    COLON = 11

    # Identifiers
    NAME = 12

    # Comments
    BLOCK_COMMENT = 13
    LINE_COMMENT = 14

    # Spacing
    EOF = 15

    def punctuator(self) -> str | None:
        match self:
            case TokenType.LEFT_PAREN:
                return "("
            case TokenType.RIGHT_PAREN:
                return ")"
            case TokenType.COMMA:
                return ","
            case TokenType.PLUS:
                return "+"
            case TokenType.MINUS:
                return "-"
            case TokenType.ASTERISK:
                return "*"
            case TokenType.SLASH:
                return "/"
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
