from typing import Iterable

from .parselets.infix_parselet import InfixParselet
from .parselets.prefix_parselet import PrefixParselet
from .token import Token
from .token_type import TokenType


class Parser:
    _tokens: Iterable[Token]
    _read: list[Token]
    _infix_parselets: dict[TokenType, InfixParselet]
    _prefix_parselets: dict[TokenType, PrefixParselet]

    def __init__(self, tokens: Iterable[Token]):
        self._tokens = tokens
        self._read = []
        self._infix_parselets = {}
        self._prefix_parselets = {}

    def register(self, token: TokenType, parselet: PrefixParselet | InfixParselet):
        if isinstance(parselet, PrefixParselet):
            self._prefix_parselets[token] = parselet
        elif isinstance(parselet, InfixParselet):
            self._infix_parselets[token] = parselet

    def parse_expression(self, precedence: int = 0):
        token = self.consume()
        prefix = self._prefix_parselets[token.get_type()]

        if prefix is None:
            raise SyntaxError(f"Could not parse '{token.get_text()}'.")

        left = prefix.parse(parser=self, token=token)

        while precedence < self._get_precedence():
            token = self.consume()
            infix = self._infix_parselets[token.get_type()]
            left = infix.parse(parser=self, left=left, token=token)

        return left

    def match(self, expected: TokenType) -> bool:
        token = self._look_ahead(0)
        if token.get_type() != expected:
            return False

        self.consume()

        return True

    def consume(self, expected: TokenType | None = None) -> Token:
        # Make sure we've read the token.
        token = self._look_ahead(0)
        if expected is not None and token.get_type() != expected:
            raise RuntimeError(
                f"Expected token {expected} and found {token.get_type()}"
            )

        return self._read.pop(0)

    def _look_ahead(self, distance: int) -> Token:
        # Read in as many as needed.
        while distance >= len(self._read):
            self._read.append(next(self._tokens))

        # Get the queued token.
        return self._read[distance]

    def _get_precedence(self) -> int:
        type = self._look_ahead(0).get_type()
        parser = self._infix_parselets[type] if type in self._infix_parselets else None

        if parser is not None:
            return parser.get_precedence()

        return 0
