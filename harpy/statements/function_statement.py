from typing import override

from ..expressions import Expression
from .statement import Statement


class FunctionStatement(Statement):
    """Represents a function declaration, e.g. `function a(b, c)\nreturn c`."""

    _name: str
    _params: list[str]
    _body: list[Statement]
    _retval: Expression

    def __init__(
        self, name: str, params: list[str], body: list[Statement], retval: Expression
    ):
        self._name = name
        self._params = params
        self._body = body
        self._retval = retval

    @override
    def print(self) -> str:
        params = ""
        for i, param in enumerate(self._params):
            if i != len(self._params) - 1:
                params += param.get_text() + ", "
            else:
                params += param.get_text()

        body = "\n".join([stmt.print() for stmt in self._body])

        return f"function {self._name}({params})\n{body}\nreturn {self._retval.print()}"
