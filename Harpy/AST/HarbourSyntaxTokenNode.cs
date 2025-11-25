using Harpy.Lexer;

namespace Harpy.AST;

public class HarbourSyntaxTokenNode(HarbourSyntaxToken token, List<HarbourAstNode> children) : HarbourAstNode(children)
{
    public override string PrettyPrint()
    {
        return token.Text;
    }
}