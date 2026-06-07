using System.Reflection.Metadata.Ecma335;
using System.Windows.Markup;
using Microsoft.Win32;

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
        if(data.Count == 0) return;
        string newFilePath = Path.Combine(_path, "sst_" + _nextIndex.ToString().PadLeft(3, '0') + ".sst");
        File.AppendAllLines(newFilePath, data);
        _nextIndex = _nextIndex + 1;
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
        SSTReader[] readers = files.Select(path => new SSTReader(path)).ToArray(); 
        PriorityQueue<(string key, string value, int sstIndex), string> minHeap = new PriorityQueue<(string key, string value, int sstIndex), string>();

        for(int i = 0; i < readers.Length; i++)
        {
            if(readers[i].HasNext())
            {
                (string key, string value) = readers[i].ReadNext();
                minHeap.Enqueue((key, value, i), key);
            }
        }

        while(minHeap.Count > 0)
        {
            (string key, string value, int sstIndex) = minHeap.Dequeue();

            if(!compactValues.ContainsKey(key))
            {
                compactValues.Add(key, key + "|" + value);
                if(value == "__tombstone__") tombstones.Add(key);
            }
            
            if(readers[sstIndex].HasNext())
            {
                (string newKey, string newValue) = readers[sstIndex].ReadNext();
                minHeap.Enqueue((newKey, newValue, sstIndex), newKey);
            }
        }

        foreach(string s in tombstones)
        {
            compactValues.Remove(s);
        }

        if(compactValues.Count > 0)
        {
            string newFilePath = Path.Combine(_path, "sst_" + "1".ToString().PadLeft(3, '0') + ".sst");
            string newFilePathTemp = newFilePath + ".tmp";
            foreach(string key in compactValues.Keys)
            {
                File.AppendAllText(newFilePathTemp, compactValues[key] + "\n");
            }
            foreach(string file in files)
            {
                File.Delete(file);
            }
            File.Move(newFilePathTemp, newFilePath);
            _nextIndex = 2;
        }
        else
        {
            foreach(string file in files)
            {
                File.Delete(file);
            }
        }
    }
}