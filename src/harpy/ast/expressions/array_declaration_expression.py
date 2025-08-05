from typing import override

from .expression import Expression


class ArrayDeclarationExpression(Expression):
    """An array declaration like `{ }` or `{ b, c, d }`."""

    _elems: list[Expression]

    def __init__(self, elems: list[Expression]):
        super().__init__(left_expr=False)

        self._elems = elems

    @override
    def print(self):
        elemstr = ""
        for i, elem in enumerate(self._elems):
            elemstr += elem.print()
            if i < len(self._elems) - 1:
                elemstr += ", "
            else:
                elemstr += " "

        return "{ " + elemstr + "}"
