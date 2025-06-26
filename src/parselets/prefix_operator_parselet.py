from ..expressions import Expression, PrefixExpression
from ..parser import Parser
from ..token import Token
from .prefix_parselet import PrefixParselet


class PrefixOperatorParselet(PrefixParselet):
    """Generic prefix parselet for an unary arithmetic operator. Parses prefix
    unary '-', '+', '~', and '!' expressions.
    """

    def parse(self, parser: Parser, token: Token) -> Expression:
        operand = parser.parse_expression()
        return PrefixExpression(type=token.get_type(), right=operand)
