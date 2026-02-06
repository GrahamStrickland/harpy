using Harpy.AST.Expressions;
using Harpy.Lexer;

namespace Harpy.AST.Statements;

/// <summary>
///     Represents a local variable declaration, e.g. <c>local a</c> or <c>local b := 1</c>.
/// </summary>
public class LocalVariableDeclaration(HarbourSyntaxToken scope, HarbourSyntaxToken name, Expression? assignment)
    : VariableDeclaration(scope, name, assignment);
