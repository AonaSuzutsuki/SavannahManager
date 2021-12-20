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

        private TimeSpan _baseTime;
        private DateTime _thresholdTime;

        private bool isRequestStop;
        private readonly IMainWindowServerStart _model;

        #endregion

        #region Properties

        public bool IsRestarting { get; private set; }

        #endregion

        #region Events

        private Subject<TimeSpan> _timeProgress = new Subject<TimeSpan>();
        public IObservable<TimeSpan> TimeProgress => _timeProgress;

        #endregion

        public AutoRestart(IMainWindowServerStart model, TimeSpan thresholdTime)
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
                while (!isRequestStop)
                {
                    if (!isStop && DateTime.Now >= _thresholdTime)
                    {
                        IsRestarting = true;
                        _model.ServerStop();
                        isStop = true;
                    }

                    if (isStop)
                    {
                        if (!_model.IsConnected)
                        {
                            await _model.ServerStart();
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
            isRequestStop = true;
        }
    }
}
