public static class Database
{
    public static void Init(string connection)
    {
        WriteAheadLog.InitLog(connection);
        SSTManager.InitSST(connection);
    }

    public static void Load()
    {
        string[] commands = WriteAheadLog.LoadWAL();
        foreach (string s in commands)
        {
            Memtable.ExecuteWALCommand(s);  
        }
    }

    public static void Get(string key) 
    {
        string value = Memtable.Get(key);
        if (string.IsNullOrEmpty(value))
        {
            value = SSTManager.Get(key);   
        }
        if(string.IsNullOrEmpty(value) || value == "__tombstone__")
        {
            Console.WriteLine("(Not found)");
        }
        else { Console.WriteLine(value); }
    }
    public static void Set(string key, string value)
    {
        WriteAheadLog.WriteToWAL($"set|{key}|{value}");
        Memtable.Set(key, value);
        if (Memtable.IsInThreshold())
        {
            WriteToSTT();
        }
    }

    public static void WriteToSTT()
    {
        SSTManager.CopyData(Memtable.ToDataList());
        Memtable.Flush();
        WriteAheadLog.Flush();
    }

    public static void Delete(string key)
    {
        WriteAheadLog.WriteToWAL($"delete|{key}");
        Memtable.Delete(key);
    }

    public static void Compaction()
    {
        WriteToSTT();
        SSTManager.Compact();
    }
}