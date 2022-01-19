using System;
using System.IO;
using CommonStyleLib.File;

namespace _7dtd_svmanager_fix_mvvm.Models.Setup
{
    public class AdminPageModel : PageModelBase
    {
        public AdminPageModel(InitializeData initializeData) : base(initializeData)
        {
            ServerConfigPathText = initializeData.ServerAdminConfigFilePath;
        }

        #region PropertiesForViewModel
        private string _serverConfigPathText = string.Empty;
        public string ServerConfigPathText
        {
            get => _serverConfigPathText;
            set
            {
                SetProperty(ref _serverConfigPathText, value);
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
