from ..src.harbour_parser import HarbourParser
from ..src.lexer import Lexer


class TestHarbourParser:
    def test_parse(self):
        # Function call.
        self._test("a()", "a()")
        self._test("a(b)", "a(b)")
        self._test("a(b, c)", "a(b, c)")
        self._test("a(b)(c)", "a(b)(c)")
        self._test("a(b) + c(d)", "(a(b) + c(d))")
        self._test("a(b ? c : d, e + f)", "a((b ? c : d), (e + f))")

        # Unary precedence.
        self._test("~!-+a", "(~(!(-(+a))))")
        self._test("a!!!", "(((a!)!)!)")

        # Unary and binary predecence.
        self._test("-a * b", "((-a) * b)")
        self._test("!a + b", "((!a) + b)")
        self._test("~a ^ b", "((~a) ^ b)")
        self._test("-a!", "(-(a!))")
        self._test("!a!", "(!(a!))")

        # Binary precedence.
        self._test("a = b + c * d ^ e - f / g", "(a = ((b + (c * (d ^ e))) - (f / g)))")

        # Binary associativity.
        self._test("a = b = c", "(a = (b = c))")
        self._test("a + b - c", "((a + b) - c)")
        self._test("a * b / c", "((a * b) / c)")
        self._test("a ^ b ^ c", "(a ^ (b ^ c))")

        # Conditional operator.
        self._test("a ? b : c ? d : e", "(a ? b : (c ? d : e))")
        self._test("a ? b ? c : d : e", "(a ? (b ? c : d) : e)")
        self._test("a + b ? c * d : e / f", "((a + b) ? (c * d) : (e / f))")

        # Grouping.
        self._test("a + (b + c) + d", "((a + (b + c)) + d)")
        self._test("a ^ (b + c)", "(a ^ (b + c))")
        self._test("(!a)!", "((!a)!)")

    def _test(self, source: str, expected: str):
        """Parses the given chunk of code and verifies that it matches the expected
        pretty-printed result.
        """
        lexer = Lexer(text=source)
        parser = HarbourParser(lexer=lexer)

        result = parser.parse_expression()
        actual = result.print()

        assert actual == expected
