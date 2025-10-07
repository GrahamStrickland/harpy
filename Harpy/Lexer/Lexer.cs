using System.Data;

namespace Harpy.Lexer
{
    /// <summary>
    /// Takes a string and splits it into a series of <c>Token</c>s. 
    /// Operators and punctuation are mapped to unique keywords.    
    /// </summary>
    internal class Lexer(string text)
    {
        private static readonly Dictionary<string, SyntaxType> _directives = new()
        {
             { "include", SyntaxType.INCLUDE_DIRECTIVE },
             { "define", SyntaxType.DEFINE_DIRECTIVE },
             { "ifdef", SyntaxType.IFDEF_DIRECTIVE },
             { "ifndef", SyntaxType.IFNDEF_DIRECTIVE },
             { "elif", SyntaxType.ELIF_DIRECTIVE },
             { "else", SyntaxType.ELSE_DIRECTIVE },
             { "endif", SyntaxType.ENDIF_DIRECTIVE },
             { "undef", SyntaxType.UNDEF_DIRECTIVE },
             { "pragma", SyntaxType.PRAGMA_DIRECTIVE },
             { "command", SyntaxType.COMMAND_DIRECTIVE },
             { "xcommand", SyntaxType.XCOMMAND_DIRECTIVE },
             { "translate", SyntaxType.TRANSLATE_DIRECTIVE },
             { "xtranslate", SyntaxType.XTRANSLATE_DIRECTIVE },
             { "error", SyntaxType.ERROR_DIRECTIVE },
             { "stdout", SyntaxType.STDOUT_DIRECTIVE }
        };
        private static readonly Dictionary<string, SyntaxType> _keywords = new()
        {
             { "function", SyntaxType.FUNCTION },
             { "procedure", SyntaxType.PROCEDURE },
             { "return", SyntaxType.RETURN },
             { "nil", SyntaxType.NIL },
             { "local", SyntaxType.LOCAL },
             { "static", SyntaxType.STATIC },
             { "iif", SyntaxType.IIF },
             { "if", SyntaxType.IF },
             { "else", SyntaxType.ELSE },
             { "elseif", SyntaxType.ELSEIF },
             { "end", SyntaxType.END },
             { "endif", SyntaxType.ENDIF },
             { "enderr", SyntaxType.ENDERR },
             { "while", SyntaxType.WHILE },
             { "endwhile", SyntaxType.ENDWHILE }
        };
        private static readonly Dictionary<string, SyntaxType> _compoundOperators = new()
        {
             { ":=", SyntaxType.ASSIGN },
             { "+=", SyntaxType.PLUSEQ },
             { "-=", SyntaxType.MINUSEQ },
             { "*=", SyntaxType.MULTEQ },
             { "/=", SyntaxType.DIVEQ },
             { "%=", SyntaxType.MODEQ },
             { "^=", SyntaxType.EXPEQ },
             { ".and.", SyntaxType.AND },
             { ".or.", SyntaxType.OR },
             { "==", SyntaxType.EQ1 },
             { "!=", SyntaxType.NE2 },
             { "<=", SyntaxType.LE },
             { ">=", SyntaxType.GE },
             { "=>", SyntaxType.HASHOP }
        };
        private static readonly Dictionary<string, SyntaxType> _simpleOperators = new()
        {
             { "(", SyntaxType.LEFT_PAREN },
             { ")", SyntaxType.RIGHT_PAREN },
             { "[", SyntaxType.LEFT_BRACKET },
             { "]", SyntaxType.RIGHT_BRACKET },
             { "{", SyntaxType.LEFT_BRACE },
             { "}", SyntaxType.RIGHT_BRACE },
             { "|", SyntaxType.PIPE },
             { ",", SyntaxType.COMMA },
             { "=", SyntaxType.EQ2 },
             { "#", SyntaxType.NE1 },
             { "<", SyntaxType.LT },
             { ">", SyntaxType.GT },
             { "$", SyntaxType.DOLLAR },
             { "+", SyntaxType.PLUS },
             { "-", SyntaxType.MINUS },
             { "*", SyntaxType.ASTERISK },
             { "/", SyntaxType.SLASH },
             { "%", SyntaxType.PERCENT },
             { "^", SyntaxType.CARET },
             { "!", SyntaxType.NOT },
             { "?", SyntaxType.QUESTION },
             { ":", SyntaxType.COLON },
             { "@", SyntaxType.AT }
        };
        private List<string> _names = [];

        private int _index = 0;
        private int _line = 1;
        private int _pos = 0;
        private int _resetIndex = 0;

        private readonly string _text = text;

        public IEnumerable<Token> GetTokens()
        {
            while (_index < _text.Length)
            {
                char c = Advance();

                switch (c)
                {
                    case '#':
                        {
                            yield return ReadPreprocessorDirective();
                            break;
                        }
                    case '/':
                        {
                            switch (Peek())
                            {
                                case '/':
                                    {
                                        yield return ReadLineComment();
                                        break;
                                    }
                                case '*':
                                    {
                                        yield return ReadBlockComment();
                                        break;
                                    }
                                default:
                                    {
                                        yield return new Token(
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
                                yield return ReadStringLiteralOrBrackets(c);
                            }
                            else
                            {
                                yield return new Token(SyntaxType.LEFT_BRACKET, c.ToString(), _line, _pos);
                            }

                            break;
                        }
                    case '"':
                    case '\'':
                        {
                            yield return ReadStringLiteralOrBrackets(c);
                            break;
                        }
                    case '.':
                        {
                            if (Char.IsLetter(Peek()))
                            {
                                yield return ReadBooleanLiteralOrLogical(c);
                                break;
                            }
                            else
                            {
                                yield return ReadNumericLiteral(c);
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
                            yield return ReadNumericLiteral(c);
                            break;
                        }
                    default:
                        {
                            Token? keyword = ReadKeyword(c);
                            string op = c.ToString() + Peek();

                            if (keyword != null)
                            {
                                yield return keyword;
                            }
                            else if (_compoundOperators.ContainsKey(op))
                            {
                                Advance();
                                yield return new Token(
                                    _compoundOperators[op],
                                    op,
                                    _line,
                                    _pos
                                );
                            }
                            else if (_simpleOperators.ContainsKey(c.ToString()))
                            {
                                yield return new Token(
                                    _simpleOperators[c.ToString()],
                                    c.ToString(),
                                    _line,
                                    _pos
                                );
                            }
                            else if (Char.IsLetterOrDigit(c))
                            {
                                yield return ReadName();
                            }
                            else if (c == '\n')
                            {
                                _line += 1;
                                _pos = 0;
                            }

                            // Ignore all other characters (whitespace, etc.)
                            break;
                        }
                }
            }

            // Once we've reached the end of the string, just return EOF tokens. We'll
            // just keep returning them as many times as we're asked so that the
            // parser's lookahead doesn't have to worry about running out of tokens.
            yield return new Token(SyntaxType.EOF, "\0", _line, _pos);
        }

        private Token ReadPreprocessorDirective()
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
                                return new Token(
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

            return new Token(SyntaxType.NE1, "#", _line, _pos);
        }

        private Token ReadLineComment()
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
                            return new Token(
                                SyntaxType.LINE_COMMENT,
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

        private Token ReadBlockComment()
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
                                        return new Token(
                                            SyntaxType.BLOCK_COMMENT,
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

        private Token ReadBooleanLiteralOrLogical(char c)
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
                        return new Token(
                            SyntaxType.BOOL_LITERAL,
                            literal,
                            _line,
                            _pos
                        );
                    }
                case ".or.":
                    {
                        return new Token(
                            SyntaxType.OR,
                            literal,
                            _line,
                            _pos
                        );
                    }
                case ".and.":
                    {
                        return new Token(
                            SyntaxType.AND,
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

        private Token ReadNumericLiteral(char c)
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
                            return new Token(
                                SyntaxType.NUM_LITERAL,
                                literal,
                                _line,
                                _pos
                            );
                        }
                    default:
                        {
                            return new Token(
                                SyntaxType.NUM_LITERAL,
                                literal,
                                _line,
                                _pos
                            );
                        }
                }
                c = Peek();
            }

            return new Token(
                SyntaxType.NUM_LITERAL,
                literal,
                _line,
                _pos
            );
        }

        private Token ReadStringLiteralOrBrackets(char c)
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
                return new Token(SyntaxType.LEFT_BRACKET, "[", _line, _pos);
            }

            return new Token(
                SyntaxType.STR_LITERAL,
                literal + Advance().ToString(),
                _line,
                _pos
            );
        }

        private Token? ReadKeyword(char c)
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
                return new Token(
                    _keywords[keyword.ToLower()],
                    keyword,
                    _line,
                    _pos
                );
            }

            Reset();
            return null;
        }

        private Token ReadName()
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

            return new Token(
                SyntaxType.NAME,
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
                // Be careful not to reset further than the same line, since the line number will now be incorrect
                _pos = 0;
            }

            _index = _resetIndex + offset;
            _resetIndex = 0;
        }
    }
}
