from harpy import ExpressionParser, Lexer, SourceReader


class TestExpressionParser:
    def test_call(self):
        self._test("a()", "a()")
        self._test("a(b)", "a(b)")
        self._test("a(b, c)", "a(b, c)")
        self._test("a(b)(c)", "a(b)(c)")
        self._test("a(b) + c(d)", "(a(b) + c(d))")
        self._test("a(@b) + c(@d)", "(a((@b)) + c((@d)))")

    def test_unary_precedence(self):
        self._test("!-+a", "(!(-(+a)))")
        self._test("!!!a", "(!(!(!a)))")
        self._test("@a", "(@a)")

    def test_unary_binary_precedence(self):
        self._test("-a * b", "((-a) * b)")
        self._test("-a % b", "((-a) % b)")
        self._test("!a + b", "((!a) + b)")
        self._test("!a ^ b", "((!a) ^ b)")
        self._test("-a", "(-a)")
        self._test("!a", "(!a)")

    def test_binary_precedence(self):
        self._test(
            "a := b + c * d ^ e - f / g", "(a := ((b + (c * (d ^ e))) - (f / g)))"
        )

    def test_binary_associativity(self):
        self._test("a := b := c", "(a := (b := c))")
        self._test("a := b = c", "(a := (b = c))")
        self._test("a := b == c", "(a := (b == c))")
        self._test("a := b < c", "(a := (b < c))")
        self._test("a := b <= c", "(a := (b <= c))")
        self._test("a := b # c", "(a := (b # c))")
        self._test("a := b != c", "(a := (b != c))")
        self._test("a += b *= c", "(a += (b *= c))")
        self._test("a %= b -= c", "((a %= b) -= c)")
        self._test("a + b - c", "((a + b) - c)")
        self._test("a + b > c", "((a + b) > c)")
        self._test("a + b >= c", "((a + b) >= c)")
        self._test("a * b / c", "((a * b) / c)")
        self._test("a ^ b ^ c", "(a ^ (b ^ c))")
        self._test("a^b^c", "(a ^ (b ^ c))")
        self._test("a * b % c", "((a * b) % c)")
        self._test("a $ b $ c", "((a $ b) $ c)")
        self._test("a .or. b", "(a .or. b)")
        self._test("a .and. b", "(a .and. b)")
        self._test("a .and. b .or. c", "((a .and. b) .or. c)")
        self._test("a .or. b .and. c", "(a .or. (b .and. c))")
        self._test("!a .and. !b .or. !c", "(((!a) .and. (!b)) .or. (!c))")
        self._test("!(a .or. b .and. c)", "(!(a .or. (b .and. c)))")

    def test_grouping(self):
        self._test("a + (b + c) + d", "((a + (b + c)) + d)")
        self._test("a ^ (b + c)", "(a ^ (b + c))")
        self._test("(!a)", "(!a)")

    def test_literals(self):
        self._test(".t.", ".t.")
        self._test("123", "123")
        self._test("'hello'", "'hello'")

    def test_object_access(self):
        self._test("a:b", "a:b")
        self._test("a:b:c", "a:b:c")
        self._test("a:b()", "a:b()")
        self._test("a:b(c)", "a:b(c)")
        self._test("a:b():c", "a:b():c")
        self._test("a:b()[1]", "a:b()[1]")
        self._test("a:b()['key']", "a:b()['key']")
        self._test("a:b()[1 + c]", "a:b()[(1 + c)]")

    def _test(self, source: str, expected: str):
        """Parses the given chunk of code and verifies that it matches the expected
        pretty-printed result.
        """
        lexer = Lexer(text=source)
        reader = SourceReader(tokens=lexer)
        parser = ExpressionParser(source_reader=reader)

        result = parser.parse()
        actual = result.print()

        assert actual == expected
