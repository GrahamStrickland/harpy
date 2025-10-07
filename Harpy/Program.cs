#if DEBUG
using System.Runtime.CompilerServices;
[assembly:InternalsVisibleTo("HarpyTests")]
#endif

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
    }
}
