from typing import override

from harpy.ast.expressions import CodeblockExpression, NameExpression, Expression
from harpy.lexer import Token, TokenType

from ..parser import Parser
from ..precedence import Precedence
from .prefix_parselet import PrefixParselet


class CodeblockParselet(PrefixParselet):
    """Parselet for a codeblock like `{ |a| b, c }`."""

    @override
    def parse(self, parser: Parser, token: Token):
        parser.consume(TokenType.PIPE)

        params: list[NameExpression] = []

        # There may be no arguments at all.
        if not parser.match(TokenType.PIPE):
            params.append(self._parse_param(parser=parser))
            while parser.match(TokenType.COMMA):
                parser.consume(TokenType.COMMA)
                params.append(self._parse_param(parser=parser))

        parser.consume(TokenType.PIPE)

        exprs: list[Expression] = []
        if not parser.match(TokenType.RIGHT_BRACE):
            exprs.append(parser.parse())
            while parser.match(TokenType.COMMA):
                parser.consume(TokenType.COMMA)
                exprs.append(parser.parse())

        parser.consume(TokenType.RIGHT_BRACE)

        return CodeblockExpression(params=params, exprs=exprs)

    @override
    def get_precedence(self) -> int:
        return Precedence.CALL.value

    def _parse_param(self, parser: Parser) -> NameExpression:
        name = parser.parse()
        if not isinstance(name, NameExpression):
            raise SyntaxError(
                f"Unable to parse parameter '{name.print()}' in codeblock."
            )

        return name
