from typing import override

from .expression import Expression


class ConditionalExpression(Expression):
    """A conditional or 'ternary' operator, like `iif(a, b, c)`."""

    _if_arm: Expression
    _then_arm: Expression
    _else_arm: Expression

    def __init__(self, if_arm: Expression, then_arm: Expression, else_arm: Expression):
        super().__init__(left_expr=True)

        self._if_arm = if_arm
        self._then_arm = then_arm
        self._else_arm = else_arm

    @override
    def print(self):
        return f"iif({self._if_arm.print()}, {self._then_arm.print()}, {self._else_arm.print()})"
