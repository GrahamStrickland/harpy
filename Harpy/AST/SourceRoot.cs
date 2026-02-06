using Harpy.AST.Statements;
using Harpy.CodeGen;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Harpy.AST;

/// <summary>
///     The root AST node of a parsed Harbour source file.
/// </summary>
public class SourceRoot(string name, List<HarbourAstNode> children) : HarbourAstNode(children)
{
    public override string PrettyPrint(int indent = 0)
    {
        return $"SourceRoot(name='{name}')" + base.PrettyPrint(indent);
    }

    public override SyntaxNode Walk(CodeGenContext context)
    {
        var classMembers = new List<MemberDeclarationSyntax>();
        var topLevelStatements = new List<StatementSyntax>();

        foreach (var child in Children)
        {
            switch (child)
            {
                case FunctionStatement or ProcedureStatement or LocalVariableDeclaration or StaticVariableDeclaration:
                    {
                        // Functions, procedures, and file-level statics become members of the partial class
                        classMembers.Add((MemberDeclarationSyntax)child.Walk(context));
                        break;
                    }
                case Statement statement:
                    {
                        var syntax = statement.WalkStatement(context);
                        topLevelStatements.Add(syntax);

                        break;
                    }
            }
        }

        if (topLevelStatements.Count > 0)
        {
            var methodName = $"Initialize_{name.Replace(".", "_")}";
            var initMethod = SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                    methodName)
                .AddModifiers(
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                .WithBody(SyntaxFactory.Block(topLevelStatements));

            classMembers.Add(initMethod);
        }

        var partialClass = SyntaxFactory.ClassDeclaration(context.PartialClassName)
            .AddModifiers(
                SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                SyntaxFactory.Token(SyntaxKind.PartialKeyword))
            .AddMembers([.. classMembers]);

        return SyntaxFactory.CompilationUnit()
            .AddMembers(partialClass)
            .NormalizeWhitespace();
    }
}
