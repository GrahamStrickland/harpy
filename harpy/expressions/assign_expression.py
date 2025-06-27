from typing import override

from .expression import Expression


class AssignExpression(Expression):
    """An assignment expression like `a := b`."""

    _name: str
    _right: Expression

    def __init__(self, name: str, right: Expression):
        self._name = name
        self._right = right

    @override
    def print(self):
        return f"({self._name} := {self._right.print()})"
