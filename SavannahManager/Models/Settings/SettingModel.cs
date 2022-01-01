using System.IO;
using CommonStyleLib.File;
using CommonStyleLib.Models;

namespace _7dtd_svmanager_fix_mvvm.Models.Settings
{
    public class SettingModel : ModelBase
    {
        private string _exeFilePath;
        private string _configFilePath;
        private string _adminFilePath;
        private bool _isLogGetter;
        private int _consoleLength;
        private int _telnetWaitTime = 2;
        private bool _isBetaMode;
        private bool _isAutoUpdate = true;
        private bool _isEncryptPassword;
        private string _backupDirPath;
        private string _restoreDirPath;
        private readonly SettingLoader _setting;

        public string ExeFilePath
        {
            get => _exeFilePath;
            set => base.SetProperty(ref _exeFilePath, value);
        }
        public string ConfigFilePath
        {
            get => _configFilePath;
            set => base.SetProperty(ref _configFilePath, value);
        }
        public string AdminFilePath
        {
            get => _adminFilePath;
            set => base.SetProperty(ref _adminFilePath, value);
        }

        public bool IsLogGetter
        {
            get => _isLogGetter;
            set => base.SetProperty(ref _isLogGetter, value);
        }
        private int ConsoleLength
        {
            get => _consoleLength;
            set
            {
                _consoleLength = value;
                ConsoleLengthText = value.ToString();
            }
        }
        public string ConsoleLengthText
        {
            get => _consoleLength.ToString();
            set
            {
                int.TryParse(value, out _consoleLength);
                base.OnPropertyChanged(this);
            }
        }

        public int TelnetWaitTime
        {
            get => _telnetWaitTime;
            set => SetProperty(ref _telnetWaitTime, value);
        }

        public bool IsBetaMode
        {
            get => _isBetaMode;
            set => base.SetProperty(ref _isBetaMode, value);
        }
        public bool IsAutoUpdate
        {
            get => _isAutoUpdate;
            set => base.SetProperty(ref _isAutoUpdate, value);
        }

        public bool IsEncryptPassword
        {
            get => _isEncryptPassword;
            set => SetProperty(ref _isEncryptPassword, value);
        }

        public string BackupDirPath
        {
            get => _backupDirPath;
            set => SetProperty(ref _backupDirPath, value);
        }

        public string RestoreDirPath
        {
            get => _restoreDirPath;
            set => SetProperty(ref _restoreDirPath, value);
        }


        public ShortcutKeyManager ShortcutKeyManager { get; }


        public SettingModel(SettingLoader setting, ShortcutKeyManager shortcutKeyManager)
        {
            _setting = setting;
            ShortcutKeyManager = shortcutKeyManager;

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
                IsEncryptPassword = setting.IsEncryptPassword;
                BackupDirPath = setting.BackupDirPath;
                RestoreDirPath = setting.RestoreDirPath;
            }
        }

        public void EnabledEncryptionData(string password)
        {
            _setting.SetEncryptionPassword(password);
        }

        public void Save()
        {
            if (_setting != null)
            {
                _setting.ExeFilePath = ExeFilePath;
                _setting.ConfigFilePath = ConfigFilePath;
                _setting.AdminFilePath = AdminFilePath;
                _setting.IsLogGetter = IsLogGetter;
                _setting.ConsoleTextLength = _consoleLength;
                _setting.TelnetWaitTime = TelnetWaitTime;
                _setting.IsBetaMode = IsBetaMode;
                _setting.IsAutoUpdate = IsAutoUpdate;
                _setting.IsEncryptPassword = IsEncryptPassword;
                _setting.BackupDirPath = BackupDirPath;
                _setting.RestoreDirPath = RestoreDirPath;
                _setting.Save();
            }
        }

        public void GetServerFilePath()
        {
            var exeFilePath = GetFilePath(_exeFilePath, LangResources.SettingsResources.Filter_ExcutableFile, ConstantValues.ServerClientName);
            if (!string.IsNullOrEmpty(exeFilePath))
                ExeFilePath = exeFilePath;
        }
        public void GetConfFilePath()
        {
            var confFilePath = GetFilePath(_configFilePath, LangResources.SettingsResources.Filter_XmlFile, ConstantValues.ServerConfigName);
            if (!string.IsNullOrEmpty(confFilePath))
                ConfigFilePath = confFilePath;
        }
        public void GetAdminFilePath()
        {
            var adminFilePath = GetFilePath(_adminFilePath, LangResources.SettingsResources.Filter_XmlFile, ConstantValues.ServerConfigName);
            if (!string.IsNullOrEmpty(adminFilePath))
                AdminFilePath = adminFilePath;
        }

        public void GetBackupDirPath()
        {
            var dirPath = DirectorySelector.GetDirPath(_backupDirPath);
            if (!string.IsNullOrEmpty(dirPath))
                BackupDirPath = dirPath;
        }

        public void GetRestoreDirPath()
        {
            var dirPath = DirectorySelector.GetDirPath(_restoreDirPath);
            if (!string.IsNullOrEmpty(dirPath))
                RestoreDirPath = dirPath;
        }

        private string GetFilePath(string filePathForDir, string filter, string fileName)
        {
            var directoryPath = ConstantValues.DefaultDirectoryPath;
            if (!string.IsNullOrEmpty(filePathForDir))
            {
                var di = new DirectoryInfo(Path.GetDirectoryName(filePathForDir) ?? string.Empty);
                if (di.Exists)
                {
                    directoryPath = di.FullName;
                }
            }
            return FileSelector.GetFilePath(directoryPath, filter, fileName, FileSelector.FileSelectorType.Read);
        }
    }
}
