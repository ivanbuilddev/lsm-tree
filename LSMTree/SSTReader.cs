public class SSTReader
{
    private StreamReader _reader;
    private (string key, string value)? _next;

    public SSTReader(string path)
    {
        _reader = new StreamReader(path);
        Advance();
    }

    public bool HasNext() => _next.HasValue;
    public (string key, string value) Peek() => _next!.Value;

    public (string key, string value) ReadNext()
    {
        var current = _next!.Value;
        Advance();
        return current;
    }

    private void Advance()
    {
        string? line = _reader.ReadLine();
        if(line == null)
        {
            _next = null;
            _reader.Close();
            return;
        }

        string[] parts = line.Split('|');
        _next = (parts[0], parts[1]);
    }
}