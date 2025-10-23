using Harpy.Lexer;

namespace Harpy.AST.Expressions;

/// <summary>
///     A postfix unary arithmetic expression like <c>a!</c>.
/// </summary>
public class PostfixExpression(Expression left, HarbourSyntaxToken @operator) : Expression(false)
{
    public override IHarbourAstNode? Parent { get; set; }

    public override string PrettyPrint()
    {
        return
            $"({left.PrettyPrint()}{HarbourSyntaxToken.SimpleOperator(@operator.Text) ?? HarbourSyntaxToken.CompoundOperator(@operator.Text)})";
    }

    public override void Walk()
    {
        Console.WriteLine(PrettyPrint());
    }
}