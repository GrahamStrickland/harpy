using System.Data;

using Harpy.Lexer;

namespace HarpyTests.LexerTests
{
    [TestClass]
    public sealed class TestLexer
    {
        private static Lexer? lexer;

        [TestMethod]
        public void TestPreprocessorDirective()
        {
            var obs = GetObservedTokens("#define slBool .f.");
            var expected = new List<HarbourSyntaxToken> {
                new(HarbourSyntaxKind.EOF, "\0", 1, 18,
                    [
                        new(
                            HarbourSyntaxKind.DEFINE_DIRECTIVE,
                            "#define slBool .f.",
                            1,
                            18
                        )
                    ],
                    []
                )
            };

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected, true);

            obs = GetObservedTokens("#ifdef SOMETHING");
            expected = [
                    new(HarbourSyntaxKind.EOF, "\0", 1, 16,
                        [
                            new(
                                HarbourSyntaxKind.IFDEF_DIRECTIVE,
                                "#ifdef SOMETHING",
                                1,
                                16
                            ),
                        ],
                        []
                    ),
                ];

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected, true);

            obs = GetObservedTokens("#pragma -ko+");
            expected = [
                    new(HarbourSyntaxKind.EOF, "\0", 1, 12,
                        [
                            new(
                                HarbourSyntaxKind.PRAGMA_DIRECTIVE,
                                "#pragma -ko+",
                                1,
                                12
                            ),
                        ],
                        []
                    ),
                ];

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected, true);
        }

        [TestMethod]
        public void TestBooleanLiteral()
        {
            var obs = GetObservedTokens(".t.");
            var expected = new List<HarbourSyntaxToken> {
                new(HarbourSyntaxKind.BOOL_LITERAL, ".t.", 1, 3),
                new(HarbourSyntaxKind.EOF, "\0", 1, 3),
            };

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);

            obs = GetObservedTokens(".F.");
            expected = [
                new(HarbourSyntaxKind.BOOL_LITERAL, ".F.", 1, 3),
                new(HarbourSyntaxKind.EOF, "\0", 1, 3),
            ];

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);
        }

        [TestMethod]
        public void TestNumericLiteral()
        {
            var obs = GetObservedTokens("0");
            var expected = new List<HarbourSyntaxToken> {
                new(HarbourSyntaxKind.NUM_LITERAL, "0", 1, 1),
                new(HarbourSyntaxKind.EOF, "\0", 1, 1),
            };

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);

            obs = GetObservedTokens("123");
            expected = [
                new(HarbourSyntaxKind.NUM_LITERAL, "123", 1, 3),
                new(HarbourSyntaxKind.EOF, "\0", 1, 3),
            ];

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);

            obs = GetObservedTokens("50000");
            expected = [
                new(HarbourSyntaxKind.NUM_LITERAL, "50000", 1, 5),
                new(HarbourSyntaxKind.EOF, "\0", 1, 5),
            ];

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);

            obs = GetObservedTokens("12000.123");
            expected = [
                new(HarbourSyntaxKind.NUM_LITERAL, "12000.123", 1, 9),
                new(HarbourSyntaxKind.EOF, "\0", 1, 9),
            ];

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);

            obs = GetObservedTokens("0xABAB");
            expected = [
                new(HarbourSyntaxKind.NUM_LITERAL, "0xABAB", 1, 6),
                new(HarbourSyntaxKind.EOF, "\0", 1, 6),
            ];

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);

            obs = GetObservedTokens(".12");
            expected = [
                new(HarbourSyntaxKind.NUM_LITERAL, ".12", 1, 3),
                new(HarbourSyntaxKind.EOF, "\0", 1, 3),
            ];

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);

            Assert.ThrowsException<SyntaxErrorException>(() => GetObservedTokens("1x001"));

            Assert.ThrowsException<SyntaxErrorException>(() => GetObservedTokens("123a"));

            Assert.ThrowsException<SyntaxErrorException>(() => GetObservedTokens("1.1.1"));
        }

        [TestMethod]
        public void TestStringLiteral()
        {
            var obs = GetObservedTokens("'This is a string'");
            var expected = new List<HarbourSyntaxToken> {
                new(
                    HarbourSyntaxKind.STR_LITERAL,
                    "'This is a string'",
                    1,
                    18
                ),
                new(HarbourSyntaxKind.EOF, "\0", 1, 18),
            };

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);

            obs = GetObservedTokens("\"This is also a string\"");
            expected = [
                new(HarbourSyntaxKind.STR_LITERAL, "\"This is also a string\"", 1, 23),
                new(HarbourSyntaxKind.EOF, "\0", 1, 23),
            ];

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);

            obs = GetObservedTokens("[Also a string]");
            expected = [
                new(
                    HarbourSyntaxKind.STR_LITERAL, "[Also a string]", 1, 15
                ),
                new(HarbourSyntaxKind.EOF, "\0", 1, 15),
            ];

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);

            obs = GetObservedTokens("['Actually a hash key']");
            expected = [
                new(HarbourSyntaxKind.LEFT_BRACKET, "[", 1, 1),
                new(
                    HarbourSyntaxKind.STR_LITERAL,
                    "'Actually a hash key'",
                    1,
                    22
                ),
                new(HarbourSyntaxKind.RIGHT_BRACKET, "]", 1, 23),
                new(HarbourSyntaxKind.EOF, "\0", 1, 23),
            ];

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);

            obs = GetObservedTokens("function a(b)\n    local i := 1\nreturn b[i]");
            expected = [
                new(HarbourSyntaxKind.FUNCTION, "function", 1, 8),
                new(HarbourSyntaxKind.NAME, "a", 1, 10),
                new(HarbourSyntaxKind.LEFT_PAREN, "(", 1, 11),
                new(HarbourSyntaxKind.NAME, "b", 1, 12),
                new(HarbourSyntaxKind.RIGHT_PAREN, ")", 1, 13),
                new(HarbourSyntaxKind.LOCAL, "local", 2, 9),
                new(HarbourSyntaxKind.NAME, "i", 2, 11),
                new(HarbourSyntaxKind.ASSIGN, ":=", 2, 14),
                new(HarbourSyntaxKind.NUM_LITERAL, "1", 2, 16),
                new(HarbourSyntaxKind.RETURN, "return", 3, 6),
                new(HarbourSyntaxKind.NAME, "b", 3, 8),
                new(HarbourSyntaxKind.LEFT_BRACKET, "[", 3, 9),
                new(HarbourSyntaxKind.NAME, "i", 3, 10),
                new(HarbourSyntaxKind.RIGHT_BRACKET, "]", 3, 11),
                new(HarbourSyntaxKind.EOF, "\0", 3, 11),
            ];

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);

            obs = GetObservedTokens("\"\"");
            expected = [
                new(HarbourSyntaxKind.STR_LITERAL, "\"\"", 1, 2),
                new(HarbourSyntaxKind.EOF, "\0", 1, 2),
            ];

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);

            obs = GetObservedTokens("''");
            expected = [
                new(HarbourSyntaxKind.STR_LITERAL, "''", 1, 2),
                new(HarbourSyntaxKind.EOF, "\0", 1, 2),
            ];

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);

            Assert.ThrowsException<SyntaxErrorException>(() => GetObservedTokens("'This string does not finish"));

            Assert.ThrowsException<SyntaxErrorException>(() => GetObservedTokens("'"));
        }

        [TestMethod]
        public void TestAssignVsComma()
        {
            var obs = GetObservedTokens("a := b");
            var expected = new List<HarbourSyntaxToken> {
                new(HarbourSyntaxKind.NAME, "a", 1, 1),
                new(HarbourSyntaxKind.ASSIGN, ":=", 1, 4),
                new(HarbourSyntaxKind.NAME, "b", 1, 6),
                new(HarbourSyntaxKind.EOF, "\0", 1, 6),
            };

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);

            obs = GetObservedTokens("a:b");
            expected = [
                new(HarbourSyntaxKind.NAME, "a", 1, 1),
                new(HarbourSyntaxKind.COLON, ":", 1, 2),
                new(HarbourSyntaxKind.NAME, "b", 1, 3),
                new(HarbourSyntaxKind.EOF, "\0", 1, 3),
            ];

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);
        }

        [TestMethod]
        public void TestLogicalOperators()
        {
            var obs = GetObservedTokens("a .and. b");
            var expected = new List<HarbourSyntaxToken> {
                new(HarbourSyntaxKind.NAME, "a", 1, 1),
                new(HarbourSyntaxKind.AND, ".and.", 1, 7),
                new(HarbourSyntaxKind.NAME, "b", 1, 9),
                new(HarbourSyntaxKind.EOF, "\0", 1, 9),
            };

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);

            obs = GetObservedTokens("a .and. !b");
            expected = [
                new(HarbourSyntaxKind.NAME, "a", 1, 1),
                new(HarbourSyntaxKind.AND, ".and.", 1, 7),
                new(HarbourSyntaxKind.NOT, "!", 1, 9),
                new(HarbourSyntaxKind.NAME, "b", 1, 10),
                new(HarbourSyntaxKind.EOF, "\0", 1, 10),
            ];

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);

            obs = GetObservedTokens("!a .and. !b");
            expected = [
                new(HarbourSyntaxKind.NOT, "!", 1, 1),
                new(HarbourSyntaxKind.NAME, "a", 1, 2),
                new(HarbourSyntaxKind.AND, ".and.", 1, 8),
                new(HarbourSyntaxKind.NOT, "!", 1, 10),
                new(HarbourSyntaxKind.NAME, "b", 1, 11),
                new(HarbourSyntaxKind.EOF, "\0", 1, 11),
            ];

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);

            obs = GetObservedTokens("!a .and. !b .or. !c");
            expected = [
                new(HarbourSyntaxKind.NOT, "!", 1, 1),
                new(HarbourSyntaxKind.NAME, "a", 1, 2),
                new(HarbourSyntaxKind.AND, ".and.", 1, 8),
                new(HarbourSyntaxKind.NOT, "!", 1, 10),
                new(HarbourSyntaxKind.NAME, "b", 1, 11),
                new(HarbourSyntaxKind.OR, ".or.", 1, 16),
                new(HarbourSyntaxKind.NOT, "!", 1, 18),
                new(HarbourSyntaxKind.NAME, "c", 1, 19),
                new(HarbourSyntaxKind.EOF, "\0", 1, 19),
            ];

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);
        }

        [TestMethod]
        public void TestRelations()
        {
            var obs = GetObservedTokens("a == b");
            var expected = new List<HarbourSyntaxToken> {
                new(HarbourSyntaxKind.NAME, "a", 1, 1),
                new(HarbourSyntaxKind.EQ1, "==", 1, 4),
                new(HarbourSyntaxKind.NAME, "b", 1, 6),
                new(HarbourSyntaxKind.EOF, "\0", 1, 6),
            };

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);

            obs = GetObservedTokens("a <= b");
            expected = [
                new(HarbourSyntaxKind.NAME, "a", 1, 1),
                new(HarbourSyntaxKind.LE, "<=", 1, 4),
                new(HarbourSyntaxKind.NAME, "b", 1, 6),
                new(HarbourSyntaxKind.EOF, "\0", 1, 6),
            ];

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);
        }

        [TestMethod]
        public void TestLineComment()
        {
            var obs = GetObservedTokens("// This is a line comment.");
            var expected = new List<HarbourSyntaxToken> {
                new(HarbourSyntaxKind.EOF, "\0", 1, 26,
                    [
                        new(
                            HarbourSyntaxKind.LINE_COMMENT,
                            "// This is a line comment.",
                            1,
                            26
                        )
                    ],
                    []
                ),
            };

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);
        }

        [TestMethod]
        public void TestBlockComment()
        {
            var obs = GetObservedTokens("/* This is a block comment.*/");
            var expected = new List<HarbourSyntaxToken> {
                new(HarbourSyntaxKind.EOF, "\0", 1, 29,
                    [
                        new(
                            HarbourSyntaxKind.BLOCK_COMMENT,
                            "/* This is a block comment.*/",
                            1,
                            29
                        )
                    ],
                    []
                ),
            };

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);

            obs = GetObservedTokens("\n/* This is also a\n * block\n * comment.\n */");
            expected = [
                new(HarbourSyntaxKind.EOF, "\0", 5, 3,
                    [
                        new(
                            HarbourSyntaxKind.BLOCK_COMMENT,
                            "/* This is also a\n * block\n * comment.\n */",
                            2,
                            3
                        )
                    ],
                    []
                ),
            ];

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);

            Assert.ThrowsException<SyntaxErrorException>(() => GetObservedTokens("/* This is an unfinished block comment.*"));

            Assert.ThrowsException<SyntaxErrorException>(() => GetObservedTokens("/* This one is even worse./*"));
        }

        [TestMethod]
        public void TestFunctionDef()
        {
            var obs = GetObservedTokens("FUNCTION a()\nRETURN b");
            var expected = new List<HarbourSyntaxToken> {
                new(HarbourSyntaxKind.FUNCTION, "FUNCTION", 1, 8),
                new(HarbourSyntaxKind.NAME, "a", 1, 10),
                new(HarbourSyntaxKind.LEFT_PAREN, "(", 1, 11),
                new(HarbourSyntaxKind.RIGHT_PAREN, ")", 1, 12),
                new(HarbourSyntaxKind.RETURN, "RETURN", 2, 6),
                new(HarbourSyntaxKind.NAME, "b", 2, 8),
                new(HarbourSyntaxKind.EOF, "\0", 2, 8),
            };

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);

            obs = GetObservedTokens("function a(b, c, d)\n\nreturn e");
            expected = [
                new(HarbourSyntaxKind.FUNCTION, "function", 1, 8),
                new(HarbourSyntaxKind.NAME, "a", 1, 10),
                new(HarbourSyntaxKind.LEFT_PAREN, "(", 1, 11),
                new(HarbourSyntaxKind.NAME, "b", 1, 12),
                new(HarbourSyntaxKind.COMMA, ",", 1, 13),
                new(HarbourSyntaxKind.NAME, "c", 1, 15),
                new(HarbourSyntaxKind.COMMA, ",", 1, 16),
                new(HarbourSyntaxKind.NAME, "d", 1, 18),
                new(HarbourSyntaxKind.RIGHT_PAREN, ")", 1, 19),
                new(HarbourSyntaxKind.RETURN, "return", 3, 6),
                new(HarbourSyntaxKind.NAME, "e", 3, 8),
                new(HarbourSyntaxKind.EOF, "\0", 3, 8),
            ];

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);
        }

        [TestMethod]
        public void TestProcedureDef()
        {
            var obs = GetObservedTokens("procedure a()\nRETURN");
            var expected = new List<HarbourSyntaxToken> {
                new(HarbourSyntaxKind.PROCEDURE, "procedure", 1, 9),
                new(HarbourSyntaxKind.NAME, "a", 1, 11),
                new(HarbourSyntaxKind.LEFT_PAREN, "(", 1, 12),
                new(HarbourSyntaxKind.RIGHT_PAREN, ")", 1, 13),
                new(HarbourSyntaxKind.RETURN, "RETURN", 2, 6),
                new(HarbourSyntaxKind.EOF, "\0", 2, 6),
            };

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);

            obs = GetObservedTokens("procedure a(b, c, d)\n\nreturn");
            expected = [
                new(HarbourSyntaxKind.PROCEDURE, "procedure", 1, 9),
                new(HarbourSyntaxKind.NAME, "a", 1, 11),
                new(HarbourSyntaxKind.LEFT_PAREN, "(", 1, 12),
                new(HarbourSyntaxKind.NAME, "b", 1, 13),
                new(HarbourSyntaxKind.COMMA, ",", 1, 14),
                new(HarbourSyntaxKind.NAME, "c", 1, 16),
                new(HarbourSyntaxKind.COMMA, ",", 1, 17),
                new(HarbourSyntaxKind.NAME, "d", 1, 19),
                new(HarbourSyntaxKind.RIGHT_PAREN, ")", 1, 20),
                new(HarbourSyntaxKind.RETURN, "return", 3, 6),
                new(HarbourSyntaxKind.EOF, "\0", 3, 6),
            ];

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);
        }

        [TestMethod]
        public void TestIfElse()
        {
            var obs = GetObservedTokens(
                "if a <= b\n    b()\nelseif c\n    d()\nelse\n    e()\nendif"
            );
            var expected = new List<HarbourSyntaxToken> {
                new(HarbourSyntaxKind.IF, "if", 1, 2),
                new(HarbourSyntaxKind.NAME, "a", 1, 4),
                new(HarbourSyntaxKind.LE, "<=", 1, 7),
                new(HarbourSyntaxKind.NAME, "b", 1, 9),
                new(HarbourSyntaxKind.NAME, "b", 2, 5),
                new(HarbourSyntaxKind.LEFT_PAREN, "(", 2, 6),
                new(HarbourSyntaxKind.RIGHT_PAREN, ")", 2, 7),
                new(HarbourSyntaxKind.ELSEIF, "elseif", 3, 6),
                new(HarbourSyntaxKind.NAME, "c", 3, 8),
                new(HarbourSyntaxKind.NAME, "d", 4, 5),
                new(HarbourSyntaxKind.LEFT_PAREN, "(", 4, 6),
                new(HarbourSyntaxKind.RIGHT_PAREN, ")", 4, 7),
                new(HarbourSyntaxKind.ELSE, "else", 5, 4),
                new(HarbourSyntaxKind.NAME, "e", 6, 5),
                new(HarbourSyntaxKind.LEFT_PAREN, "(", 6, 6),
                new(HarbourSyntaxKind.RIGHT_PAREN, ")", 6, 7),
                new(HarbourSyntaxKind.ENDIF, "endif", 7, 5),
                new(HarbourSyntaxKind.EOF, "\0", 7, 5),
            };

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);
        }

        [TestMethod]
        public void TestConditional()
        {
            var obs = GetObservedTokens("iif(a, b, c)");
            var expected = new List<HarbourSyntaxToken> {
                new(HarbourSyntaxKind.IIF, "iif", 1, 3),
                new(HarbourSyntaxKind.LEFT_PAREN, "(", 1, 4),
                new(HarbourSyntaxKind.NAME, "a", 1, 5),
                new(HarbourSyntaxKind.COMMA, ",", 1, 6),
                new(HarbourSyntaxKind.NAME, "b", 1, 8),
                new(HarbourSyntaxKind.COMMA, ",", 1, 9),
                new(HarbourSyntaxKind.NAME, "c", 1, 11),
                new(HarbourSyntaxKind.RIGHT_PAREN, ")", 1, 12),
                new(HarbourSyntaxKind.EOF, "\0", 1, 12),
            };

            Utils.SyntaxTokenUtils.AssertTokenListsEqual(obs, expected);
        }

        private static List<HarbourSyntaxToken> GetObservedTokens(string source)
        {
            lexer = new Lexer(source);

            var obs = new List<HarbourSyntaxToken>();

            foreach (var token in lexer)
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
