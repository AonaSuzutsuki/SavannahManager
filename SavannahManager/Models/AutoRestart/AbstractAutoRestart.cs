using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using _7dtd_svmanager_fix_mvvm.Models.Interfaces;

namespace _7dtd_svmanager_fix_mvvm.Models.AutoRestart;

public abstract class AbstractAutoRestart : IDisposable
{
    #region Fields

    protected bool IsRequestStop;
    protected readonly MainWindowServerStart Model;

    protected readonly TimeSpan BaseTime;
    protected DateTime ThresholdTime;
    protected DateTime MessageDateTime;

    private readonly bool _isAutoRestartSendMessage;
    private readonly int _autoRestartSendingMessageStartTime;
    private readonly int _autoRestartSendingMessageStartTimeMode;
    private readonly int _autoRestartSendingMessageIntervalTime;
    private readonly int _autoRestartSendingMessageIntervalTimeMode;

    #endregion

    #region Properties

    public bool IsRestarting { get; protected set; }

    #endregion

    #region Events

    protected readonly Subject<AutoRestartWaitingTimeEventArgs> TimeProgressSubject = new();
    public IObservable<AutoRestartWaitingTimeEventArgs> TimeProgress => TimeProgressSubject;

    protected readonly Subject<TimeSpan> FewRemainingSubject = new();
    public IObservable<TimeSpan> FewRemaining => FewRemainingSubject;

    #endregion

    protected AbstractAutoRestart(MainWindowServerStart model)
    {
        Model = model;
        var setting = model.Model.Setting;

        BaseTime = setting.IntervalTimeMode switch
        {
            0 => new TimeSpan(0, 0, setting.IntervalTime),
            1 => new TimeSpan(0, setting.IntervalTime, 0),
            _ => new TimeSpan(setting.IntervalTime, 0, 0)
        };
        ThresholdTime = CalculateThresholdTime(BaseTime);

        _isAutoRestartSendMessage = setting.IsAutoRestartSendMessage;
        _autoRestartSendingMessageStartTime = setting.AutoRestartSendingMessageStartTime;
        _autoRestartSendingMessageStartTimeMode = setting.AutoRestartSendingMessageStartTimeMode;
        _autoRestartSendingMessageIntervalTime = setting.AutoRestartSendingMessageIntervalTime;
        _autoRestartSendingMessageIntervalTimeMode = setting.AutoRestartSendingMessageIntervalTimeMode;
    }

    public void Start()
    {
        Task.Factory.StartNew(async () =>
        {
            while (!IsRequestStop)
            {
                await InnerStart();

                await Task.Delay(500);
            }

            TimeProgressSubject.OnCompleted();
            FewRemainingSubject.OnCompleted();
        });
    }

    protected virtual async Task InnerStart()
    {
        while (!IsRequestStop)
        {
            // Stop server
            if (StopServer())
                break;

            await Task.Delay(500);
        }

        while (!IsRequestStop)
        {
            if (AfterStopTelnet())
                break;

            await Task.Delay(500);
        }

        while (!IsRequestStop)
        {
            // Restarting
            if (CanRestart())
            {
                if (!Model.IsSsh)
                {
                    if (!await Model.Model.ServerStart())
                    {
                        IsRequestStop = true;
                        return;
                    }
                }
                else
                {
                    if (!await Model.Model.ServerStartWithSsh())
                    {
                        IsRequestStop = true;
                        return;
                    }
                }
                IsRestarting = false;
                ThresholdTime = CalculateThresholdTime(BaseTime);

                AfterStartSever();

                break;
            }

            WaitingStartServer();

            await Task.Delay(500);
        }

        IsRestarting = false;
    }

    protected virtual bool StopServer()
    {
        if (DateTime.Now >= ThresholdTime)
        {
            IsRestarting = true;
            Model.Model.ServerStop();
            return true;
        }

        // Waiting to stop server
        TimeProgressSubject.OnNext(new AutoRestartWaitingTimeEventArgs(AutoRestartWaitingTimeEventArgs.WaitingType.RestartWait, ThresholdTime - DateTime.Now));

        if (CanSendMessage())
        {
            FewRemainingSubject.OnNext(ThresholdTime - DateTime.Now);
        }

        return false;
    }

    protected abstract bool AfterStopTelnet();

    protected abstract bool CanRestart();

    protected virtual void AfterStartSever() {}

    protected virtual void WaitingStartServer() {}

    protected bool CanSendMessage()
    {
        if (!_isAutoRestartSendMessage)
            return false;

        var startTimeSpan = _autoRestartSendingMessageStartTimeMode switch
        {
            0 => new TimeSpan(0, 0, _autoRestartSendingMessageStartTime),
            _ => new TimeSpan(0, _autoRestartSendingMessageStartTime, 0)
        };
        var intervalTimeSpan = _autoRestartSendingMessageIntervalTimeMode switch
        {
            0 => new TimeSpan(0, 0, _autoRestartSendingMessageIntervalTime),
            _ => new TimeSpan(0, _autoRestartSendingMessageIntervalTime, 0)
        };

        if (ThresholdTime - DateTime.Now <= startTimeSpan)
        {
            if (MessageDateTime == DateTime.MinValue)
            {
                MessageDateTime = DateTime.Now;
            }

            if (DateTime.Now - MessageDateTime >= intervalTimeSpan)
            {
                MessageDateTime = DateTime.Now;
                return true;
            }
        }

        return false;
    }

    public void Dispose()
    {
        IsRequestStop = true;
    }

    protected static DateTime CalculateThresholdTime(TimeSpan baseTime)
    {
        return DateTime.Now + baseTime;
    }
}