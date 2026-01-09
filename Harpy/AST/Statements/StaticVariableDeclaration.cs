using Harpy.AST.Expressions;
using Harpy.CodeGen;
using Harpy.Lexer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Harpy.AST.Statements;

/// <summary>
///     Represents a static variable declaration, e.g. <c>static a</c> or <c>static b := 1</c>.
/// </summary>
public class StaticVariableDeclaration(HarbourSyntaxToken scope, HarbourSyntaxToken name, Expression? assignment)
    : VariableDeclaration(scope, name, assignment)
{
    public override SyntaxNode Walk(CodeGenContext context)
    {
        var typeName = TypeInference.InferType(Name.Text);
        var variableType = SyntaxFactory.ParseTypeName(typeName);

        var variableDeclarator = SyntaxFactory.VariableDeclarator(Name.Text);

        if (Assignment != null)
        {
            ExpressionSyntax initializer;
            if (Assignment is AssignmentExpression assignmentExpr)
            {
                initializer = (ExpressionSyntax)assignmentExpr.Right.Walk(context);
            }
            else
            {
                initializer = (ExpressionSyntax)Assignment.Walk(context);
            }

            variableDeclarator = variableDeclarator.WithInitializer(
                SyntaxFactory.EqualsValueClause(initializer));
        }

        var variableDeclaration = SyntaxFactory.VariableDeclaration(variableType)
            .AddVariables(variableDeclarator);

        var fieldDeclaration = SyntaxFactory.FieldDeclaration(variableDeclaration)
            .AddModifiers(
                SyntaxFactory.Token(SyntaxKind.PrivateKeyword),
                SyntaxFactory.Token(SyntaxKind.StaticKeyword));

        context.TopLevelMembers.Add(fieldDeclaration);
        
        return fieldDeclaration;
    }

    public override StatementSyntax WalkStatement(CodeGenContext context)
    {
        throw new NotImplementedException("StaticVariableDeclaration.WalkStatement not implemented for local scope yet.");
    }
}