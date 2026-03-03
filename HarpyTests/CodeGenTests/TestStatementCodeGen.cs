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

}
