from ..expressions import Expression, PrefixExpression
from ..parser import Parser
from ..token import Token
from .prefix_parselet import PrefixParselet


class PrefixOperatorParselet(PrefixParselet):
    """Generic prefix parselet for an unary arithmetic operator. Parses prefix
    unary `-`, `+`, `~`, and `!` expressions.
    """

    _precedence: int

    def __init__(self, precedence: int):
        self._precedence = precedence

    def parse(self, parser: Parser, token: Token) -> Expression:
        """To handle right-associative operators like `^`, we allow a slightly
        lower precedence when parsing the right-hand side. This will let a
        parselet with the same precedence appear on the right, which will then
        take *this* parselet's result as its left-hand argument
        """
        right = parser.parse_expression(self._precedence)

        return PrefixExpression(type=token.get_type(), right=right)

    def get_precedence(self) -> int:
        return self._precedence
