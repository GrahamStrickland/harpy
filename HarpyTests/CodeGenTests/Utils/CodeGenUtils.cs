using Harpy.Lexer;
using Harpy.Parser;
using Harpy.CodeGen;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace HarpyTests.CodeGenTests.Utils;

internal static class CodeGenUtils
{
    public static void AssertCodeGenEqualsExpected(string source, string expected)
    {
        var lexer = new Lexer(source);
        var parser = new Parser(lexer);
        var root = parser.Parse("TestProgram");

        var context = new CodeGenContext("TestProgram");
        var syntaxNode = root.Walk(context);

        var actual = syntaxNode.NormalizeWhitespace().ToFullString();
        var expectedNormalized = SyntaxFactory.ParseCompilationUnit(expected)
            .NormalizeWhitespace().ToFullString();

        Assert.AreEqual(expectedNormalized, actual);
    }
}
