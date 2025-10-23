using Harpy.Lexer;

namespace Harpy.AST.Expressions;

/// <summary>
///     A prefix unary arithmetic expression like <c>!a</c> or <c>-b</c>.
/// </summary>
public class PrefixExpression(HarbourSyntaxToken @operator, Expression right) : Expression(false)
{
    public override IHarbourAstNode? Parent { get; set; }

    public override string PrettyPrint()
    {
        return
            $"({HarbourSyntaxToken.SimpleOperator(@operator.Text) ?? HarbourSyntaxToken.CompoundOperator(@operator.Text)}{right.PrettyPrint()})";
    }

    public override void Walk()
    {
        Console.WriteLine(PrettyPrint());
    }
}