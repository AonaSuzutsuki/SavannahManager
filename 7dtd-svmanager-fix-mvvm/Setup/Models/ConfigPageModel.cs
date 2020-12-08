using CommonStyleLib.File;
using Microsoft.Win32;
using Prism.Mvvm;
using SvManagerLibrary.SteamLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonStyleLib.ExMessageBox;

namespace _7dtd_svmanager_fix_mvvm.Setup.Models
{
    public class ConfigPageModel : PageModelBase
    {
        public ConfigPageModel(InitializeData initializeData) : base(initializeData)
        {
            ServerConfigPathText = initializeData.ServerConfigFilePath;
        }

        #region PropertiesForViewModel
        private string _serverConfigPathText = string.Empty;
        public string ServerConfigPathText
        {
            get => _serverConfigPathText;
            set
            {
                SetProperty(ref _serverConfigPathText, value);
                InitializeData.ServerConfigFilePath = value;
                var canChanged = !string.IsNullOrEmpty(value);
                OnCanChanged(this, canChanged);
            }
        }
        #endregion

        public void SelectAndGetFilePath()
        {
            var filter = LangResources.SetupResource.Filter_XmlFile;
            var directoryPath = ConstantValues.DefaultDirectoryPath;
            var filename = FileSelector.GetFilePath(directoryPath, filter, "serverconfig.xml", FileSelector.FileSelectorType.Read);

            if (!string.IsNullOrEmpty(filename))
                ServerConfigPathText = filename;
        }
        public void AutoSearchAndGetFilePath()
        {
            string steamPath;
            using (var rKey = Registry.CurrentUser.OpenSubKey(ConstantValues.RegSteamPath))
            {
                if (rKey == null)
                {
                    ExMessageBoxBase.Show(LangResources.SetupResource.UI_SteamNotInstalled, "", ExMessageBoxBase.MessageType.Exclamation);
                    return;
                }
                steamPath = (string)rKey.GetValue(ConstantValues.RegSteamKey);
            }

            var filename = GetFileName(steamPath);

            if (!string.IsNullOrEmpty(filename))
            {
                ServerConfigPathText = filename;
            }
        }

        protected static string GetFileName(string steamPath)
        {
            var filename = GetFileName(steamPath, ConstantValues.ServerClientPath, ConstantValues.ServerConfigName);

            if (string.IsNullOrEmpty(filename))
                filename = GetFileName(steamPath, ConstantValues.GameClientPath, ConstantValues.ServerConfigName);

            return filename;
        }
    }
}
