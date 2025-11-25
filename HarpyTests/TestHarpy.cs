using Harpy.Lexer;
using Harpy.Parser;

namespace HarpyTests;

[TestClass]
public class TestHarpy
{
    [TestMethod]
    public void TestMultipleFunctions()
    {
        AssertParsedEqualsExpected(
            "/*******\n* Function header\n*******/\n\n#include \"header.ch\"\n\nstatic s_a := .f.\n\nfunction a(b)\n\n    local c := b\n\n    if b > 0\n        return 0\n    endif\n\nreturn c\n\nfunction d(e)\n\n    local c := e\n\n    if e > 0\n        return 0\n    endif\n\nreturn 1",
            "static (s_a := .f.)function a(b)\nlocal (c := b)\nif (b > 0)\nreturn 0\nendif\nreturn cfunction d(e)\nlocal (c := e)\nif (e > 0)\nreturn 0\nendif\nreturn 1"
        );
    }

    // TODO: Move this to testing code generation when that is implemented.
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