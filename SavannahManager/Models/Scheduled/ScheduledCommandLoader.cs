using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace _7dtd_svmanager_fix_mvvm.Models.Scheduled;

public class ScheduledCommandLoader
{
    private static readonly string FileName = ConstantValues.AppDirectoryPath + @"\Scheduled\Commands.dat";

    private static readonly SemaphoreSlim Semaphore = new(1);

    public List<ScheduledCommand> Commands { get; private set; } = new();

    public void AddCommand(string command, TimeSpan waitTime, TimeSpan interval)
    {
    }

    public void AddCommand(ScheduledCommand command) => Commands.Add(command);

    public void EditCommand(int index, ScheduledCommand command)
    {
        if (index < 0 || index >= Commands.Count)
            return;

        Commands[index] = command;
    }

    public bool RemoveCommand(int index)
    {
        if (index < 0 || !Commands.Any())
            return false;

        Commands.RemoveAt(index);
        return true;
    }

    public void RemoveCommand(string command)
    {
        var item = Commands.FirstOrDefault(x => x.Command == command);
        if (item == null)
            return;

        Commands.Remove(item);
    }

    public async Task LoadFromFileAsync()
    {
        Commands = await InternalLoad();
    }

    public async Task SaveToFileAsync()
    {
        await InternalSave(Commands);
    }

    private static async Task<List<ScheduledCommand>> InternalLoad()
    {
        try
        {
            await Semaphore.WaitAsync();

            var fileInfo = new FileInfo(FileName);
            if (!fileInfo.Exists)
                return new List<ScheduledCommand>(0);

            var json = await File.ReadAllTextAsync(fileInfo.FullName);
            var list = JsonConvert.DeserializeObject<List<ScheduledCommand>>(json);
            return list;
        }
        finally
        {
            Semaphore.Release();
        }
    }

    private static async Task InternalSave(object obj)
    {

        try
        {
            await Semaphore.WaitAsync();

            var fileInfo = new FileInfo(FileName);
            var di = fileInfo.Directory;
            if (di == null)
                return;

            if (!di.Exists)
                di.Create();

            var json = JsonConvert.SerializeObject(obj);
            await using var stream = new FileStream(fileInfo.FullName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            await using var sw = new StreamWriter(stream);
            await sw.WriteAsync(json);
            await sw.FlushAsync();
        }
        finally
        {
            Semaphore.Release();
        }
    }
}