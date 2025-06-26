from typing import override

from ..token_type import TokenType
from .expression import Expression


class OperatorExpression(Expression):
    """A binary arithmetic expression like `a + b` or `c ^ d`."""

    _left: Expression
    _right: Expression
    _operator: TokenType

    def __init__(self, left: Expression, operator: TokenType, right: Expression):
        self._left = left
        self._right = right
        self._operator = operator

    @override
    def print(self) -> str:
        return f"({self._left.print()} {self._operator.punctuator()} {self._right.print()})"
