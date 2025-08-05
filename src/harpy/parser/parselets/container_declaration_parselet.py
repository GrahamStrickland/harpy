from typing import override

from harpy.ast.expressions import (ArrayDeclarationExpression,
                                   HashDeclarationExpression)
from harpy.lexer import Token, TokenType

from ..parser import Parser
from ..precedence import Precedence
from .prefix_parselet import PrefixParselet


class ContainerDeclarationParselet(PrefixParselet):
    """Parselet to parse an array declaration like `{}` or `{a}` or a hash declaration like `{ => }` or `{ "a" => 1 }`."""

    @override
    def parse(self, parser: Parser, token: Token):
        elems = []
        keyvalues = {}

        if parser.match(TokenType.RIGHT_BRACE):
            parser.consume(TokenType.RIGHT_BRACE)
            return ArrayDeclarationExpression(elems=elems)
        elif parser.match(TokenType.HASHOP):
            parser.consume(TokenType.HASHOP)
            parser.consume(TokenType.RIGHT_BRACE)
            return HashDeclarationExpression(keyvalues=keyvalues)
        else:
            first_expr = parser.parse()
            if parser.match(TokenType.HASHOP):
                parser.consume(TokenType.HASHOP)
                v = parser.parse()
                keyvalues = {first_expr: v}
                while parser.match(TokenType.COMMA):
                    parser.consume(TokenType.COMMA)

                    k = parser.parse()
                    parser.consume(TokenType.HASHOP)
                    v = parser.parse()

                    keyvalues[k] = v
            else:
                elems = [first_expr]

                while parser.match(TokenType.COMMA):
                    parser.consume(TokenType.COMMA)

                    elem = parser.parse()

                    elems.append(elem)

        parser.consume(TokenType.RIGHT_BRACE)

        if len(elems) > 0:
            return ArrayDeclarationExpression(elems=elems)
        if len(keyvalues) > 0:
            return HashDeclarationExpression(keyvalues=keyvalues)

    @override
    def get_precedence(self) -> int:
        return Precedence.NONE.value
