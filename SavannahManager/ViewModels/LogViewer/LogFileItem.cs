using System;
using _7dtd_svmanager_fix_mvvm.Models.LogViewer;
using Prism.Mvvm;

namespace _7dtd_svmanager_fix_mvvm.ViewModels.LogViewer;

public class LogFileItem : BindableBase
{
    private LogFileInfo _info;
    private string _encodingName;

    public string Title { get; set; }
    public long FileSize { get; set; }
    public string FileSizeText { get; set; }

    public string EncodingName => _info.EncodingName;

    public LogFileItem(LogFileInfo info)
    {
        Title = info.Name;
        FileSize = info.Info.Length;
        _info = info;

        if (FileSize < 1024)
            FileSizeText = $"{FileSize}Bytes";
        else if (FileSize < 1024 * 1024)
            FileSizeText = $"{Math.Floor((double)FileSize / 1024)}KB";
        else if (FileSize < 1024 * 1024 * 1024)
            FileSizeText = $"{Math.Floor((double)FileSize / (1024 * 1024))}MB";
    }
}