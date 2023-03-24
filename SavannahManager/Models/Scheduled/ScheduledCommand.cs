using System;
using CommonExtensionLib.Extensions;
using Newtonsoft.Json;

namespace _7dtd_svmanager_fix_mvvm.Models.Scheduled;

public class ScheduledCommand : ICloneable
{
    public string Command { get; }

    [JsonIgnore]
    public TimeSpan WaitTime { get; }

    [JsonIgnore]
    public TimeSpan Interval { get; set; }

    [JsonProperty("WaitTime")]
    public int WaitTimeValue { get; set; }
    [JsonProperty("Interval")]
    public int IntervalValue { get; set; }
    public int WaitTimeMode { get; set; }
    public int IntervalTimeMode { get; set; }

    public ScheduledCommand(string command, int waitTime, int interval, int waitTimeMode, int intervalTimeMode)
    {
        Command = command;
        WaitTimeValue = waitTime;
        IntervalValue = interval;
        WaitTimeMode = waitTimeMode;
        IntervalTimeMode = intervalTimeMode;

        var time = ConvertTimes(waitTime, interval, waitTimeMode, intervalTimeMode);

        WaitTime = time.waitTime;
        Interval = time.interval;
    }

    private static (TimeSpan interval, TimeSpan waitTime) ConvertTimes(int waitTimeText, int intervalText, int waitTimeMode, int intervalTimeMode)
    {
        var intervalSpan = intervalTimeMode switch
        {
            0 => new TimeSpan(0, 0, intervalText),
            1 => new TimeSpan(0, intervalText, 0),
            _ => new TimeSpan(intervalText, 0, 0)
        };

        var waitTimeSpan = waitTimeMode switch
        {
            0 => new TimeSpan(0, 0, waitTimeText),
            1 => new TimeSpan(0, waitTimeText, 0),
            _ => new TimeSpan(waitTimeText, 0, 0)
        };

        return (intervalSpan, waitTimeSpan);
    }

    public object Clone()
    {
        var command = new ScheduledCommand(Command, WaitTimeValue, IntervalValue, WaitTimeMode, IntervalTimeMode);
        return command;
    }
}