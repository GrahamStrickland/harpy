from ..expressions import Expression, NameExpression
from ..parser import Parser
from ..token import Token
from .prefix_parselet import PrefixParselet


class NameParselet(PrefixParselet):
    """Simple parselet for a named variable like 'abc'."""

    def parse(self, parser: Parser, token: Token) -> Expression:
        return NameExpression(name=token.get_text())
