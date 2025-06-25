from .prefix_parselet import PrefixParselet
from ..expressions import PrefixExpression
from ..parser import Parser
from ..token import Token


class PrefixOperatorParselet(PrefixParselet):
    def parse(self, parser: Parser, token: Token):
        operand = parser.parse_expression()
        return PrefixExpression(type=token.get_type(), operand=operand)
