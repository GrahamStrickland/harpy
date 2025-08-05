from typing import override

from harpy.ast.expressions import ConditionalExpression
from harpy.lexer import Token, TokenType

from ..parser import Parser
from ..precedence import Precedence
from .prefix_parselet import PrefixParselet


class ConditionalParselet(PrefixParselet):
    """Parselet for the conditional or 'ternary' operator, like `iif(a, b, c)`."""

    @override
    def parse(self, parser: Parser, token: Token) -> ConditionalExpression:
        parser.consume(TokenType.LEFT_PAREN)

        if_arm = parser.parse()
        parser.consume(TokenType.COMMA)
        then_arm = parser.parse()
        parser.consume(TokenType.COMMA)
        else_arm = parser.parse()

        parser.consume(TokenType.RIGHT_PAREN)

        return ConditionalExpression(
            if_arm=if_arm, then_arm=then_arm, else_arm=else_arm
        )

    @override
    def get_precedence(self) -> int:
        return Precedence.NONE.value
