from ..ast_node import ASTNode


class Expression(ASTNode):
    """Interface for all expression AST node classes."""

    _left_expr: bool

    def left_expr(self) -> bool:
        return self._left_expr
