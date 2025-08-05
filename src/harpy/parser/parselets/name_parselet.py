from harpy.ast.expressions import Expression, NameExpression
from harpy.lexer import Token

from ..parser_base import ParserBase
from .prefix_parselet import PrefixParselet


class NameParselet(PrefixParselet):
    """Simple parselet for a named variable like 'abc'."""

    def parse(self, parser: ParserBase, token: Token) -> Expression:
        del parser

        return NameExpression(name=token.text)
