using Harpy.AST.Expressions;
using Harpy.Lexer;

namespace Harpy.AST.Statements;

/// <summary>
///     Represents a local or static variable declaration, e.g. <c>local a</c> or <c>static b := 1</c>.
/// </summary>
public abstract class VariableDeclaration : Statement
{
    private readonly Expression? _assignment;
    private readonly HarbourSyntaxToken _name;
    private readonly string _scope;

    /// <summary>
    ///     Represents a local or static variable declaration, e.g. <c>local a</c> or <c>static b := 1</c>.
    /// </summary>
    /// <param name="scope">String representing variable scope, e.g., <c>local</c>, <c>static</c>, etc.</param>
    /// <param name="name">Name of variable.</param>
    /// <param name="assignment">Optional assignment value.</param>
    protected VariableDeclaration(string scope, HarbourSyntaxToken name, Expression? assignment) : base([])
    {
        _scope = scope;
        _name = name;
        _assignment = assignment;

        var nameNode = new HarbourSyntaxTokenNode(_name, []);
        {
            Parent = this;
        }
        Children.Add(nameNode);

        if (_assignment == null) return;
        _assignment.Parent = this;
        Children.Add(_assignment);
    }

    public override string PrettyPrint()
    {
        return _assignment == null ? $"{_scope} {_name.Text}" : $"{_scope} {_assignment.PrettyPrint()}";
    }
}