from typing import override

from harpy.ast.expressions import CallExpression, Expression
from harpy.lexer import Token, TokenType

from ..parser import Parser
from ..precedence import Precedence
from .infix_parselet import InfixParselet


class CallParselet(InfixParselet):
    """Parselet to parse a function call like `a(b, c, d)`."""

    @override
    def parse(self, parser: Parser, left: Expression, token: Token) -> CallExpression:
        # Parse the comma-separated arguments until we hit ", )".
        args: list[Expression] = []

        # There may be no arguments at all.
        if not parser.match(TokenType.RIGHT_PAREN):
            args.append(parser.parse())
            while parser.match(TokenType.COMMA):
                parser.consume(TokenType.COMMA)
                args.append(parser.parse())

        parser.consume(TokenType.RIGHT_PAREN)

        return CallExpression(function=left, args=args)

    @override
    def get_precedence(self) -> int:
        return Precedence.CALL.value
