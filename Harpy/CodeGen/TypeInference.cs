namespace Harpy.CodeGen;

/// <summary>
///     Helper class for inferring C# types from Harbour variable names using Hungarian notation.
/// </summary>
public static class TypeInference
{
    /// <summary>
    ///     Infers a C# type from a Harbour variable name based on Hungarian notation.
    /// </summary>
    /// <param name="variableName">The Harbour variable name</param>
    /// <returns>A C# type name as a string</returns>
    public static string InferType(string variableName)
    {
        if (string.IsNullOrEmpty(variableName))
            return "dynamic";

        // Extract the prefix (first character + optional second character for two-char prefixes)
        char prefix = char.ToLower(variableName[0]);

        return prefix switch
        {
            'l' => "bool",              // lVariable => boolean
            'n' => "double",            // nVariable => numeric (using double for flexibility)
            'c' => "string",            // cVariable => string
            'd' => "DateTime",          // dVariable => date
            'h' => "Dictionary<string, dynamic>",  // hVariable => hash map
            'a' => "List<dynamic>",     // aVariable => array
            'o' => "dynamic",           // oVariable => object (could be enhanced with more inference)
            'p' => "IntPtr",            // pVariable => pointer
            _ => "dynamic"              // default to dynamic
        };
    }

    /// <summary>
    ///     Checks if a variable name follows Hungarian notation.
    /// </summary>
    public static bool IsHungarianNotation(string variableName)
    {
        if (string.IsNullOrEmpty(variableName) || variableName.Length < 2)
            return false;

        char prefix = char.ToLower(variableName[0]);
        char secondChar = variableName[1];

        // Check if first char is a known prefix and second char is uppercase (indicating CamelCase)
        return "lncdhaop".Contains(prefix) && char.IsUpper(secondChar);
    }

    /// <summary>
    ///     Gets the default value for a given type.
    /// </summary>
    public static string GetDefaultValue(string typeName)
    {
        return typeName switch
        {
            "bool" => "false",
            "double" => "0.0",
            "int" => "0",
            "string" => "\"\"",
            "DateTime" => "DateTime.MinValue",
            "List<dynamic>" => "new List<dynamic>()",
            "Dictionary<string, dynamic>" => "new Dictionary<string, dynamic>()",
            "IntPtr" => "IntPtr.Zero",
            _ => "null"
        };
    }
}
