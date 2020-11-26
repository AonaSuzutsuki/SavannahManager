using CommonStyleLib.File;
using Microsoft.Win32;
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
    public class AdminPageModel : PageModelBase
    {
        public AdminPageModel(InitializeData initializeData) : base(initializeData)
        {
            ServerConfigPathText = initializeData.ServerAdminConfigFilePath;
        }

        #region PropertiesForViewModel
        private string serverConfigPathText = string.Empty;
        public string ServerConfigPathText
        {
            get => serverConfigPathText;
            set
            {
                SetProperty(ref serverConfigPathText, value);
                InitializeData.ServerAdminConfigFilePath = value;
                var canChanged = !string.IsNullOrEmpty(value);
                OnCanChanged(this, canChanged);
            }
        }
        #endregion

        public void SelectAndGetFilePath()
        {
            var filter = LangResources.SetupResource.Filter_XmlFile;
            var directoryPath = ConstantValues.DefaultDirectoryPath;
            var filename = FileSelector.GetFilePath(directoryPath, filter, "serveradmin.xml", FileSelector.FileSelectorType.Read);

            if (!string.IsNullOrEmpty(filename))
                ServerConfigPathText = filename;
        }
        public void AutoSearchAndGetFilePath()
        {
            var roamingDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var serverAdminPath = $"{roamingDirectory}\\7DaysToDie\\Saves\\serveradmin.xml";
            if (File.Exists(serverAdminPath))
            {
                ServerConfigPathText = serverAdminPath;
            }
        }
    }
}
