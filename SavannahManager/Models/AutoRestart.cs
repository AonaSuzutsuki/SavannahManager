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

        private bool _isRequestStop;
        private readonly MainWindowServerStart _model;

        #endregion

        #region Properties

        public bool IsRestarting { get; private set; }

        #endregion

        #region Events

        private readonly Subject<TimeSpan> _timeProgress = new();
        public IObservable<TimeSpan> TimeProgress => _timeProgress;

        #endregion

        public AutoRestart(MainWindowServerStart model, TimeSpan thresholdTime)
        {
            _model = model;
            _baseTime = thresholdTime;
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

                    await Task.Delay(1000);
                }

                IsRestarting = false;
                _timeProgress.OnCompleted();
            });
        }

        public void Dispose()
        {
            _isRequestStop = true;
            GC.SuppressFinalize(this);
        }
    }
}
