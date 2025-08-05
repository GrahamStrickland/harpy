from typing import override

from .expression import Expression


class HashDeclarationExpression(Expression):
    """A hash declaration like `{ => }` or `{ "a" => 1, "b" => 2 }`."""

    _keyvalues: dict[Expression, Expression]

    def __init__(self, keyvalues: dict[Expression, Expression]):
        super().__init__(left_expr=False)

        self._keyvalues = keyvalues

    @override
    def print(self):
        i = 0
        hashstr = ""

        if len(self._keyvalues) > 0:
            for k, v in self._keyvalues.items():
                hashstr += f"{k.print()} => {v.print()}"
                if i < len(self._keyvalues) - 1:
                    hashstr += ", "
                    i += 1
                else:
                    hashstr += " "

            return "{ " + hashstr + "}"

        return "{ => }"
