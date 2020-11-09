using CommonStyleLib.ExMessageBox;
using CommonStyleLib.File;
using Microsoft.Win32;
using Prism.Mvvm;
using SvManagerLibrary.SteamLibrary;
using System;
using System.IO;

namespace _7dtd_svmanager_fix_mvvm.Setup.Models
{
    public class ExecutablePageModel : BindableBase
    {

        public event CanChangedEventHandler CanChenged;
        private void OnCanChenged(object sender, bool canChanged)
        {
            CanChenged?.Invoke(sender, new CanChangedEventArgs(canChanged));
        }

        public ExecutablePageModel(InitializeData initializeData)
        {
            this.initializeData = initializeData;

            ServerFilePathText = initializeData.ServerFilePath;
        }

        private string serverFilePathText = string.Empty;
        public string ServerFilePathText
        {
            get => serverFilePathText;
            set
            {
                SetProperty(ref serverFilePathText, value);
                initializeData.ServerFilePath = value;
                var canChanged = !string.IsNullOrEmpty(value);
                OnCanChenged(this, canChanged);
            }
        }

        private InitializeData initializeData;

        public void SelectAndGetFilePath()
        {
            string filter = LangResources.SetupResource.Filter_ExcutableFile;
            string directoryPath = ConstantValues.DefaultDirectoryPath;
            string filename = FileSelector.GetFilePath(directoryPath, filter, "7DaysToDieServer.exe", FileSelector.FileSelectorType.Read);

            if (!string.IsNullOrEmpty(filename))
                ServerFilePathText = filename;
        }
        public void AutoSearchAndGetFilePath()
        {
            string steamPath = string.Empty;
            using (var rKey = Registry.CurrentUser.OpenSubKey(ConstantValues.RegSteamPath))
            {
                if (rKey == null)
                {
                    ExMessageBoxBase.Show(LangResources.SetupResource.UI_SteamNotInstalled, "", ExMessageBoxBase.MessageType.Exclamation);
                    return;
                }
                steamPath = (string)rKey.GetValue(ConstantValues.RegSteamKey);
            }

            string filename = GetFileName(steamPath);

            if (!string.IsNullOrEmpty(filename)) { ServerFilePathText = filename; }
        }

        private string GetFileName(string steamPath)
        {
            string filename = GetFileName(steamPath, ConstantValues.ServerClientPath, ConstantValues.ServerClientName);

            if (string.IsNullOrEmpty(filename))
                filename = GetFileName(steamPath, ConstantValues.GameClientPath, ConstantValues.GameClientName);

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
