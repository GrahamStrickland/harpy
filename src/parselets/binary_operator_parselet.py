from ..expressions import Expression, OperatorExpression
from ..parser import Parser
from ..token import Token
from .infix_parselets import InfixParselet


class BinaryOperatorParselet(InfixParselet):
    """Generic infix parselet for a binary arithmetic operator. The only
    difference when parsing, '+', '-', '*', '/', and '^' is precedence and
    associativity, so we can use a single parselet class for all of those.
    """

    def parse(self, parser: Parser, left: Expression, token: Token) -> Expression:
        right = parser.parse_expression()
        return OperatorExpression(left=left, operator=token.get_type(), right=right)
