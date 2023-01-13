using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonExtensionLib.Extensions;
using CommonStyleLib.Models;

namespace _7dtd_svmanager_fix_mvvm.Models.AutoRestart
{
    public class ProcessSelectorModel : ModelBase, IDisposable
    {
        public int ProcessId { get; set; } = -1;

        public Dictionary<int, Process> Processes { get; set; } = new();

        public ObservableCollection<Process> FilteredProcesses { get; set; } = new();

        public ProcessSelectorModel()
        {
        }

        public void Load(string filter)
        {
            var processes = Process.GetProcesses();
            foreach (var process in processes)
            {
                Processes.Add(process.Id, process);
            }
            FilteredProcesses.AddRange(processes);

            if (!string.IsNullOrEmpty(filter))
                FilterProcesses(filter);
        }

        public void FilterProcesses(string filter)
        {
            FilteredProcesses.Clear();
            foreach (var keyValuePair in Processes.Where(x => x.Value.ProcessName.Contains(filter)))
            {
                FilteredProcesses.Add(keyValuePair.Value);
            }
        }

        public void SelectProcess(int processId)
        {
            ProcessId = processId;
        }

        public void Dispose()
        {
            foreach (var process in Processes)
            {
                process.Value.Dispose();
            }
        }
    }
}
