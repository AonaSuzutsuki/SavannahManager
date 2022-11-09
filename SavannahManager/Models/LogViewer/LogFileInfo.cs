using System.IO;

namespace _7dtd_svmanager_fix_mvvm.Models.LogViewer;

public class LogFileInfo
{
    public string Name => Info.Name;
    public string FullPath => Info.FullName;
    public FileInfo Info { get; }

    private string _cache;

    public StringReader GetStringReader()
    {
        if (_cache != null)
            return new StringReader(_cache);

        _cache = File.ReadAllText(FullPath);
        return new StringReader(_cache);
    }

    public void ClearCache()
    {
        _cache = null;
    }

    public LogFileInfo(string path)
    {
        Info = new FileInfo(path);
    }
}