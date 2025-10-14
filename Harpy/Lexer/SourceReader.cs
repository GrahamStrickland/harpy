using System.Collections;

namespace Harpy.Lexer;

/// <summary>
///     An intermediary between the <c>Parser</c> and <c>Lexer</c>. Handles the <c>IEnumerable</c>
///     for the <c>Parser</c>, with options for checking if a token matches, consuming a token, putting back a token,
///     and look-ahead with optional distance.
/// </summary>
internal class SourceReader(Lexer lexer) : IEnumerable<HarbourSyntaxToken>, IEnumerator<HarbourSyntaxToken>
{
    private readonly Lexer _lexer = lexer;
    private HarbourSyntaxToken? _current;
    private LinkedList<HarbourSyntaxToken> _read = [];
    private Queue<HarbourSyntaxToken> _resetBuffer = [];
    private bool _endOfFile = false;

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
        
        if (_read.Count == 0)
            // TODO: Check this for efficiency, there may be a better way to implement it.
            foreach (var t in _lexer)
                _read.AddLast(t);

        var token = _read.First();
        _read.RemoveFirst();
        _resetBuffer.Enqueue(token);
        _current = token;
        
        if (token.Kind != HarbourSyntaxKind.EOF) return _current != null;
        
        _endOfFile = true;
        return true;
    }

    public void Reset()
    {
        _read = [];
        _resetBuffer = [];
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
        var token = _read.Count == 0 ? LookAhead(0) : _read.First();

        return token.Kind == expected;
    }

    public HarbourSyntaxToken LookAhead(int distance)
    {
        if (_read.Count > distance) return _read.ElementAt(distance);
        // Read in as many as needed.
        foreach (var token in this)
        {
            if (_read.Count > distance) break;
            _read.AddLast(token);
        }

        return _read.ElementAt(distance);
    }
}