using System.Collections;
using System.Data;

namespace Harpy.Lexer;

/// <summary>
///     Takes a string and splits it into a series of <c>Token</c>s.
///     Operators and punctuation are mapped to unique keywords.
/// </summary>
internal class Lexer(string text) : IEnumerable<HarbourSyntaxToken>, IEnumerator<HarbourSyntaxToken>
{
    private static readonly Dictionary<string, HarbourSyntaxKind> Directives = new()
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

    private static readonly Dictionary<string, HarbourSyntaxKind> Keywords = new()
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

    private static readonly Dictionary<string, HarbourSyntaxKind> CompoundOperators = new()
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

    private static readonly Dictionary<string, HarbourSyntaxKind> SimpleOperators = new()
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

    private readonly List<string> _names = [];

    private readonly string _text = text;
    private HarbourSyntaxToken? _current;

    private int _index;
    private int _line = 1;
    private int _pos;
    private int _resetIndex;

    public IEnumerator<HarbourSyntaxToken> GetEnumerator()
    {
        List<HarbourSyntaxTrivia> leadingTrivia = [];
        List<HarbourSyntaxTrivia> trailingTrivia = [];
        HarbourSyntaxToken? oldToken = null;
        HarbourSyntaxToken? newToken = null;
        var newLineEncountered = false;

        while (_index < _text.Length)
        {
            var c = Advance();

            switch (c)
            {
                case '#':
                {
                    if (newLineEncountered)
                        leadingTrivia.Add((HarbourSyntaxTrivia)ReadPreprocessorDirectiveOrNeOp());
                    else
                        trailingTrivia.Add((HarbourSyntaxTrivia)ReadPreprocessorDirectiveOrNeOp());

                    break;
                }
                case '/':
                {
                    switch (Peek())
                    {
                        case '/':
                        {
                            if (newLineEncountered)
                                leadingTrivia.Add(ReadLineComment());
                            else
                                trailingTrivia.Add(ReadLineComment());
                            break;
                        }
                        case '*':
                        {
                            if (newLineEncountered)
                                leadingTrivia.Add(ReadBlockComment());
                            else
                                trailingTrivia.Add(ReadBlockComment());
                            break;
                        }
                        default:
                        {
                            newToken = new HarbourSyntaxToken(
                                SimpleOperators[c.ToString()],
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
                    newToken = char.IsLetterOrDigit(Peek())
                        ? ReadStringLiteralOrBrackets(c)
                        : new HarbourSyntaxToken(HarbourSyntaxKind.LEFT_BRACKET, c.ToString(), _line, _pos);

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
                    newToken = char.IsLetter(Peek()) ? ReadBooleanLiteralOrLogical(c) : ReadNumericLiteral(c);
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
                    var keyword = ReadKeyword(c);
                    var op = c.ToString() + Peek();

                    if (keyword != null)
                    {
                        newToken = keyword;
                    }
                    else if (CompoundOperators.TryGetValue(op, out var @operator))
                    {
                        Advance();
                        newToken = new HarbourSyntaxToken(
                            @operator,
                            op,
                            _line,
                            _pos
                        );
                    }
                    else if (SimpleOperators.ContainsKey(c.ToString()))
                    {
                        newToken = new HarbourSyntaxToken(
                            SimpleOperators[c.ToString()],
                            c.ToString(),
                            _line,
                            _pos
                        );
                    }
                    else if (char.IsLetterOrDigit(c))
                    {
                        newToken = ReadName();
                    }
                    else if (char.IsWhiteSpace(c) || c == ';')
                    {
                        HarbourSyntaxKind type;
                        if (op == "\r\n")
                        {
                            Advance();
                            type = HarbourSyntaxKind.NEWLINE;
                        }
                        else
                        {
                            type = c switch
                            {
                                '\n' => HarbourSyntaxKind.NEWLINE,
                                '\r' => HarbourSyntaxKind.CARRIAGE_RETURN,
                                '\t' => HarbourSyntaxKind.TAB,
                                ' ' => HarbourSyntaxKind.SPACE,
                                ';' => HarbourSyntaxKind.LINE_CONTINUATION,
                                _ => throw new SyntaxErrorException($"Unknown character '{c}' encountered in source.")
                            };
                        }

                        if (newLineEncountered)
                            leadingTrivia.Add(
                                new HarbourSyntaxTrivia(
                                    type,
                                    c.ToString(),
                                    _line,
                                    _pos
                                )
                            );
                        else
                            trailingTrivia.Add(
                                new HarbourSyntaxTrivia(
                                    type,
                                    c.ToString(),
                                    _line,
                                    _pos
                                )
                            );

                        if (type is HarbourSyntaxKind.NEWLINE or HarbourSyntaxKind.CARRIAGE_RETURN)
                        {
                            _line += 1;
                            _pos = 0;
                            newLineEncountered = true;
                        }
                    }

                    break;
                }
            }

            if (newToken == null) continue; // Search for more trivia.
            newLineEncountered = false;
            newToken.LeadingTrivia = leadingTrivia;
            leadingTrivia = [];

            if (oldToken != null)
            {
                oldToken.TrailingTrivia = trailingTrivia;
                trailingTrivia = [];
                _current = oldToken;
                yield return oldToken;
            }

            oldToken = newToken;
            newToken = null;
        }

        if (oldToken != null)
        {
            oldToken.TrailingTrivia = trailingTrivia;
            _current = oldToken;
            yield return oldToken;
        }

        // Once we've reached the end of the string, just return EOF tokens. We'll
        // just keep returning them as many times as we're asked so that the
        // parser's lookahead doesn't have to worry about running out of tokens.
        leadingTrivia.AddRange(trailingTrivia);
        yield return new HarbourSyntaxToken(HarbourSyntaxKind.EOF, "\0", _line, _pos, leadingTrivia, []);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    object IEnumerator.Current => _current!;
    HarbourSyntaxToken IEnumerator<HarbourSyntaxToken>.Current => _current!;

    bool IEnumerator.MoveNext()
    {
        if (_index != _text.Length && _current is not { Kind: HarbourSyntaxKind.EOF }) return false;
        using var enumerator = GetEnumerator();
        _current = enumerator.Current;
        return true;
    }

    void IEnumerator.Reset()
    {
        _index = 0;
        _line = 1;
        _pos = 0;
        _resetIndex = 0;
        _current = null;
    }

    void IDisposable.Dispose()
    {
    }

    private HarbourSyntaxElement ReadPreprocessorDirectiveOrNeOp()
    {
        var startIndex = _index - 1;
        SetResetIndex(startIndex);
        var directive = "";
        char c;

        while (char.IsLetterOrDigit(c = Advance())) directive += c;

        if (Directives.ContainsKey(directive.ToLower()))
            while (true)
                switch (Peek())
                {
                    case '\n':
                    case '\r':
                    case '\0':
                    {
                        return new HarbourSyntaxTrivia(
                            Directives[directive.ToLower()],
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

        ResetIndex(1);

        return new HarbourSyntaxToken(HarbourSyntaxKind.NE1, "#", _line, _pos);
    }

    private HarbourSyntaxTrivia ReadLineComment()
    {
        var startIndex = _index - 1;

        Advance();

        while (true)
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

    private HarbourSyntaxTrivia ReadBlockComment()
    {
        var startIndex = _index - 1;
        var startLine = _line;

        while (true)
        {
            var c = Advance();

            switch (c)
            {
                case '*':
                {
                    var c1 = Advance();

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
        var literal = c.ToString();

        while ((c = Advance()) != '.' && c != '\0') literal += c;

        literal += '.';

        return literal.ToLower() switch
        {
            ".t." or ".f." => new HarbourSyntaxToken(HarbourSyntaxKind.BOOL_LITERAL, literal, _line, _pos),
            ".or." => new HarbourSyntaxToken(HarbourSyntaxKind.OR, literal, _line, _pos),
            ".and." => new HarbourSyntaxToken(HarbourSyntaxKind.AND, literal, _line, _pos),
            _ => throw new SyntaxErrorException($"Unable to read token '{literal}'.")
        };
    }

    private HarbourSyntaxToken ReadNumericLiteral(char c)
    {
        var literal = c.ToString();
        var dotFound = false;
        var hexNum = false;

        c = Peek();
        while (!char.IsWhiteSpace(c))
        {
            switch (char.ToLower(c))
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
                    break;
                }
                case 'x':
                {
                    if (literal != "0")
                        throw new SyntaxErrorException($"Unterminated hexadecimal literal '{literal + c}'.");
                    literal += c;
                    hexNum = true;
                    break;
                }
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                {
                    if (!hexNum) throw new SyntaxErrorException($"Invalid numeric literal '{literal + c}'.");
                    literal += c;
                    break;
                }
                case '.':
                {
                    if (dotFound)
                        throw new SyntaxErrorException($"Second decimal point found in literal '{literal + c}'.");
                    literal += c;
                    dotFound = true;
                    break;
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

            Advance();

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
        var startIndex = _index;
        SetResetIndex(startIndex);
        var literal = c.ToString();
        var endQuote = c == '[' ? ']' : c;

        while ((c = Peek()) != endQuote)
        {
            switch (c)
            {
                case '\0':
                {
                    if (literal.Length > 1 && literal.EndsWith(endQuote)) break;
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
        if (endQuote != ']' || !_names.Contains(literal[1..]))
            return new HarbourSyntaxToken(
                HarbourSyntaxKind.STR_LITERAL,
                literal + Advance(),
                _line,
                _pos
            );
        ResetIndex();
        return new HarbourSyntaxToken(HarbourSyntaxKind.LEFT_BRACKET, "[", _line, _pos);
    }

    private HarbourSyntaxToken? ReadKeyword(char c)
    {
        var keyword = c.ToString();
        SetResetIndex(_index);

        c = Peek();
        while (char.IsLetterOrDigit(c))
        {
            keyword += c;
            Advance();
            c = Peek();
        }

        if (Keywords.ContainsKey(keyword.ToLower()))
            return new HarbourSyntaxToken(
                Keywords[keyword.ToLower()],
                keyword,
                _line,
                _pos
            );

        ResetIndex();
        return null;
    }

    private HarbourSyntaxToken ReadName()
    {
        var startIndex = _index - 1;
        SetResetIndex(startIndex);

        while (Peek() != '\0')
        {
            if (!char.IsLetterOrDigit(_text[_index]) && _text[_index] != '_') break;

            Advance();
        }

        var name = _text[startIndex.._index];

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
        return _text.Length > _index ? _text[_index] : '\0';
    }

    private char Advance()
    {
        var c = '\0';

        if (_text.Length <= _index) return c;
        c = _text[_index];
        _index += 1;
        _pos += 1;

        return c;
    }

    private void SetResetIndex(int index)
    {
        _resetIndex = index;
    }

    private void ResetIndex(int offset = 0)
    {
        _pos -= _index - _resetIndex + offset;
        if (_pos < 0)
            // Be careful not to reset further than the same line, since the line number will now be incorrect.
            _pos = 0;

        _index = _resetIndex + offset;
        _resetIndex = 0;
    }
}