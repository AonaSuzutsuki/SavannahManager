using CommonStyleLib.ExMessageBox;
using CommonStyleLib.File;
using Microsoft.Win32;
using Prism.Mvvm;
using SvManagerLibrary.SteamLibrary;
using System;
using System.IO;

namespace _7dtd_svmanager_fix_mvvm.Setup.Models
{
    public class ExecutablePageModel : PageModelBase
    {
        public ExecutablePageModel(InitializeData initializeData) : base(initializeData)
        {
            ServerFilePathText = initializeData.ServerFilePath;
        }

        private string _serverFilePathText = string.Empty;
        public string ServerFilePathText
        {
            get => _serverFilePathText;
            set
            {
                SetProperty(ref _serverFilePathText, value);
                InitializeData.ServerFilePath = value;
                var canChanged = !string.IsNullOrEmpty(value);
                OnCanChanged(this, canChanged);
            }
        }

        public void SelectAndGetFilePath()
        {
            var filter = LangResources.SetupResource.Filter_ExcutableFile;
            var directoryPath = ConstantValues.DefaultDirectoryPath;
            var filename = FileSelector.GetFilePath(directoryPath, filter, "7DaysToDieServer.exe", FileSelector.FileSelectorType.Read);

            if (!string.IsNullOrEmpty(filename))
                ServerFilePathText = filename;
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

            if (!string.IsNullOrEmpty(filename)) { ServerFilePathText = filename; }
        }

        private static string GetFileName(string steamPath)
        {
            var filename = GetFileName(steamPath, ConstantValues.ServerClientPath, ConstantValues.ServerClientName);

            if (string.IsNullOrEmpty(filename))
                filename = GetFileName(steamPath, ConstantValues.GameClientPath, ConstantValues.GameClientName);

            return filename;
        }
    }
}
