using _7dtd_svmanager_fix_mvvm.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7dtd_svmanager_fix_mvvm.Models.AutoRestart;

public class AutoRestartCoolTime : AbstractAutoRestart
{
    #region Fields

    private readonly TimeSpan _rebootBaseTime;
    private DateTime _rebootThresholdTime;

    #endregion

    public AutoRestartCoolTime(MainWindowServerStart model) : base(model)
    {
        var setting = model.Model.Setting;

        _rebootBaseTime = setting.RebootIntervalTimeMode switch
        {
            0 => new TimeSpan(0, 0, setting.RebootIntervalTime),
            1 => new TimeSpan(0, setting.RebootIntervalTime, 0),
            _ => new TimeSpan(setting.RebootIntervalTime, 0, 0)
        };
        _rebootThresholdTime = DateTime.MinValue;
    }

    protected override bool AfterStopTelnet()
    {
        if (base.AfterStopTelnet() && !Model.Model.IsConnected && _rebootThresholdTime == DateTime.MinValue)
        {
            _rebootThresholdTime = CalculateThresholdTime(_rebootBaseTime);
            return true;
        }

        return false;
    }

    protected override void AfterStartSever()
    {
        base.AfterStartSever();

        _rebootThresholdTime = DateTime.MinValue;
    }

    protected override void WaitingStartServer()
    {
        base.WaitingStartServer();

        // Waiting for restart cool time
        if (DateTime.Now < _rebootThresholdTime)
        {
            TimeProgressSubject.OnNext(new AutoRestartWaitingTimeEventArgs(AutoRestartWaitingTimeEventArgs.WaitingType.RebootCoolTime, _rebootThresholdTime - DateTime.Now));
        }
    }

    protected override bool CanRestart()
    {
        if (DateTime.Now >= _rebootThresholdTime)
            return !Model.Model.IsConnected;

        return false;
    }
}