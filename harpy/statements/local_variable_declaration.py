from ..expressions import Expression
from .variable_declaration import VariableDeclaration


class LocalVariableDeclaration(VariableDeclaration):
    """Represents a local variable declaration, e.g. `local a` or `local b := 1`."""

    def __init__(self, name: str, assign_expr: Expression | None):
        super().__init__(variable_type="local", name=name, assign_expr=assign_expr)
