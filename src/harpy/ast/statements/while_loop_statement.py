from typing import override

from ..expressions import Expression
from .statement import Statement


class WhileLoopStatement(Statement):
    """Represents a `while` loop."""

    _cond: Expression
    _body: list[Statement]

    def __init__(
        self,
        cond: Expression,
        body: list[Statement],
    ):
        self._cond = cond 
        self._body = body

    @override
    def print(self) -> str:
        loop_stmt = f"while {self._cond.print()}\n"
        for stmt in self._body:
            loop_stmt += stmt.print() + "\n"

        return loop_stmt + "end while"
