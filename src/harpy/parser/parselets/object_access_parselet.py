from typing import override

from harpy.ast.expressions import Expression, ObjectAccessExpression
from harpy.lexer import Token

from ..parser import Parser
from ..precedence import Precedence
from .infix_parselet import InfixParselet


class ObjectAccessParselet(InfixParselet):
    """Parselet to parse an object method call or field access like `a:b(c)` or `a:b`."""

    @override
    def parse(self, parser: Parser, left: Expression, token: Token):
        right = parser.parse()

        return ObjectAccessExpression(left=left, right=right)

    @override
    def get_precedence(self) -> int:
        return Precedence.CALL.value
