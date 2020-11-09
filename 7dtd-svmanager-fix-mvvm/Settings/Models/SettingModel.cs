using _7dtd_svmanager_fix_mvvm.Models;
using CommonStyleLib.Models;
using CommonStyleLib.File;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace _7dtd_svmanager_fix_mvvm.Settings.Models
{
    public class SettingModel : ModelBase
    {
        private string exeFilePath;
        public string ExeFilePath
        {
            get => exeFilePath;
            set => base.SetProperty(ref exeFilePath, value);
        }
        private string configFilePath;
        public string ConfigFilePath
        {
            get => configFilePath;
            set => base.SetProperty(ref configFilePath, value);
        }
        private string adminFilePath;
        public string AdminFilePath
        {
            get => adminFilePath;
            set => base.SetProperty(ref adminFilePath, value);
        }

        private bool isLogGetter = false;
        public bool IsLogGetter
        {
            get => isLogGetter;
            set => base.SetProperty(ref isLogGetter, value);
        }
        private int consoleLength = 0;
        private int ConsoleLength
        {
            get => consoleLength;
            set
            {
                consoleLength = value;
                ConsoleLengthText = value.ToString();
            }
        }
        public string ConsoleLengthText
        {
            get => consoleLength.ToString();
            set
            {
                int.TryParse(value, out consoleLength);
                base.OnPropertyChanged(this);
            }
        }

        private int telnetWaitTime = 2;
        public int TelnetWaitTime
        {
            get => telnetWaitTime;
            set => SetProperty(ref telnetWaitTime, value);
        }

        private bool isBetaMode = false;
        public bool IsBetaMode
        {
            get => isBetaMode;
            set => base.SetProperty(ref isBetaMode, value);
        }
        private bool isAutoUpdate = true;
        public bool IsAutoUpdate
        {
            get => isAutoUpdate;
            set => base.SetProperty(ref isAutoUpdate, value);
        }

        private string backupDirPath;
        public string BackupDirPath
        {
            get => backupDirPath;
            set => SetProperty(ref backupDirPath, value);
        }


        public ShortcutKeyManager ShortcutKeyManager { get; }

        private readonly SettingLoader setting;

        public SettingModel(SettingLoader setting, ShortcutKeyManager shortcutKeyManager)
        {
            this.setting = setting;
            this.ShortcutKeyManager = shortcutKeyManager;

            if (setting != null)
            {
                ExeFilePath = setting.ExeFilePath;
                ConfigFilePath = setting.ConfigFilePath;
                AdminFilePath = setting.AdminFilePath;
                IsLogGetter = setting.IsLogGetter;
                ConsoleLength = setting.ConsoleTextLength;
                TelnetWaitTime = setting.TelnetWaitTime;
                IsBetaMode = setting.IsBetaMode;
                IsAutoUpdate = setting.IsAutoUpdate;
                BackupDirPath = setting.BackupDirPath;
            }
        }

        public void Save()
        {
            if (setting != null)
            {
                setting.ExeFilePath = ExeFilePath;
                setting.ConfigFilePath = ConfigFilePath;
                setting.AdminFilePath = AdminFilePath;
                setting.IsLogGetter = IsLogGetter;
                setting.ConsoleTextLength = consoleLength;
                setting.TelnetWaitTime = TelnetWaitTime;
                setting.IsBetaMode = IsBetaMode;
                setting.IsAutoUpdate = IsAutoUpdate;
                setting.BackupDirPath = BackupDirPath;
                setting.Save();
            }
        }

        public void GetServerFilePath()
        {
            string exeFilePath = GetFilePath(this.exeFilePath, LangResources.SettingsResources.Filter_ExcutableFile, ConstantValues.ServerClientName);
            if (!string.IsNullOrEmpty(exeFilePath))
                ExeFilePath = exeFilePath;
        }
        public void GetConfFilePath()
        {
            string confFilePath = GetFilePath(this.configFilePath, LangResources.SettingsResources.Filter_XmlFile, ConstantValues.ServerConfigName);
            if (!string.IsNullOrEmpty(confFilePath))
                ConfigFilePath = confFilePath;
        }
        public void GetAdminFilePath()
        {
            string adminFilePath = GetFilePath(this.adminFilePath, LangResources.SettingsResources.Filter_XmlFile, ConstantValues.ServerConfigName);
            if (!string.IsNullOrEmpty(adminFilePath))
                AdminFilePath = adminFilePath;
        }

        public void GetBackupDirPath()
        {
            var dirPath = DirectorySelector.GetDirPath(this.backupDirPath);
            if (!string.IsNullOrEmpty(dirPath))
                BackupDirPath = dirPath;
        }

        private string GetFilePath(string filePathForDir, string filter, string fileName)
        {
            string directoryPath = ConstantValues.DefaultDirectoryPath;
            if (!string.IsNullOrEmpty(filePathForDir))
            {
                DirectoryInfo di = new DirectoryInfo(System.IO.Path.GetDirectoryName(filePathForDir));
                if (di.Exists)
                {
                    directoryPath = di.FullName;
                }
            }
            return FileSelector.GetFilePath(directoryPath, filter, fileName, FileSelector.FileSelectorType.Read);
        }
    }
}
