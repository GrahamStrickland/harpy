from collections import deque
from typing import Iterable

from .token import Token
from .token_type import TokenType


class SourceReader:
    _tokens: Iterable[Token]
    _read: deque[Token]

    def __init__(self, tokens: Iterable[Token]):
        self._tokens = tokens
        self._read = deque()

    def __iter__(self):
        return self

    def __next__(self):
        if len(self._read) > 0:
            return self._read.popleft()
        else:
            return next(self._tokens)

    def match(self, expected: TokenType) -> bool:
        if len(self._read) == 0:
            token = self.look_ahead(0)
        else:
            token = self._read[0]

        return token.get_type() == expected

    def consume(self, expected: TokenType | None = None) -> Token:
        # Make sure we've read the token.
        if len(self._read) == 0:
            token = self.look_ahead(0)
        else:
            token = self._read[0]

        if expected is not None and token.get_type() != expected:
            raise RuntimeError(
                f"Expected token type '{expected.name}' and found '{token.get_type().name}'"
            )

        return self._read.popleft()

    def look_ahead(self, distance: int) -> Token:
        # Read in as many as needed.
        while distance >= len(self._read):
            self._read.append(next(self._tokens))

        # Get the queued token.
        return self._read[distance]

    def put_back(self, token: Token):
        self._read.appendleft(token)
