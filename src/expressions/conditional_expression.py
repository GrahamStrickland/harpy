from typing import override

from .expression import Expression


class ConditionalExpression(Expression):
    """A ternary conditional expression like 'a ? b : c'."""

    _condition: Expression
    _then_arm: Expression
    _else_arm: Expression

    def __init__(
        self, condition: Expression, then_arm: Expression, else_arm: Expression
    ):
        self._condition = condition
        self._then_arm = then_arm
        self._elese_arm = else_arm

    @override
    def print(self) -> str:
        return f"({self._condition.print()} ? {self._then_arm.print()} : {self._else_arm.print()})"
