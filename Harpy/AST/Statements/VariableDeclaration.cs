using Harpy.AST.Expressions;
using Harpy.CodeGen;
using Harpy.Lexer;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Harpy.AST.Statements;

/// <summary>
///     Represents a local or static variable declaration, e.g. <c>local a</c> or <c>static b := 1</c>.
/// </summary>
public abstract class VariableDeclaration : Statement
{
    public Expression? Assignment { get; }
    public HarbourSyntaxToken Name { get; }
    public HarbourSyntaxToken Scope { get; }

    /// <summary>
    ///     Represents a local or static variable declaration, e.g. <c>local a</c> or <c>static b := 1</c>.
    /// </summary>
    /// <param name="scope">Token representing variable scope, e.g., <c>local</c>, <c>static</c>, etc.</param>
    /// <param name="name">Name of variable.</param>
    /// <param name="assignment">Optional assignment value.</param>
    protected VariableDeclaration(HarbourSyntaxToken scope, HarbourSyntaxToken name, Expression? assignment) : base([])
    {
        Scope = scope;
        Name = name;
        Assignment = assignment;

        var nameNode = new HarbourSyntaxTokenNode(Name, [])
        {
            Parent = this
        };
        Children.Add(nameNode);

        if (Assignment == null) return;
        Assignment.Parent = this;
        Children.Add(Assignment);
    }

    public override string PrettyPrint(int indent = 0)
    {
        return NodeLine(indent) + "VariableDeclaration(\n" + BlankLine(indent + 1) + "scope\n" +
               ChildNodeLine(indent + 1) + Scope.PrettyPrint(indent + 2) + "\n" +
               BlankLine(indent + 1) + "name\n" + ChildNodeLine(indent + 1) +
               Name.PrettyPrint(indent + 2) + "\n" + (Assignment == null
                   ? ""
                   : BlankLine(indent + 1) + "assignment\n" + ChildNodeLine(indent + 1) +
                     Assignment.PrettyPrint(indent + 2))
               + "\n" + BlankLine(indent) + ")";
    }

    public override StatementSyntax WalkStatement(CodeGenContext context)
    {
        // TODO: Implement variable declaration code generation
        throw new NotImplementedException("VariableDeclaration.WalkStatement not yet implemented");
    }
}
