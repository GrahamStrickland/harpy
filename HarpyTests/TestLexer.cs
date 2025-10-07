using System.Data;

using Harpy.Lexer;

namespace HarpyTests
{
    [TestClass]
    public sealed class TestLexer
    {
        private static Lexer? lexer;

        [TestMethod]
        public void TestGetTokens()
        {
            var obs = GetObservedTokens("from + offset(time)");
            var expected = new List<Token> {
                new(SyntaxType.NAME, "from", 1, 4),
                new(SyntaxType.PLUS, "+", 1, 6),
                new(SyntaxType.NAME, "offset", 1, 13),
                new(SyntaxType.LEFT_PAREN, "(", 1, 14),
                new(SyntaxType.NAME, "time", 1, 18),
                new(SyntaxType.RIGHT_PAREN, ")", 1, 19)
            };

            AssertEqual(obs, expected);
        }

        [TestMethod]
        public void TestPreprocessorDirective()
        {
            var obs = GetObservedTokens("#define slBool .f.");
            var expected = new List<Token> {
                new(
                    SyntaxType.DEFINE_DIRECTIVE,
                    "#define slBool .f.",
                    1,
                    18
                )
            };

            AssertEqual(obs, expected);

            obs = GetObservedTokens("#ifdef SOMETHING");
            expected = [
                    new(
                    SyntaxType.IFDEF_DIRECTIVE,
                    "#ifdef SOMETHING",
                    1,
                    16
                )
                ];

            AssertEqual(obs, expected);

            obs = GetObservedTokens("#pragma -ko+");
            expected = [
                    new(
                    SyntaxType.PRAGMA_DIRECTIVE,
                    "#pragma -ko+",
                    1,
                    12
                )
                ];

            AssertEqual(obs, expected);
        }

        [TestMethod]
        public void TestBooleanLiteral()
        {
            var obs = GetObservedTokens(".t.");
            var expected = new List<Token> { new(SyntaxType.BOOL_LITERAL, ".t.", 1, 3) };

            AssertEqual(obs, expected);

            obs = GetObservedTokens(".F.");
            expected = [new(SyntaxType.BOOL_LITERAL, ".F.", 1, 3)];

            AssertEqual(obs, expected);
        }

        [TestMethod]
        public void TestNumericLiteral()
        {
            var obs = GetObservedTokens("0");
            var expected = new List<Token> { new(SyntaxType.NUM_LITERAL, "0", 1, 1) };

            AssertEqual(obs, expected);

            obs = GetObservedTokens("123");
            expected = [new(SyntaxType.NUM_LITERAL, "123", 1, 3)];

            AssertEqual(obs, expected);

            obs = GetObservedTokens("50000");
            expected = [new(SyntaxType.NUM_LITERAL, "50000", 1, 5)];

            AssertEqual(obs, expected);

            obs = GetObservedTokens("12000.123");
            expected = [
                    new(SyntaxType.NUM_LITERAL, "12000.123", 1, 9)
                ];

            AssertEqual(obs, expected);

            obs = GetObservedTokens("0xABAB");
            expected = [
                    new(SyntaxType.NUM_LITERAL, "0xABAB", 1, 6)
                ];

            AssertEqual(obs, expected);

            obs = GetObservedTokens(".12");
            expected = [new(SyntaxType.NUM_LITERAL, ".12", 1, 3)];

            AssertEqual(obs, expected);

            Assert.ThrowsException<SyntaxErrorException>(() => GetObservedTokens("1x001"));

            Assert.ThrowsException<SyntaxErrorException>(() => GetObservedTokens("123a"));

            Assert.ThrowsException<SyntaxErrorException>(() => GetObservedTokens("1.1.1"));
        }

        [TestMethod]
        public void TestStringLiteral()
        {
            var obs = GetObservedTokens("'This is a string'");
            var expected = new List<Token> {
                new(
                    SyntaxType.STR_LITERAL,
                    "'This is a string'",
                    1,
                    18
                )
            };

            AssertEqual(obs, expected);

            obs = GetObservedTokens("\"This is also a string\"");
            expected = [
                    new(SyntaxType.STR_LITERAL, "\"This is also a string\"", 1, 23)
                ];

            AssertEqual(obs, expected);

            obs = GetObservedTokens("[Also a string]");
            expected = [
                    new(
                    SyntaxType.STR_LITERAL, "[Also a string]", 1, 15
                )
                ];

            AssertEqual(obs, expected);

            obs = GetObservedTokens("['Actually a hash key']");
            expected = [
                    new(SyntaxType.LEFT_BRACKET, "[", 1, 1),
                new(
                    SyntaxType.STR_LITERAL,
                    "'Actually a hash key'",
                    1,
                    22
                ),
                new(SyntaxType.RIGHT_BRACKET, "]", 1, 23),
            ];

            AssertEqual(obs, expected);

            obs = GetObservedTokens("function a(b)\n    local i := 1\nreturn b[i]");
            expected = [
                    new(SyntaxType.FUNCTION, "function", 1, 8),
                new(SyntaxType.NAME, "a", 1, 10),
                new(SyntaxType.LEFT_PAREN, "(", 1, 11),
                new(SyntaxType.NAME, "b", 1, 12),
                new(SyntaxType.RIGHT_PAREN, ")", 1, 13),
                new(SyntaxType.LOCAL, "local", 2, 9),
                new(SyntaxType.NAME, "i", 2, 11),
                new(SyntaxType.ASSIGN, ":=", 2, 14),
                new(SyntaxType.NUM_LITERAL, "1", 2, 16),
                new(SyntaxType.RETURN, "return", 3, 6),
                new(SyntaxType.NAME, "b", 3, 8),
                new(SyntaxType.LEFT_BRACKET, "[", 3, 9),
                new(SyntaxType.NAME, "i", 3, 10),
                new(SyntaxType.RIGHT_BRACKET, "]", 3, 11),
            ];

            AssertEqual(obs, expected);

            obs = GetObservedTokens("\"\"");
            expected = [new(SyntaxType.STR_LITERAL, "\"\"", 1, 2)];

            AssertEqual(obs, expected);

            obs = GetObservedTokens("''");
            expected = [new(SyntaxType.STR_LITERAL, "''", 1, 2)];

            AssertEqual(obs, expected);

            Assert.ThrowsException<SyntaxErrorException>(() => GetObservedTokens("'This string does not finish"));

            Assert.ThrowsException<SyntaxErrorException>(() => GetObservedTokens("'"));
        }

        [TestMethod]
        public void TestAssignVsComma()
        {
            var obs = GetObservedTokens("a := b");
            var expected = new List<Token> {
                new(SyntaxType.NAME, "a", 1, 1),
                new(SyntaxType.ASSIGN, ":=", 1, 4),
                new(SyntaxType.NAME, "b", 1, 6),
            };

            AssertEqual(obs, expected);

            obs = GetObservedTokens("a:b");
            expected = [
                    new(SyntaxType.NAME, "a", 1, 1),
                new(SyntaxType.COLON, ":", 1, 2),
                new(SyntaxType.NAME, "b", 1, 3),
            ];

            AssertEqual(obs, expected);
        }

        [TestMethod]
        public void TestLogicalOperators()
        {
            var obs = GetObservedTokens("a .and. b");
            var expected = new List<Token> {
                new(SyntaxType.NAME, "a", 1, 1),
                new(SyntaxType.AND, ".and.", 1, 7),
                new(SyntaxType.NAME, "b", 1, 9),
            };

            AssertEqual(obs, expected);

            obs = GetObservedTokens("a .and. !b");
            expected = [
                    new(SyntaxType.NAME, "a", 1, 1),
                new(SyntaxType.AND, ".and.", 1, 7),
                new(SyntaxType.NOT, "!", 1, 9),
                new(SyntaxType.NAME, "b", 1, 10),
            ];

            AssertEqual(obs, expected);

            obs = GetObservedTokens("!a .and. !b");
            expected = [
                    new(SyntaxType.NOT, "!", 1, 1),
                new(SyntaxType.NAME, "a", 1, 2),
                new(SyntaxType.AND, ".and.", 1, 8),
                new(SyntaxType.NOT, "!", 1, 10),
                new(SyntaxType.NAME, "b", 1, 11),
            ];

            AssertEqual(obs, expected);

            obs = GetObservedTokens("!a .and. !b .or. !c");
            expected = [
                    new(SyntaxType.NOT, "!", 1, 1),
                new(SyntaxType.NAME, "a", 1, 2),
                new(SyntaxType.AND, ".and.", 1, 8),
                new(SyntaxType.NOT, "!", 1, 10),
                new(SyntaxType.NAME, "b", 1, 11),
                new(SyntaxType.OR, ".or.", 1, 16),
                new(SyntaxType.NOT, "!", 1, 18),
                new(SyntaxType.NAME, "c", 1, 19),
            ];

            AssertEqual(obs, expected);
        }

        [TestMethod]
        public void TestRelations()
        {
            var obs = GetObservedTokens("a == b");
            var expected = new List<Token> {
                new(SyntaxType.NAME, "a", 1, 1),
                new(SyntaxType.EQ1, "==", 1, 4),
                new(SyntaxType.NAME, "b", 1, 6),
            };

            AssertEqual(obs, expected);

            obs = GetObservedTokens("a <= b");
            expected = [
                    new(SyntaxType.NAME, "a", 1, 1),
                new(SyntaxType.LE, "<=", 1, 4),
                new(SyntaxType.NAME, "b", 1, 6),
            ];

            AssertEqual(obs, expected);
        }

        [TestMethod]
        public void TestLineComment()
        {
            var obs = GetObservedTokens("// This is a line comment.");
            var expected = new List<Token> {
                new(
                    SyntaxType.LINE_COMMENT,
                    "// This is a line comment.",
                    1,
                    26
                )
            };

            AssertEqual(obs, expected);
        }

        [TestMethod]
        public void TestBlockComment()
        {
            var obs = GetObservedTokens("/* This is a block comment.*/");
            var expected = new List<Token> {
                new(
                    SyntaxType.BLOCK_COMMENT,
                    "/* This is a block comment.*/",
                    1,
                    29
                )
            };

            AssertEqual(obs, expected);

            obs = GetObservedTokens("\n/* This is also a\n * block\n * comment.\n */");
            expected = [
                    new(
                    SyntaxType.BLOCK_COMMENT,
                    "/* This is also a\n * block\n * comment.\n */",
                    2,
                    3
                )
                ];

            AssertEqual(obs, expected);

            Assert.ThrowsException<SyntaxErrorException>(() => GetObservedTokens("/* This is an unfinished block comment.*"));

            Assert.ThrowsException<SyntaxErrorException>(() => GetObservedTokens("/* This one is even worse./*"));
        }

        [TestMethod]
        public void TestFunctionDef()
        {
            var obs = GetObservedTokens("FUNCTION a()\nRETURN b");
            var expected = new List<Token> {
                new(SyntaxType.FUNCTION, "FUNCTION", 1, 8),
                new(SyntaxType.NAME, "a", 1, 10),
                new(SyntaxType.LEFT_PAREN, "(", 1, 11),
                new(SyntaxType.RIGHT_PAREN, ")", 1, 12),
                new(SyntaxType.RETURN, "RETURN", 2, 6),
                new(SyntaxType.NAME, "b", 2, 8),
            };

            AssertEqual(obs, expected);

            obs = GetObservedTokens("function a(b, c, d)\n\nreturn e");
            expected = [
                    new(SyntaxType.FUNCTION, "function", 1, 8),
                new(SyntaxType.NAME, "a", 1, 10),
                new(SyntaxType.LEFT_PAREN, "(", 1, 11),
                new(SyntaxType.NAME, "b", 1, 12),
                new(SyntaxType.COMMA, ",", 1, 13),
                new(SyntaxType.NAME, "c", 1, 15),
                new(SyntaxType.COMMA, ",", 1, 16),
                new(SyntaxType.NAME, "d", 1, 18),
                new(SyntaxType.RIGHT_PAREN, ")", 1, 19),
                new(SyntaxType.RETURN, "return", 3, 6),
                new(SyntaxType.NAME, "e", 3, 8),
            ];

            AssertEqual(obs, expected);
        }

        [TestMethod]
        public void TestProcedureDef()
        {
            var obs = GetObservedTokens("procedure a()\nRETURN");
            var expected = new List<Token> {
                new(SyntaxType.PROCEDURE, "procedure", 1, 9),
                new(SyntaxType.NAME, "a", 1, 11),
                new(SyntaxType.LEFT_PAREN, "(", 1, 12),
                new(SyntaxType.RIGHT_PAREN, ")", 1, 13),
                new(SyntaxType.RETURN, "RETURN", 2, 6),
            };

            AssertEqual(obs, expected);

            obs = GetObservedTokens("procedure a(b, c, d)\n\nreturn");
            expected = [
                    new(SyntaxType.PROCEDURE, "procedure", 1, 9),
                new(SyntaxType.NAME, "a", 1, 11),
                new(SyntaxType.LEFT_PAREN, "(", 1, 12),
                new(SyntaxType.NAME, "b", 1, 13),
                new(SyntaxType.COMMA, ",", 1, 14),
                new(SyntaxType.NAME, "c", 1, 16),
                new(SyntaxType.COMMA, ",", 1, 17),
                new(SyntaxType.NAME, "d", 1, 19),
                new(SyntaxType.RIGHT_PAREN, ")", 1, 20),
                new(SyntaxType.RETURN, "return", 3, 6),
            ];

            AssertEqual(obs, expected);
        }

        [TestMethod]
        public void TestIfElse()
        {
            var obs = GetObservedTokens(
                "if a <= b\n    b()\nelseif c\n    d()\nelse\n    e()\nendif"
            );
            var expected = new List<Token> {
                new(SyntaxType.IF, "if", 1, 2),
                new(SyntaxType.NAME, "a", 1, 4),
                new(SyntaxType.LE, "<=", 1, 7),
                new(SyntaxType.NAME, "b", 1, 9),
                new(SyntaxType.NAME, "b", 2, 5),
                new(SyntaxType.LEFT_PAREN, "(", 2, 6),
                new(SyntaxType.RIGHT_PAREN, ")", 2, 7),
                new(SyntaxType.ELSEIF, "elseif", 3, 6),
                new(SyntaxType.NAME, "c", 3, 8),
                new(SyntaxType.NAME, "d", 4, 5),
                new(SyntaxType.LEFT_PAREN, "(", 4, 6),
                new(SyntaxType.RIGHT_PAREN, ")", 4, 7),
                new(SyntaxType.ELSE, "else", 5, 4),
                new(SyntaxType.NAME, "e", 6, 5),
                new(SyntaxType.LEFT_PAREN, "(", 6, 6),
                new(SyntaxType.RIGHT_PAREN, ")", 6, 7),
                new(SyntaxType.ENDIF, "endif", 7, 5),
            };

            AssertEqual(obs, expected);
        }

        [TestMethod]
        public void TestConditional()
        {
            var obs = GetObservedTokens("iif(a, b, c)");
            var expected = new List<Token> {
                new(SyntaxType.IIF, "iif", 1, 3),
                new(SyntaxType.LEFT_PAREN, "(", 1, 4),
                new(SyntaxType.NAME, "a", 1, 5),
                new(SyntaxType.COMMA, ",", 1, 6),
                new(SyntaxType.NAME, "b", 1, 8),
                new(SyntaxType.COMMA, ",", 1, 9),
                new(SyntaxType.NAME, "c", 1, 11),
                new(SyntaxType.RIGHT_PAREN, ")", 1, 12),
            };

            AssertEqual(obs, expected);
        }


        private static List<Token> GetObservedTokens(string source)
        {
            lexer = new Lexer(source);

            var obs = new List<Token>();

            foreach (var token in lexer.GetTokens())
            {
                if (token.Type == SyntaxType.EOF)
                    break;

                obs.Add(token);
            }

            return obs;
        }

        private static bool AssertEqual(List<Token> obs, List<Token> expected)
        {
            if (obs.Count != expected.Count)
            {
                Assert.Fail($"Token count mismatch. Expected {expected.Count}, but got {obs.Count}.");
                return false;
            }
            for (int i = 0; i < obs.Count; i++)
            {
                if (obs[i].Type != expected[i].Type ||
                    obs[i].Text != expected[i].Text ||
                    obs[i].Line != expected[i].Line ||
                    obs[i].Start != expected[i].Start ||
                    obs[i].End != expected[i].End)
                {
                    Assert.Fail(
                        $"Token lines mismatch at index {i}. "
                        + $"Expected Token({expected[i].Type}, '{expected[i].Text}', {expected[i].Line}, {expected[i].Start}, {expected[i].End}), "
                        + $"but got Token({obs[i].Type}, '{obs[i].Text}', {obs[i].Line}, {obs[i].Start}, {obs[i].End})."
                    );
                    return false;
                }
            }
            return true;
        }
    }
}
