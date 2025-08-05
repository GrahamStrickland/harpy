from ..ast_node import ASTNode


class Statement(ASTNode):
    """Interface for all statement AST node classes."""

    def __init__(self):
        super().__init__()
