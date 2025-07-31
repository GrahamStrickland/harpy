from typing import override

from ..expressions import Expression
from .statement import Statement


class IfStatement(Statement):
    """Represents an `if` statement with optional `elseif` and `else` conditions."""

    _ifcond: Expression
    _elifs: list[tuple[Expression, list[Statement]]]
    _ifbody: list[Statement]
    _elsebody: list[Statement]

    def __init__(
        self,
        ifcond: Expression,
        elifs: list[tuple[Expression, list[Statement]]],
        ifbody: list[Statement],
        elsebody: list[Statement],
    ):
        self._ifcond = ifcond
        self._elifs = elifs
        self._ifbody = ifbody
        self._elsebody = elsebody

    @override
    def print(self) -> str:
        outer_stmt = f"if {self._ifcond.print()}\n"
        for stmt in self._ifbody:
            outer_stmt += stmt.print() + "\n"
        for cond, body in self._elifs:
            elifbody = "\n".join([stmt.print() for stmt in body])
            outer_stmt += f"elseif {cond.print()}\n{elifbody}\n"
        if len(self._elsebody) > 0:
            outer_stmt += "else\n"
            for stmt in self._elsebody:
                outer_stmt += f"{stmt.print()}\n"

        return outer_stmt + "endif"
