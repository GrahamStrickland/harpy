using Harpy.Lexer;
using Harpy.Parser;

namespace HarpyTests.ParserTests;

[TestClass]
public class TestParser
{
    [TestMethod]
    public void TestFunctionDefinition()
    {
        AssertParsedEqualsExpected(
            "function a()\n\n    local c := b\n\nreturn c",
            "function a()\nlocal (c := b)\nreturn c"
        );
        AssertParsedEqualsExpected(
            "function a(b)\n\n    local c := b\n\nreturn c",
            "function a(b)\nlocal (c := b)\nreturn c"
        );
        AssertParsedEqualsExpected(
            "function a(b, c, d)\n\n    local c := b\n\nreturn c",
            "function a(b, c, d)\nlocal (c := b)\nreturn c"
        );
    }

    [TestMethod]
    public void TestProcedureDefinition()
    {
        AssertParsedEqualsExpected(
            "procedure a()\n\n    local c := b\n\nreturn",
            "procedure a()\nlocal (c := b)\nreturn"
        );
        AssertParsedEqualsExpected(
            "procedure a(b)\n\n    local c := b\n\nreturn",
            "procedure a(b)\nlocal (c := b)\nreturn"
        );
        AssertParsedEqualsExpected(
            "procedure a(b, c, d)\n\n    local c := b\n\nreturn",
            "procedure a(b, c, d)\nlocal (c := b)\nreturn"
        );
    }

    [TestMethod]
    public void TestLocalDeclarationStatementIndexing()
    {
        AssertParsedEqualsExpected(
            "function a(b)\n\n    local c := b[1]\n\nreturn c",
            "function a(b)\nlocal (c := b[1])\nreturn c"
        );
        AssertParsedEqualsExpected(
            "function a(b)\n\n    local c := b['key']\n\nreturn c",
            "function a(b)\nlocal (c := b['key'])\nreturn c"
        );
        // AssertParsedEqualsExpected(
        //     "function a(b)\n\n    local c := b[[key]]\n\nreturn c",
        //     "function a(b)\nlocal (c := b[[key]])\nreturn c"
        // );
        // AssertParsedEqualsExpected(
        //     "function a()\n\n    local b := [hello]\n\nreturn b",
        //     "function a()\nlocal (b := [hello])\nreturn b"
        // );
        AssertParsedEqualsExpected(
            "function a(b)\n\n    local c := b[1,2]\n\nreturn c",
            "function a(b)\nlocal (c := b[1, 2])\nreturn c"
        );
    }

    [TestMethod]
    public void TestStatics()
    {
        AssertParsedEqualsExpected(
            "static a := b",
            "static (a := b)"
        );
        AssertParsedEqualsExpected(
            "static a := nil",
            "static (a := nil)"
        );
        AssertParsedEqualsExpected(
            "static function a()\n\n    local c := b\n\nreturn c",
            "static function a()\nlocal (c := b)\nreturn c"
        );
        AssertParsedEqualsExpected(
            "function a()\n\n    static c := b\n\nreturn c",
            "function a()\nstatic (c := b)\nreturn c"
        );
    }

    [TestMethod]
    public void TestIfStatement()
    {
        AssertParsedEqualsExpected(
            "if a(b, c, d)\n\n    e()\n\nendif",
            "if a(b, c, d)\ne()\nendif"
        );
        AssertParsedEqualsExpected(
            "if !a(b, c, d) .and. !e\n\n    f()\n\nendif",
            "if ((!a(b, c, d)) .and. (!e))\nf()\nendif"
        );
        AssertParsedEqualsExpected(
            "if a(b, c, d)\n\n    e()\n\nelse\n\n    f()\n\nendif",
            "if a(b, c, d)\ne()\nelse\nf()\nendif"
        );
        AssertParsedEqualsExpected(
            "if a(b, c, d)\n\n    e()\n\nelseif f(b)\n\n    g()\n\n\n\nendif",
            "if a(b, c, d)\ne()\nelseif f(b)\ng()\nendif"
        );
        AssertParsedEqualsExpected(
            "if a(b, /*@*/c, d)\n\n    e()\n\nelseif f(b)\n\n    g()\n\nelse\n\n    h()\n\nendif",
            "if a(b, c, d)\ne()\nelseif f(b)\ng()\nelse\nh()\nendif"
        );
    }

    [TestMethod]
    public void TestWhileLoop()
    {
        AssertParsedEqualsExpected(
            "while a(b, c, d)\n\n    e()\n\nend",
            "while a(b, c, d)\ne()\nend while"
        );
        AssertParsedEqualsExpected(
            "while a(b, c, d)\n\n    e()\n    f()\n\nend",
            "while a(b, c, d)\ne()\nf()\nend while"
        );
        AssertParsedEqualsExpected(
            "while a(b, c, d)\n\n    e()\n    f()\n\nend while",
            "while a(b, c, d)\ne()\nf()\nend while"
        );
        AssertParsedEqualsExpected(
            "  // Be careful with this\nwhile a(b, c, d)\n\n    e()\n    f()\n\nendwhile",
            "while a(b, c, d)\ne()\nf()\nend while"
        );
    }

    [TestMethod]
    public void TestForLoop()
    {
        AssertParsedEqualsExpected(
            "for i := 1 to 10\n\n    a(i)\n\nnext",
            "for (i := 1) to 10\na(i)\nnext"
        );
        AssertParsedEqualsExpected(
            "for i := 10 to 1 step -1\n\n    a(i)\n\nnext",
            "for (i := 10) to 1 step (-1)\na(i)\nnext"
        );
        AssertParsedEqualsExpected(
            "for i := 10 to 1 step -1\n\n    if c(i)\n\n\n\n        d(i)\n\n    else\n\n        loop\n\n    endif\n\nnext",
            "for (i := 10) to 1 step (-1)\nif c(i)\nd(i)\nelse\nloop\nendif\nnext"
        );
    }

    [TestMethod]
    public void TestAssignmentStatement()
    {
        AssertParsedEqualsExpected(
            "if a(b, c, d)\n\n    e := 1\n\nendif",
            "if a(b, c, d)\n(e := 1)\nendif"
        );
        AssertParsedEqualsExpected(
            "if a(b, c, d)\n\n    e:f := 1\n\nendif",
            "if a(b, c, d)\n(e:f := 1)\nendif"
        );
    }

    /// <summary>
    ///     Parses the given chunk of code and verifies that it matches the expected
    ///     pretty-printed result.
    /// </summary>
    private static void AssertParsedEqualsExpected(string source, string expected)
    {
        var lexer = new Lexer(source);
        var parser = new Parser(lexer);

        var root = parser.Parse();
        var actual = root.Children.Aggregate("", (current, node) => current + node.PrettyPrint());

        Assert.AreEqual(expected, actual);
    }
}