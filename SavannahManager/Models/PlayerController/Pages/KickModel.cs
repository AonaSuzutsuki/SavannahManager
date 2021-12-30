using System;
using _7dtd_svmanager_fix_mvvm.Models.Interfaces;
using _7dtd_svmanager_fix_mvvm.Views.PlayerController.Pages;
using CommonStyleLib.ExMessageBox;
using Prism.Mvvm;

namespace _7dtd_svmanager_fix_mvvm.Models.PlayerController.Pages
{
    public class KickModel : BindableBase, IPlayerPage
    {
        #region EndedEvent
        public event EventHandler Ended;
        protected virtual void OnEnded(EventArgs e)
        {
            Ended?.Invoke(this, e);
        }
        #endregion

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        #region Fiels
        private string _name;
        private readonly IMainWindowTelnet _telnet;
        #endregion

        public KickModel(IMainWindowTelnet telnet)
        {
            _telnet = telnet;
        }

        public void Kick(string reason)
        {
            if (string.IsNullOrEmpty(Name))
            {
                ExMessageBoxBase.Show(string.Format(LangResources.Resources._0_is_Empty, "ID or Name"), LangResources.CommonResources.Error, ExMessageBoxBase.MessageType.Exclamation);
                return;
            }

            var cmd = $"kick {Name} {reason}";
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
