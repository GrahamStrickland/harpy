from abs import ABC, abstractmethod

from ..expressions import Expression
from ..parser import Parser
from ..token import Token


class PrefixParselet(ABC):
    """One of the two interfaces used by the Pratt parser. A `PrefixParselet` is
    associated with a token that appears at the beginning of an expression. Its
    `parse()` method will be called with the consumed leading token, and the
    parselet is responsible for parsing anything that comes after that token.
    This interface is also used for single-token expressions like variables, in
    which case `parse()` simply doesn't consume any more tokens.
    """

    @abstractmethod
    def parse(self, parser: Parser, token: Token) -> Expression:
        pass
