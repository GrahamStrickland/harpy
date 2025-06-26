from abc import ABC, abstractmethod


class Expression(ABC):
    """Interface for all expression AST node classes."""

    @abstractmethod
    def print(self) -> str:
        """Pretty-print the expression to a string."""
        pass
