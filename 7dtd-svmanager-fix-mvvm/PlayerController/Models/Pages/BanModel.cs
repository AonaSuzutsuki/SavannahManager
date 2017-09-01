using _7dtd_svmanager_fix_mvvm.Models;
using _7dtd_svmanager_fix_mvvm.PlayerController.Views.Pages;
using CommonLib.ExMessageBox;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7dtd_svmanager_fix_mvvm.PlayerController.Models.Pages
{
    public class DurationInfo
    {
        public string Name { get; set; }
        public string Command { get; set; }
    }

    public class BanModel : BindableBase, IPlayerPage
    {
        #region EndedEvent
        public event EventHandler Ended;
        protected virtual void OnEnded(EventArgs e)
        {
            Ended?.Invoke(this, e);
        }
        #endregion

        #region Properties
        private string name = default;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        private int duration;
        public int Duration
        {
            get => duration;
            set => SetProperty(ref duration, value);
        }
        #endregion

        private ObservableCollection<DurationInfo> durationList;
        public ObservableCollection<DurationInfo> DurationList
        {
            get => durationList;
            set => SetProperty(ref durationList, value);
        }

        #region Fiels
        private IMainWindowTelnet telnet;
        #endregion

        public BanModel(IMainWindowTelnet telnet)
        {
            this.telnet = telnet;

            DurationList = new ObservableCollection<DurationInfo>
            {
                new DurationInfo() { Name = "minutes", Command = "minutes" },
                new DurationInfo() { Name = "hours", Command = "hours" },
                new DurationInfo() { Name = "weeks", Command = "weeks" },
                new DurationInfo() { Name = "months", Command = "months" },
                new DurationInfo() { Name = "years", Command = "years" }
            };
        }

        public void AddBan(int index)
        {
            if (string.IsNullOrEmpty(Name))
            {
                ExMessageBoxBase.Show(string.Format(LangResources.Resources._0_is_Empty, "ID or Name"), LangResources.CommonResources.Error, ExMessageBoxBase.MessageType.Exclamation);
                return;
            }
            else if (index < 0)
            {
                ExMessageBoxBase.Show(string.Format(LangResources.Resources._0_is_Empty, "Duration Unit"), LangResources.CommonResources.Error, ExMessageBoxBase.MessageType.Exclamation);
                return;
            }

            var durationUnit = DurationList[index].Command;
            string cmd = string.Format("ban {0} {1} {2}", Name, Duration.ToString(), durationUnit);
            bool isSended = telnet.SocTelnetSendNRT(cmd);
            if (!isSended)
                return;

            OnEnded(new EventArgs());
        }

        public void Cancel()
        {
            OnEnded(new EventArgs());
        }
    }
}
