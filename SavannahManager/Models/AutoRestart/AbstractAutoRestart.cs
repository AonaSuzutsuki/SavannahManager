using System;
using System.Diagnostics;
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
    private readonly TimeSpan _startTimeSpan;
    private readonly TimeSpan _intervalTimeSpan;
    private readonly bool _isAutoRestartRunScript;
    private readonly string _runningScript;
    private bool _isRanScript;

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
        _isAutoRestartRunScript = setting.IsAutoRestartRunScriptEnabled;
        _runningScript = setting.AutoRestartRunningScript;
        
        _startTimeSpan = _autoRestartSendingMessageStartTimeMode switch
        {
            0 => new TimeSpan(0, 0, _autoRestartSendingMessageStartTime),
            _ => new TimeSpan(0, _autoRestartSendingMessageStartTime, 0)
        };
        _intervalTimeSpan = _autoRestartSendingMessageIntervalTimeMode switch
        {
            0 => new TimeSpan(0, 0, _autoRestartSendingMessageIntervalTime),
            _ => new TimeSpan(0, _autoRestartSendingMessageIntervalTime, 0)
        };
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
        var now = DateTime.Now;
        if (now >= ThresholdTime)
        {
            IsRestarting = true;
            Model.Model.ServerStop();
            return true;
        }

        // Waiting to stop server
        TimeProgressSubject.OnNext(new AutoRestartWaitingTimeEventArgs(AutoRestartWaitingTimeEventArgs.WaitingType.RestartWait, ThresholdTime - now));

        if (CanSendMessage(now))
        {
            FewRemainingSubject.OnNext(ThresholdTime - now);
        }

        return false;
    }

    protected virtual bool AfterStopTelnet()
    {
        if (!_isRanScript && _isAutoRestartRunScript)
        {
            ExecuteCommand(_runningScript);
            _isRanScript = true;
        }

        return true;
    }

    protected abstract bool CanRestart();

    protected virtual void AfterStartSever() {}

    protected virtual void WaitingStartServer() {}

    protected bool CanSendMessage(DateTime now)
    {
        if (!_isAutoRestartSendMessage)
            return false;

        if (ThresholdTime - now <= _startTimeSpan)
        {
            if (MessageDateTime == DateTime.MinValue)
            {
                MessageDateTime = now;
            }

            if (now - MessageDateTime >= _intervalTimeSpan)
            {
                MessageDateTime = now;
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

    protected static void ExecuteCommand(string command)
    {
        var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
        {
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true
        };

        var process = Process.Start(processInfo);

        if (process == null)
            return;

        process.OutputDataReceived += (sender, e) =>
            Debug.WriteLine("output>>" + e.Data);
        process.BeginOutputReadLine();

        process.ErrorDataReceived += (sender, e) =>
            Debug.WriteLine("error>>" + e.Data);
        process.BeginErrorReadLine();

        process.WaitForExit();

        Debug.WriteLine($"ExitCode: {process.ExitCode}");
        process.Close();
    }
}