using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _7dtd_svmanager_fix_mvvm.Models.Interfaces;
using Prism.Mvvm;

namespace _7dtd_svmanager_fix_mvvm.Models.Scheduled
{
    public class ScheduledCommandExecutor : BindableBase
    {
        #region Fields

        private bool _isStopTask;
        private DateTime _startTime;

        private readonly IMainWindowTelnet _telnet;

        #endregion


        #region Properties

        public DateTime PreviousTime { get; set; }

        public DateTime NextTime { get; set; }

        public Task CurrentTask { get; private set; }

        public ScheduledCommand Command { get; }

        public bool IsStopCommand { get; private set; } = true;

        #endregion

        public ScheduledCommandExecutor(IMainWindowTelnet telnet, ScheduledCommand command)
        {
            _telnet = telnet;
            Command = command;
        }

        public void Start()
        {
            _isStopTask = false;
            IsStopCommand = false;

            _startTime = DateTime.Now + Command.WaitTime - Command.Interval;
            PreviousTime = DateTime.MinValue;
            NextTime = _startTime + Command.Interval;

            CurrentTask = Task.Factory.StartNew(async () =>
            {
                while (!_isStopTask)
                {
                    var now = DateTime.Now;

                    if (now >= _startTime && now >= NextTime)
                    {
                        _telnet.SocTelnetSendNrtNer(Command.Command, true);
                        PreviousTime = now;
                        NextTime = now + Command.Interval;
                    }

                    Debug.WriteLine($"{Command.Command}: {NextTime - now}");

                    await Task.Delay(100);
                }
                IsStopCommand = true;
            });
        }

        public void Stop()
        {
            _isStopTask = true;
        }
    }
}
