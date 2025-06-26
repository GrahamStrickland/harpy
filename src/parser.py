from parselets import InfixParselet, PrefixOperatorParselet, PrefixParselet

from .token_type import TokenType


class Parser:
    _infix_parselets: dict[TokenType, InfixParselet]
    _prefix_parselets: dict[TokenType, PrefixParselet]

    def __init__(self):
        self._prefix_parselets = {}

    def parse_expression(self):
        token = self.consume()
        prefix = self._prefix_parselets[token.get_type()]

        if prefix is None:
            raise SyntaxError(f"Could not parse '{token.get_text()}'.")

        left = prefix.parse(parser=self, token=token)

        token = self._look_ahead(0)
        infix = self._infix_parselets[token.get_type()]

        # No infix expression at this point, so we're done.
        if infix is None:
            return left

        self.consume()
        return infix.parse(parser=self, left=left, token=token)

    def register(self, token: TokenType, parselet: PrefixParselet | InfixParselet):
        if isinstance(parselet, PrefixParselet):
            self._prefix_parselets[token] = parselet
        elif isinstance(parselet, InfixParselet):
            self._infix_parselets[token] = parselet

    def prefix(self, token: TokenType):
        self.register(token=token, parselet=PrefixOperatorParselet())
