using System;
using _7dtd_svmanager_fix_mvvm.Models.LogViewer;

namespace _7dtd_svmanager_fix_mvvm.ViewModels.LogViewer;

public class LogFileItem
{
    public string Title { get; set; }
    public long FileSize { get; set; }
    public string FileSizeText { get; set; }

    public LogFileItem(LogFileInfo info)
    {
        Title = info.Name;
        FileSize = info.Info.Length;

        if (FileSize < 1024)
            FileSizeText = $"{FileSize}Bytes";
        else if (FileSize < 1024 * 1024)
            FileSizeText = $"{Math.Floor((double)FileSize / 1024)}KB";
        else if (FileSize < 1024 * 1024 * 1024)
            FileSizeText = $"{Math.Floor((double)FileSize / (1024 * 1024))}MB";
    }
}