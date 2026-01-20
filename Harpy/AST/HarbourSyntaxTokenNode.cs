using Harpy.CodeGen;
using Harpy.Lexer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Harpy.AST;

public class HarbourSyntaxTokenNode(HarbourSyntaxToken token, List<HarbourAstNode> children) : HarbourAstNode(children)
{
    public HarbourSyntaxToken token = token;

    public override string PrettyPrint(int indent = 0)
    {
        return token.PrettyPrint(indent);
    }

    public override SyntaxNode Walk(CodeGenContext context)
    {
        // Token nodes are typically handled by their parent nodes
        if (token.Kind == HarbourSyntaxKind.NAME) return SyntaxFactory.IdentifierName(token.Text);

        throw new NotImplementedException($"Walk not implemented for token type {token.Kind}");
    }
}