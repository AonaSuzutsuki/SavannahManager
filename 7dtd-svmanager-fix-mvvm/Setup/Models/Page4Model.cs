using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace _7dtd_svmanager_fix_mvvm.Setup.Models
{
    public class Page4Model : BindableBase
    {
        #region Fields

        private InitializeData _initializeData;

        #endregion

        public Page4Model(InitializeData initializeData)
        {
            _initializeData = initializeData;
        }

        public void ApplySetting()
        {
            var setting = _initializeData.Setting;
            setting.ConfigFilePath = _initializeData.ServerConfigFilePath;
            setting.ExeFilePath = _initializeData.ServerFilePath;
            setting.IsFirstBoot = false;
        }
    }
}
