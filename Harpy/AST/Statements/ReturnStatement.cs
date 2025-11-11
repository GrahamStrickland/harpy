using Harpy.AST.Expressions;

namespace Harpy.AST.Statements;

public class ReturnStatement(Expression? returnValue) : Statement
{
    public override IHarbourAstNode? Parent { get; set; }

    public override string PrettyPrint()
    {
        return "return" + (returnValue == null ? "" : $" {returnValue.PrettyPrint()}");
    }

    public override void Walk()
    {
        throw new NotImplementedException();
    }

    public Expression? ReturnValue()
    {
        return returnValue;
    }
}