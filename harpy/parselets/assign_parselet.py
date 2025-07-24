from typing import override

from ..expressions import AssignExpression, Expression, NameExpression
from ..parser import Parser
from ..precedence import Precedence
from ..token import Token
from .infix_parselet import InfixParselet


class AssignParselet(InfixParselet):
    """Parses assignment expressions like `a := b`. The left side of an assignment
    expression must be a simple name like 'a', and expressions are
    right-associative. (In other words, `a := b := c` is parsed as `a := (b := c)`).
    """

    @override
    def parse(self, parser: Parser, left: Expression, token: Token):
        right = parser.parse(Precedence.ASSIGNMENT.value - 1)

        if not isinstance(left, NameExpression):
            raise SyntaxError("The left-hand side of an assignment must be a name.")

        name = left.print()
        return AssignExpression(name=name, right=right)

    @override
    def get_precedence(self) -> int:
        return Precedence.ASSIGNMENT.value
