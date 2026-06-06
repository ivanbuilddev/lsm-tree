public static class SSTManager
{
    private static string _path = "";
    private static int _nextIndex = 0;

    public static void InitSST(string folderConnection)
    {
        _path  = Path.Combine(AppContext.BaseDirectory, folderConnection, "SSTs");
        Directory.CreateDirectory(_path);

        string[] files = GetFiles();
        int maxIndex = 0;
        foreach(string s in files)
        {
            int index = Int32.Parse(s.Split('_')[1].Split('.')[0]);
            if(maxIndex < index) maxIndex = index;
        }
        _nextIndex = maxIndex + 1;
    }

    public static void CopyData(List<string> data)
    {
        string newFilePath = Path.Combine(_path, "sst_" + _nextIndex.ToString().PadLeft(3, '0') + ".sst");
        File.AppendAllLines(newFilePath, data);
    }

    private static string[] GetFiles() => Directory.GetFiles(_path, "*.sst");
    public static string Get(string key)
    {
        List<string> files = GetFiles().OrderByDescending(f => f).ToList();
        foreach(string file in files)
        {
            string[] lines = File.ReadAllLines(file);
            foreach(string line in lines)
            {
                string[] parts = line.Split('|');
                if(parts[0] == key) return parts[1];
            }
        }
        return "";
    }
    public static void Compact()
    {
        Dictionary<string, string> compactValues = new Dictionary<string, string>();
        List<string> files = GetFiles().OrderByDescending(f => f).ToList();
        List<string> tombstones = new List<string>();
        foreach(string file in files)
        {
            foreach(string line in File.ReadAllLines(file))
            {
                string[] parts = line.Split('|');
                if(!compactValues.ContainsKey(parts[0]))
                {
                    compactValues.Add(parts[0], parts[0] + "|" + parts[1]);
                    if(parts[1] == "__tombstone__")
                        tombstones.Add(parts[0]);
                }
            }
        }

        foreach(string s in tombstones)
        {
            compactValues.Remove(s);
        }

        foreach(string key in compactValues.Keys)
        {
            string newFilePath = Path.Combine(_path, "sst_" + _nextIndex.ToString().PadLeft(3, '0') + ".sst");
            string newFilePathTemp = newFilePath + ".tmp";
            File.AppendAllText(newFilePath, compactValues[key] + "\n");
            foreach(string file in files)
            {
                File.Delete(file);
            }
            File.Move(newFilePathTemp, newFilePath);
        }
    }
}