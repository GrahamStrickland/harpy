using Harpy.Lexer;
using Harpy.Parser;

namespace HarpyTests.ParserTests;

[TestClass]
public sealed class TestExpressionParser
{
    [TestMethod]
    public void TestCall()
    {
        AssertParsedEqualsExpected("a()", "a()");
        AssertParsedEqualsExpected("a(b)", "a(b)");
        AssertParsedEqualsExpected("a(b, c)", "a(b, c)");
        AssertParsedEqualsExpected("a(b)(c)", "a(b)(c)");
        AssertParsedEqualsExpected("a(b) + c(d)", "(a(b) + c(d))");
        AssertParsedEqualsExpected("a(@b) + c(@d)", "(a((@b)) + c((@d)))");
    }

    [TestMethod]
    public void TestUnaryPrecedence()
    {
        AssertParsedEqualsExpected("++a", "(++a)");
        AssertParsedEqualsExpected("++--a", "(++(--a))");
        AssertParsedEqualsExpected("++a--", "(++(a--))");
        AssertParsedEqualsExpected("!-+a", "(!(-(+a)))");
        AssertParsedEqualsExpected("!-++a", "(!(-(++a)))");
        AssertParsedEqualsExpected("!-a++", "(!(-(a++)))");
        AssertParsedEqualsExpected("!!!a", "(!(!(!a)))");
        AssertParsedEqualsExpected("@a", "(@a)");
    }

    [TestMethod]
    public void TestUnaryOrBinaryPrecedence()
    {
        AssertParsedEqualsExpected("-a * b", "((-a) * b)");
        AssertParsedEqualsExpected("-a % b", "((-a) % b)");
        AssertParsedEqualsExpected("!a + b", "((!a) + b)");
        AssertParsedEqualsExpected("!a ^ b", "((!a) ^ b)");
        AssertParsedEqualsExpected("-a", "(-a)");
        AssertParsedEqualsExpected("!a", "(!a)");
    }

    [TestMethod]
    public void TestBinaryPrecedence()
    {
        AssertParsedEqualsExpected(
            "a := b + c * d ^ e - f / g", "(a := ((b + (c * (d ^ e))) - (f / g)))"
        );
    }

    [TestMethod]
    public void TestBinaryAssociativity()
    {
        AssertParsedEqualsExpected("a := b := c", "(a := (b := c))");
        AssertParsedEqualsExpected("a := b = c", "(a := (b = c))");
        AssertParsedEqualsExpected("a := b == c", "(a := (b == c))");
        AssertParsedEqualsExpected("a := b < c", "(a := (b < c))");
        AssertParsedEqualsExpected("a := b <= c", "(a := (b <= c))");
        AssertParsedEqualsExpected("a := b # c", "(a := (b # c))");
        AssertParsedEqualsExpected("a := b != c", "(a := (b != c))");
        AssertParsedEqualsExpected("a += b *= c", "(a += (b *= c))");
        AssertParsedEqualsExpected("a %= b -= c", "((a %= b) -= c)");
        AssertParsedEqualsExpected("a + b - c", "((a + b) - c)");
        AssertParsedEqualsExpected("a + b > c", "((a + b) > c)");
        AssertParsedEqualsExpected("a + b >= c", "((a + b) >= c)");
        AssertParsedEqualsExpected("a * b / c", "((a * b) / c)");
        AssertParsedEqualsExpected("a ^ b ^ c", "(a ^ (b ^ c))");
        AssertParsedEqualsExpected("a^b^c", "(a ^ (b ^ c))");
        AssertParsedEqualsExpected("a * b % c", "((a * b) % c)");
        AssertParsedEqualsExpected("a $ b $ c", "((a $ b) $ c)");
        AssertParsedEqualsExpected("a .or. b", "(a .or. b)");
        AssertParsedEqualsExpected("a .and. b", "(a .and. b)");
        AssertParsedEqualsExpected("a .and. b .or. c", "((a .and. b) .or. c)");
        AssertParsedEqualsExpected("a .or. b .and. c", "(a .or. (b .and. c))");
        AssertParsedEqualsExpected("!a .and. !b .or. !c", "(((!a) .and. (!b)) .or. (!c))");
        AssertParsedEqualsExpected("!(a .or. b .and. c)", "(!(a .or. (b .and. c)))");
    }

    [TestMethod]
    public void TestGrouping()
    {
        AssertParsedEqualsExpected("a + (b + c) + d", "((a + (b + c)) + d)");
        AssertParsedEqualsExpected("a ^ (b + c)", "(a ^ (b + c))");
        AssertParsedEqualsExpected("(!a)", "(!a)");
    }

    [TestMethod]
    public void TestLiterals()
    {
        AssertParsedEqualsExpected(".t.", ".t.");
        AssertParsedEqualsExpected("123", "123");
        AssertParsedEqualsExpected("'hello'", "'hello'");
        AssertParsedEqualsExpected("NIL", "NIL");
        AssertParsedEqualsExpected("nil", "nil");
    }

    [TestMethod]
    public void TestObjectAccess()
    {
        AssertParsedEqualsExpected("a:b", "a:b");
        AssertParsedEqualsExpected("a:b:c", "a:b:c");
        AssertParsedEqualsExpected("a:b()", "a:b()");
        AssertParsedEqualsExpected("a:b(c)", "a:b(c)");
        AssertParsedEqualsExpected("a:b():c", "a:b():c");
        AssertParsedEqualsExpected("a:b()[1]", "a:b()[1]");
        AssertParsedEqualsExpected("a:b()['key']", "a:b()['key']");
        AssertParsedEqualsExpected("a:b()[1 + c]", "a:b()[(1 + c)]");
    }

    [TestMethod]
    public void TestArrayDeclaration()
    {
        AssertParsedEqualsExpected("a := { }", "(a := { })");
        AssertParsedEqualsExpected("a := { 1 }", "(a := { 1 })");
        AssertParsedEqualsExpected("a := { 1, 2 }", "(a := { 1, 2 })");
        AssertParsedEqualsExpected("a := { 1, b() }", "(a := { 1, b() })");
        AssertParsedEqualsExpected("a := { ;\n    1, ;\n    b() ;\n}", "(a := { 1, b() })");
    }

    [TestMethod]
    public void TestHashDeclaration()
    {
        AssertParsedEqualsExpected("a := { => }", "(a := { => })");
        AssertParsedEqualsExpected("a := { 'b' => 1 }", "(a := { 'b' => 1 })");
        AssertParsedEqualsExpected("a := { 'b' => 1, 'c' => 2 }", "(a := { 'b' => 1, 'c' => 2 })");
        AssertParsedEqualsExpected("a := { 'b' => 1, 'c' => d() }", "(a := { 'b' => 1, 'c' => d() })");
        AssertParsedEqualsExpected(
            "a := { ;\n    'b' => 1, ;\n    'c' => d() ;\n}",
            "(a := { 'b' => 1, 'c' => d() })"
        );
    }

    [TestMethod]
    public void TestCodeblock()
    {
        AssertParsedEqualsExpected("a := { || b() }", "(a := { || b() })");
        AssertParsedEqualsExpected("a := { |b| c(b) }", "(a := { |b| c(b) })");
        AssertParsedEqualsExpected("a := { |b,c| d(b, c) }", "(a := { |b, c| d(b, c) })");
        AssertParsedEqualsExpected("a := { |b,c| d(b), e(c) }", "(a := { |b, c| d(b), e(c) })");
    }

    [TestMethod]
    public void TestConditional()
    {
        AssertParsedEqualsExpected("a := iif(b, 1, 0)", "(a := iif(b, 1, 0))");
    }

    [TestMethod]
    public void TestComments()
    {
        AssertParsedEqualsExpected("a:b(c/* Not sure if this should be here */)", "a:b(c)");
        AssertParsedEqualsExpected("a:b(/* Not sure if this should be here */c)", "a:b(c)");
        AssertParsedEqualsExpected("a:b(c) // Comment at the end", "a:b(c)");
    }

    private static void AssertParsedEqualsExpected(string source, string expected)
    {
        var lexer = new Lexer(source);
        var reader = new SourceReader(lexer);
        var parser = new ExpressionParser(reader);

        var result = parser.Parse();
        var actual = result?.PrettyPrint();
        Assert.AreEqual(expected, actual);
    }
}