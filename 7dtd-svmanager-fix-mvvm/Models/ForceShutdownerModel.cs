using CommonLib.ExMessageBox;
using CommonLib.Models;
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
        ObservableCollection<ProcessTab> processData = new ObservableCollection<ProcessTab>();
        public ObservableCollection<ProcessTab> ProcessData
        {
            get => processData;
        }

        List<int> ProcessIDs = new List<int>();
        
        public void Refresh()
        {
            var ps = Process.GetProcessesByName("7DaysToDieServer");

            processData.Clear();

            foreach (var p in ps)
            {
                ProcessData.Add(new ProcessTab()
                {
                    PID = p.Id.ToString(),
                    Name = p.ProcessName,
                });
                ProcessIDs.Add(p.Id);
            }
        }

        public void KillProcess(int index)
        {
            try
            {
                var p = Process.GetProcessById(ProcessIDs[index]);
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
