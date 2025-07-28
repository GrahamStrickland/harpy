from typing import override

from ..expressions import Expression, IndexExpression
from ..parser import Parser
from ..precedence import Precedence
from ..token import Token
from ..token_type import TokenType
from .infix_parselet import InfixParselet


class IndexParselet(InfixParselet):
    """Parselet to parse a function call like `a[b]`."""

    @override
    def parse(self, parser: Parser, left: Expression, token: Token):
        index_exprs = [parser.parse()]

        if not parser.match(TokenType.RIGHT_BRACKET):
            while parser.match(TokenType.COMMA):
                parser.consume(TokenType.COMMA)
                index_exprs.append(parser.parse())

        parser.consume(TokenType.RIGHT_BRACKET)

        return IndexExpression(left=left, right=index_exprs)

    @override
    def get_precedence(self) -> int:
        return Precedence.CALL.value
