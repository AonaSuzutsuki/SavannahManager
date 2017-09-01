﻿using CommonLib.ExMessageBox;
using KimamaLib.File;
using Microsoft.Win32;
using Prism.Mvvm;
using SvManagerLibrary.SteamLibrary;
using System.IO;

namespace _7dtd_svmanager_fix_mvvm.Setup.Models
{
    public class Page2Model : BindableBase
    {

        public event CanChangedEventHandler CanChenged;
        private void OnCanChenged(object sender, bool canChanged)
        {
            CanChenged?.Invoke(sender, new CanChangedEventArgs(canChanged));
        }

        public Page2Model(InitializeData initializeData)
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
                bool canChanged = false;
                if (!string.IsNullOrEmpty(value)) canChanged = true;
                OnCanChenged(this, canChanged);
            }
        }

        private InitializeData initializeData;

        public void SelectAndGetFilePath()
        {
            string filter = LangResources.SetupResource.Filter_ExcutableFile;
            string directoryPath = ConstantValues.DirectoryPath;
            string filename = FileSelector.GetFilePath(directoryPath, filter, "7DaysToDieServer.exe", FileSelector.FileSelectorType.Read);

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
                }
                steamPath = (string)rKey.GetValue(ConstantValues.RegSteamKey);
            }

            string filename = GetFileName(steamPath);

            if (!string.IsNullOrEmpty(filename)) { ServerFilePathText = filename; }
        }

        private string GetFileName(string steamPath)
        {
            string filename = _GetFileName(steamPath);

            if (string.IsNullOrEmpty(filename))
            {
                var slLoader = new SteamLibraryLoader(steamPath + ConstantValues.SteamLibraryPath);
                var dirPaths = slLoader.SteamLibraryPathList;
                foreach (SteamLibraryPath dirPath in dirPaths)
                    filename = _GetFileName(dirPath.SteamDirPath);
            }

            return filename;
        }
        private string _GetFileName(string steamPath)
        {
            var filename = GetSvPath(steamPath + ConstantValues.ServerClientPath, ConstantValues.ServerClientName);
            if (string.IsNullOrEmpty(filename))
                filename = GetSvPath(steamPath + ConstantValues.GameClientPath, ConstantValues.GameClientName);
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
