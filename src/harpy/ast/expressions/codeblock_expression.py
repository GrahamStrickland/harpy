
from typing import override

from .expression import Expression
from .name_expression import NameExpression


class CodeblockExpression(Expression):
    """A codeblock like `{ |a| b, c }`."""

    _params: list[NameExpression]
    _exprs: list[Expression]

    def __init__(self, params: list[NameExpression], exprs: list[Expression]):
        self._params = params
        self._exprs = exprs

    @override
    def print(self):
        paramstr = ""
        for i, param in enumerate(self._params):
            paramstr += param.print()
            if i < len(self._params) - 1:
                paramstr += ", "

        exprstr = ""
        for i, expr in enumerate(self._exprs):
            exprstr += expr.print()
            if i < len(self._exprs) - 1:
                exprstr += ", "

        return f"{{ |{paramstr}| {exprstr} }}"
