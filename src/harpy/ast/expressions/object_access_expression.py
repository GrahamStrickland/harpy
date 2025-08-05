from typing import override

from .expression import Expression


class ObjectAccessExpression(Expression):
    """An object method call or field access like `a:b(c)` or `a:b`."""

    _left: Expression
    _right: Expression

    def __init__(self, left: Expression, right: Expression):
        super().__init__(left_expr=True)

        self._left = left
        self._right = right

    @override
    def print(self):
        return f"{self._left.print()}:{self._right.print()}"
