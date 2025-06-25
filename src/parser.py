from .token_type import TokenType
from parselets import PrefixParselet, PrefixOperatorParselet


class Parser:
    _prefix_parselets: dict[TokenType, PrefixParselet]

    def __init__(self):
        self._prefix_parselets = {}

    def parse_expression(self):
        token = self._consume()
        prefix = self._prefix_parselets[token.get_type()]

        if prefix is None:
            raise SyntaxError(f"Could not parse '{token.get_text()}'.")

        return prefix.parse(self, token)

    def register(self, token: TokenType, parselet: PrefixParselet):
        self._prefix_parselets[token] = parselet

    def prefix(self, token: TokenType):
        self.register(token=token, parselet=PrefixOperatorParselet())
