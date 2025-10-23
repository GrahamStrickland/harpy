using System.Collections;

namespace Harpy.Lexer;

/// <summary>
///     An intermediary between the <see cref="Parser" /> and <see cref="Lexer" />. Handles the <see cref="IEnumerable" />
///     for the <see cref="Parser" />, with options for checking if a token matches, consuming a token, putting back a
///     token,
///     and look-ahead with optional distance.
/// </summary>
public class SourceReader : IEnumerable<HarbourSyntaxToken>, IEnumerator<HarbourSyntaxToken>
{
    private readonly Lexer _lexer;
    private HarbourSyntaxToken? _current;
    private bool _endOfFile;
    private LinkedList<HarbourSyntaxToken> _tokens = [];
    private Stack<HarbourSyntaxToken> _undoBuffer = [];

    /// <summary>
    ///     An intermediary between the <see cref="Parser" /> and <see cref="Lexer" />. Handles the <see cref="IEnumerable" />
    ///     for the <see cref="Parser" />, with options for checking if a token matches, consuming a token, putting back a
    ///     token,
    ///     and look-ahead with optional distance.
    /// </summary>
    public SourceReader(Lexer lexer)
    {
        _lexer = lexer;
        foreach (var t in _lexer)
            _tokens.AddLast(t);
    }

    public IEnumerator<HarbourSyntaxToken> GetEnumerator()
    {
        while (true)
        {
            if (!MoveNext() || _current == null) continue;
            yield return _current;

            if (_current.Kind == HarbourSyntaxKind.EOF)
                break;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool MoveNext()
    {
        if (_endOfFile)
            return false;

        var token = _tokens.First();
        _tokens.RemoveFirst();
        _undoBuffer.Push(token);
        _current = token;

        if (token.Kind != HarbourSyntaxKind.EOF) return _current != null;

        _endOfFile = true;
        return true;
    }

    public void Reset()
    {
        _tokens = [];
        _undoBuffer = [];
        _current = null;
        using var enumerator = _lexer.GetEnumerator();
        enumerator.Reset();
    }

    object IEnumerator.Current => _current!;
    HarbourSyntaxToken IEnumerator<HarbourSyntaxToken>.Current => _current!;

    public void Dispose()
    {
    }

    public bool Match(HarbourSyntaxKind expected)
    {
        return _tokens.First().Kind == expected;
    }

    public HarbourSyntaxToken Consume(HarbourSyntaxKind? expected = null)
    {
        var token = _tokens.First();

        if (expected != null && token.Kind != expected)
            throw new InvalidSyntaxException(
                $"Expected token kind '{expected}' and found '{token.Kind}' with text '{token.Text}' at line {token.Line}, column {token.Start}.");

        _tokens.RemoveFirst();
        _undoBuffer.Push(token);

        return token;
    }

    public HarbourSyntaxToken LookAhead(int distance = 0)
    {
        return _tokens.ElementAt(distance);
    }

    public void PutBack(HarbourSyntaxToken token)
    {
        _tokens.AddFirst(token);
    }

    public void SetUndoPoint()
    {
        _undoBuffer.Clear();
    }

    public void Undo()
    {
        while (_undoBuffer.Count > 0) _tokens.AddFirst(_undoBuffer.Pop());

        _undoBuffer.Clear();
    }
}