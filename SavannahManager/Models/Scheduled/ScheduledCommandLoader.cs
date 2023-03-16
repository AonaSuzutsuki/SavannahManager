using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace _7dtd_svmanager_fix_mvvm.Models.Scheduled;

public class ScheduledCommandLoader
{
    private static readonly string FileName = ConstantValues.AppDirectoryPath + @"\Scheduled\Commands.dat";

    public List<ScheduledCommand> Commands { get; private set; } = new();

    public void AddCommand(string command, TimeSpan waitTime, TimeSpan interval)
    {
        Commands.Add(new ScheduledCommand(command, waitTime, interval));
    }

    public void LoadFromFile()
    {
        var fileInfo = new FileInfo(FileName);
        if (!fileInfo.Exists)
            return;
            
        var json = File.ReadAllText(fileInfo.FullName);
        var list = JsonConvert.DeserializeObject<List<ScheduledCommand>>(json);
        Commands = list;
    }

    public void SaveToFile()
    {
        var fileInfo = new FileInfo(FileName);
        var di = fileInfo.Directory;
        if (di == null)
            return;

        if (!di.Exists)
            di.Create();

        var json = JsonConvert.SerializeObject(Commands);
        using var stream = new FileStream(fileInfo.FullName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        using var sw = new StreamWriter(stream);
        sw.Write(json);
        sw.Flush();
    }
}