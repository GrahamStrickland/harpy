using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Harpy.CodeGen;

/// <summary>
///     Context object that tracks state during code generation.
/// </summary>
/// <remarks>
///     Initializes a new instance of the CodeGenContext class.
/// </remarks>
/// <param name="partialClassName">The name of the partial class to use for top-level statements.</param>
public class CodeGenContext(string partialClassName)
{
    /// <summary>
    ///     The name of the partial class to which top-level statements will be added.
    /// </summary>
    public string PartialClassName { get; set; } = partialClassName;

    /// <summary>
    ///     Dictionary mapping variable names to their inferred C# types.
    /// </summary>
    public Dictionary<string, string> VariableTypes { get; } = [];

    /// <summary>
    ///     Stack tracking current scope depth (for nested blocks).
    /// </summary>
    public Stack<string> ScopeStack { get; } = new();

    /// <summary>
    ///     Whether we're currently inside a loop (affects break/continue generation).
    /// </summary>
    public bool InLoop { get; set; }

    /// <summary>
    ///     Whether we're currently inside a function or procedure definition.
    /// </summary>
    public bool InFunctionOrProcedure { get; set; }

    /// <summary>
    ///     List of generated top-level members (functions, procedures, etc.)
    /// </summary>
    public List<MemberDeclarationSyntax> TopLevelMembers { get; } = [];

    /// <summary>
    ///     Gets the current scope name, or "global" if at top level.
    /// </summary>
    public string CurrentScope => ScopeStack.Count > 0 ? ScopeStack.Peek() : "global";

    /// <summary>
    ///     Enters a new scope with the given name.
    /// </summary>
    public void EnterScope(string scopeName)
    {
        ScopeStack.Push(scopeName);
    }

    /// <summary>
    ///     Exits the current scope.
    /// </summary>
    public void ExitScope()
    {
        if (ScopeStack.Count > 0)
            ScopeStack.Pop();
    }

    /// <summary>
    ///     Registers a variable with its inferred type.
    /// </summary>
    public void RegisterVariable(string name, string type)
    {
        VariableTypes[name] = type;
    }

    /// <summary>
    ///     Gets the type of variable, or "dynamic" if not found.
    /// </summary>
    public string GetVariableType(string name)
    {
        return VariableTypes.GetValueOrDefault(name, "dynamic");
    }
}