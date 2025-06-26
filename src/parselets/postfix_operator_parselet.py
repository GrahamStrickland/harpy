from ..expressions import Expression, PostfixExpression
from ..parser import Parser
from ..token import Token
from .infix_parselets import InfixParselet


class PostfixOperatorParselet(InfixParselet):
    """Generic infix parselet for an unary arithmetic operator. Parses postfix
    unary '?' expressions."""

    def parse(self, parser: Parser, left: Expression, token: Token) -> Expression:
        return PostfixExpression(left=left, operator=token.get_type())
