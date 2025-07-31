from ..expressions import Expression
from .variable_declaration import VariableDeclaration


class StaticVariableDeclaration(VariableDeclaration):
    """Represents a static variable declaration, e.g. `static a` or `static b := 1."""

    _name: str
    _assign_expr: Expression | None

    def __init__(self, name: str, assign_expr: Expression | None):
        super().__init__(variable_type="static", name=name, assign_expr=assign_expr)
