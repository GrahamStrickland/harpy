from typing import override

from harpy.ast.expressions import ConditionalExpression
from harpy.lexer import Token, TokenType

from ..parser_base import ParserBase
from ..precedence import Precedence
from .prefix_parselet import PrefixParselet


class ConditionalParselet(PrefixParselet):
    """Parselet for the conditional or 'ternary' operator, like `iif(a, b, c)`."""

    @override
    def parse(self, parser: ParserBase, token: Token) -> ConditionalExpression:
        del token

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
