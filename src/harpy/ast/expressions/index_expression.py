from typing import override

from .expression import Expression


class IndexExpression(Expression):
    """An index into a hash or array like `a[b]`."""

    _left: Expression
    _right: Expression

    def __init__(self, left: Expression, right: list[Expression]):
        self._left_expr = True
        self._left = left
        self._right = right

    @override
    def print(self):
        index_exprs = ""
        for i, expr in enumerate(self._right):
            index_exprs += expr.print()
            if i < len(self._right) - 1:
                index_exprs += ", "

        return f"{self._left.print()}[{index_exprs}]"
