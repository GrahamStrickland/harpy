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

    public override string PrettyPrint(int indent = 0)
    {
        var result = NodeLine(indent) + "ReturnStatement(";
        
        if (_returnValue != null)
        {
            result += "\n" + BlankLine(indent + 1) + "returnValue\n" + ChildNodeLine(indent + 1) + 
                      _returnValue.PrettyPrint(indent + 2) + "\n" + BlankLine(indent);
        }
        
        return result + ")";
    }

    public Expression? ReturnValue()
    {
        return _returnValue;
    }
}
