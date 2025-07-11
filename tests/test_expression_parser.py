from ..harpy.expression_parser import ExpressionParser
from ..harpy.lexer import Lexer


class TestExpressionParser:
    def test_call(self):
        self._test("a()", "a()")
        self._test("a(b)", "a(b)")
        self._test("a(b, c)", "a(b, c)")
        self._test("a(b)(c)", "a(b)(c)")
        self._test("a(b) + c(d)", "(a(b) + c(d))")

    def test_unary_precedence(self):
        self._test("!-+a", "(!(-(+a)))")
        self._test("!!!a", "(!(!(!a)))")

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
        self._test("a + b - c", "((a + b) - c)")
        self._test("a + b > c", "((a + b) > c)")
        self._test("a + b >= c", "((a + b) >= c)")
        self._test("a * b / c", "((a * b) / c)")
        self._test("a ^ b ^ c", "(a ^ (b ^ c))")
        self._test("a^b^c", "(a ^ (b ^ c))")
        self._test("a * b % c", "((a * b) % c)")

    def test_grouping(self):
        self._test("a + (b + c) + d", "((a + (b + c)) + d)")
        self._test("a ^ (b + c)", "(a ^ (b + c))")
        self._test("(!a)", "(!a)")

    def _test(self, source: str, expected: str):
        """Parses the given chunk of code and verifies that it matches the expected
        pretty-printed result.
        """
        lexer = Lexer(text=source)
        parser = ExpressionParser(lexer=lexer)

        result = parser.parse_expression()
        actual = result.print()

        assert actual == expected
