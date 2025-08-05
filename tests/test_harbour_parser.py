from harpy import HarbourParser, Lexer


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

    def test__local_decln_stmt_indexing(self):
        self._test(
            "function a(b)\n\n    local c := b[1]\n\nreturn c",
            "function a(b)\nlocal (c := b[1])\nreturn c",
        )
        self._test(
            "function a(b)\n\n    local c := b['key']\n\nreturn c",
            "function a(b)\nlocal (c := b['key'])\nreturn c",
        )
        self._test(
            "function a(b)\n\n    local c := b[[key]]\n\nreturn c",
            "function a(b)\nlocal (c := b[[key]])\nreturn c",
        )
        self._test(
            "function a()\n\n    local b := [hello]\n\nreturn b",
            "function a()\nlocal (b := [hello])\nreturn b",
        )
        self._test(
            "function a(b)\n\n    local c := b[1,2]\n\nreturn c",
            "function a(b)\nlocal (c := b[1, 2])\nreturn c",
        )

    def test__statics(self):
        self._test(
            "static a := b",
            "static (a := b)",
        )
        self._test(
            "static a := nil",
            "static (a := nil)",
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
            "if a(b, c, d)\n\n    e()\n\nelseif f(b)\n\n    g()\n\n\n\nendif",
            "if a(b, c, d)\ne()\nelseif f(b)\ng()\nendif",
        )
        self._test(
            "if a(b, c, d)\n\n    e()\n\nelseif f(b)\n\n    g()\n\nelse\n\n    h()\n\nendif",
            "if a(b, c, d)\ne()\nelseif f(b)\ng()\nelse\nh()\nendif",
        )

    def test__while_loop(self):
        self._test(
            "while a(b, c, d)\n\n    e()\n\nend",
            "while a(b, c, d)\ne()\nend while",
        )
        self._test(
            "while a(b, c, d)\n\n    e()\n    f()\n\nend",
            "while a(b, c, d)\ne()\nf()\nend while",
        )
        self._test(
            "while a(b, c, d)\n\n    e()\n    f()\n\nend while",
            "while a(b, c, d)\ne()\nf()\nend while",
        )
        self._test(
            "while a(b, c, d)\n\n    e()\n    f()\n\nendwhile",
            "while a(b, c, d)\ne()\nf()\nend while",
        )

    def test_assignment_stmt(self):
        self._test(
            "if a(b, c, d)\n\n    e := 1\n\nendif",
            "if a(b, c, d)\n(e := 1)\nendif",
        )
        self._test(
            "if a(b, c, d)\n\n    e:f := 1\n\nendif",
            "if a(b, c, d)\n(e:f := 1)\nendif",
        )

    def _test(self, source: str, expected: str):
        """Parses the given chunk of code and verifies that it matches the expected
        pretty-printed result.
        """
        lexer = Lexer(text=source)
        parser = HarbourParser(lexer=lexer)

        root = parser.parse()
        actual = ""
        for node in root:
            actual += node.print()

        assert actual == expected
