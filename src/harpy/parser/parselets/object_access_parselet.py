from typing import override

from harpy.ast.expressions import Expression, ObjectAccessExpression
from harpy.lexer import Token

from ..parser_base import ParserBase
from ..precedence import Precedence
from .infix_parselet import InfixParselet


class ObjectAccessParselet(InfixParselet):
    """Parselet to parse an object method call or field access like `a:b(c)` or `a:b`."""

    @override
    def parse(self, parser: ParserBase, left: Expression, token: Token):
        del token

        right = parser.parse(precedence=self.get_precedence())

        return ObjectAccessExpression(left=left, right=right)

    @override
    def get_precedence(self) -> int:
        return Precedence.ACCESS.value
