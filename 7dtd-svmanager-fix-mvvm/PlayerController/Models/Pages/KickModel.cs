using _7dtd_svmanager_fix_mvvm.Models;
using _7dtd_svmanager_fix_mvvm.PlayerController.Views.Pages;
using CommonLib.ExMessageBox;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7dtd_svmanager_fix_mvvm.PlayerController.Models.Pages
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

        private string name = default;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        #region Fiels
        private IMainWindowTelnet telnet;
        #endregion

        public KickModel(IMainWindowTelnet telnet)
        {
            this.telnet = telnet;
        }

        public void Kick(string reason)
        {
            if (string.IsNullOrEmpty(Name))
            {
                ExMessageBoxBase.Show(string.Format(LangResources.Resources._0_is_Empty, "ID or Name"), LangResources.CommonResources.Error, ExMessageBoxBase.MessageType.Exclamation);
                return;
            }

            string cmd = string.Format("kick {0} {1}", Name, reason);
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
