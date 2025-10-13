#if DEBUG
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("HarpyTests")]
#endif

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
    }
}