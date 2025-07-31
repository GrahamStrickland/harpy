from typing import override

from .expression import Expression


class ArrayDeclarationExpression(Expression):
    """An array declaration like `{ }` or `{ b, c, d }`."""

    _elems: list[Expression]

    def __init__(self, elems: list[Expression]):
        self._elems = elems

    @override
    def print(self):
        elemstr = ""
        for i, arg in enumerate(self._args):
            elemstr += arg.print()
            if i < len(self._args) - 1:
                elemstr += ", "

        return "{ " + elemstr + "}"
