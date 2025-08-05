from typing import override

from .expression import Expression


class NameExpression(Expression):
    """A simple variable name expression like 'abc'."""

    _name: str

    def __init__(self, name: str):
        super().__init__(left_expr=True)

        self._name = name

    @override
    def print(self) -> str:
        return self._name
