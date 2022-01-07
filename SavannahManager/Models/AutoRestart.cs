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
    public class AutoRestart : IDisposable
    {
        #region Fields

        private readonly TimeSpan _baseTime;
        private DateTime _thresholdTime;
        private DateTime _messageDateTime;

        private bool _isRequestStop;
        private readonly MainWindowServerStart _model;
        private readonly SettingLoader _setting;

        #endregion

        #region Properties

        public bool IsRestarting { get; private set; }

        #endregion

        #region Events

        private readonly Subject<TimeSpan> _timeProgress = new();
        public IObservable<TimeSpan> TimeProgress => _timeProgress;

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
                    if (!isStop && DateTime.Now >= _thresholdTime)
                    {
                        IsRestarting = true;
                        _model.Model.ServerStop();
                        isStop = true;
                    }

                    if (isStop)
                    {
                        if (!_model.Model.IsConnected)
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
                            isStop = false;
                            _thresholdTime = CalculateThresholdTime(_baseTime);
                        }

                        await Task.Delay(1000);
                    }

                    _timeProgress.OnNext(_thresholdTime - DateTime.Now);
                    
                    if (CanSendMessage())
                    {
                        _fewRemaining.OnNext(_thresholdTime - DateTime.Now);
                    }

                    await Task.Delay(500);
                }

                IsRestarting = false;
                _timeProgress.OnCompleted();
            });
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
