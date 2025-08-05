from typing import override

from harpy.ast.expressions import AssignExpression, Expression
from harpy.lexer import Token

from ..parser_base import ParserBase
from ..precedence import Precedence
from .infix_parselet import InfixParselet


class AssignParselet(InfixParselet):
    """Parses assignment expressions like `a := b`. The left side of an assignment
    expression must be a an expression where `left.left_expr() == True` and expressions
    are right-associative. (In other words, `a := b := c` is parsed as `a := (b := c)`).
    """

    @override
    def parse(self, parser: ParserBase, left: Expression, token: Token):
        del token

        right = parser.parse(Precedence.ASSIGNMENT.value - 1)

        return AssignExpression(left=left, right=right)

    @override
    def get_precedence(self) -> int:
        return Precedence.ASSIGNMENT.value
