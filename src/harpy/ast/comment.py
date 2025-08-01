from typing import override

from harpy.ast import ASTNode
from harpy.lexer import Token


class Comment(ASTNode):
    """AST node class for line and block comments."""

    _token: Token

    def __init__(self, token):
        self._token = token

    @override
    def print(self):
        return self._token.text
