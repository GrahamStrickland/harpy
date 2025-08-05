from typing import override

from harpy.lexer.token_type import TokenType

from .expression import Expression


class PrefixExpression(Expression):
    """A prefix unary arithmetic expression like `!a` or `-b`."""

    _operator: TokenType
    _right: Expression

    def __init__(self, operator: TokenType, right: Expression):
        self._left_expr = False
        self._operator = operator
        self._right = right

    @override
    def print(self) -> str:
        return f"({self._operator.simple_operator()}{self._right.print()})"
