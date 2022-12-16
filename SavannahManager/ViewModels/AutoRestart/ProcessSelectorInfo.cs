using System;
using System.Diagnostics;
using _7dtd_svmanager_fix_mvvm.Models.Comparer;
using SavannahManagerStyleLib.Attributes;

namespace _7dtd_svmanager_fix_mvvm.ViewModels.AutoRestart;

public class ProcessSelectorInfo
{
    public int ProcessId { get; }
    public string Name { get; }

    [Sorting(AscComparer = typeof(MemoryAscComparer), DescComparer = typeof(MemoryDescComparer))]
    public string MemoryText { get; }

    public long Memory { get; }

    public ProcessSelectorInfo(Process process)
    {
        ProcessId = process.Id;
        Name = process.ProcessName;
        var memory = process.PrivateMemorySize64;
        Memory = memory;

        MemoryText = memory switch
        {
            >= 1024L * 1024L => $"{Math.Floor((double)memory / 1024L):##,###}KB",
            >= 1024L => $"{memory}Bytes",
            _ => $"{memory}Bytes"
        };
    }
}