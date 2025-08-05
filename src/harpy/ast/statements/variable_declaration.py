from typing import override

from ..expressions import Expression
from .statement import Statement


class VariableDeclaration(Statement):
    """Represents a local or static variable declaration, e.g. `local a` or `static b := 1`."""

    _variable_type: str
    _name: str
    _assign_expr: Expression | None

    def __init__(self, variable_type: str, name: str, assign_expr: Expression | None):
        super().__init__()

        self._variable_type = variable_type
        self._name = name
        self._assign_expr = assign_expr

    @override
    def print(self) -> str:
        if self._assign_expr is None:
            return f"{self._variable_type} {self._name}"
        else:
            return f"{self._variable_type} {self._assign_expr.print()}"
