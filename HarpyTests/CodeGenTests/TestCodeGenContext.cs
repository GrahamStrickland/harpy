using Harpy.CodeGen;
using Microsoft.CodeAnalysis.CSharp;

namespace HarpyTests.CodeGenTests;

[TestClass]
public class TestCodeGenContext
{
    [TestMethod]
    public void TestConstructor_SetsPartialClassName()
    {
        var context = new CodeGenContext("MyProgram");

        Assert.AreEqual("MyProgram", context.PartialClassName);
    }

    [TestMethod]
    public void TestVariableTypes_InitiallyEmpty()
    {
        var context = new CodeGenContext("TestProgram");

        Assert.AreEqual(0, context.VariableTypes.Count);
    }

    [TestMethod]
    public void TestScopeStack_InitiallyEmpty()
    {
        var context = new CodeGenContext("TestProgram");

        Assert.AreEqual(0, context.ScopeStack.Count);
    }

    [TestMethod]
    public void TestTopLevelMembers_InitiallyEmpty()
    {
        var context = new CodeGenContext("TestProgram");

        Assert.AreEqual(0, context.TopLevelMembers.Count);
    }

    [TestMethod]
    public void TestInLoop_DefaultsFalse()
    {
        var context = new CodeGenContext("TestProgram");

        Assert.IsFalse(context.InLoop);
    }

    [TestMethod]
    public void TestInFunctionOrProcedure_DefaultsFalse()
    {
        var context = new CodeGenContext("TestProgram");

        Assert.IsFalse(context.InFunctionOrProcedure);
    }

    [TestMethod]
    public void TestRegisterVariable_AddsToVariableTypes()
    {
        var context = new CodeGenContext("TestProgram");

        context.RegisterVariable("myVar", "int");

        Assert.AreEqual(1, context.VariableTypes.Count);
        Assert.AreEqual("int", context.VariableTypes["myVar"]);
    }

    [TestMethod]
    public void TestRegisterVariable_OverwritesExisting()
    {
        var context = new CodeGenContext("TestProgram");

        context.RegisterVariable("myVar", "int");
        context.RegisterVariable("myVar", "string");

        Assert.AreEqual(1, context.VariableTypes.Count);
        Assert.AreEqual("string", context.VariableTypes["myVar"]);
    }

    [TestMethod]
    public void TestGetVariableType_ReturnsRegisteredType()
    {
        var context = new CodeGenContext("TestProgram");
        context.RegisterVariable("myVar", "bool");

        Assert.AreEqual("bool", context.GetVariableType("myVar"));
    }

    [TestMethod]
    public void TestGetVariableType_UnknownVariable_ReturnsDynamic()
    {
        var context = new CodeGenContext("TestProgram");

        Assert.AreEqual("dynamic", context.GetVariableType("unknownVar"));
    }

    [TestMethod]
    public void TestEnterScope_PushesScope()
    {
        var context = new CodeGenContext("TestProgram");

        context.EnterScope("function1");

        Assert.AreEqual(1, context.ScopeStack.Count);
        Assert.AreEqual("function1", context.CurrentScope);
    }

    [TestMethod]
    public void TestEnterScope_MultipleScopes_Stacks()
    {
        var context = new CodeGenContext("TestProgram");

        context.EnterScope("function1");
        context.EnterScope("block1");
        context.EnterScope("block2");

        Assert.AreEqual(3, context.ScopeStack.Count);
        Assert.AreEqual("block2", context.CurrentScope);
    }

    [TestMethod]
    public void TestExitScope_PopsScope()
    {
        var context = new CodeGenContext("TestProgram");
        context.EnterScope("function1");
        context.EnterScope("block1");

        context.ExitScope();

        Assert.AreEqual(1, context.ScopeStack.Count);
        Assert.AreEqual("function1", context.CurrentScope);
    }

    [TestMethod]
    public void TestExitScope_EmptyStack_NoError()
    {
        var context = new CodeGenContext("TestProgram");

        context.ExitScope(); // Should not throw

        Assert.AreEqual(0, context.ScopeStack.Count);
    }

    [TestMethod]
    public void TestCurrentScope_EmptyStack_ReturnsGlobal()
    {
        var context = new CodeGenContext("TestProgram");

        Assert.AreEqual("global", context.CurrentScope);
    }

    [TestMethod]
    public void TestCurrentScope_AfterExitingAllScopes_ReturnsGlobal()
    {
        var context = new CodeGenContext("TestProgram");
        context.EnterScope("function1");
        context.ExitScope();

        Assert.AreEqual("global", context.CurrentScope);
    }

    [TestMethod]
    public void TestInLoop_CanBeSet()
    {
        var context = new CodeGenContext("TestProgram");

        context.InLoop = true;
        Assert.IsTrue(context.InLoop);

        context.InLoop = false;
        Assert.IsFalse(context.InLoop);
    }

    [TestMethod]
    public void TestInFunctionOrProcedure_CanBeSet()
    {
        var context = new CodeGenContext("TestProgram");

        context.InFunctionOrProcedure = true;
        Assert.IsTrue(context.InFunctionOrProcedure);

        context.InFunctionOrProcedure = false;
        Assert.IsFalse(context.InFunctionOrProcedure);
    }

    [TestMethod]
    public void TestTopLevelMembers_CanAddMembers()
    {
        var context = new CodeGenContext("TestProgram");

        var method = SyntaxFactory.MethodDeclaration(
            SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
            "TestMethod");

        context.TopLevelMembers.Add(method);

        Assert.AreEqual(1, context.TopLevelMembers.Count);
        Assert.AreEqual(method, context.TopLevelMembers[0]);
    }

    [TestMethod]
    public void TestMultipleVariables_IndependentTracking()
    {
        var context = new CodeGenContext("TestProgram");

        context.RegisterVariable("var1", "int");
        context.RegisterVariable("var2", "string");
        context.RegisterVariable("var3", "bool");

        Assert.AreEqual(3, context.VariableTypes.Count);
        Assert.AreEqual("int", context.GetVariableType("var1"));
        Assert.AreEqual("string", context.GetVariableType("var2"));
        Assert.AreEqual("bool", context.GetVariableType("var3"));
    }

    [TestMethod]
    public void TestScopeStack_NestedScopesAndTracking()
    {
        var context = new CodeGenContext("TestProgram");

        Assert.AreEqual("global", context.CurrentScope);

        context.EnterScope("main");
        Assert.AreEqual("main", context.CurrentScope);

        context.EnterScope("if_block");
        Assert.AreEqual("if_block", context.CurrentScope);

        context.EnterScope("for_loop");
        Assert.AreEqual("for_loop", context.CurrentScope);

        context.ExitScope();
        Assert.AreEqual("if_block", context.CurrentScope);

        context.ExitScope();
        Assert.AreEqual("main", context.CurrentScope);

        context.ExitScope();
        Assert.AreEqual("global", context.CurrentScope);
    }
}