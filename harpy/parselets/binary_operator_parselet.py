from ..expressions import Expression, OperatorExpression
from ..parser import Parser
from ..token import Token
from .infix_parselet import InfixParselet


class BinaryOperatorParselet(InfixParselet):
    """Generic infix parselet for a binary arithmetic operator. The only
    difference when parsing, '+', '-', '*', '/', and '^' is precedence and
    associativity, so we can use a single parselet class for all of those.
    """

    _precedence: int
    _is_right: bool

    def __init__(self, precedence: int, is_right: bool):
        self._precedence = precedence
        self._is_right = is_right

    def parse(self, parser: Parser, left: Expression, token: Token) -> Expression:
        """To handle right-associative operators like '^', we allow a slightly
        lower precedence when parsing the right-hand side. This will let a
        parselet with the same precedence appear on the right, which will then
        take *this* parselet's result as its left-hand argument.
        """
        right = parser.parse(self._precedence - (1 if self._is_right else 0))
        return OperatorExpression(left=left, operator=token.type, right=right)

    def get_precedence(self) -> int:
        return self._precedence
