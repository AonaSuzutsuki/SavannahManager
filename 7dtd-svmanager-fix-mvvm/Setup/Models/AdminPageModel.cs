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
    public class AdminPageModel : BindableBase
    {
        public event CanChangedEventHandler CanChenged;
        private void OnCanChenged(object sender, bool canChanged)
        {
            CanChenged?.Invoke(sender, new CanChangedEventArgs(canChanged));
        }

        public AdminPageModel(InitializeData initializeData)
        {
            this.initializeData = initializeData;

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
                initializeData.ServerAdminConfigFilePath = value;
                bool canChanged = false;
                if (!string.IsNullOrEmpty(value)) canChanged = true;
                OnCanChenged(this, canChanged);
            }
        }
        #endregion

        InitializeData initializeData;

        public void SelectAndGetFilePath()
        {
            string filter = LangResources.SetupResource.Filter_XmlFile;
            string directoryPath = ConstantValues.DefaultDirectoryPath;
            string filename = FileSelector.GetFilePath(directoryPath, filter, "serveradmin.xml", FileSelector.FileSelectorType.Read);

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

        private string GetFileName(string steamPath)
        {
            string filename = GetFileName(steamPath, ConstantValues.ServerClientPath, ConstantValues.ServerConfigName);

            if (string.IsNullOrEmpty(filename))
                filename = GetFileName(steamPath, ConstantValues.GameClientPath, ConstantValues.ServerConfigName);

            return filename;
        }
        private string GetFileName(string steamPath, string target, string name)
        {
            string filename = GetSvPath(steamPath + target, name);

            if (string.IsNullOrEmpty(filename))
            {
                try
                {
                    var slLoader = new SteamLibraryLoader(steamPath + ConstantValues.SteamLibraryPath);
                    var dirPaths = slLoader.SteamLibraryPathList;
                    foreach (SteamLibraryPath dirPath in dirPaths)
                        filename = GetSvPath(dirPath.SteamDirPath + target, name);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return filename;
        }
        private string GetSvPath(string dirPath, string exeName)
        {
            string filename = string.Empty;
            if (Directory.Exists(dirPath))
            {
                var fi = new FileInfo(dirPath + @"\" + exeName);
                if (fi.Exists)
                    filename = fi.FullName;
            }
            return filename;
        }
    }
}
