using HarpyTests.CodeGenTests.Utils;

namespace HarpyTests.CodeGenTests;

[TestClass]
public class TestStatementCodeGen
{
    [TestMethod]
    public void TestLocalVariableDeclaration()
    {
        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local lOK",
            """
            public static partial class TestProgram
            {
                private bool lOK;
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local lOK := .t.",
            """
            public static partial class TestProgram
            {
                private bool lOK = true;
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
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
        CodeGenUtils.AssertCodeGenEqualsExpected(
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
        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local lVar := GetBool()",
            """
            public static partial class TestProgram
            {
                private bool lVar = GetBool();
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local cString := \"hello\"\nlocal lVar := GetBool(cString)",
            """
            public static partial class TestProgram
            {
                private string cString = "hello";
                private bool lVar = GetBool(cString);
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
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
        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local lBool := .t.\nlocal lVar := iif(lBool, .t., )",
            """
            public static partial class TestProgram
            {
                private bool lBool = true;
                private bool lVar = lBool ? true : null;
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local lBool := .t.\nlocal lVar := iif(lBool, , .f.)",
            """
            public static partial class TestProgram
            {
                private bool lBool = true;
                private bool lVar = lBool ? null : false;
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
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
        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local nNumber := 1 + 2",
            """
            public static partial class TestProgram
            {
                private double nNumber = 1 + 2;
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local nNumber := 1 * 2",
            """
            public static partial class TestProgram
            {
                private double nNumber = 1 * 2;
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local cString := \"Hello\" + \", world\"",
            """
            public static partial class TestProgram
            {
                private string cString = "Hello" + ", world";
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local nNumber := 2 ^ 3",
            """
            public static partial class TestProgram
            {
                private double nNumber = Math.Pow(2, 3);
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local nNumber := (4 / 2) ^ 3",
            """
            public static partial class TestProgram
            {
                private double nNumber = Math.Pow(4 / 2, 3);
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local nNumber := ((4 * 6) / 2) ^ 3",
            """
            public static partial class TestProgram
            {
                private double nNumber = Math.Pow(4 * 6 / 2, 3);
            }
            """
        );
    }
}
