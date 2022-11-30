using System;
using _7dtd_svmanager_fix_mvvm.Models.Comparer;
using _7dtd_svmanager_fix_mvvm.Models.LogViewer;
using Prism.Mvvm;
using SavannahManagerStyleLib.Attributes;

namespace _7dtd_svmanager_fix_mvvm.ViewModels.LogViewer;

public class LogFileItem : BindableBase
{
    public LogFileInfo Info { get; }

    public string Date { get; set; }
    public string Title { get; set; }
    public long FileSize { get; set; }

    [Sorting(AscComparer = typeof(FileSizeAscComparer), DescComparer = typeof(FileSizeDescComparer))]
    public string FileSizeText { get; set; }

    public string EncodingName => Info.EncodingName;

    public LogFileItem(LogFileInfo info)
    {
        Title = info.Name;
        FileSize = info.Info.Length;
        Info = info;
        Date = info.Date;

        if (FileSize < 1024)
            FileSizeText = $"{FileSize}Bytes";
        else if (FileSize < 1024 * 1024)
            FileSizeText = $"{Math.Floor((double)FileSize / 1024)}KB";
        else if (FileSize < 1024 * 1024 * 1024)
            FileSizeText = $"{Math.Floor((double)FileSize / (1024 * 1024))}MB";
    }
}