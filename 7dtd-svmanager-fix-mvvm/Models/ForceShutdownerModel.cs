using CommonStyleLib.ExMessageBox;
using CommonStyleLib.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7dtd_svmanager_fix_mvvm.Models
{
    public class ProcessTab
    {
        public string PID { get; set; }
        public string Name { get; set; }
    }

    public class ForceShutdownerModel : ModelBase
    {
        public ObservableCollection<ProcessTab> ProcessData { get; } = new ObservableCollection<ProcessTab>();

        public bool ProcessSelected
        {
            get => processSelected;
            set => SetProperty(ref processSelected, value);
        }

        private readonly List<int> processIds = new List<int>();
        private bool processSelected;
        
        public void Refresh()
        {
            var ps = Process.GetProcessesByName("7DaysToDieServer");

            ProcessData.Clear();

            foreach (var p in ps)
            {
                ProcessData.Add(new ProcessTab()
                {
                    PID = p.Id.ToString(),
                    Name = p.ProcessName,
                });
                processIds.Add(p.Id);
            }
        }

        public void KillProcess(int index)
        {
            try
            {
                var p = Process.GetProcessById(processIds[index]);
                p.Kill();

                System.Threading.Thread.Sleep(500);
            }
            catch (System.ArgumentException sae)
            {
                ExMessageBoxBase.Show(sae.Message, LangResources.CommonResources.Error, ExMessageBoxBase.MessageType.Exclamation);
                return;
            }
        }
    }
}
