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

        // Check for single character variable names (typically integers)
        if (variableName.Length == 1)
            return "int";

        // Check for keywords that suggest integer types (case-insensitive)
        string lowerName = variableName.ToLower();
        string[] intKeywords = ["column", "col", "error", "handle", "index", "length", "lines", "row"];
        
        foreach (string keyword in intKeywords)
        {
            if (lowerName.Contains(keyword))
                return "int";
        }

        // Extract the prefix (first character + optional second character for two-char prefixes)
        char prefix = char.ToLower(variableName[0]);

        // Handle static variables: if first letter is 's', use the second letter for type inference
        if (prefix == 's' && variableName.Length > 1)
        {
            prefix = char.ToLower(variableName[1]);
        }

        return prefix switch
        {
            'l' => "bool",              // lVariable or slVariable => boolean
            'n' => "double",            // nVariable or snVariable => numeric (using double for flexibility)
            'c' => "string",            // cVariable or scVariable => string
            'd' => "DateTime",          // dVariable or sdVariable => date
            'h' => "Dictionary<string, dynamic>",  // hVariable or shVariable => hash map
            'a' => "List<dynamic>",     // aVariable or saVariable => array
            'o' => "dynamic",           // oVariable or soVariable => object (could be enhanced with more inference)
            'p' => "IntPtr",            // pVariable or spVariable => pointer
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

        // Check for static variable prefix 's' followed by type prefix
        if (prefix == 's' && variableName.Length >= 3)
        {
            char typePrefix = char.ToLower(variableName[1]);
            char thirdChar = variableName[2];
            // Check if second char is a known prefix and third char is uppercase
            return "lncdhaop".Contains(typePrefix) && char.IsUpper(thirdChar);
        }

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
