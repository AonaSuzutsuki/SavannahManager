using _7dtd_svmanager_fix_mvvm.Models;
using _7dtd_svmanager_fix_mvvm.Models.Interfaces;
using _7dtd_svmanager_fix_mvvm.PlayerController.Views.Pages;
using CommonStyleLib.ExMessageBox;
using CommonStyleLib.Models;
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

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public int Permission
        {
            get => _permission;
            set => SetProperty(ref _permission, value);
        }

        #region Fiels
        private string _name;
        private int _permission;
        private readonly string _commandHead;
        private readonly IMainWindowTelnet _telnet;
        #endregion

        public AdminAddModel(IMainWindowTelnet telnet, AddType addType)
        {
            _telnet = telnet;

            _commandHead = addType.ToCommand();
        }
        
        public void AddAdmin()
        {
            if (string.IsNullOrEmpty(Name))
            {
                ExMessageBoxBase.Show(string.Format(LangResources.Resources._0_is_Empty, "ID or Name"), LangResources.CommonResources.Error, ExMessageBoxBase.MessageType.Exclamation);
                return;
            }

            var cmd = string.Format("{0} add {1} {2}", _commandHead, Name, Permission.ToString());
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
