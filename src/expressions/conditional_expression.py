from dataclasses import dataclass

from .expression import Expression


@dataclass
class ConditionalExpression(Expression):
    condition: Expression
    then_arm: Expression
    else_arm: Expression
