from typing import override

from ..expressions import Expression
from .statement import Statement


class LocalVariableDeclaration(Statement):
    """Represents a local variable declaration, e.g. `local a := 1`."""

    _name: str
    _assign_expr: Expression | None

    def __init__(self, name: str, assign_expr: Expression | None):
        self._name = name
        self._assign_expr = assign_expr

    @override
    def print(self) -> str:
        if self._assign_expr is None:
            return f"local {self._name}"
        else:
            return f"local {self._assign_expr.print()}"
