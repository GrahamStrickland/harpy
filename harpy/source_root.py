from .ast_node import ASTNode


class SourceRoot:
    """The root AST node of a parsed Harbour source file."""

    _nodes: list[ASTNode]

    def __init__(self):
        self._nodes = []

    def add(self, node: ASTNode):
        self._nodes.append(node)

    def __iter__(self):
        return self

    def __next__(self):
        if len(self._nodes) == 0:
            raise StopIteration("No more nodes in source root")

        return self._nodes.pop(0) 

