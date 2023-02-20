using System;
using System.IO;
using _7dtd_svmanager_fix_mvvm.Models.Interfaces;
using CommonStyleLib.File;
using CommonStyleLib.Models;
using Reactive.Bindings;

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
        private int _intervalTime;
        private int _intervalTimeSelectedIndex;
        private bool _isAutoRestartSendMessage;
        private int _autoRestartSendingMessageStartTime;
        private int _autoRestartSendingMessageStartTimeMode;
        private int _autoRestartSendingMessageIntervalTime;
        private int _autoRestartSendingMessageIntervalTimeMode;
        private string _autoRestartSendingMessageFormat;
        private int _autoRestartRebootingWaitMode;
        private int _autoRestartRebootCoolTime;
        private int _autoRestartRebootCoolTimeMode;
        private bool _isAutoRestartRunScriptEnabled;
        private string _autoRestartRunningScript;
        private bool _isAutoRestartWaitRunningScript;
        private int _autoRestartScriptWaitTime;
        private int _autoRestartScriptWaitTimeMode;
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

        public int IntervalTime
        {
            get => _intervalTime;
            set => SetProperty(ref _intervalTime, value);
        }

        public int IntervalTimeSelectedIndex
        {
            get => _intervalTimeSelectedIndex;
            set => SetProperty(ref _intervalTimeSelectedIndex, value);
        }

        public bool IsAutoRestartSendMessage
        {
            get => _isAutoRestartSendMessage;
            set => SetProperty(ref _isAutoRestartSendMessage, value);
        }

        public int AutoRestartSendingMessageStartTime
        {
            get => _autoRestartSendingMessageStartTime;
            set => SetProperty(ref _autoRestartSendingMessageStartTime, value);
        }

        public int AutoRestartSendingMessageStartTimeMode
        {
            get => _autoRestartSendingMessageStartTimeMode;
            set => SetProperty(ref _autoRestartSendingMessageStartTimeMode, value);
        }

        public int AutoRestartSendingMessageIntervalTime
        {
            get => _autoRestartSendingMessageIntervalTime;
            set => SetProperty(ref _autoRestartSendingMessageIntervalTime, value);
        }

        public int AutoRestartSendingMessageIntervalTimeMode
        {
            get => _autoRestartSendingMessageIntervalTimeMode;
            set => SetProperty(ref _autoRestartSendingMessageIntervalTimeMode, value);
        }

        public string AutoRestartSendingMessageFormat
        {
            get => _autoRestartSendingMessageFormat;
            set => SetProperty(ref _autoRestartSendingMessageFormat, value);
        }

        public int AutoRestartRebootingWaitMode
        {
            get => _autoRestartRebootingWaitMode;
            set => SetProperty(ref _autoRestartRebootingWaitMode, value);
        }

        public int AutoRestartRebootCoolTime
        {
            get => _autoRestartRebootCoolTime;
            set => SetProperty(ref _autoRestartRebootCoolTime, value);
        }

        public int AutoRestartRebootCoolTimeMode
        {
            get => _autoRestartRebootCoolTimeMode;
            set => SetProperty(ref _autoRestartRebootCoolTimeMode, value);
        }

        public bool IsAutoRestartRunScriptEnabled
        {
            get => _isAutoRestartRunScriptEnabled;
            set => SetProperty(ref _isAutoRestartRunScriptEnabled, value);
        }

        public string AutoRestartRunningScript
        {
            get => _autoRestartRunningScript;
            set => SetProperty(ref _autoRestartRunningScript, value);
        }

        public bool IsAutoRestartWaitRunningScript
        {
            get => _isAutoRestartWaitRunningScript;
            set => SetProperty(ref _isAutoRestartWaitRunningScript, value);
        }
        public int AutoRestartScriptWaitTime
        {
            get => _autoRestartScriptWaitTime;
            set => SetProperty(ref _autoRestartScriptWaitTime, value);
        }
        public int AutoRestartScriptWaitTimeMode
        {
            get => _autoRestartScriptWaitTimeMode;
            set => SetProperty(ref _autoRestartScriptWaitTimeMode, value);
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

        public bool IsRequirePassword => !_setting.CanEncrypt;

        public ShortcutKeyManager ShortcutKeyManager { get; }

        public ISettingMainWindowModel MainWindowModel { get; }


        public SettingModel(SettingLoader setting, ShortcutKeyManager shortcutKeyManager, ISettingMainWindowModel mainWindowModel)
        {
            _setting = setting;
            ShortcutKeyManager = shortcutKeyManager;
            MainWindowModel = mainWindowModel;

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
                IntervalTime = setting.IntervalTime;
                IntervalTimeSelectedIndex = setting.IntervalTimeMode;
                IsAutoRestartSendMessage = setting.IsAutoRestartSendMessage;
                AutoRestartSendingMessageStartTime = setting.AutoRestartSendingMessageStartTime;
                AutoRestartSendingMessageStartTimeMode = setting.AutoRestartSendingMessageStartTimeMode;
                AutoRestartSendingMessageIntervalTime = setting.AutoRestartSendingMessageIntervalTime;
                AutoRestartSendingMessageIntervalTimeMode = setting.AutoRestartSendingMessageIntervalTimeMode;
                AutoRestartSendingMessageFormat = setting.AutoRestartSendingMessageFormat;
                AutoRestartRebootingWaitMode = setting.RebootingWaitMode;
                AutoRestartRebootCoolTime = setting.RebootIntervalTime;
                AutoRestartRebootCoolTimeMode = setting.RebootIntervalTimeMode;
                IsAutoRestartRunScriptEnabled = setting.IsAutoRestartRunScriptEnabled;
                AutoRestartRunningScript = setting.AutoRestartRunningScript;
                IsAutoRestartWaitRunningScript = setting.IsAutoRestartWaitRunningScript;
                AutoRestartScriptWaitTime = setting.AutoRestartScriptWaitTime;
                AutoRestartScriptWaitTimeMode = setting.AutoRestartScriptWaitTimeMode;
                BackupDirPath = setting.BackupDirPath;
                RestoreDirPath = setting.RestoreDirPath;
            }
        }

        public void EnabledEncryptionData(string password, string salt)
        {
            salt = string.IsNullOrEmpty(salt) ? null : salt;
            _setting.SetEncryptionPassword(password, salt);
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
                _setting.IntervalTime = IntervalTime;
                _setting.IntervalTimeMode = IntervalTimeSelectedIndex;
                _setting.IsAutoRestartSendMessage = IsAutoRestartSendMessage;
                _setting.AutoRestartSendingMessageStartTime = AutoRestartSendingMessageStartTime;
                _setting.AutoRestartSendingMessageStartTimeMode = AutoRestartSendingMessageStartTimeMode;
                _setting.AutoRestartSendingMessageIntervalTime = AutoRestartSendingMessageIntervalTime;
                _setting.AutoRestartSendingMessageIntervalTimeMode = AutoRestartSendingMessageIntervalTimeMode;
                _setting.AutoRestartSendingMessageFormat = AutoRestartSendingMessageFormat;
                _setting.RebootingWaitMode = AutoRestartRebootingWaitMode;
                _setting.RebootIntervalTime = AutoRestartRebootCoolTime;
                _setting.RebootIntervalTimeMode = AutoRestartRebootCoolTimeMode;
                _setting.IsAutoRestartRunScriptEnabled = IsAutoRestartRunScriptEnabled;
                _setting.AutoRestartRunningScript = AutoRestartRunningScript;
                _setting.IsAutoRestartWaitRunningScript = IsAutoRestartWaitRunningScript;
                _setting.AutoRestartScriptWaitTime = AutoRestartScriptWaitTime;
                _setting.AutoRestartScriptWaitTimeMode = AutoRestartScriptWaitTimeMode;
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
