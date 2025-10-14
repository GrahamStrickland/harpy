using Harpy.Lexer;
using HarpyTests.LexerTests.Utils;

namespace HarpyTests.LexerTests;

[TestClass]
public sealed class TestSourceReader
{
    private static SourceReader? _reader;

    [TestMethod]
    public void TestGetEnumerator()
    {
        var obs = GetObservedTokens("a + b(c)");
        var expected = new List<HarbourSyntaxToken>
        {
            new(HarbourSyntaxKind.NAME, "a", 1, 1),
            new(HarbourSyntaxKind.PLUS, "+", 1, 3),
            new(HarbourSyntaxKind.NAME, "b", 1, 5),
            new(HarbourSyntaxKind.LEFT_PAREN, "(", 1, 6),
            new(HarbourSyntaxKind.NAME, "c", 1, 7),
            new(HarbourSyntaxKind.RIGHT_PAREN, ")", 1, 8),
            new(HarbourSyntaxKind.EOF, "\0", 1, 8)
        };

        SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);
    }

    [TestMethod]
    public void TestMoveNext()
    {
        var lexer = new Lexer("a + b");
        _reader = new SourceReader(lexer);

        for (var i = 0; i < 4; i++) // Include EOF token at end for leading trivia.
            Assert.IsTrue(_reader.MoveNext());
        
        Assert.IsFalse(_reader.MoveNext());
    }

    private static List<HarbourSyntaxToken> GetObservedTokens(string source)
    {
        var lexer = new Lexer(source);
        _reader = new SourceReader(lexer);

        var obs = new List<HarbourSyntaxToken>();

        foreach (var token in _reader)
        {
            if (token.Kind == HarbourSyntaxKind.EOF)
            {
                obs.Add(token);
                break;
            }

            obs.Add(token);
        }

        return obs;
    }
}