import pytest

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

    def test_logical_operators(self):
        obs = self._get_obs(source="a .and. b")
        expected = [
            Token(TokenType.NAME, "a"),
            Token(TokenType.AND, ".and."),
            Token(TokenType.NAME, "b"),
        ]

        assert str(obs) == str(expected)

        obs = self._get_obs(source="a .and. !b")
        expected = [
            Token(TokenType.NAME, "a"),
            Token(TokenType.AND, ".and."),
            Token(TokenType.NOT, "!"),
            Token(TokenType.NAME, "b"),
        ]

        assert str(obs) == str(expected)

        obs = self._get_obs(source="!a .and. !b")
        expected = [
            Token(TokenType.NOT, "!"),
            Token(TokenType.NAME, "a"),
            Token(TokenType.AND, ".and."),
            Token(TokenType.NOT, "!"),
            Token(TokenType.NAME, "b"),
        ]

        assert str(obs) == str(expected)

        obs = self._get_obs(source="!a .and. !b .or. !c")
        expected = [
            Token(TokenType.NOT, "!"),
            Token(TokenType.NAME, "a"),
            Token(TokenType.AND, ".and."),
            Token(TokenType.NOT, "!"),
            Token(TokenType.NAME, "b"),
            Token(TokenType.OR, ".or."),
            Token(TokenType.NOT, "!"),
            Token(TokenType.NAME, "c"),
        ]

        assert str(obs) == str(expected)

    def test_relations(self):
        obs = self._get_obs(source="a == b")
        expected = [
            Token(TokenType.NAME, "a"),
            Token(TokenType.EQ1, "=="),
            Token(TokenType.NAME, "b"),
        ]

        assert str(obs) == str(expected)

        obs = self._get_obs(source="a <= b")
        expected = [
            Token(TokenType.NAME, "a"),
            Token(TokenType.LE, "<="),
            Token(TokenType.NAME, "b"),
        ]

        assert str(obs) == str(expected)

    def test_line_comment(self):
        obs = self._get_obs(source="// This is a line comment.")
        expected = [Token(TokenType.LINE_COMMENT, "// This is a line comment.")]

        assert str(obs) == str(expected)

    def test_block_comment(self):
        obs = self._get_obs(source="/* This is a block comment.*/")
        expected = [Token(TokenType.BLOCK_COMMENT, "/* This is a block comment.*/")]

        assert str(obs) == str(expected)

        obs = self._get_obs(
            source="""
/* This is also a
 * block
 * comment.
 */
 """
        )
        expected = [
            Token(
                TokenType.BLOCK_COMMENT,
                "/* This is also a\n * block\n * comment.\n */",
            )
        ]

        assert str(obs) == str(expected)

        with pytest.raises(SyntaxError):
            self._get_obs(source="/* This is an unfinished block comment.*")

        with pytest.raises(SyntaxError):
            self._get_obs(source="/* This one is even worse./*")

    def test_function_def(self):
        obs = self._get_obs(source="FUNCTION a()\nRETURN b")
        expected = [
            Token(TokenType.FUNCTION, "FUNCTION"),
            Token(TokenType.NAME, "a"),
            Token(TokenType.LEFT_PAREN, "("),
            Token(TokenType.RIGHT_PAREN, ")"),
            Token(TokenType.RETURN, "RETURN"),
            Token(TokenType.NAME, "b"),
        ]

        assert str(obs) == str(expected)

        obs = self._get_obs(source="function a(b, c, d)\n\nreturn e")
        expected = [
            Token(TokenType.FUNCTION, "function"),
            Token(TokenType.NAME, "a"),
            Token(TokenType.LEFT_PAREN, "("),
            Token(TokenType.NAME, "b"),
            Token(TokenType.COMMA, ","),
            Token(TokenType.NAME, "c"),
            Token(TokenType.COMMA, ","),
            Token(TokenType.NAME, "d"),
            Token(TokenType.RIGHT_PAREN, ")"),
            Token(TokenType.RETURN, "return"),
            Token(TokenType.NAME, "e"),
        ]

        assert str(obs) == str(expected)

    def test_procedure_def(self):
        obs = self._get_obs(source="procedure a()\nRETURN")
        expected = [
            Token(TokenType.PROCEDURE, "procedure"),
            Token(TokenType.NAME, "a"),
            Token(TokenType.LEFT_PAREN, "("),
            Token(TokenType.RIGHT_PAREN, ")"),
            Token(TokenType.RETURN, "RETURN"),
        ]

        assert str(obs) == str(expected)

        obs = self._get_obs(source="procedure a(b, c, d)\n\nreturn")
        expected = [
            Token(TokenType.PROCEDURE, "procedure"),
            Token(TokenType.NAME, "a"),
            Token(TokenType.LEFT_PAREN, "("),
            Token(TokenType.NAME, "b"),
            Token(TokenType.COMMA, ","),
            Token(TokenType.NAME, "c"),
            Token(TokenType.COMMA, ","),
            Token(TokenType.NAME, "d"),
            Token(TokenType.RIGHT_PAREN, ")"),
            Token(TokenType.RETURN, "return"),
        ]

        assert str(obs) == str(expected)

    def test_if_else(self):
        obs = self._get_obs(
            source="if a <= b\n    b()\nelseif c\n    d()\nelse    e()\nendif"
        )
        expected = [
            Token(TokenType.IF, "if"),
            Token(TokenType.NAME, "a"),
            Token(TokenType.LE, "<="),
            Token(TokenType.NAME, "b"),
            Token(TokenType.NAME, "b"),
            Token(TokenType.LEFT_PAREN, "("),
            Token(TokenType.RIGHT_PAREN, ")"),
            Token(TokenType.ELSEIF, "elseif"),
            Token(TokenType.NAME, "c"),
            Token(TokenType.NAME, "d"),
            Token(TokenType.LEFT_PAREN, "("),
            Token(TokenType.RIGHT_PAREN, ")"),
            Token(TokenType.ELSE, "else"),
            Token(TokenType.NAME, "e"),
            Token(TokenType.LEFT_PAREN, "("),
            Token(TokenType.RIGHT_PAREN, ")"),
            Token(TokenType.ENDIF, "endif"),
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
