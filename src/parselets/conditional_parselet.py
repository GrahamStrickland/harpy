from ..expressions import ConditionalExpression, Expression
from ..parser import Parser
from ..precedence import Precedence
from ..token import Token
from ..token_type import TokenType
from .infix_parselet import InfixParselet


class ConditionalParselet(InfixParselet):
    """Parselet for the condition or 'ternary' operator, like `a ? b : c`."""

    def parse(self, parser: Parser, left: Expression, token: Token) -> Expression:
        then_arm = parser.parse_expression()
        parser.consume(TokenType.COLON.value)
        else_arm = parser.parse_expression(Precedence.CONDITIONAL.value - 1)

        return ConditionalExpression(left=left, then_arm=then_arm, else_arm=else_arm)

    def get_precedence(self) -> int:
        return Precedence.CONDITIONAL.value
