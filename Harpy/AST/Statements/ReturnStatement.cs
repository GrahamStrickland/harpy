using Harpy.AST.Expressions;

namespace Harpy.AST.Statements;

public class ReturnStatement : Statement
{
    private readonly Expression? _returnValue;

    public ReturnStatement(Expression? returnValue) : base([])
    {
        _returnValue = returnValue;

        if (_returnValue == null) return;
        _returnValue.Parent = this;
        Children.Add(_returnValue);
    }

    public override string PrettyPrint()
    {
        return "return" + (_returnValue == null ? "" : $" {_returnValue.PrettyPrint()}");
    }

    public Expression? ReturnValue()
    {
        return _returnValue;
    }
}