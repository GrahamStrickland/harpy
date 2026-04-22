using HarpyTests.CodeGenTests.Utils;

namespace HarpyTests.CodeGenTests;

[TestClass]
public class TestExpressionCodeGen
{
    [TestMethod]
    public void TestArrayDeclaratorExpression()
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
    public void TestCallExpression()
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
    public void TestConditionalExpression()
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
    public void TestHashDeclaratorExpression()
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

    [TestMethod]
    public void TestIndexExpression()
    {
        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local nNo := a[1]",
            """
            public static partial class TestProgram
            {
                private double nNo = a[0];
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local nNo := a[b]",
            """
            public static partial class TestProgram
            {
                private double nNo = a[(b - 1)];
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local nNo := a[\"string\"]",
            """
            public static partial class TestProgram
            {
                private double nNo = a["string"];
            }
            """
        );
    }

    [TestMethod]
    public void TestBooleanLiteralExpression()
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
    public void TestNumericLiteralExpression()
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
    public void TestStringLiteralExpression()
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
    public void TestNilLiteralExpression()
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
    public void TestNameExpression()
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
    public void TestCodeblockDeclarationExpression()
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
    public void TestObjectAccessExpression()
    {
        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local nNo := a:nNo",
            """
            public static partial class TestProgram
            {
                private double nNo = a.nNo;
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local nNo := a:GetNo()",
            """
            public static partial class TestProgram
            {
                private double nNo = a.GetNo();
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local nNo := a:GetNo():nNo",
            """
            public static partial class TestProgram
            {
                private double nNo = a.GetNo().nNo;
            }
            """
        );
        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local nNo := a:GetNo(b):nNo",
            """
            public static partial class TestProgram
            {
                private double nNo = a.GetNo(b).nNo;
            }
            """
        );
        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local nNo := a:GetNo(b):nNo",
            """
            public static partial class TestProgram
            {
                private double nNo = a.GetNo(b).nNo;
            }
            """
        );
        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local nNo := a:GetNo(b, c:nNo):nNo",
            """
            public static partial class TestProgram
            {
                private double nNo = a.GetNo(b, c.nNo).nNo;
            }
            """
        );
    }

    [TestMethod]
    public void TestOperatorExpression()
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

    [TestMethod]
    public void TestPostfixExpression()
    {
        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local nNo := nOtherNo++",
            """
            public static partial class TestProgram
            {
                private double nNo = nOtherNo++;
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local nNo := nOtherNo--",
            """
            public static partial class TestProgram
            {
                private double nNo = nOtherNo--;
            }
            """
        );
    }

    [TestMethod]
    public void TestPrefixExpression()
    {
        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local nNo := +1",
            """
            public static partial class TestProgram
            {
                private double nNo = +1;
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local nNo := -1",
            """
            public static partial class TestProgram
            {
                private double nNo = -1;
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local nNo := +3.14159",
            """
            public static partial class TestProgram
            {
                private double nNo = +3.14159;
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local nNo := -3.14159",
            """
            public static partial class TestProgram
            {
                private double nNo = -3.14159;
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local nNo := +nOtherNo",
            """
            public static partial class TestProgram
            {
                private double nNo = +nOtherNo;
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local nNo := -nOtherNo",
            """
            public static partial class TestProgram
            {
                private double nNo = -nOtherNo;
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local lBool1 := .f.\nlocal lBool2 := !lBool1",
            """
            public static partial class TestProgram
            {
                private bool lBool1 = false;
                private bool lBool2 = !lBool1;
            }
            """
        );

        // TODO: This won't work, since we can't return ArgumentSyntax instead of ExpressionSyntax
        // CodeGenUtils.AssertCodeGenEqualsExpected(
        //     "local nNum := 1\nlocal lResult := func(@nNum)",
        //     """
        //     public static partial class TestProgram
        //     {
        //         private double nNum = 1;
        //         private bool lResult = func(ref nNum);
        //     }
        //     """
        // );

        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local nNo := ++nOtherNo",
            """
            public static partial class TestProgram
            {
                private double nNo = ++nOtherNo;
            }
            """
        );

        CodeGenUtils.AssertCodeGenEqualsExpected(
            "local nNo := --nOtherNo",
            """
            public static partial class TestProgram
            {
                private double nNo = --nOtherNo;
            }
            """
        );
    }
}