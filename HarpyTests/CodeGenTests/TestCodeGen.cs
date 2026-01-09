using Harpy.CodeGen;
using Harpy.Lexer;
using Harpy.Parser;
using Microsoft.CodeAnalysis;

namespace HarpyTests.CodeGenTests;

[TestClass]
public class TestCodeGen
{
    [TestMethod]
    public void TestBooleanLiteral()
    {
        AssertCodeGenEqualsExpected(
            "static lVar := .t.",
            @"
            public static partial class TestProgram
            {
                private static bool lVar = true;
            }
            "
        );

        AssertCodeGenEqualsExpected(
            "static lVar := .f.",
            @"
            public static partial class TestProgram
            {
                private static bool lVar = false;
            }
            "
        );
    }

    [TestMethod]
    public void TestNumericLiteral()
    {
        AssertCodeGenEqualsExpected(
            "static nVar := 123",
            @"
            public static partial class TestProgram
            {
                private static double nVar = 123;
            }
            "
        );

        AssertCodeGenEqualsExpected(
            "static nVar := 0x1234",
            @"
            public static partial class TestProgram
            {
                private static double nVar = 0x1234;
            }
            "
        );

        AssertCodeGenEqualsExpected(
            "static nVar := 12.34",
            @"
            public static partial class TestProgram
            {
                private static double nVar = 12.34;
            }
            "
        );
    }

    [TestMethod]
    public void TestStringLiteral()
    {
        AssertCodeGenEqualsExpected(
            "static cVar := 'hello'",
            @"
            public static partial class TestProgram
            {
                private static string cVar = ""hello"";
            }
            "
        );

        AssertCodeGenEqualsExpected(
            "static cVar := \"world\"",
            @"
            public static partial class TestProgram
            {
                private static string cVar = ""world"";
            }
            "
        );
    }

    [TestMethod]
    public void TestNilLiteral()
    {
        AssertCodeGenEqualsExpected(
            "static xVar := nil",
            @"
            public static partial class TestProgram
            {
                private static dynamic xVar = null;
            }
            "
        );
    }

    private static void AssertCodeGenEqualsExpected(string source, string expected)
    {
        var lexer = new Lexer(source);
        var parser = new Parser(lexer);
        var root = parser.Parse("TestProgram");

        var context = new CodeGenContext("TestProgram");
        var syntaxNode = root.Walk(context);

        // Normalize whitespace for comparison
        var actual = syntaxNode.NormalizeWhitespace().ToFullString();
        var expectedNormalized = Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ParseCompilationUnit(expected)
            .NormalizeWhitespace().ToFullString();

        Assert.AreEqual(expectedNormalized, actual);
    }
}
