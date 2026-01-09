using Harpy.CodeGen;
using Harpy.Lexer;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Harpy.AST.Statements;

/// <summary>
///     Represents a procedure declaration, e.g. <c>procedure a(b, c) ; return</c>.
/// </summary>
public class ProcedureStatement : Statement
{
    private readonly List<Statement> _body;
    private readonly bool _isStatic;
    private readonly HarbourSyntaxToken _name;
    private readonly List<HarbourSyntaxToken> _parameters;

    /// <summary>
    ///     Represents a procedure declaration, e.g. <c>procedure a(b, c) ; return</c>.
    /// </summary>
    public ProcedureStatement(HarbourSyntaxToken name,
        List<HarbourSyntaxToken> parameters,
        List<Statement> body,
        bool isStatic) : base([])
    {
        _name = name;
        _parameters = parameters;
        _body = body;
        _isStatic = isStatic;

        var nameNode = new HarbourSyntaxTokenNode(name, [])
        {
            Parent = this
        };
        Children.Add(nameNode);
        foreach (var parameterChild in parameters.Select(parameter => new HarbourSyntaxTokenNode(parameter, [])
                 {
                     Parent = this
                 }))
            Children.Add(parameterChild);

        foreach (var bodyChild in body)
        {
            bodyChild.Parent = this;
            Children.Add(bodyChild);
        }
    }

    public override string PrettyPrint(int indent = 0)
    {
        var result = NodeLine(indent) + "ProcedureStatement(" + (_isStatic ? "static" : "") + "\n";
        result += BlankLine(indent + 1) + "name\n" + ChildNodeLine(indent + 1) + 
                  Children[0].PrettyPrint(indent + 2) + "\n";
        
        if (_parameters.Count > 0)
        {
            result += BlankLine(indent + 1) + "parameters\n";
            for (var i = 0; i < _parameters.Count; i++)
                result += ChildNodeLine(indent + 1) + Children[i + 1].PrettyPrint(indent + 2) + "\n";
        }

        if (_body.Count > 0)
        {
            result += BlankLine(indent + 1) + "body\n";
            foreach (var stmt in _body)
                result += ChildNodeLine(indent + 1) + stmt.PrettyPrint(indent + 2) + "\n";
        }

        result += BlankLine(indent) + ")";
        return result;
    }

    public override StatementSyntax WalkStatement(CodeGenContext context)
    {
        // TODO: Implement procedure statement code generation
        throw new NotImplementedException("ProcedureStatement.WalkStatement not yet implemented");
    }
}