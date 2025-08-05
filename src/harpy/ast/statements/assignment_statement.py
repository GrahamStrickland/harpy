from typing import override

from ..expressions import Expression
from .statement import Statement


class AssignmentStatement(Statement):
    """Represents a variable assignment, e.g. `b := 1`."""

    _assign_expr: Expression

    def __init__(self, assign_expr: Expression):
        super().__init__()

        self._assign_expr = assign_expr

    @override
    def print(self) -> str:
        return self._assign_expr.print()
