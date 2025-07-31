from abc import ABC, abstractmethod

from harpy.ast.expressions import Expression


class Parser(ABC):
    @abstractmethod
    def parse(self) -> Expression:
        pass
