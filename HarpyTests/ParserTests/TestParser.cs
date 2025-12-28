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
        AssertParsedEqualsExpected(
            "function a(b)\n\n    local c := b\n\n    if b > 0\n        return 0\n    endif\n\nreturn c",
            "function a(b)\nlocal (c := b)\nif (b > 0)\nreturn 0\nendif\nreturn c"
        );
        AssertParsedEqualsExpected(
            "function a(b)\n\n    local c := b\n\n    if b > 0\n        return .t.\n    endif\n\nreturn .f.",
            "function a(b)\nlocal (c := b)\nif (b > 0)\nreturn .t.\nendif\nreturn .f."
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
        AssertParsedEqualsExpected(
            "procedure a(b)\n\n    local c := b\n\n    if b > 0\n        return\n    endif\n\nreturn",
            "procedure a(b)\nlocal (c := b)\nif (b > 0)\nreturn\nendif\nreturn"
        );
        AssertParsedEqualsExpected(
            "procedure a(b)\n\n    local c := b\n\n    if b > 0\n        \n        d(b)\nreturn\n    endif\n\nreturn",
            "procedure a(b)\nlocal (c := b)\nif (b > 0)\nd(b)\nreturn\nendif\nreturn"
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
            "for each a in b\n\n    c(a)\n\nnext",
            "for each a in b\nc(a)\nnext"
        );
        AssertParsedEqualsExpected(
            "for i := 10 to 1 step -1\n\n    if c(i)\n\n\n\n        d(i)\n\n    else\n\n        loop\n\n    endif\n\nnext",
            "for (i := 10) to 1 step (-1)\nif c(i)\nd(i)\nelse\nloop\nendif\nnext"
        );
        AssertParsedEqualsExpected(
            "for each a in b\n\n    if c(a)\n\n\n\n        d(a)\n\n    else\n\n        loop\n\n    endif\n\nnext",
            "for each a in b\nif c(a)\nd(a)\nelse\nloop\nendif\nnext"
        );
        AssertParsedEqualsExpected(
            "for i := 10 to 1 step -1\n\n    if c(i)\n\n\n\n        d(i)\n\n    else\n\n        exit\n\n    endif\n\nnext",
            "for (i := 10) to 1 step (-1)\nif c(i)\nd(i)\nelse\nexit\nendif\nnext"
        );
        AssertParsedEqualsExpected(
            "for each a in b\n\n    if c(a)\n\n\n\n        d(a)\n\n    else\n\n        exit\n\n    endif\n\nnext",
            "for each a in b\nif c(a)\nd(a)\nelse\nexit\nendif\nnext"
        );
    }

    [TestMethod]
    public void TestBeginSequenceStatement()
    {
        AssertParsedEqualsExpected(
            "begin sequence\n\n    a()\n\nrecover\n\n    c()\n\nend sequence",
            "begin sequence\na()\nrecover\nc()\nend sequence"
        );
        AssertParsedEqualsExpected(
            "begin sequence\n\n    a()\n\nrecover using b\n\n    c(b)\n\nend sequence",
            "begin sequence\na()\nrecover using b\nc(b)\nend sequence"
        );
        AssertParsedEqualsExpected(
            "begin sequence with { |e| a(e) }\n\n    b()\n\nrecover using c\n\n    d(c)\n\nendsequence",
            "begin sequence with { |e| a(e) }\nb()\nrecover using c\nd(c)\nend sequence"
        );
        AssertParsedEqualsExpected(
            "begin sequence with { |e| a(e) }\n\n    b()\n\nrecover using c\n\n    d(c)\n\nalways\n    e()\n\nend",
            "begin sequence with { |e| a(e) }\nb()\nrecover using c\nd(c)\nalways\ne()\nend sequence"
        );
    }

    [TestMethod]
    public void TestCallStatement()
    {
        AssertParsedEqualsExpected(
            "a:b(c)",
            "a:b(c)"
        );
        AssertParsedEqualsExpected(
            "a:b:c(d)",
            "a:b:c(d)"
        );
        AssertParsedEqualsExpected(
            "a():b:c(d)",
            "a():b:c(d)"
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

        var root = parser.Parse("test");
        var actual = root.PrettyPrint();

        Assert.AreEqual(expected, actual);
    }
}