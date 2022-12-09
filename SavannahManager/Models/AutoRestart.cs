using _7dtd_svmanager_fix_mvvm.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace _7dtd_svmanager_fix_mvvm.Models
{
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

    public class AutoRestart : IDisposable
    {
        #region Fields

        private readonly TimeSpan _baseTime;
        private DateTime _thresholdTime;
        private DateTime _messageDateTime;

        private readonly TimeSpan _rebootBaseTime;
        private DateTime _rebootThresholdTime;

        private bool _isRequestStop;
        private readonly MainWindowServerStart _model;
        private readonly SettingLoader _setting; // ToDo: remove

        #endregion

        #region Properties

        public bool IsRestarting { get; private set; }

        public bool IsRebootingCoolTime { get; private set; }

        #endregion

        #region Events

        private readonly Subject<AutoRestartWaitingTimeEventArgs> _timeProgress = new();
        public IObservable<AutoRestartWaitingTimeEventArgs> TimeProgress => _timeProgress;

        private readonly Subject<TimeSpan> _fewRemaining = new();
        public IObservable<TimeSpan> FewRemaining => _fewRemaining;

        #endregion

        public AutoRestart(MainWindowServerStart model)
        {
            _model = model;
            _setting = model.Model.Setting;
            _baseTime = _setting.IntervalTimeMode switch
            {
                0 => new TimeSpan(0, 0, _setting.IntervalTime),
                1 => new TimeSpan(0, _setting.IntervalTime, 0),
                _ => new TimeSpan(_setting.IntervalTime, 0, 0)
            };
            _thresholdTime = CalculateThresholdTime(_baseTime);

            _rebootBaseTime = _setting.RebootIntervalTimeMode switch
            {
                0 => new TimeSpan(0, 0, _setting.RebootIntervalTime),
                1 => new TimeSpan(0, _setting.RebootIntervalTime, 0),
                _ => new TimeSpan(_setting.RebootIntervalTime, 0, 0)
            };
            _rebootThresholdTime = DateTime.MinValue;
            //_rebootThresholdTime = CalculateThresholdTime(_rebootBaseTime + _baseTime);
        }

        private static DateTime CalculateThresholdTime(TimeSpan baseTime)
        {
            return DateTime.Now + baseTime;
        }

        public void Start()
        {
            Task.Factory.StartNew(async () =>
            {
                _messageDateTime = DateTime.MinValue;
                var isStop = false;
                while (!_isRequestStop)
                {
                    if (_setting.RebootingWaitMode == 0)
                    {
                        // Stop server
                        if (!isStop && DateTime.Now >= _thresholdTime)
                        {
                            IsRestarting = true;
                            IsRebootingCoolTime = true;
                            _model.Model.ServerStop();
                            isStop = true;
                        }

                        if (isStop && !_model.Model.IsConnected && _rebootThresholdTime == DateTime.MinValue)
                        {
                            _rebootThresholdTime = CalculateThresholdTime(_rebootBaseTime);
                        }

                        // Restarting
                        if (isStop && CanRestart())
                        {
                            if (!_model.IsSsh)
                            {
                                if (!await _model.Model.ServerStart())
                                    return;
                            }
                            else
                            {
                                if (!await _model.Model.ServerStartWithSsh())
                                    return;
                            }
                            IsRestarting = false;
                            IsRebootingCoolTime = false;
                            isStop = false;
                            _thresholdTime = CalculateThresholdTime(_baseTime);
                            _rebootThresholdTime = DateTime.MinValue;
                        }

                        // Waiting to stop server
                        if (!isStop)
                        {
                            _timeProgress.OnNext(new AutoRestartWaitingTimeEventArgs(AutoRestartWaitingTimeEventArgs.WaitingType.RestartWait, _thresholdTime - DateTime.Now));

                            if (CanSendMessage())
                            {
                                _fewRemaining.OnNext(_thresholdTime - DateTime.Now);
                            }
                        }

                        // Waiting for restart cool time
                        if (isStop && DateTime.Now < _rebootThresholdTime)
                        {
                            _timeProgress.OnNext(new AutoRestartWaitingTimeEventArgs(AutoRestartWaitingTimeEventArgs.WaitingType.RebootCoolTime, _rebootThresholdTime - DateTime.Now));
                        }
                    }
                    else if (_setting.RebootingWaitMode == 1)
                    {
                        // ToDo
                    }

                    await Task.Delay(500);
                }

                IsRestarting = false;
                _timeProgress.OnCompleted();
            });
        }

        private bool CanRestart()
        {
            if (_setting.RebootingWaitMode == 0)
            {
                if (_rebootThresholdTime == DateTime.MinValue)
                    return false;
                if (DateTime.Now >= _rebootThresholdTime)
                    return !_model.Model.IsConnected;

                return false;
            }
            else
            {
                return !_model.Model.IsConnected;
            }
        }

        public bool CanSendMessage()
        {
            if (!_setting.IsAutoRestartSendMessage)
                return false;

            var startTime = _setting.AutoRestartSendingMessageStartTime;
            var startTimeMode = _setting.AutoRestartSendingMessageStartTimeMode;
            var interval = _setting.AutoRestartSendingMessageIntervalTime;
            var intervalTimeMode = _setting.AutoRestartSendingMessageIntervalTimeMode;

            var startTimeSpan = startTimeMode switch
            {
                0 => new TimeSpan(0, 0, startTime),
                _ => new TimeSpan(0, startTime, 0)
            };
            var intervalTimeSpan = intervalTimeMode switch
            {
                0 => new TimeSpan(0, 0, interval),
                _ => new TimeSpan(0, interval, 0)
            };

            if (_thresholdTime - DateTime.Now <= startTimeSpan)
            {
                if (_messageDateTime == DateTime.MinValue)
                {
                    _messageDateTime = DateTime.Now;
                }

                if (DateTime.Now - _messageDateTime >= intervalTimeSpan)
                {
                    _messageDateTime = DateTime.Now;
                    return true;
                }
            }

            return false;
        }

        public void Dispose()
        {
            _isRequestStop = true;
            GC.SuppressFinalize(this);
        }
    }
}
