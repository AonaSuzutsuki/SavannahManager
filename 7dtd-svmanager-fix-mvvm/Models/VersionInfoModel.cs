using _7dtd_svmanager_fix_mvvm.LangResources;
using CommonStyleLib.ExMessageBox;
using CommonStyleLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace _7dtd_svmanager_fix_mvvm.Models
{
    public class VersionInfoModel : ModelBase
    {

        private string version = default;
        public string Version
        {
            get => version;
            set => SetProperty(ref version, value);
        }

        public void SetVersion()
        {
            //var asm = Assembly.GetExecutingAssembly();
            //var ver = asm.GetName().Version;

            Version = ConstantValues.Version; //ver.ToString() + "b";
        }

        public void SetAddressToClipboard(string addr)
        {
            var dr = ExMessageBoxBase.Show(VersionInfoResources.Are_you_sure_copy, VersionInfoResources.MessageTitle, ExMessageBoxBase.MessageType.Question, ExMessageBoxBase.ButtonType.YesNo);
            if (dr == ExMessageBoxBase.DialogResult.Yes)
            {
                Clipboard.SetText(addr);
                ExMessageBoxBase.Show(string.Format(VersionInfoResources.CopyCompleted, addr), VersionInfoResources.MessageTitle, ExMessageBoxBase.MessageType.Asterisk);
            }
        }
    }
}
