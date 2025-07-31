from typing import override

from .expression import Expression


class CallExpression(Expression):
    """A function call like `a(b, c, d)`."""

    _function: Expression
    _args: list[Expression]

    def __init__(self, function: Expression, args: list[Expression]):
        self._function = function
        self._args = args

    @override
    def print(self):
        argstr = ""
        for i, arg in enumerate(self._args):
            argstr += arg.print()
            if i < len(self._args) - 1:
                argstr += ", "

        return f"{self._function.print()}({argstr})"
