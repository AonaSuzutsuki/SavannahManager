using System.IO;
using System.Linq;
using System.Text;
using Prism.Mvvm;

namespace _7dtd_svmanager_fix_mvvm.Models.LogViewer;

public class LogFileInfo : BindableBase
{
    public static readonly string[] EncodingNames = new[]
    {
        "UTF-8",
        "Shift-JIS"
    };

    private string _encodingName = EncodingNames.First();

    public string Name => Info.Name;
    public string FullPath => Info.FullName;
    public FileInfo Info { get; }

    public Encoding ReadEncoding { get; private set; } = Encoding.UTF8;

    public string EncodingName
    {
        get => _encodingName;
        set
        {
            SetProperty(ref _encodingName, value);
            ReadEncoding = Encoding.GetEncoding(value);
            ClearCache();
        }
    }

    private string _cache;

    public StringReader GetStringReader()
    {
        if (_cache != null)
            return new StringReader(_cache);

        _cache = File.ReadAllText(FullPath, ReadEncoding);
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