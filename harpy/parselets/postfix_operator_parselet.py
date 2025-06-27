from ..expressions import Expression, PostfixExpression
from ..parser import Parser
from ..token import Token
from .infix_parselet import InfixParselet


class PostfixOperatorParselet(InfixParselet):
    """Generic infix parselet for an unary arithmetic operator. Parses postfix
    unary `?` expressions."""

    _precedence: int

    def __init__(self, precedence: int):
        self._precedence = precedence

    def parse(self, parser: Parser, left: Expression, token: Token) -> Expression:
        return PostfixExpression(left=left, operator=token.get_type())

    def get_precedence(self) -> int:
        return self._precedence
