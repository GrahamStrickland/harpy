using System.Data;

namespace Harpy.Lexer
{
    /// <summary>
    /// Takes a string and splits it into a series of <c>Token</c>s. 
    /// Operators and punctuation are mapped to unique keywords.    
    /// </summary>
    internal class Lexer(string text)
    {
        private static readonly Dictionary<string, HarbourSyntaxKind> _directives = new()
        {
             { "include", HarbourSyntaxKind.INCLUDE_DIRECTIVE },
             { "define", HarbourSyntaxKind.DEFINE_DIRECTIVE },
             { "ifdef", HarbourSyntaxKind.IFDEF_DIRECTIVE },
             { "ifndef", HarbourSyntaxKind.IFNDEF_DIRECTIVE },
             { "elif", HarbourSyntaxKind.ELIF_DIRECTIVE },
             { "else", HarbourSyntaxKind.ELSE_DIRECTIVE },
             { "endif", HarbourSyntaxKind.ENDIF_DIRECTIVE },
             { "undef", HarbourSyntaxKind.UNDEF_DIRECTIVE },
             { "pragma", HarbourSyntaxKind.PRAGMA_DIRECTIVE },
             { "command", HarbourSyntaxKind.COMMAND_DIRECTIVE },
             { "xcommand", HarbourSyntaxKind.XCOMMAND_DIRECTIVE },
             { "translate", HarbourSyntaxKind.TRANSLATE_DIRECTIVE },
             { "xtranslate", HarbourSyntaxKind.XTRANSLATE_DIRECTIVE },
             { "error", HarbourSyntaxKind.ERROR_DIRECTIVE },
             { "stdout", HarbourSyntaxKind.STDOUT_DIRECTIVE }
        };
        private static readonly Dictionary<string, HarbourSyntaxKind> _keywords = new()
        {
             { "function", HarbourSyntaxKind.FUNCTION },
             { "procedure", HarbourSyntaxKind.PROCEDURE },
             { "return", HarbourSyntaxKind.RETURN },
             { "nil", HarbourSyntaxKind.NIL },
             { "local", HarbourSyntaxKind.LOCAL },
             { "static", HarbourSyntaxKind.STATIC },
             { "iif", HarbourSyntaxKind.IIF },
             { "if", HarbourSyntaxKind.IF },
             { "else", HarbourSyntaxKind.ELSE },
             { "elseif", HarbourSyntaxKind.ELSEIF },
             { "end", HarbourSyntaxKind.END },
             { "endif", HarbourSyntaxKind.ENDIF },
             { "enderr", HarbourSyntaxKind.ENDERR },
             { "while", HarbourSyntaxKind.WHILE },
             { "endwhile", HarbourSyntaxKind.ENDWHILE }
        };
        private static readonly Dictionary<string, HarbourSyntaxKind> _compoundOperators = new()
        {
             { ":=", HarbourSyntaxKind.ASSIGN },
             { "+=", HarbourSyntaxKind.PLUSEQ },
             { "-=", HarbourSyntaxKind.MINUSEQ },
             { "*=", HarbourSyntaxKind.MULTEQ },
             { "/=", HarbourSyntaxKind.DIVEQ },
             { "%=", HarbourSyntaxKind.MODEQ },
             { "^=", HarbourSyntaxKind.EXPEQ },
             { ".and.", HarbourSyntaxKind.AND },
             { ".or.", HarbourSyntaxKind.OR },
             { "==", HarbourSyntaxKind.EQ1 },
             { "!=", HarbourSyntaxKind.NE2 },
             { "<=", HarbourSyntaxKind.LE },
             { ">=", HarbourSyntaxKind.GE },
             { "=>", HarbourSyntaxKind.HASHOP }
        };
        private static readonly Dictionary<string, HarbourSyntaxKind> _simpleOperators = new()
        {
             { "(", HarbourSyntaxKind.LEFT_PAREN },
             { ")", HarbourSyntaxKind.RIGHT_PAREN },
             { "[", HarbourSyntaxKind.LEFT_BRACKET },
             { "]", HarbourSyntaxKind.RIGHT_BRACKET },
             { "{", HarbourSyntaxKind.LEFT_BRACE },
             { "}", HarbourSyntaxKind.RIGHT_BRACE },
             { "|", HarbourSyntaxKind.PIPE },
             { ",", HarbourSyntaxKind.COMMA },
             { "=", HarbourSyntaxKind.EQ2 },
             { "#", HarbourSyntaxKind.NE1 },
             { "<", HarbourSyntaxKind.LT },
             { ">", HarbourSyntaxKind.GT },
             { "$", HarbourSyntaxKind.DOLLAR },
             { "+", HarbourSyntaxKind.PLUS },
             { "-", HarbourSyntaxKind.MINUS },
             { "*", HarbourSyntaxKind.ASTERISK },
             { "/", HarbourSyntaxKind.SLASH },
             { "%", HarbourSyntaxKind.PERCENT },
             { "^", HarbourSyntaxKind.CARET },
             { "!", HarbourSyntaxKind.NOT },
             { "?", HarbourSyntaxKind.QUESTION },
             { ":", HarbourSyntaxKind.COLON },
             { "@", HarbourSyntaxKind.AT }
        };
        private List<string> _names = [];

        private int _index = 0;
        private int _line = 1;
        private int _pos = 0;
        private int _resetIndex = 0;

        private readonly string _text = text;

        public IEnumerable<HarbourSyntaxToken> GetTokens()
        {
            List<HarbourSyntaxTrivia> leadingTrivia = [];
            List<HarbourSyntaxTrivia> trailingTrivia = [];
            HarbourSyntaxToken? oldToken = null;

            while (_index < _text.Length)
            {
                HarbourSyntaxToken? newToken = null;
                char c = Advance();

                switch (c)
                {
                    case '#':
                        {
                            if (newToken == null)
                            {
                                leadingTrivia.Add((HarbourSyntaxTrivia)ReadPreprocessorDirectiveOrNeOp());
                            }
                            else
                            {
                                trailingTrivia.Add((HarbourSyntaxTrivia)ReadPreprocessorDirectiveOrNeOp());
                            }

                            break;
                        }
                    case '/':
                        {
                            switch (Peek())
                            {
                                case '/':
                                    {
                                        if (newToken == null)
                                        {
                                            leadingTrivia.Add(ReadLineComment());
                                        }
                                        else
                                        {
                                            trailingTrivia.Add(ReadLineComment());
                                        }
                                        break;
                                    }
                                case '*':
                                    {
                                        if (newToken == null)
                                        {
                                            leadingTrivia.Add(ReadBlockComment());
                                        }
                                        else
                                        {
                                            trailingTrivia.Add(ReadBlockComment());
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        newToken = new HarbourSyntaxToken(
                                            _simpleOperators[c.ToString()],
                                            c.ToString(),
                                            _line,
                                            _pos
                                        );
                                        break;
                                    }
                            }
                            break;
                        }
                    case '[':
                        {
                            if (Char.IsLetterOrDigit(Peek()))
                            {
                                newToken = ReadStringLiteralOrBrackets(c);
                            }
                            else
                            {
                                newToken = new HarbourSyntaxToken(HarbourSyntaxKind.LEFT_BRACKET, c.ToString(), _line, _pos);
                            }

                            break;
                        }
                    case '"':
                    case '\'':
                        {
                            newToken = ReadStringLiteralOrBrackets(c);
                            break;
                        }
                    case '.':
                        {
                            if (Char.IsLetter(Peek()))
                            {
                                newToken = ReadBooleanLiteralOrLogical(c);
                                break;
                            }
                            else
                            {
                                newToken = ReadNumericLiteral(c);
                            }
                            break;
                        }
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        {
                            newToken = ReadNumericLiteral(c);
                            break;
                        }
                    default:
                        {
                            HarbourSyntaxToken? keyword = ReadKeyword(c);
                            string op = c.ToString() + Peek();

                            if (keyword != null)
                            {
                                newToken = keyword;
                            }
                            else if (_compoundOperators.ContainsKey(op))
                            {
                                Advance();
                                newToken = new HarbourSyntaxToken(
                                    _compoundOperators[op],
                                    op,
                                    _line,
                                    _pos
                                );
                            }
                            else if (_simpleOperators.ContainsKey(c.ToString()))
                            {
                                newToken = new HarbourSyntaxToken(
                                    _simpleOperators[c.ToString()],
                                    c.ToString(),
                                    _line,
                                    _pos
                                );
                            }
                            else if (Char.IsLetterOrDigit(c))
                            {
                                newToken = ReadName();
                            }
                            else if (Char.IsWhiteSpace(c) || c == ';')
                            {
                                HarbourSyntaxKind type;
                                switch (c)
                                {
                                    case '\n':
                                        {
                                            type = HarbourSyntaxKind.NEWLINE;
                                            _line += 1;
                                            _pos = 0;
                                            break;
                                        }
                                    case '\r':
                                        {
                                            type = HarbourSyntaxKind.CARRIAGE_RETURN;
                                            _line += 1;
                                            _pos = 0;
                                            break;
                                        }
                                    case '\t':
                                        {
                                            type = HarbourSyntaxKind.TAB;
                                            break;
                                        }
                                    case ' ':
                                        {
                                            type = HarbourSyntaxKind.SPACE;
                                            break;
                                        }
                                    case ';':
                                        {
                                            type = HarbourSyntaxKind.LINE_CONTINUATION;
                                            break;
                                        }
                                    default:
                                        throw new SyntaxErrorException($"Unknown character '{c}' encountered in source.");
                                }
                                if (newToken == null)
                                {
                                    leadingTrivia.Add(
                                        new HarbourSyntaxTrivia(
                                            type,
                                            c.ToString(),
                                            _line,
                                            _pos
                                        )
                                    );
                                }
                                else
                                {
                                    trailingTrivia.Add(
                                        new HarbourSyntaxTrivia(
                                            type,
                                            c.ToString(),
                                            _line,
                                            _pos
                                        )
                                    );
                                }
                            }

                            break;
                        }
                }

                if (newToken != null)
                {
                    if (oldToken != null)
                    {
                        oldToken.TrailingTrivia = trailingTrivia;
                        leadingTrivia.Clear();
                        trailingTrivia.Clear();
                        yield return oldToken;
                    }

                    oldToken = newToken;
                    oldToken.LeadingTrivia = leadingTrivia;
                }
            }

            if (oldToken != null)
            {
                HarbourSyntaxToken currentToken = oldToken;
                oldToken = null;
                currentToken.TrailingTrivia = trailingTrivia;
                leadingTrivia.Clear();
                trailingTrivia.Clear();
                yield return currentToken;
            }
            // Once we've reached the end of the string, just return EOF tokens. We'll
            // just keep returning them as many times as we're asked so that the
            // parser's lookahead doesn't have to worry about running out of tokens.
            yield return new HarbourSyntaxToken(HarbourSyntaxKind.EOF, "\0", _line, _pos, leadingTrivia, trailingTrivia);
        }

        private HarbourSyntaxElement ReadPreprocessorDirectiveOrNeOp()
        {
            int startIndex = _index - 1;
            SetResetIndex(startIndex);
            string directive = "";
            char c;

            while (Char.IsLetterOrDigit(c = Advance()))
            {
                directive += c;
            }

            if (_directives.ContainsKey(directive.ToLower()))
            {
                while (true)
                {
                    switch (Peek())
                    {
                        case '\n':
                        case '\r':
                        case '\0':
                            {
                                return new HarbourSyntaxTrivia(
                                    _directives[directive.ToLower()],
                                    _text[startIndex.._index],
                                    _line,
                                    _pos
                                );
                            }
                        default:
                            {
                                Advance();
                                break;
                            }
                    }
                }
            }

            Reset(1);

            return new HarbourSyntaxToken(HarbourSyntaxKind.NE1, "#", _line, _pos);
        }

        private HarbourSyntaxTrivia ReadLineComment()
        {
            int startIndex = _index - 1;

            Advance();

            while (true)
            {
                switch (Peek())
                {
                    case '\n':
                    case '\r':
                    case '\0':
                        {
                            return new HarbourSyntaxTrivia(
                                HarbourSyntaxKind.LINE_COMMENT,
                                _text[startIndex.._index],
                                _line,
                                _pos
                            );
                        }
                    default:
                        {
                            Advance();
                            break;
                        }
                }
            }
        }

        private HarbourSyntaxTrivia ReadBlockComment()
        {
            int startIndex = _index - 1;
            int startLine = _line;

            while (true)
            {
                char c = Advance();

                switch (c)
                {
                    case '*':
                        {
                            char c1 = Advance();

                            switch (c1)
                            {
                                case '/':
                                    {
                                        return new HarbourSyntaxTrivia(
                                            HarbourSyntaxKind.BLOCK_COMMENT,
                                            _text[startIndex.._index],
                                            startLine,
                                            _pos
                                        );
                                    }
                                case '\0':
                                    {
                                        throw new SyntaxErrorException($"Unterminated block comment starting at line {startLine}.");
                                    }
                                default:
                                    {
                                        if (c1 == '\n')
                                        {
                                            _line += 1;
                                            _pos = 0;
                                        }
                                        break;
                                    }
                            }
                            break;
                        }
                    case '\0':
                        {
                            throw new SyntaxErrorException($"Unterminated block comment starting at line {startLine}.");
                        }
                    default:
                        {
                            if (c == '\n')
                            {
                                _line += 1;
                                _pos = 0;
                            }
                            break;
                        }
                }
            }
        }

        private HarbourSyntaxToken ReadBooleanLiteralOrLogical(char c)
        {
            string literal = c.ToString();

            while ((c = Advance()) != '.' && c != '\0')
            {
                literal += c;
            }

            literal += '.';

            switch (literal.ToLower())
            {
                case ".t.":
                case ".f.":
                    {
                        return new HarbourSyntaxToken(
                            HarbourSyntaxKind.BOOL_LITERAL,
                            literal,
                            _line,
                            _pos
                        );
                    }
                case ".or.":
                    {
                        return new HarbourSyntaxToken(
                            HarbourSyntaxKind.OR,
                            literal,
                            _line,
                            _pos
                        );
                    }
                case ".and.":
                    {
                        return new HarbourSyntaxToken(
                            HarbourSyntaxKind.AND,
                            literal,
                            _line,
                            _pos
                        );
                    }
                default:
                    {
                        throw new SyntaxErrorException($"Unable to read token '{literal}'.");
                    }
            }
        }

        private HarbourSyntaxToken ReadNumericLiteral(char c)
        {
            string literal = c.ToString();
            bool dotFound = false;
            bool hexNum = false;

            c = Peek();
            while (!Char.IsWhiteSpace(c))
            {
                switch (Char.ToLower(c))
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        {
                            literal += c;
                            Advance();
                            break;
                        }
                    case 'x':
                        {
                            if (literal == "0")
                            {
                                literal += c;
                                hexNum = true;
                                Advance();
                                break;
                            }

                            throw new SyntaxErrorException($"Unterminated hexadecimal literal '{literal + c}'.");
                        }
                    case 'a':
                    case 'b':
                    case 'c':
                    case 'd':
                    case 'e':
                    case 'f':
                        {
                            if (hexNum)
                            {
                                literal += c;
                                Advance();
                                break;
                            }

                            throw new SyntaxErrorException($"Invalid numeric literal '{literal + c}'.");
                        }
                    case '.':
                        {
                            if (!dotFound)
                            {
                                literal += c;
                                dotFound = true;
                                Advance();
                                break;
                            }

                            throw new SyntaxErrorException($"Second decimal point found in literal '{literal + c}'.");
                        }
                    case '\0':
                        {
                            return new HarbourSyntaxToken(
                                HarbourSyntaxKind.NUM_LITERAL,
                                literal,
                                _line,
                                _pos
                            );
                        }
                    default:
                        {
                            return new HarbourSyntaxToken(
                                HarbourSyntaxKind.NUM_LITERAL,
                                literal,
                                _line,
                                _pos
                            );
                        }
                }
                c = Peek();
            }

            return new HarbourSyntaxToken(
                HarbourSyntaxKind.NUM_LITERAL,
                literal,
                _line,
                _pos
            );
        }

        private HarbourSyntaxToken ReadStringLiteralOrBrackets(char c)
        {
            int startIndex = _index;
            SetResetIndex(startIndex);
            string literal = c.ToString();
            char endquote = c == '[' ? ']' : c;

            while ((c = Peek()) != endquote)
            {
                switch (c)
                {
                    case '\0':
                        {
                            if (literal.Length > 1 && literal.EndsWith(endquote))
                            {
                                break;
                            }
                            throw new SyntaxErrorException($"Unterminated string literal '{literal}'.");
                        }
                    default:
                        {
                            literal += c;
                            break;
                        }
                }
                Advance();
            }

            // TODO: This won't work unless we handle include files/preprocessor parsing first,
            // e.g., `IF !(aHWFlag[F_HWCardReader] .OR. aHWFlag[F_HWDallasKey]) .OR. oPOSStatus:oFree:lFlag3`
            if (endquote == ']' && _names.Contains(literal[1..]))
            {
                Reset();
                return new HarbourSyntaxToken(HarbourSyntaxKind.LEFT_BRACKET, "[", _line, _pos);
            }

            return new HarbourSyntaxToken(
                HarbourSyntaxKind.STR_LITERAL,
                literal + Advance().ToString(),
                _line,
                _pos
            );
        }

        private HarbourSyntaxToken? ReadKeyword(char c)
        {
            string keyword = c.ToString();
            SetResetIndex(_index);

            c = Peek();
            while (char.IsLetterOrDigit(c))
            {
                keyword += c;
                Advance();
                c = Peek();
            }

            if (_keywords.ContainsKey(keyword.ToLower()))
            {
                return new HarbourSyntaxToken(
                    _keywords[keyword.ToLower()],
                    keyword,
                    _line,
                    _pos
                );
            }

            Reset();
            return null;
        }

        private HarbourSyntaxToken ReadName()
        {
            string name;
            int startIndex = _index - 1;
            SetResetIndex(startIndex);

            while (Peek() != '\0')
            {
                if (!Char.IsLetterOrDigit(_text[_index]) && _text[_index] != '_')
                {
                    break;
                }

                Advance();
            }

            name = _text[startIndex.._index];

            _names.Add(name);

            return new HarbourSyntaxToken(
                HarbourSyntaxKind.NAME,
                name,
                _line,
                _pos
            );
        }

        private char Peek()
        {
            if (_text.Length > _index)
            {
                return _text[_index];
            }

            return '\0';
        }

        private char Advance()
        {
            char c = '\0';

            if (_text.Length > _index)
            {
                c = _text[_index];
                _index += 1;
                _pos += 1;
            }

            return c;
        }

        private void SetResetIndex(int index)
        {
            _resetIndex = index;
        }

        private void Reset(int offset = 0)
        {
            _pos -= _index - _resetIndex + offset;
            if (_pos < 0)
            {
                // Be careful not to reset further than the same line, since the line number will now be incorrect.
                _pos = 0;
            }

            _index = _resetIndex + offset;
            _resetIndex = 0;
        }
    }
}
