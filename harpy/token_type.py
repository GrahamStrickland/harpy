from enum import Enum


class TokenType(Enum):
    LEFT_PAREN = 0
    RIGHT_PAREN = 1
    COMMA = 2
    ASSIGN = 3
    PLUS = 4
    MINUS = 5
    ASTERISK = 6
    SLASH = 7
    CARET = 8
    TILDE = 9
    BANG = 10
    QUESTION = 11
    COLON = 12
    NAME = 13
    EOF = 14

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
            case TokenType.TILDE:
                return "~"
            case TokenType.BANG:
                return "!"
            case TokenType.QUESTION:
                return "?"
            case TokenType.COLON:
                return ":"
            case _:
                return None
