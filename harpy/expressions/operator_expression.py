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
        if self._operator.punctuator() is None:
            op = None
            match self._operator:
                case TokenType.EQ:
                    op = "=="
                case TokenType.LE:
                    op = "<="
                case TokenType.GE:
                    op = ">="
            return f"({self._left.print()} {op} {self._right.print()})"
        else:
            return f"({self._left.print()} {self._operator.punctuator()} {self._right.print()})"
