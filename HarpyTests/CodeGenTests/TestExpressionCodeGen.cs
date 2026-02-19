using HarpyTests.CodeGenTests.Utils;

namespace HarpyTests.CodeGenTests;

[TestClass]
public class TestExpressionCodeGen
{
    [TestMethod]
    public void TestBooleanLiteral()
    {
        CodeGenUtils.AssertCodeGenEqualsExpected(
            "static lVar := .t.",
            """
            public static partial class TestProgram
            {
                private static bool lVar = true;
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
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
        CodeGenUtils.AssertCodeGenEqualsExpected(
            "static nVar := 123",
            """
            public static partial class TestProgram
            {
                private static double nVar = 123;
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
            "static nVar := 0x1234",
            """
            public static partial class TestProgram
            {
                private static double nVar = 0x1234;
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
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
        CodeGenUtils.AssertCodeGenEqualsExpected(
            "static cVar := 'hello'",
            """

            public static partial class TestProgram
            {
                private static string cVar = "hello";
            }
            
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
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
        CodeGenUtils.AssertCodeGenEqualsExpected(
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
    public void TestArrayDeclarator()
    {
        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local aValues := {}",
            """
            public static partial class TestProgram
            {
                private List<dynamic> aValues = new List<dynamic>{};
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local aValues := { 1 }",
            """
            public static partial class TestProgram
            {
                private List<dynamic> aValues = new List<dynamic>{1};
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local aValues := { 1, 2 }",
            """
            public static partial class TestProgram
            {
                private List<dynamic> aValues = new List<dynamic>{1, 2};
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local aValues := { 1, nil }",
            """
            public static partial class TestProgram
            {
                private List<dynamic> aValues = new List<dynamic>{1, null};
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local aValues := { 1, \"two\" }",
            """
            public static partial class TestProgram
            {
                private List<dynamic> aValues = new List<dynamic>{1, "two"};
            }
            """
        );

    }

    [TestMethod]
    public void TestCodeblockDeclaration()
    {
        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local bCodeblock := { || }",
            """
            public static partial class TestProgram
            {
                private dynamic bCodeblock = () => { };
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local bCodeblock := { |a| b(a) }",
            """
            public static partial class TestProgram
            {
                private dynamic bCodeblock = a => { b(a); };
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local bCodeblock := { |a, b| c(a, b) }",
            """
            public static partial class TestProgram
            {
                private dynamic bCodeblock = (a, b) => { c(a, b); };
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local bCodeblock := { |a, b| c(a), d(b) }",
            """
            public static partial class TestProgram
            {
                private dynamic bCodeblock = (a, b) => { c(a); d(b); };
            }
            """
        );
    }

    [TestMethod]
    public void TestHashDeclarator()
    {
        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local hHash := { => }",
            """
            public static partial class TestProgram
            {
                private Dictionary<string, dynamic> hHash = new Dictionary<string, dynamic>();
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local hHash := { \"key1\" => 1 }",
            """
            public static partial class TestProgram
            {
                private Dictionary<string, dynamic> hHash = new Dictionary<string, dynamic>{ {"key1", 1} };
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local hHash := { \"key1\" => 1, \"key2\" => 2 }",
            """
            public static partial class TestProgram
            {
                private Dictionary<string, dynamic> hHash = new Dictionary<string, dynamic>{ {"key1", 1}, {"key2", 2} };
            }
            """
        );
    }
}
