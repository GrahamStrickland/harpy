from abc import ABC, abstractmethod


class Statement(ABC):
    """Interface for all statement AST node classes."""

    @abstractmethod
    def print(self) -> str:
        """Pretty-print the statement to a string."""
        pass
