using Harpy.AST.Expressions;
using Harpy.Lexer;

namespace Harpy.AST.Statements;

/// <summary>
///     Represents a static variable declaration, e.g. <c>static a</c> or <c>static b := 1</c>.
/// </summary>
public class StaticVariableDeclaration(HarbourSyntaxToken scope, HarbourSyntaxToken name, Expression? assignment)
    : VariableDeclaration(scope, name, assignment);
