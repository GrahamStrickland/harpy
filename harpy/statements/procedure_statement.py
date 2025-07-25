from typing import override

from .statement import Statement


class ProcedureStatement(Statement):
    """Represents a procedure declaration, e.g. `procedure a(b, c)\nreturn`."""

    _name: str
    _params: list[str]
    _body: list[Statement]
    _static: bool

    def __init__(
        self,
        name: str,
        params: list[str],
        body: list[Statement],
        static: bool = False,
    ):
        self._name = name
        self._params = params
        self._body = body
        self._static = static

    @override
    def print(self) -> str:
        params = ""
        for i, param in enumerate(self._params):
            if i != len(self._params) - 1:
                params += param.get_text() + ", "
            else:
                params += param.get_text()

        body = "\n".join([stmt.print() for stmt in self._body])

        return f"{'static ' if self._static else ''}procedure {self._name}({params})\n{body}\nreturn"
