#if DEBUG
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("HarpyTests")]
#endif

namespace Harpy;

internal static class Program
{
    private static void Main(string[] args)
    {
        string sourceFile;

        if (args is ["--src", _, ..])
        {
            sourceFile = args[1];
        }
        else
        {
            Console.Write("""

                          usage: harpy --src <source_file>

                          Harbour to Python transpiler

                          options:
                            --src       source file to transpile, ending in either '.prg' or '.ch

                          """);
            return;
        }

        var extension = Path.GetExtension(sourceFile);
        if (extension != ".prg" && extension != ".ch")
            throw new InvalidDataException($"Invalid source path '{sourceFile}' with extension '{extension}'");

        var source = File.ReadAllText(sourceFile);
        var lexer = new Lexer.Lexer(source);
        var parser = new Parser.Parser(lexer);

        Console.WriteLine(parser.Parse().PrettyPrint());
    }
}