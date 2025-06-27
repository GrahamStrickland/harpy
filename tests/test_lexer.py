from ..harpy.lexer import Lexer
from ..harpy.token import Token
from ..harpy.token_type import TokenType


class TestLexer:
    def test__next__(self):
        obs = self._get_obs(source="from + offset(time)")
        expected = [
            Token(TokenType.NAME, "from"),
            Token(TokenType.PLUS, "+"),
            Token(TokenType.NAME, "offset"),
            Token(TokenType.LEFT_PAREN, "("),
            Token(TokenType.NAME, "time"),
            Token(TokenType.RIGHT_PAREN, ")"),
        ]

        assert str(obs) == str(expected)

    def test_assign_vs_comma(self):
        obs = self._get_obs(source="a := b")
        expected = [
            Token(TokenType.NAME, "a"),
            Token(TokenType.ASSIGN, ":="),
            Token(TokenType.NAME, "b"),
        ]

        assert str(obs) == str(expected)

        obs = self._get_obs(source="a:b")
        expected = [
            Token(TokenType.NAME, "a"),
            Token(TokenType.COLON, ":"),
            Token(TokenType.NAME, "b"),
        ]

        assert str(obs) == str(expected)

    def _get_obs(self, source: str) -> list[Token]:
        lexer = Lexer(text=source)
        obs = []
        for token in lexer:
            if token.get_type() == TokenType.EOF:
                break
            else:
                obs.append(token)

        return obs
