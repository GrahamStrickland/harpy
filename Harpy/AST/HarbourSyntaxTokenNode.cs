using Harpy.Lexer;

namespace Harpy.AST;

public class HarbourSyntaxTokenNode(HarbourSyntaxToken token, List<HarbourAstNode> children) : HarbourAstNode(children)
{
    public override string PrettyPrint(int indent = 0)
    {
        return token.PrettyPrint(indent);
    }
}
