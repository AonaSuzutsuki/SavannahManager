using System;

namespace _7dtd_svmanager_fix_mvvm.Models.AutoRestart;

public class AutoRestartWaitingTimeEventArgs : EventArgs
{
    public enum WaitingType
    {
        RestartWait,
        RebootCoolTime
    }

    public WaitingType EventType { get; set; }

    public TimeSpan WaitingTime { get; }

    public AutoRestartWaitingTimeEventArgs(WaitingType eventType, TimeSpan waitingTime)
    {
        EventType = eventType;
        WaitingTime = waitingTime;
    }
}