using System;

namespace _7dtd_svmanager_fix_mvvm.Models.Scheduled;

public class ScheduledCommand : ICloneable
{
    public string Command { get; }

    public TimeSpan WaitTime { get; }

    public TimeSpan Interval { get; set; }

    public ScheduledCommand(string command, TimeSpan waitTime, TimeSpan interval)
    {
        Command = command;
        WaitTime = waitTime;
        Interval = interval;
    }

    public object Clone()
    {
        var command = new ScheduledCommand(Command, WaitTime, Interval);
        return command;
    }
}