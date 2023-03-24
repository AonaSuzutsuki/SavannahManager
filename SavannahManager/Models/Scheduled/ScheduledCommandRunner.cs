using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using _7dtd_svmanager_fix_mvvm.Models.Interfaces;

namespace _7dtd_svmanager_fix_mvvm.Models.Scheduled
{
    public class ScheduledCommandRunner
    {
        private readonly IMainWindowTelnet _telnet;

        private readonly List<ScheduledCommandExecutor> _scheduledCommands = new();

        public IEnumerable<ScheduledCommandExecutor> ScheduledCommands => _scheduledCommands;

        public ScheduledCommandLoader Loader { get; } = new();

        public bool IsStop => _scheduledCommands.Count(x => x.IsStopCommand) == _scheduledCommands.Count;

        public ScheduledCommandRunner(IMainWindowTelnet telnet)
        {
            _telnet = telnet;
        }

        public async Task LoadAsync()
        {
            await Loader.LoadFromFileAsync();
            
            var executors = Loader.Commands.Select(x => 
                new ScheduledCommandExecutor(_telnet, (ScheduledCommand)x.Clone()));
            _scheduledCommands.Clear();
            _scheduledCommands.AddRange(executors);
        }

        public void Start()
        {
            foreach (var scheduledCommand in _scheduledCommands)
            {
                scheduledCommand.Start();
            }
        }

        public void Stop()
        {
            foreach (var scheduledCommand in _scheduledCommands)
            {
                scheduledCommand.Stop();
            }
        }
    }
}
