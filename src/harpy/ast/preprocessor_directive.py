from typing import override

from harpy.ast import ASTNode
from harpy.lexer import Token


class PreprocessorDirective(ASTNode):
    """AST node class for preprocessor directives."""

    _token: Token

    def __init__(self, token):
        self._token = token

    @override
    def print(self):
        return self._token.text
