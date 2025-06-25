from .prefix_parselet import PrefixParselet
from ..expressions import NameExpression
from ..parser import Parser
from ..token import Token


class NameParselet(PrefixParselet):
    def parse(self, parser: Parser, token: Token):
        return NameExpression(text=token.get_text())
