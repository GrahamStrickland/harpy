from abc import ABC, abstractmethod

from harpy.ast.expressions import Expression


class ParserBase(ABC):
    @abstractmethod
    def parse(self) -> Expression:
        pass
