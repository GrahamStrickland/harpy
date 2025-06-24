from ..src.lexer import Lexer
from ..src.token import Token
from ..src.token_type import TokenType


class TestLexer:
    _lexer = Lexer("from + offset(time)")

    def test__next__(self):
        obs = []
        for token in self._lexer:
            if token.get_type() == TokenType.EOF:
                break
            else:
                obs.append(token)

        expected = [
            Token(TokenType.NAME, "from"),
            Token(TokenType.PLUS, "+"),
            Token(TokenType.NAME, "offset"),
            Token(TokenType.LEFT_PAREN, "("),
            Token(TokenType.NAME, "time"),
            Token(TokenType.RIGHT_PAREN, ")"),
        ]

        assert str(obs) == repr(expected)
