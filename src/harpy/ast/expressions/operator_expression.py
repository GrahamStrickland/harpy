from typing import override

from harpy.lexer import TokenType

from .expression import Expression


class OperatorExpression(Expression):
    """A binary arithmetic expression like `a + b` or `c ^ d`."""

    _left: Expression
    _right: Expression
    _operator: TokenType

    def __init__(self, left: Expression, operator: TokenType, right: Expression):
        super().__init__(left_expr=False)

        self._left = left
        self._right = right
        self._operator = operator

    @override
    def print(self) -> str:
        if self._operator.compound_operator() is not None:
            return f"({self._left.print()} {self._operator.compound_operator()} {self._right.print()})"
        elif self._operator.simple_operator() is not None:
            return f"({self._left.print()} {self._operator.simple_operator()} {self._right.print()})"
        else:
            raise SyntaxError(f"Invalid operator '{self._operator}")
