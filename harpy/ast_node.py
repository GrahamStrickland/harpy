from abc import ABC, abstractmethod


class ASTNode(ABC):
    @abstractmethod
    def print(self) -> str:
        """Pretty-print the node to a string."""
        pass
