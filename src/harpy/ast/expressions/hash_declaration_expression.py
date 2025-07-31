from typing import override

from .expression import Expression


class HashDeclarationExpression(Expression):
    """A hash declaration like `{ => }` or `{ "a" => 1, "b" => 2 }`."""

    _keyvalues: dict[Expression, Expression]

    def __init__(self, elems: list[Expression]):
        self._keyvalues = elems

    @override
    def print(self):
        i = 1
        hashstr = ""

        for k, v in self._keyvalues:
            hashstr += f"{k.print()} => {v.print()}"
            if i < len(self._keyvalues) - 1:
                hashstr += ", "
                i += 1

        return "{ " + hashstr + "}"
