from ..ast_node import ASTNode


class Expression(ASTNode):
    """Interface for all expression AST node classes."""

    _left_expr: bool

    def __init__(self, left_expr: bool):
        super().__init__()

        self._lext_expr = left_expr

    def left_expr(self) -> bool:
        return self._left_expr
