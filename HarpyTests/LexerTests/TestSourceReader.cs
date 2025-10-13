using Harpy.Lexer;

namespace HarpyTests.LexerTests
{
    [TestClass]
    public sealed class TestSourceReader
    {
        private static SourceReader? _reader;

        [TestMethod]
        public void TestGetEnumerator()
        {
            var obs = GetObservedTokens("from + offset(time)");
            var expected = new List<HarbourSyntaxToken> {
                new(HarbourSyntaxKind.NAME, "from", 1, 4),
                new(HarbourSyntaxKind.PLUS, "+", 1, 6),
                new(HarbourSyntaxKind.NAME, "offset", 1, 13),
                new(HarbourSyntaxKind.LEFT_PAREN, "(", 1, 14),
                new(HarbourSyntaxKind.NAME, "time", 1, 18),
                new(HarbourSyntaxKind.RIGHT_PAREN, ")", 1, 19),
                new(HarbourSyntaxKind.EOF, "\0", 1, 19),
            };

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);
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
}
