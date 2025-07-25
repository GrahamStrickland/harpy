from ..harpy.harbour_parser import HarbourParser
from ..harpy.lexer import Lexer


class TestHarbourParser:
    def test__function_defn(self):
        self._test(
            "function a()\n\n    local c := b\n\nreturn c",
            "function a()\nlocal (c := b)\nreturn c",
        )
        self._test(
            "function a(b)\n\n    local c := b\n\nreturn c",
            "function a(b)\nlocal (c := b)\nreturn c",
        )
        self._test(
            "function a(b, c, d)\n\n    local c := b\n\nreturn c",
            "function a(b, c, d)\nlocal (c := b)\nreturn c",
        )

    def test__procedure_defn(self):
        self._test(
            "procedure a()\n\n    local c := b\n\nreturn",
            "procedure a()\nlocal (c := b)\nreturn",
        )
        self._test(
            "procedure a(b)\n\n    local c := b\n\nreturn",
            "procedure a(b)\nlocal (c := b)\nreturn",
        )
        self._test(
            "procedure a(b, c, d)\n\n    local c := b\n\nreturn",
            "procedure a(b, c, d)\nlocal (c := b)\nreturn",
        )

    def test__statics(self):
        self._test(
            "static a := b",
            "static (a := b)",
        )
        self._test(
            "static function a()\n\n    local c := b\n\nreturn c",
            "static function a()\nlocal (c := b)\nreturn c",
        )
        self._test(
            "function a()\n\n    static c := b\n\nreturn c",
            "function a()\nstatic (c := b)\nreturn c",
        )

    def test__if_stmt(self):
        self._test(
            "if a(b, c, d)\n\n    e()\n\nendif",
            "if a(b, c, d)\ne()\nendif",
        )
        self._test(
            "if !a(b, c, d) .and. !e\n\n    f()\n\nendif",
            "if ((!a(b, c, d)) .and. (!e))\nf()\nendif",
        )
        self._test(
            "if a(b, c, d)\n\n    e()\n\nelse\n\n    f()\n\nendif",
            "if a(b, c, d)\ne()\nelse\nf()\nendif",
        )
        self._test(
            "if a(b, c, d)\n\n    e()\n\nelseif f(b)\n\n    g()\n\nelse\n\n    h()\n\nendif",
            "if a(b, c, d)\ne()\nelseif f(b)\ng()\nelse\nh()\nendif",
        )

    def _test(self, source: str, expected: str):
        """Parses the given chunk of code and verifies that it matches the expected
        pretty-printed result.
        """
        lexer = Lexer(text=source)
        parser = HarbourParser(lexer=lexer)

        result = parser.parse()
        actual = ""
        for stmt in result:
            actual += stmt.print()

        assert actual == expected
