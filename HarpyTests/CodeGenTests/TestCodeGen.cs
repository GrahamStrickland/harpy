using Harpy.CodeGen;
using Harpy.Lexer;
using Harpy.Parser;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace HarpyTests.CodeGenTests;

[TestClass]
public class TestCodeGen
{
    [TestMethod]
    public void TestBooleanLiteral()
    {
        AssertCodeGenEqualsExpected(
            "static lVar := .t.",
            """
            public static partial class TestProgram
            {
                private static bool lVar = true;
            }
            """
        );

        AssertCodeGenEqualsExpected(
            "static lVar := .f.",
            """
            public static partial class TestProgram
            {
                private static bool lVar = false;
            }
            """
        );
    }

    [TestMethod]
    public void TestNumericLiteral()
    {
        AssertCodeGenEqualsExpected(
            "static nVar := 123",
            """
            public static partial class TestProgram
            {
                private static double nVar = 123;
            }
            """
        );

        AssertCodeGenEqualsExpected(
            "static nVar := 0x1234",
            """
            public static partial class TestProgram
            {
                private static double nVar = 0x1234;
            }
            """
        );

        AssertCodeGenEqualsExpected(
            "static nVar := 12.34",
            """
            public static partial class TestProgram
            {
                private static double nVar = 12.34;
            }
            """
        );
    }

    [TestMethod]
    public void TestStringLiteral()
    {
        AssertCodeGenEqualsExpected(
            "static cVar := 'hello'",
            """

            public static partial class TestProgram
            {
                private static string cVar = "hello";
            }
            
            """
        );

        AssertCodeGenEqualsExpected(
            "static cVar := \"world\"",
            """

            public static partial class TestProgram
            {
                private static string cVar = "world";
            }
            
            """
        );
    }

    [TestMethod]
    public void TestNilLiteral()
    {
        AssertCodeGenEqualsExpected(
            "static xVar := nil",
            """
            public static partial class TestProgram
            {
                private static dynamic xVar = null;
            }
            """
        );
    }

    [TestMethod]
    public void TestLocalVariableDeclaration()
    {
        AssertCodeGenEqualsExpected(
            "local lOK",
            """
            public static partial class TestProgram
            {
                private bool lOK;
            }
            """
        );

        AssertCodeGenEqualsExpected(
            "local lOK := .t.",
            """
            public static partial class TestProgram
            {
                private bool lOK = true;
            }
            """
        );

        AssertCodeGenEqualsExpected(
            "local nNumber := 1.2345",
            """
            public static partial class TestProgram
            {
                private double nNumber = 1.2345;
            }
            """
        );
    }

    [TestMethod]
    public void TestVariableDeclarationWithNameExpression()
    {
        AssertCodeGenEqualsExpected(
            "local lVar := lLogical",
            """
            public static partial class TestProgram
            {
                private bool lVar = lLogical;
            }
            """
        );
    }

    [TestMethod]
    public void TestVariableDeclarationWithFunctionCall()
    {
        AssertCodeGenEqualsExpected(
            "local lVar := GetBool()",
            """
            public static partial class TestProgram
            {
                private bool lVar = GetBool();
            }
            """
        );

        AssertCodeGenEqualsExpected(
            "local cString := \"hello\"\nlocal lVar := GetBool(cString)",
            """
            public static partial class TestProgram
            {
                private string cString = "hello";
                private bool lVar = GetBool(cString);
            }
            """
        );

        AssertCodeGenEqualsExpected(
            "local cString := \"hello\"\nlocal nNumber := 1.2345\nlocal lVar := GetBool(cString, nNumber)",
            """
            public static partial class TestProgram
            {
                private string cString = "hello";
                private double nNumber = 1.2345;
                private bool lVar = GetBool(cString, nNumber);
            }
            """
        );
    }

    // TODO: Figure out how to make the types nullable from the assignment.
    [TestMethod]
    public void TestVariableDeclarationWithConditional()
    {
        AssertCodeGenEqualsExpected(
            "local lBool := .t.\nlocal lVar := iif(lBool, .t., )",
            """
            public static partial class TestProgram
            {
                private bool lBool = true;
                private bool lVar = lBool ? true : null;
            }
            """
        );

        AssertCodeGenEqualsExpected(
            "local lBool := .t.\nlocal lVar := iif(lBool, , .f.)",
            """
            public static partial class TestProgram
            {
                private bool lBool = true;
                private bool lVar = lBool ? null : false;
            }
            """
        );

        AssertCodeGenEqualsExpected(
            "local lBool := .t.\nlocal lVar := iif(lBool, .t., .f.)",
            """
            public static partial class TestProgram
            {
                private bool lBool = true;
                private bool lVar = lBool ? true : false;
            }
            """
        );
    }

    [TestMethod]
    public void TestVariableDeclarationWithOperatorExpression()
    {
        AssertCodeGenEqualsExpected(
            "local nNumber := 1 + 2",
            """
            public static partial class TestProgram
            {
                private double nNumber = 1 + 2;
            }
            """
        );

        AssertCodeGenEqualsExpected(
            "local nNumber := 1 * 2",
            """
            public static partial class TestProgram
            {
                private double nNumber = 1 * 2;
            }
            """
        );

        AssertCodeGenEqualsExpected(
            "local cString := \"Hello\" + \", world\"",
            """
            public static partial class TestProgram
            {
                private string cString = "Hello" + ", world";
            }
            """
        );

        AssertCodeGenEqualsExpected(
            "local nNumber := 2 ^ 3",
            """
            public static partial class TestProgram
            {
                private double nNumber = Math.Pow(2, 3);
            }
            """
        );

        AssertCodeGenEqualsExpected(
            "local nNumber := (4 / 2) ^ 3",
            """
            public static partial class TestProgram
            {
                private double nNumber = Math.Pow(4 / 2, 3);
            }
            """
        );

        AssertCodeGenEqualsExpected(
            "local nNumber := ((4 * 6) / 2) ^ 3",
            """
            public static partial class TestProgram
            {
                private double nNumber = Math.Pow(4 * 6 / 2, 3);
            }
            """
        );
    }

    [TestMethod]
    public void TestArrayDeclaration()
    {
        AssertCodeGenEqualsExpected(
            "local aValues := {}",
            """
            public static partial class TestProgram
            {
                private List<dynamic> aValues = new List<dynamic>{};
            }
            """
        );

        AssertCodeGenEqualsExpected(
            "local aValues := { 1 }",
            """
            public static partial class TestProgram
            {
                private List<dynamic> aValues = new List<dynamic>{1};
            }
            """
        );

        AssertCodeGenEqualsExpected(
            "local aValues := { 1, 2 }",
            """
            public static partial class TestProgram
            {
                private List<dynamic> aValues = new List<dynamic>{1, 2};
            }
            """
        );

        AssertCodeGenEqualsExpected(
            "local aValues := { 1, nil }",
            """
            public static partial class TestProgram
            {
                private List<dynamic> aValues = new List<dynamic>{1, null};
            }
            """
        );

        AssertCodeGenEqualsExpected(
            "local aValues := { 1, \"two\" }",
            """
            public static partial class TestProgram
            {
                private List<dynamic> aValues = new List<dynamic>{1, "two"};
            }
            """
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
        var expectedNormalized = SyntaxFactory.ParseCompilationUnit(expected)
            .NormalizeWhitespace().ToFullString();

        Assert.AreEqual(expectedNormalized, actual);
    }
}
