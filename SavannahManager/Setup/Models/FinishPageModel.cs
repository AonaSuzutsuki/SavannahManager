using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace _7dtd_svmanager_fix_mvvm.Setup.Models
{
    public class FinishPageModel : PageModelBase
    {
        #region Fields

        #endregion

        public FinishPageModel(InitializeData initializeData) : base(initializeData)
        {
        }

        public void ApplySetting()
        {
            var setting = InitializeData.Setting;
            setting.ConfigFilePath = InitializeData.ServerConfigFilePath;
            setting.ExeFilePath = InitializeData.ServerFilePath;
            setting.AdminFilePath = InitializeData.ServerAdminConfigFilePath;
            setting.IsFirstBoot = false;
        }
    }
}
