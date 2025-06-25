from abs import ABC, abstractmethod

from ..expressions import Expression
from ..parser import Parser
from ..token import Token


class PrefixParselet(ABC):
    @abstractmethod
    def parse(self, parser: Parser, token: Token) -> Expression:
        pass
