using System;
using System.Collections.ObjectModel;
using _7dtd_svmanager_fix_mvvm.Models.Interfaces;
using _7dtd_svmanager_fix_mvvm.Views.PlayerController.Pages;
using CommonStyleLib.ExMessageBox;
using Prism.Mvvm;

namespace _7dtd_svmanager_fix_mvvm.Models.PlayerController.Pages
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
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public int Duration
        {
            get => _duration;
            set => SetProperty(ref _duration, value);
        }
        #endregion

        public ObservableCollection<DurationInfo> DurationList
        {
            get => _durationList;
            set => SetProperty(ref _durationList, value);
        }

        #region Fiels
        private string _name;
        private int _duration;
        private ObservableCollection<DurationInfo> _durationList;

        private readonly IMainWindowTelnet _telnet;
        #endregion

        public BanModel(IMainWindowTelnet telnet)
        {
            _telnet = telnet;

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
            var cmd = string.Format("ban {0} {1} {2}", Name, Duration.ToString(), durationUnit);
            var isSended = _telnet.SocTelnetSendNrt(cmd);
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
