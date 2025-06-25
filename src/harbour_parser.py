from .parselets import NameParselet
from .parser import Parser
from .token_type import TokenType


class HarbourParser(Parser):
    def __init__(self):
        self.register(token=TokenType.NAME, parselet=NameParselet())
        self.prefix(token=TokenType.PLUS)
        self.prefix(token=TokenType.MINUS)
        self.prefix(token=TokenType.TILDE)
        self.prefix(token=TokenType.BANG)

