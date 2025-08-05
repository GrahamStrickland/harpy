from typing import override

from ..expressions import Expression
from .statement import Statement


class FunctionStatement(Statement):
    """Represents a function declaration, e.g. `function a(b, c)\nreturn c`."""

    _name: str
    _params: list[str]
    _body: list[Statement]
    _retval: Expression
    _static: bool

    def __init__(
        self,
        name: str,
        params: list[str],
        body: list[Statement],
        retval: Expression,
        static: bool = False,
    ):
        super().__init__()

        self._name = name
        self._params = params
        self._body = body
        self._retval = retval
        self._static = static

    @override
    def print(self) -> str:
        params = ""
        for i, param in enumerate(self._params):
            if i != len(self._params) - 1:
                params += param.text + ", "
            else:
                params += param.text

        body = "\n".join([stmt.print() for stmt in self._body])

        return f"{'static ' if self._static else ''}function {self._name}({params})\n{body}\nreturn {self._retval.print()}"
