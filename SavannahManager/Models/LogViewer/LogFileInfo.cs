using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using _7dtd_svmanager_fix_mvvm.Models.LogViewer.Versions.a20;
using Prism.Mvvm;

namespace _7dtd_svmanager_fix_mvvm.Models.LogViewer;

public class LogFileInfo : BindableBase
{
    public static readonly string[] EncodingNames = new[]
    {
        "UTF-8",
        "Shift-JIS"
    };


    public static readonly Dictionary<string, ILogAnalyzer> AnalyzePlans = new()
    {
        { A20LogAnalyzer.AnalyzerName, new A20LogAnalyzer() },
        { A21LogAnalyzer.AnalyzerName, new A21LogAnalyzer() },
        { ServerFixesAnalyzer.AnalyzerName, new ServerFixesAnalyzer() }
    };

    private string _encodingName = EncodingNames.First();

    public string Date { get; set; }
    public string Name => Info.Name;
    public string FullPath => Info.FullName;
    public FileInfo Info { get; }

    public Encoding ReadEncoding { get; private set; } = Encoding.UTF8;

    public ILogAnalyzer LogAnalyzer { get; private set; } = new A20LogAnalyzer();

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

    public void ChangeLogAnalyzer(string name)
    {
        if (!AnalyzePlans.ContainsKey(name))
            return;

        LogAnalyzer = AnalyzePlans[name];
    }

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

        var fileName = Info.Name;

        const string pattern = "(?<year>[0-9]{4})-(?<month>[0-9]{2})-(?<day>[0-9]{2})- (?<hour>[0-9]{2})-(?<minute>[0-9]{2})-(?<second>[0-9]{2})";
        var regex = new Regex(pattern);
        var match = regex.Match(fileName);
        if (match.Success)
        {
            var year = match.Groups["year"].Value;
            var month = match.Groups["month"].Value;
            var day = match.Groups["day"].Value;

            Date = $"{year}-{month}-{day}";
        }
        else
        {
            Date = Info.CreationTime.ToString("yyyy-MM-dd");
        }
    }
}