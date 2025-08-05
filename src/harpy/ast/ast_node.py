from abc import abstractmethod

from harpy.lexer import Token


class ASTNode:
    _comments: list[Token]

    def __init__(self):
        self._comments = []

    @abstractmethod
    def print(self) -> str:
        """Pretty-print the node to a string."""
        pass

    def add_comment(self, comment: Token):
        """Add a parsed comment to the AST node."""
        self._comments.append(comment)

    def comments(self):
        return self._comments
