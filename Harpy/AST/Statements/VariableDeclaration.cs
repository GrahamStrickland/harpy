using Harpy.AST.Expressions;
using Harpy.CodeGen;
using Harpy.Lexer;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Harpy.AST.Statements;

/// <summary>
///     Represents a local or static variable declaration, e.g. <c>local a</c> or <c>static b := 1</c>.
/// </summary>
public abstract class VariableDeclaration : Statement
{
    /// <summary>
    ///     Represents a local or static variable declaration, e.g. <c>local a</c> or <c>static b := 1</c>.
    /// </summary>
    /// <param name="scope">Token representing variable scope, e.g., <c>local</c>, <c>static</c>, etc.</param>
    /// <param name="name">Name of variable.</param>
    /// <param name="assignment">Optional assignment value.</param>
    protected VariableDeclaration(HarbourSyntaxToken scope, HarbourSyntaxToken name, Expression? assignment) : base([])
    {
        Scope = scope;
        Name = name;
        Assignment = assignment;

        var nameNode = new HarbourSyntaxTokenNode(Name, [])
        {
            Parent = this
        };
        Children.Add(nameNode);

        if (Assignment == null) return;

        Assignment.Parent = this;
        Children.Add(Assignment);
    }

    protected Expression? Assignment { get; }
    protected HarbourSyntaxToken Name { get; }
    private HarbourSyntaxToken Scope { get; }

    public override string PrettyPrint(int indent = 0)
    {
        return NodeLine(indent) + "VariableDeclaration(\n" + BlankLine(indent + 1) + "scope\n" +
               ChildNodeLine(indent + 1) + Scope.PrettyPrint(indent + 2) + "\n" +
               BlankLine(indent + 1) + "name\n" + ChildNodeLine(indent + 1) +
               Name.PrettyPrint(indent + 2) + "\n" + (Assignment == null
                   ? ""
                   : BlankLine(indent + 1) + "assignment\n" + ChildNodeLine(indent + 1) +
                     Assignment.PrettyPrint(indent + 2))
               + "\n" + BlankLine(indent) + ")";
    }

    public override LocalDeclarationStatementSyntax Walk(CodeGenContext context)
    {
        return SyntaxFactory.LocalDeclarationStatement(BuildDeclaration(context));
    }

    /// <summary>
    ///     Emit this variable declaration as a class-level field, used when the declaration appears at file scope
    ///     and gets promoted to a member of the generated partial class.
    /// </summary>
    public FieldDeclarationSyntax ToFieldDeclaration(CodeGenContext context)
    {
        var fieldDeclaration = SyntaxFactory.FieldDeclaration(BuildDeclaration(context))
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));

        if (Scope.Keyword() == "static")
            fieldDeclaration = fieldDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.StaticKeyword));

        context.TopLevelMembers.Add(fieldDeclaration);

        return fieldDeclaration;
    }

    private VariableDeclarationSyntax BuildDeclaration(CodeGenContext context)
    {
        var variableType = SyntaxFactory.ParseTypeName(TypeInference.InferType(Name.Text));
        var variableDeclarator = SyntaxFactory.VariableDeclarator(Name.Text);

        if (Assignment != null)
        {
            var initializer = Assignment is AssignmentExpression assignmentExpr
                ? (ExpressionSyntax)assignmentExpr.Right.Walk(context)
                : (ExpressionSyntax)Assignment.Walk(context);

            variableDeclarator = variableDeclarator.WithInitializer(
                SyntaxFactory.EqualsValueClause(initializer));
        }

        return SyntaxFactory.VariableDeclaration(variableType).AddVariables(variableDeclarator);
    }
}
