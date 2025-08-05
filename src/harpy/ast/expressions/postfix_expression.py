from typing import override

from harpy.lexer import TokenType

from .expression import Expression


class PostfixExpression(Expression):
    """A postfix unary arithmetic expression like `a!`."""

    _left: Expression
    _operator: TokenType

    def __init__(self, left: Expression, operator: TokenType):
        super().__init__(left_expr=False)

        self._left = left
        self._operator = operator

    @override
    def print(self) -> str:
        return f"({self._left.print()}{self._operator.simple_operator()})"
