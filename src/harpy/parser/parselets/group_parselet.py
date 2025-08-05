from typing import override

from harpy.ast.expressions import Expression
from harpy.lexer import Token, TokenType

from ..parser import Parser
from .prefix_parselet import PrefixParselet


class GroupParselet(PrefixParselet):
    """Parses parentheses used to group an expression, like `a * (b + c)`."""

    @override
    def parse(self, parser: Parser, token: Token) -> Expression:
        del token

        expression = parser.parse()
        parser.consume(TokenType.RIGHT_PAREN)
        return expression
