from typing import override

from ..token import Token
from .expression import Expression


class LiteralExpression(Expression):
    """A boolean, numeric, or string literal, e.g., .t., 123, or 'hello'."""

    _literal: Expression

    def __init__(self, literal: Token):
        self._literal = literal

    @override
    def print(self):
        return self._literal.text
