from typing import override

from ..expressions import Expression
from ..parser import Parser
from ..token import Token
from ..token_type import TokenType
from .prefix_parselet import PrefixParselet


class GroupParselet(PrefixParselet):
    """Parses parentheses used to group an expression, like `a * (b + c)`."""

    @override
    def parse(self, parser: Parser, token: Token) -> Expression:
        expression = parser.parse()
        parser.consume(TokenType.RIGHT_PAREN)
        return expression
