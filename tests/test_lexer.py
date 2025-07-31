import pytest

from ..harpy.lexer import Lexer
from ..harpy.token import Token
from ..harpy.token_type import TokenType


class TestLexer:
    def test__next__(self):
        obs = self._get_obs(source="from + offset(time)")
        expected = [
            Token(type=TokenType.NAME, text="from", line=1, position=4),
            Token(type=TokenType.PLUS, text="+", line=1, position=6),
            Token(type=TokenType.NAME, text="offset", line=1, position=13),
            Token(type=TokenType.LEFT_PAREN, text="(", line=1, position=14),
            Token(type=TokenType.NAME, text="time", line=1, position=18),
            Token(type=TokenType.RIGHT_PAREN, text=")", line=1, position=19),
        ]

        assert str(obs) == str(expected)

    def test_preprocessor_directive(self):
        obs = self._get_obs(source="#define slBool .f.")
        expected = [
            Token(
                type=TokenType.DEFINE_DIRECTIVE,
                text="#define slBool .f.",
                line=1,
                position=18,
            )
        ]

        assert str(obs) == str(expected)

        obs = self._get_obs(source="#ifdef SOMETHING")
        expected = [
            Token(
                type=TokenType.IFDEF_DIRECTIVE,
                text="#ifdef SOMETHING",
                line=1,
                position=16,
            )
        ]

        assert str(obs) == str(expected)

        obs = self._get_obs(source="#pragma -ko+")
        expected = [
            Token(
                type=TokenType.PRAGMA_DIRECTIVE, text="#pragma -ko+", line=1, position=12
            )
        ]

        assert str(obs) == str(expected)

    def test_bool_literal(self):
        obs = self._get_obs(source=".t.")
        expected = [Token(type=TokenType.BOOL_LITERAL, text=".t.", line=1, position=3)]

        assert str(obs) == str(expected)

        obs = self._get_obs(source=".F.")
        expected = [Token(type=TokenType.BOOL_LITERAL, text=".F.", line=1, position=3)]

        assert str(obs) == str(expected)

    def test_num_literal(self):
        obs = self._get_obs(source="0")
        expected = [Token(type=TokenType.NUM_LITERAL, text="0", line=1, position=1)]

        assert str(obs) == str(expected)

        obs = self._get_obs(source="123")
        expected = [Token(type=TokenType.NUM_LITERAL, text="123", line=1, position=3)]

        assert str(obs) == str(expected)

        obs = self._get_obs(source="50000")
        expected = [Token(type=TokenType.NUM_LITERAL, text="50000", line=1, position=5)]

        assert str(obs) == str(expected)

        obs = self._get_obs(source="12000.123")
        expected = [
            Token(type=TokenType.NUM_LITERAL, text="12000.123", line=1, position=9)
        ]

        assert str(obs) == str(expected)

        obs = self._get_obs(source="0xABAB")
        expected = [
            Token(type=TokenType.NUM_LITERAL, text="0xABAB", line=1, position=6)
        ]

        assert str(obs) == str(expected)

        obs = self._get_obs(source=".12")
        expected = [Token(type=TokenType.NUM_LITERAL, text=".12", line=1, position=3)]

        assert str(obs) == str(expected)

        with pytest.raises(SyntaxError, match="Unterminated hexadecimal literal '1x'"):
            _ = self._get_obs(source="1x001")

        with pytest.raises(SyntaxError, match="Invalid numeric literal '123a'"):
            _ = self._get_obs(source="123a")

        with pytest.raises(
            SyntaxError, match="Second decimal point found in literal '1.1.'."
        ):
            _ = self._get_obs(source="1.1.1")

    def test_str_literal(self):
        obs = self._get_obs(source="'This is a string'")
        expected = [
            Token(
                type=TokenType.STR_LITERAL,
                text="'This is a string'",
                line=1,
                position=18,
            )
        ]

        assert str(obs) == str(expected)

        obs = self._get_obs(source='"This is also a string"')
        expected = [
            Token(TokenType.STR_LITERAL, '"This is also a string"', line=1, position=23)
        ]

        assert str(obs) == str(expected)

        obs = self._get_obs(source="[Also a string]")
        expected = [
            Token(
                type=TokenType.STR_LITERAL, text="[Also a string]", line=1, position=15
            )
        ]

        assert str(obs) == str(expected)

        obs = self._get_obs(source="['Actually a hash key']")
        expected = [
            Token(type=TokenType.LEFT_BRACKET, text="[", line=1, position=1),
            Token(
                type=TokenType.STR_LITERAL,
                text="'Actually a hash key'",
                line=1,
                position=22,
            ),
            Token(type=TokenType.RIGHT_BRACKET, text="]", line=1, position=23),
        ]

        assert str(obs) == str(expected)

        obs = self._get_obs(source="function a(b)\n    local i := 1\nreturn b[i]")
        expected = [
            Token(type=TokenType.FUNCTION, text="function", line=1, position=8),
            Token(type=TokenType.NAME, text="a", line=1, position=10),
            Token(type=TokenType.LEFT_PAREN, text="(", line=1, position=11),
            Token(type=TokenType.NAME, text="b", line=1, position=12),
            Token(type=TokenType.RIGHT_PAREN, text=")", line=1, position=13),
            Token(type=TokenType.LOCAL, text="local", line=2, position=9),
            Token(type=TokenType.NAME, text="i", line=2, position=11),
            Token(type=TokenType.ASSIGN, text=":=", line=2, position=14),
            Token(type=TokenType.NUM_LITERAL, text="1", line=2, position=16),
            Token(type=TokenType.RETURN, text="return", line=3, position=6),
            Token(type=TokenType.NAME, text="b", line=3, position=8),
            Token(type=TokenType.LEFT_BRACKET, text="[", line=3, position=9),
            Token(type=TokenType.NAME, text="i", line=3, position=10),
            Token(type=TokenType.RIGHT_BRACKET, text="]", line=3, position=11),
        ]

        assert str(obs) == str(expected)

        obs = self._get_obs(source='""')
        expected = [Token(TokenType.STR_LITERAL, '""', line=1, position=2)]

        assert str(obs) == str(expected)

        obs = self._get_obs(source="''")
        expected = [Token(type=TokenType.STR_LITERAL, text="''", line=1, position=2)]

        assert str(obs) == str(expected)

        with pytest.raises(SyntaxError):
            _ = self._get_obs(source="'This string does not finish")

        with pytest.raises(SyntaxError):
            _ = self._get_obs(source="'")

    def test_assign_vs_comma(self):
        obs = self._get_obs(source="a := b")
        expected = [
            Token(type=TokenType.NAME, text="a", line=1, position=1),
            Token(type=TokenType.ASSIGN, text=":=", line=1, position=4),
            Token(type=TokenType.NAME, text="b", line=1, position=6),
        ]

        assert str(obs) == str(expected)

        obs = self._get_obs(source="a:b")
        expected = [
            Token(type=TokenType.NAME, text="a", line=1, position=1),
            Token(type=TokenType.COLON, text=":", line=1, position=2),
            Token(type=TokenType.NAME, text="b", line=1, position=3),
        ]

        assert str(obs) == str(expected)

    def test_logical_operators(self):
        obs = self._get_obs(source="a .and. b")
        expected = [
            Token(type=TokenType.NAME, text="a", line=1, position=1),
            Token(type=TokenType.AND, text=".and.", line=1, position=7),
            Token(type=TokenType.NAME, text="b", line=1, position=9),
        ]

        assert str(obs) == str(expected)

        obs = self._get_obs(source="a .and. !b")
        expected = [
            Token(type=TokenType.NAME, text="a", line=1, position=1),
            Token(type=TokenType.AND, text=".and.", line=1, position=7),
            Token(type=TokenType.NOT, text="!", line=1, position=9),
            Token(type=TokenType.NAME, text="b", line=1, position=10),
        ]

        assert str(obs) == str(expected)

        obs = self._get_obs(source="!a .and. !b")
        expected = [
            Token(type=TokenType.NOT, text="!", line=1, position=1),
            Token(type=TokenType.NAME, text="a", line=1, position=2),
            Token(type=TokenType.AND, text=".and.", line=1, position=8),
            Token(type=TokenType.NOT, text="!", line=1, position=10),
            Token(type=TokenType.NAME, text="b", line=1, position=11),
        ]

        assert str(obs) == str(expected)

        obs = self._get_obs(source="!a .and. !b .or. !c")
        expected = [
            Token(type=TokenType.NOT, text="!", line=1, position=1),
            Token(type=TokenType.NAME, text="a", line=1, position=2),
            Token(type=TokenType.AND, text=".and.", line=1, position=8),
            Token(type=TokenType.NOT, text="!", line=1, position=10),
            Token(type=TokenType.NAME, text="b", line=1, position=11),
            Token(type=TokenType.OR, text=".or.", line=1, position=16),
            Token(type=TokenType.NOT, text="!", line=1, position=18),
            Token(type=TokenType.NAME, text="c", line=1, position=19),
        ]

        assert str(obs) == str(expected)

    def test_relations(self):
        obs = self._get_obs(source="a == b")
        expected = [
            Token(type=TokenType.NAME, text="a", line=1, position=1),
            Token(type=TokenType.EQ1, text="==", line=1, position=4),
            Token(type=TokenType.NAME, text="b", line=1, position=6),
        ]

        assert str(obs) == str(expected)

        obs = self._get_obs(source="a <= b")
        expected = [
            Token(type=TokenType.NAME, text="a", line=1, position=1),
            Token(type=TokenType.LE, text="<=", line=1, position=4),
            Token(type=TokenType.NAME, text="b", line=1, position=6),
        ]

        assert str(obs) == str(expected)

    def test_line_comment(self):
        obs = self._get_obs(source="// This is a line comment.")
        expected = [
            Token(
                type=TokenType.LINE_COMMENT,
                text="// This is a line comment.",
                line=1,
                position=26,
            )
        ]

        assert str(obs) == str(expected)

    def test_block_comment(self):
        obs = self._get_obs(source="/* This is a block comment.*/")
        expected = [
            Token(
                type=TokenType.BLOCK_COMMENT,
                text="/* This is a block comment.*/",
                line=1,
                position=29,
            )
        ]

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
                line=2,
                position=0,
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
            Token(type=TokenType.FUNCTION, text="FUNCTION", line=1, position=8),
            Token(type=TokenType.NAME, text="a", line=1, position=10),
            Token(type=TokenType.LEFT_PAREN, text="(", line=1, position=11),
            Token(type=TokenType.RIGHT_PAREN, text=")", line=1, position=12),
            Token(type=TokenType.RETURN, text="RETURN", line=2, position=6),
            Token(type=TokenType.NAME, text="b", line=2, position=8),
        ]

        assert str(obs) == str(expected)

        obs = self._get_obs(source="function a(b, c, d)\n\nreturn e")
        expected = [
            Token(type=TokenType.FUNCTION, text="function", line=1, position=8),
            Token(type=TokenType.NAME, text="a", line=1, position=10),
            Token(type=TokenType.LEFT_PAREN, text="(", line=1, position=11),
            Token(type=TokenType.NAME, text="b", line=1, position=12),
            Token(type=TokenType.COMMA, text=",", line=1, position=13),
            Token(type=TokenType.NAME, text="c", line=1, position=15),
            Token(type=TokenType.COMMA, text=",", line=1, position=16),
            Token(type=TokenType.NAME, text="d", line=1, position=18),
            Token(type=TokenType.RIGHT_PAREN, text=")", line=1, position=19),
            Token(type=TokenType.RETURN, text="return", line=3, position=6),
            Token(type=TokenType.NAME, text="e", line=3, position=8),
        ]

        assert str(obs) == str(expected)

    def test_procedure_def(self):
        obs = self._get_obs(source="procedure a()\nRETURN")
        expected = [
            Token(type=TokenType.PROCEDURE, text="procedure", line=1, position=9),
            Token(type=TokenType.NAME, text="a", line=1, position=11),
            Token(type=TokenType.LEFT_PAREN, text="(", line=1, position=12),
            Token(type=TokenType.RIGHT_PAREN, text=")", line=1, position=13),
            Token(type=TokenType.RETURN, text="RETURN", line=2, position=6),
        ]

        assert str(obs) == str(expected)

        obs = self._get_obs(source="procedure a(b, c, d)\n\nreturn")
        expected = [
            Token(type=TokenType.PROCEDURE, text="procedure", line=1, position=9),
            Token(type=TokenType.NAME, text="a", line=1, position=11),
            Token(type=TokenType.LEFT_PAREN, text="(", line=1, position=12),
            Token(type=TokenType.NAME, text="b", line=1, position=13),
            Token(type=TokenType.COMMA, text=",", line=1, position=14),
            Token(type=TokenType.NAME, text="c", line=1, position=16),
            Token(type=TokenType.COMMA, text=",", line=1, position=17),
            Token(type=TokenType.NAME, text="d", line=1, position=19),
            Token(type=TokenType.RIGHT_PAREN, text=")", line=1, position=20),
            Token(type=TokenType.RETURN, text="return", line=3, position=6),
        ]

        assert str(obs) == str(expected)

    def test_if_else(self):
        obs = self._get_obs(
            source="if a <= b\n    b()\nelseif c\n    d()\nelse\n    e()\nendif"
        )
        expected = [
            Token(type=TokenType.IF, text="if", line=1, position=2),
            Token(type=TokenType.NAME, text="a", line=1, position=4),
            Token(type=TokenType.LE, text="<=", line=1, position=7),
            Token(type=TokenType.NAME, text="b", line=1, position=9),
            Token(type=TokenType.NAME, text="b", line=2, position=5),
            Token(type=TokenType.LEFT_PAREN, text="(", line=2, position=6),
            Token(type=TokenType.RIGHT_PAREN, text=")", line=2, position=7),
            Token(type=TokenType.ELSEIF, text="elseif", line=3, position=6),
            Token(type=TokenType.NAME, text="c", line=3, position=8),
            Token(type=TokenType.NAME, text="d", line=4, position=5),
            Token(type=TokenType.LEFT_PAREN, text="(", line=4, position=6),
            Token(type=TokenType.RIGHT_PAREN, text=")", line=4, position=7),
            Token(type=TokenType.ELSE, text="else", line=5, position=4),
            Token(type=TokenType.NAME, text="e", line=6, position=5),
            Token(type=TokenType.LEFT_PAREN, text="(", line=6, position=6),
            Token(type=TokenType.RIGHT_PAREN, text=")", line=6, position=7),
            Token(type=TokenType.ENDIF, text="endif", line=7, position=5),
        ]

        assert str(obs) == str(expected)

    def _get_obs(self, source: str) -> list[Token]:
        lexer = Lexer(text=source)
        obs = []
        for token in lexer:
            if token.type == TokenType.EOF:
                break
            else:
                obs.append(token)

        return obs
