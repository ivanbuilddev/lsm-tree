using System.Reflection.Metadata.Ecma335;

public static class Memtable
{
    private const string _tombstone = "__tombstone__";
    private const int _threshold = 10;
    private static SortedDictionary<string, string> _memtable = new SortedDictionary<string, string>();

    public static bool IsInThreshold() => _memtable.Count >= _threshold;

    public static void Flush() => _memtable.Clear();

    public static List<string> ToDataList()
    {
        List<string> data = new List<string>();
        foreach(string key in _memtable.Keys)
        {
            data.Add(key + "|" + _memtable[key]);
        }
        return data;
    }

    public static string Get(string key, bool verbose = true)
    { 
        try
        {
            if(_memtable.ContainsKey(key) && _memtable[key] != _tombstone) 
            {
                return _memtable[key];
            }
            else 
            {
                return "";
            }
        }
        catch { return ""; }
    }
    public static bool Set(string key, string value, bool verbose = true)
    {
        try
        {
            if(_memtable.ContainsKey(key))
            {
                _memtable[key] = value;
            }
            else
            {
                _memtable.Add(key, value);
            }
            return true;
        }
        catch { return false; }
    }
    public static bool Delete(string key, bool verbose = true)
    {
        try
        {
            if(!_memtable.ContainsKey(key))
            {
                return false;
            }
            else
            {
                _memtable[key] = _tombstone;
                return true;
            }
        }
        catch { return false; }
    }

    public static bool ExecuteWALCommand(string command, bool verbose = true)
    {
        if(string.IsNullOrEmpty(command)) return false;
        string[] _parts = command.Split('|', StringSplitOptions.RemoveEmptyEntries);
        switch(_parts[0].Trim())
        {
            case "set":
                if(_parts.Length < 3) return false;
                return Set(_parts[1], _parts[2], verbose);
            case "delete":
                if(_parts.Length < 2) return false;
                Delete(_parts[1], verbose);
                return true;
            default:
                return false;
        }
    }
}