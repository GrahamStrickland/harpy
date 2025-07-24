from typing import override

from ..expressions import CallExpression
from .statement import Statement


class CallStatement(Statement):
    """Represents a function call, e.g. `a(b, c)`."""

    _call_expr: CallExpression

    def __init__(self, call_expr: CallExpression):
        self._call_expr = call_expr

    @override
    def print(self) -> str:
        return self._call_expr.print()
