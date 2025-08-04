from typing import override

from ..expressions import Expression
from .statement import Statement


class ReturnStatement(Statement):
    """Represents a `return` statement with optional return value."""

    _retval: Expression | None

    def __init__(self, retval: Expression | None):
        self._retval = retval

    @override
    def print(self) -> str:
        return "return" + (
            " " + self._retval.print() if self._retval is not None else ""
        )

    def return_value(self) -> Expression | None:
        return self._retval
