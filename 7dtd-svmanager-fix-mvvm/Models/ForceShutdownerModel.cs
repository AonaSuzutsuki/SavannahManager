using ExMessageBox;
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

        private int currentIndex = -1;
        public int CurrentIndex
        {
            get => currentIndex;
            set => SetProperty(ref currentIndex, value);
        }


        public void Refresh()
        {
            System.Diagnostics.Process[] ps = System.Diagnostics.Process.GetProcessesByName("7DaysToDieServer");

            processData.Clear();
            ObservableCollection<ProcessTab> listData = new ObservableCollection<ProcessTab>();

            foreach (System.Diagnostics.Process p in ps)
            {
                listData.Add(new ProcessTab()
                {
                    PID = p.Id.ToString(),
                    Name = p.ProcessName,
                });
                ProcessIDs.Add(p.Id);
            }
        }

        public void KillProcess()
        {
            try
            {
                Process p = Process.GetProcessById(ProcessIDs[CurrentIndex]);

                p.Kill();

                System.Threading.Thread.Sleep(500);
                Refresh();
            }
            catch (System.ArgumentException sae)
            {
                ExMessageBoxBase.Show(sae.Message, LangResources.ForceShutdownerResources.Error, ExMessageBoxBase.MessageType.Exclamation);
                return;
            }
        }
    }
}
