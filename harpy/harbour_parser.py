from .lexer import Lexer
from .parselets import (AssignParselet, BinaryOperatorParselet, CallParselet,
                        GroupParselet, NameParselet, PostfixOperatorParselet,
                        PrefixOperatorParselet)
from .parser import Parser
from .precedence import Precedence
from .token_type import TokenType


class HarbourParser(Parser):
    """Extends the generic Parser class with support for parsing the actual Harbour
    grammar.
    """

    def __init__(self, lexer: Lexer):
        super().__init__(tokens=lexer)

        # Register all of the parselets for the grammar.

        # Register the ones that need special parselets.
        self.register(token=TokenType.NAME, parselet=NameParselet())
        self.register(token=TokenType.ASSIGN, parselet=AssignParselet())
        self.register(token=TokenType.LEFT_PAREN, parselet=GroupParselet())
        self.register(token=TokenType.LEFT_PAREN, parselet=CallParselet())

        # Register the simple operator parselets.
        self.prefix(token=TokenType.PLUS, precedence=Precedence.PREFIX)
        self.prefix(token=TokenType.MINUS, precedence=Precedence.PREFIX)
        self.prefix(token=TokenType.BANG, precedence=Precedence.PREFIX)

        self.infix_left(token=TokenType.PLUS, precedence=Precedence.SUM)
        self.infix_left(token=TokenType.MINUS, precedence=Precedence.SUM)
        self.infix_left(token=TokenType.ASTERISK, precedence=Precedence.PRODUCT)
        self.infix_left(token=TokenType.SLASH, precedence=Precedence.PRODUCT)
        self.infix_right(token=TokenType.CARET, precedence=Precedence.EXPONENT)

    def postfix(self, token: TokenType, precedence: Precedence):
        """Registers a postfix unary operator parselet for the given token and
        precedence.
        """
        self.register(
            token=token, parselet=PostfixOperatorParselet(precedence=precedence.value)
        )

    def prefix(self, token: TokenType, precedence: Precedence):
        """Registers a prefix unary operator parselet for the given token and
        precedence.
        """
        self.register(
            token=token, parselet=PrefixOperatorParselet(precedence=precedence.value)
        )

    def infix_left(self, token: TokenType, precedence: Precedence):
        """Registers a left-associative binary operator parselet for the given token
        and precedence.
        """

        self.register(
            token=token,
            parselet=BinaryOperatorParselet(
                precedence=precedence.value, is_right=False
            ),
        )

    def infix_right(self, token: TokenType, precedence: Precedence):
        """Registers a right-associative binary operator parselet for the given token
        and precedence.
        """

        self.register(
            token=token,
            parselet=BinaryOperatorParselet(precedence=precedence.value, is_right=True),
        )
