from typing import override

from harpy.lexer import Token

from .expression import Expression


class LiteralExpression(Expression):
    """A boolean, numeric, string, or nil literal, e.g., `.t.`, `123`, `'hello'`, or `NIL`."""

    _literal: Expression

    def __init__(self, literal: Token):
        self._literal = literal

    @override
    def print(self):
        return self._literal.text
