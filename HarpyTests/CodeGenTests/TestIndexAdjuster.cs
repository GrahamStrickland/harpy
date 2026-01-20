using Harpy;
using Harpy.CodeGen;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HarpyTests.CodeGenTests;

[TestClass]
public class TestIndexAdjuster
{
    private CodeGenContext _context = null!;

    [TestInitialize]
    public void Setup()
    {
        _context = new CodeGenContext("TestProgram");
    }

    [TestMethod]
    public void TestAdjustIndex_IntegerLiteral_SubtractsOne()
    {
        var indexExpr = SyntaxFactory.LiteralExpression(
            SyntaxKind.NumericLiteralExpression,
            SyntaxFactory.Literal(5));

        var result = IndexAdjuster.AdjustIndex(indexExpr, _context);

        Assert.IsInstanceOfType<LiteralExpressionSyntax>(result);
        var literal = (LiteralExpressionSyntax)result;
        Assert.AreEqual(4, literal.Token.Value);
    }

    [TestMethod]
    public void TestAdjustIndex_IntegerLiteralOne_ReturnsZero()
    {
        var indexExpr = SyntaxFactory.LiteralExpression(
            SyntaxKind.NumericLiteralExpression,
            SyntaxFactory.Literal(1));

        var result = IndexAdjuster.AdjustIndex(indexExpr, _context);

        Assert.IsInstanceOfType<LiteralExpressionSyntax>(result);
        var literal = (LiteralExpressionSyntax)result;
        Assert.AreEqual(0, literal.Token.Value);
    }

    [TestMethod]
    public void TestAdjustIndex_IntegerLiteralZero_ReturnsNegativeOne()
    {
        var indexExpr = SyntaxFactory.LiteralExpression(
            SyntaxKind.NumericLiteralExpression,
            SyntaxFactory.Literal(0));

        var result = IndexAdjuster.AdjustIndex(indexExpr, _context);

        Assert.IsInstanceOfType<LiteralExpressionSyntax>(result);
        var literal = (LiteralExpressionSyntax)result;
        Assert.AreEqual(-1, literal.Token.Value);
    }

    [TestMethod]
    public void TestAdjustIndex_LargeIntegerLiteral_SubtractsOne()
    {
        var indexExpr = SyntaxFactory.LiteralExpression(
            SyntaxKind.NumericLiteralExpression,
            SyntaxFactory.Literal(1000));

        var result = IndexAdjuster.AdjustIndex(indexExpr, _context);

        Assert.IsInstanceOfType<LiteralExpressionSyntax>(result);
        var literal = (LiteralExpressionSyntax)result;
        Assert.AreEqual(999, literal.Token.Value);
    }

    [TestMethod]
    public void TestAdjustIndex_DoubleLiteral_ThrowsException()
    {
        var indexExpr = SyntaxFactory.LiteralExpression(
            SyntaxKind.NumericLiteralExpression,
            SyntaxFactory.Literal(5.5));

        Assert.ThrowsException<InvalidSyntaxException>(() =>
            IndexAdjuster.AdjustIndex(indexExpr, _context));
    }

    [TestMethod]
    public void TestAdjustIndex_IdentifierExpression_CreatesBinarySubtraction()
    {
        var indexExpr = SyntaxFactory.IdentifierName("myIndex");

        var result = IndexAdjuster.AdjustIndex(indexExpr, _context);

        Assert.IsInstanceOfType<ParenthesizedExpressionSyntax>(result);
        var paren = (ParenthesizedExpressionSyntax)result;
        Assert.IsInstanceOfType<BinaryExpressionSyntax>(paren.Expression);

        var binary = (BinaryExpressionSyntax)paren.Expression;
        Assert.AreEqual(SyntaxKind.SubtractExpression, binary.Kind());
        Assert.AreEqual("myIndex", binary.Left.ToString());
        Assert.AreEqual("1", binary.Right.ToString());
    }

    [TestMethod]
    public void TestAdjustIndex_ComplexExpression_CreatesParenthesizedSubtraction()
    {
        // Create expression: (a + b)
        var complexExpr = SyntaxFactory.ParenthesizedExpression(
            SyntaxFactory.BinaryExpression(
                SyntaxKind.AddExpression,
                SyntaxFactory.IdentifierName("a"),
                SyntaxFactory.IdentifierName("b")));

        var result = IndexAdjuster.AdjustIndex(complexExpr, _context);

        Assert.IsInstanceOfType<ParenthesizedExpressionSyntax>(result);
        var paren = (ParenthesizedExpressionSyntax)result;
        Assert.IsInstanceOfType<BinaryExpressionSyntax>(paren.Expression);

        var binary = (BinaryExpressionSyntax)paren.Expression;
        Assert.AreEqual(SyntaxKind.SubtractExpression, binary.Kind());
        Assert.AreEqual("(a+b)", binary.Left.ToString());
        Assert.AreEqual("1", binary.Right.ToString());
    }

    [TestMethod]
    public void TestAdjustIndex_MemberAccessExpression_CreatesParenthesizedSubtraction()
    {
        var memberExpr = SyntaxFactory.MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            SyntaxFactory.IdentifierName("obj"),
            SyntaxFactory.IdentifierName("index"));

        var result = IndexAdjuster.AdjustIndex(memberExpr, _context);

        Assert.IsInstanceOfType<ParenthesizedExpressionSyntax>(result);
        var paren = (ParenthesizedExpressionSyntax)result;
        Assert.IsInstanceOfType<BinaryExpressionSyntax>(paren.Expression);

        var binary = (BinaryExpressionSyntax)paren.Expression;
        Assert.AreEqual(SyntaxKind.SubtractExpression, binary.Kind());
        Assert.AreEqual("obj.index", binary.Left.ToString());
        Assert.AreEqual("1", binary.Right.ToString());
    }

    [TestMethod]
    public void TestAdjustIndex_InvocationExpression_CreatesParenthesizedSubtraction()
    {
        var invocationExpr = SyntaxFactory.InvocationExpression(
            SyntaxFactory.IdentifierName("GetIndex"));

        var result = IndexAdjuster.AdjustIndex(invocationExpr, _context);

        Assert.IsInstanceOfType<ParenthesizedExpressionSyntax>(result);
        var paren = (ParenthesizedExpressionSyntax)result;
        Assert.IsInstanceOfType<BinaryExpressionSyntax>(paren.Expression);

        var binary = (BinaryExpressionSyntax)paren.Expression;
        Assert.AreEqual(SyntaxKind.SubtractExpression, binary.Kind());
        Assert.AreEqual("GetIndex()", binary.Left.ToString());
        Assert.AreEqual("1", binary.Right.ToString());
    }

    [TestMethod]
    public void TestNeedsAdjustment_AlwaysReturnsTrue()
    {
        // This test verifies the current behavior - always returns true
        // In the future, if heuristics are added, this test may need updating
        Assert.IsTrue(IndexAdjuster.NeedsAdjustment(null!));
    }

    [TestMethod]
    public void TestAdjustIndex_VerifyOutputFormat_LiteralInteger()
    {
        var indexExpr = SyntaxFactory.LiteralExpression(
            SyntaxKind.NumericLiteralExpression,
            SyntaxFactory.Literal(10));

        var result = IndexAdjuster.AdjustIndex(indexExpr, _context);

        // Verify the result is exactly what we expect
        Assert.AreEqual("9", result.ToString());
    }

    [TestMethod]
    public void TestAdjustIndex_VerifyOutputFormat_Expression()
    {
        var indexExpr = SyntaxFactory.IdentifierName("i");

        var result = IndexAdjuster.AdjustIndex(indexExpr, _context);

        // Verify the result includes parentheses and subtraction
        Assert.AreEqual("(i-1)", result.ToString());
    }

    [TestMethod]
    public void TestAdjustIndex_MultipleAdjustments_Independent()
    {
        var expr1 = SyntaxFactory.LiteralExpression(
            SyntaxKind.NumericLiteralExpression,
            SyntaxFactory.Literal(5));
        var expr2 = SyntaxFactory.LiteralExpression(
            SyntaxKind.NumericLiteralExpression,
            SyntaxFactory.Literal(10));

        var result1 = IndexAdjuster.AdjustIndex(expr1, _context);
        var result2 = IndexAdjuster.AdjustIndex(expr2, _context);

        var literal1 = (LiteralExpressionSyntax)result1;
        var literal2 = (LiteralExpressionSyntax)result2;

        Assert.AreEqual(4, literal1.Token.Value);
        Assert.AreEqual(9, literal2.Token.Value);
    }

    [TestMethod]
    public void TestAdjustIndex_NegativeInteger_StillSubtractsOne()
    {
        var indexExpr = SyntaxFactory.LiteralExpression(
            SyntaxKind.NumericLiteralExpression,
            SyntaxFactory.Literal(-5));

        var result = IndexAdjuster.AdjustIndex(indexExpr, _context);

        Assert.IsInstanceOfType<LiteralExpressionSyntax>(result);
        var literal = (LiteralExpressionSyntax)result;
        Assert.AreEqual(-6, literal.Token.Value);
    }
}