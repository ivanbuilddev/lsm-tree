public static class WriteAheadLog
{
    private static string _path = "";
    private const string _WALFileName = "WAL.txt";
    public static void InitLog(string folderConnection)
    {
        _path = Path.Combine(AppContext.BaseDirectory, folderConnection, _WALFileName);
        string? directory = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if(!File.Exists(_path))
        {
            File.Create(_path).Dispose();
        }
    }

    public static string[] LoadWAL()
    {
        return File.ReadAllLines(_path);
    }

    public static void WriteToWAL(string command)
    {
        File.AppendAllText(_path, command + "\n");
    }

    public static void Flush()
    {
        File.WriteAllText(_path, string.Empty);
    }
}