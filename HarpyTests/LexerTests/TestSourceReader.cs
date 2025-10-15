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

    [TestMethod]
    public void TestMatch()
    {
        var lexer = new Lexer("a + b");
        _reader = new SourceReader(lexer);
        List<HarbourSyntaxKind> tokenKinds =
            [HarbourSyntaxKind.NAME, HarbourSyntaxKind.PLUS, HarbourSyntaxKind.NAME, HarbourSyntaxKind.EOF];
        var i = 0;

        foreach (var _ in _reader)
        {
            i++;
            if (i < tokenKinds.Count)
                Assert.IsTrue(_reader.Match(tokenKinds[i]));
        }
    }

    [TestMethod]
    public void TestConsume()
    {
        var lexer = new Lexer("a + b");
        _reader = new SourceReader(lexer);
        List<HarbourSyntaxKind> tokenKinds =
            [HarbourSyntaxKind.NAME, HarbourSyntaxKind.PLUS, HarbourSyntaxKind.NAME, HarbourSyntaxKind.EOF];

        foreach (var tokenKind in tokenKinds)
            if (_reader.Match(tokenKind))
            {
                var token = _reader.Consume(tokenKind);
                Assert.AreEqual(token.Kind, tokenKind);
            }
            else
            {
                Assert.Fail($"Did not match tokenKind {tokenKind}.");
            }
    }

    [TestMethod]
    public void TestLookAhead()
    {
        var lexer = new Lexer("a + b");
        _reader = new SourceReader(lexer);

        var token = _reader.LookAhead(0);
        Assert.AreEqual(HarbourSyntaxKind.NAME, token.Kind);

        token = _reader.LookAhead(1);
        Assert.AreEqual(HarbourSyntaxKind.PLUS, token.Kind);

        token = _reader.LookAhead(2);
        Assert.AreEqual(HarbourSyntaxKind.NAME, token.Kind);

        token = _reader.LookAhead(3);
        Assert.AreEqual(HarbourSyntaxKind.EOF, token.Kind);
    }

    [TestMethod]
    public void TestPutBack()
    {
        var lexer = new Lexer("a + b");
        _reader = new SourceReader(lexer);

        Assert.IsTrue(_reader.Match(HarbourSyntaxKind.NAME));
        var token = _reader.Consume(HarbourSyntaxKind.NAME);

        Assert.IsTrue(_reader.Match(HarbourSyntaxKind.PLUS));
        _reader.PutBack(token);
        Assert.IsTrue(_reader.Match(HarbourSyntaxKind.NAME));
    }

    [TestMethod]
    public void TestSetUndoPoint()
    {
        var lexer = new Lexer("a + b");
        _reader = new SourceReader(lexer);

        Assert.IsTrue(_reader.Match(HarbourSyntaxKind.NAME));
        _reader.Consume(HarbourSyntaxKind.NAME);

        _reader.SetUndoPoint();
        Assert.IsTrue(_reader.Match(HarbourSyntaxKind.PLUS));
        _reader.Consume(HarbourSyntaxKind.PLUS);

        Assert.IsTrue(_reader.Match(HarbourSyntaxKind.NAME));
        _reader.Consume(HarbourSyntaxKind.NAME);

        _reader.Undo();
        Assert.IsTrue(_reader.Match(HarbourSyntaxKind.PLUS));
    }

    [TestMethod]
    public void TestUndo()
    {
        var lexer = new Lexer("a + b + c + d");
        _reader = new SourceReader(lexer);

        for (var i = 0; i < 3; i++)
        {
            Assert.IsTrue(_reader.Match(HarbourSyntaxKind.NAME));
            _reader.Consume(HarbourSyntaxKind.NAME);

            Assert.IsTrue(_reader.Match(HarbourSyntaxKind.PLUS));
            _reader.Consume(HarbourSyntaxKind.PLUS);
        }

        Assert.IsTrue(_reader.Match(HarbourSyntaxKind.NAME));
        _reader.Consume(HarbourSyntaxKind.NAME);

        Assert.IsTrue(_reader.Match(HarbourSyntaxKind.EOF));
        _reader.Consume(HarbourSyntaxKind.EOF);

        _reader.Undo();

        for (var i = 0; i < 3; i++)
        {
            Assert.IsTrue(_reader.Match(HarbourSyntaxKind.NAME));
            _reader.Consume(HarbourSyntaxKind.NAME);

            Assert.IsTrue(_reader.Match(HarbourSyntaxKind.PLUS));
            _reader.Consume(HarbourSyntaxKind.PLUS);
        }

        Assert.IsTrue(_reader.Match(HarbourSyntaxKind.NAME));
        _reader.Consume(HarbourSyntaxKind.NAME);

        Assert.IsTrue(_reader.Match(HarbourSyntaxKind.EOF));
        _reader.Consume(HarbourSyntaxKind.EOF);
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