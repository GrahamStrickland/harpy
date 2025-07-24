from typing import override

from .expressions import Expression
from .parselets import (AssignParselet, BinaryOperatorParselet, CallParselet,
                        GroupParselet, InfixParselet, NameParselet,
                        PostfixOperatorParselet, PrefixOperatorParselet,
                        PrefixParselet)
from .parser import Parser
from .precedence import Precedence
from .source_reader import SourceReader
from .token import Token
from .token_type import TokenType


class ExpressionParser(Parser):
    """Extends the generic Parser class with support for parsing the Harbour
    expression grammar.
    """

    _reader: SourceReader
    _infix_parselets: dict[TokenType, InfixParselet]
    _prefix_parselets: dict[TokenType, PrefixParselet]

    def __init__(self, source_reader: SourceReader):
        self._reader = source_reader
        self._infix_parselets = {}
        self._prefix_parselets = {}

        # Register all of the parselets for the grammar.

        # Register the ones that need special parselets.
        self.register(token=TokenType.NAME, parselet=NameParselet())
        self.register(token=TokenType.ASSIGN, parselet=AssignParselet())
        self.register(token=TokenType.LEFT_PAREN, parselet=GroupParselet())
        self.register(token=TokenType.LEFT_PAREN, parselet=CallParselet())

        # Register the simple operator parselets.
        self.prefix(token=TokenType.PLUS, precedence=Precedence.PREFIX)
        self.prefix(token=TokenType.MINUS, precedence=Precedence.PREFIX)
        self.prefix(token=TokenType.NOT, precedence=Precedence.PREFIX)

        self.infix_right(token=TokenType.PLUSEQ, precedence=Precedence.SUMEQ)
        self.infix_right(token=TokenType.MINUSEQ, precedence=Precedence.SUMEQ)
        self.infix_right(token=TokenType.MULTEQ, precedence=Precedence.MULTEQ)
        self.infix_right(token=TokenType.DIVEQ, precedence=Precedence.MULTEQ)
        self.infix_right(token=TokenType.MODEQ, precedence=Precedence.MULTEQ)
        self.infix_right(token=TokenType.EXPEQ, precedence=Precedence.EXPEQ)

        self.infix_right(token=TokenType.OR, precedence=Precedence.OR)
        self.infix_right(token=TokenType.AND, precedence=Precedence.AND)

        self.infix_left(token=TokenType.EQ1, precedence=Precedence.EQRELATION)
        self.infix_left(token=TokenType.EQ2, precedence=Precedence.EQRELATION)
        self.infix_left(token=TokenType.NE1, precedence=Precedence.EQRELATION)
        self.infix_left(token=TokenType.NE2, precedence=Precedence.EQRELATION)
        self.infix_left(token=TokenType.LE, precedence=Precedence.ORDRELATION)
        self.infix_left(token=TokenType.GE, precedence=Precedence.ORDRELATION)
        self.infix_left(token=TokenType.LT, precedence=Precedence.ORDRELATION)
        self.infix_left(token=TokenType.GT, precedence=Precedence.ORDRELATION)
        self.infix_left(token=TokenType.DOLLAR, precedence=Precedence.ORDRELATION)

        self.infix_left(token=TokenType.PLUS, precedence=Precedence.SUM)
        self.infix_left(token=TokenType.MINUS, precedence=Precedence.SUM)
        self.infix_left(token=TokenType.ASTERISK, precedence=Precedence.PRODUCT)
        self.infix_left(token=TokenType.SLASH, precedence=Precedence.PRODUCT)
        self.infix_left(token=TokenType.PERCENT, precedence=Precedence.PRODUCT)
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

    def register(self, token: TokenType, parselet: PrefixParselet | InfixParselet):
        if isinstance(parselet, PrefixParselet):
            self._prefix_parselets[token] = parselet
        elif isinstance(parselet, InfixParselet):
            self._infix_parselets[token] = parselet

    @override
    def parse(self, precedence: int = 0) -> Expression | None:
        token = self._reader.look_ahead(0)

        if not token.get_type().keyword():
            token = self._reader.consume()
            prefix = self._prefix_parselets[token.get_type()]

            if prefix is None:
                raise SyntaxError(f"Could not parse '{token.get_text()}'.")

            left = prefix.parse(parser=self, token=token)

            while precedence < self._get_precedence():
                token = self._reader.consume()
                infix = self._infix_parselets[token.get_type()]
                left = infix.parse(parser=self, left=left, token=token)

            return left

    def _get_precedence(self) -> int:
        type = self._reader.look_ahead(0).get_type()
        parser = self._infix_parselets[type] if type in self._infix_parselets else None

        if parser is not None:
            return parser.get_precedence()

        return 0

    def match(self, expected: TokenType) -> bool:
        return self._reader.match(expected)

    def consume(self, expected: TokenType | None = None) -> Token:
        return self._reader.consume(expected)
