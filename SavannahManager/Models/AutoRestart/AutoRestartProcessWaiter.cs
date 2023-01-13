using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _7dtd_svmanager_fix_mvvm.Models.Interfaces;

namespace _7dtd_svmanager_fix_mvvm.Models.AutoRestart
{
    public class AutoRestartProcessWaiter : AbstractAutoRestart
    {
        public enum ServerType
        {
            Client,
            Server
        }

        private Process _process;

        public bool IsAttach { get; private set; }

        public Func<int> SelectProcessFunc { get; set; }

        public AutoRestartProcessWaiter(MainWindowServerStart model, Func<int> selectProcessFunc) : base(model)
        {
            SelectProcessFunc = selectProcessFunc;
        }

        public void Initialize()
        {
            // can get process when ran a local server.
            var processId = Model.Model.CurrentProcessId;
            if (processId > 0)
            {
                AttachProcess(processId);
                return;
            }

            // search server process.
            _process = AttachProcess(ServerType.Server) ?? AttachProcess(ServerType.Client);
            IsAttach = _process != null;

            if (IsAttach)
                return;

            // open select window if the process cannot be retrieved.
            var process = SelectProcessFunc();
            if (process < 0)
            {
                IsRequestStop = true;
                return;
            }

            AttachProcess(process);
        }

        protected override bool AfterStopTelnet()
        {
            TimeProgressSubject.OnNext(new AutoRestartWaitingTimeEventArgs(AutoRestartWaitingTimeEventArgs.WaitingType.ProcessWait, TimeSpan.Zero));

            return _process.HasExited;
        }

        protected override bool CanRestart()
        {
            return _process.HasExited;
        }

        public void AttachProcess(int processId)
        {
            var process = Process.GetProcessById(processId);
            _process = process;
            IsAttach = _process != null;

            ThresholdTime = CalculateThresholdTime(BaseTime);
        }

        protected override void AfterStartSever()
        {
            _process?.Dispose();
            _process = null;
            IsAttach = false;

            Initialize();
        }

        public override void Dispose()
        {
            base.Dispose();

            _process?.Dispose();
        }

        private static Process AttachProcess(ServerType type)
        {
            var name = type == ServerType.Server ? "7DaysToDieServer" : "7DaysToDie";
            var processes = Process.GetProcessesByName(name);

            if (processes.Length <= 1)
                return processes.FirstOrDefault();
            
            foreach (var process in processes)
            {
                process.Dispose();
            }

            return null;
        }
    }
}
