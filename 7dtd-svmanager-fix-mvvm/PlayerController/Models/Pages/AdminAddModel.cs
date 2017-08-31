using _7dtd_svmanager_fix_mvvm.Models;
using _7dtd_svmanager_fix_mvvm.PlayerController.Views.Pages;
using CommonLib.ExMessageBox;
using CommonLib.Models;
using Prism.Mvvm;
using SvManagerLibrary.Telnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7dtd_svmanager_fix_mvvm.PlayerController.Models.Pages
{
    public class AdminAddModel : BindableBase, IPlayerPage
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

        private int permission;
        public int Permission
        {
            get => permission;
            set => SetProperty(ref permission, value);
        }

        #region Fiels
        public string commandHead;
        private IMainWindowTelnet telnet;
        #endregion

        public AdminAddModel(IMainWindowTelnet telnet, AddType addType)
        {
            this.telnet = telnet;

            commandHead = addType.ToCommand();
        }
        
        public void AddAdmin()
        {
            if (string.IsNullOrEmpty(Name))
            {
                ExMessageBoxBase.Show(string.Format(LangResources.Resources._0_is_Empty, "ID or Name"), LangResources.CommonResources.Error, ExMessageBoxBase.MessageType.Exclamation);
                return;
            }

            string cmd = string.Format("{0} add {1} {2}", commandHead, Name, Permission.ToString());
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
