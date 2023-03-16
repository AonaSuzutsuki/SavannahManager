using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _7dtd_svmanager_fix_mvvm.Models.Interfaces;

namespace _7dtd_svmanager_fix_mvvm.Models.Scheduled
{
    public class ScheduledCommandExecutor
    {
        private bool _isStopTask;

        private DateTime _previousTime;
        private DateTime _startTime;

        private readonly IMainWindowTelnet _telnet;

        public Task CurrentTask { get; private set; }

        public ScheduledCommand Command { get; }

        public ScheduledCommandExecutor(IMainWindowTelnet telnet, ScheduledCommand command)
        {
            _telnet = telnet;
            Command = command;
        }

        public void Start()
        {
            _isStopTask = false;

            _startTime = DateTime.Now + Command.WaitTime;
            _previousTime = DateTime.Now;

            CurrentTask = Task.Factory.StartNew(async () =>
            {
                while (!_isStopTask)
                {
                    var now = DateTime.Now;

                    if (now >= _startTime && now >= _previousTime + Command.Interval)
                    {
                        _telnet.SocTelnetSendNrtNer(Command.Command, true);
                        _previousTime = now;
                    }

                    Debug.WriteLine($"{Command}: {(_previousTime + Command.Interval) - now}");

                    await Task.Delay(100);
                }
            });
        }

        public void Stop()
        {
            _isStopTask = true;
        }
    }
}
