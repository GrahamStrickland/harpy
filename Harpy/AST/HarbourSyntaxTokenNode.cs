using Harpy.CodeGen;
using Harpy.Lexer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Harpy.AST;

public class HarbourSyntaxTokenNode(HarbourSyntaxToken token, List<HarbourAstNode> children) : HarbourAstNode(children)
{
    public readonly HarbourSyntaxToken Token = token;

    public override string PrettyPrint(int indent = 0)
    {
        return Token.PrettyPrint(indent);
    }

    public override SyntaxNode Walk(CodeGenContext context)
    {
        // Token nodes are typically handled by their parent nodes
        return Token.Kind == HarbourSyntaxKind.NAME
            ? SyntaxFactory.IdentifierName(Token.Text)
            : throw new NotImplementedException($"Walk not implemented for token type {Token.Kind}");
    }
}