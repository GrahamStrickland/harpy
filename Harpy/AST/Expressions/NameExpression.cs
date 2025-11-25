namespace Harpy.AST.Expressions;

/// <summary>
///     A simple variable name expression like <c>abc</c>.
/// </summary>
public class NameExpression : Expression
{
    private readonly string _name;

    /// <summary>
    ///     A simple variable name expression like <c>abc</c>.
    /// </summary>
    public NameExpression(string name) : base(false, [])
    {
        _name = name;
    }

    public override string PrettyPrint()
    {
        return _name;
    }
}