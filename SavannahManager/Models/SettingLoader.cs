using System;
using System.Diagnostics;
using System.IO;
using _7dtd_svmanager_fix_mvvm.LangResources;
using _7dtd_svmanager_fix_mvvm.Views.UserControls;
using CommonCoreLib.CommonPath;
using CommonCoreLib.Ini;
using SavannahManagerStyleLib.Models;
using SvManagerLibrary.Crypto;

namespace _7dtd_svmanager_fix_mvvm.Models
{
    public sealed class SettingLoader : AbstractSettingLoader, IDisposable
    {

        private const string MainClassName = "Main";
        private const string AutoRestartClassName = "AutoRestart";
        private const string ServerClassName = "Server";
        private const string BackupClassName = "Backup";

        #region Properties

        public int Width { get; set; } = ConstantValues.Width;

        public int Height { get; set; } = ConstantValues.Height;

        public string ExeFilePath { get; set; } = string.Empty;

        public string ConfigFilePath { get; set; } = string.Empty;

        public string AdminFilePath { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public int Port { get; set; } = ConstantValues.DefaultPort;

        public string Password { get; set; } = string.Empty;

        public bool LocalMode { get; set; } = true;

        public string CultureName { get; set; } = string.Empty;

        public int ConsoleTextLength { get; set; } = 32768;

        public int TelnetWaitTime { get; set; } = 2;

        public bool IsBetaMode { get; set; } = true;

        public bool IsLogGetter { get; set; }

        public bool IsFirstBoot { get; set; } = true;

        public bool IsAutoUpdate { get; set; }

        public bool IsEncryptPassword { get; set; }


        #region Auto Restart

        public int IntervalTime { get; set; }

        public int IntervalTimeMode { get; set; }

        public bool IsAutoRestartSendMessage { get; set; }

        public int AutoRestartSendingMessageStartTime { get; set; }

        public int AutoRestartSendingMessageStartTimeMode { get; set; }

        public int AutoRestartSendingMessageIntervalTime { get; set; }

        public int AutoRestartSendingMessageIntervalTimeMode { get; set; }

        public string AutoRestartSendingMessageFormat { get; set; }

        #endregion


        public string BackupDirPath { get; set; }

        public string RestoreDirPath { get; set; }

        public bool IsConsoleLogTextWrapping { get; set; }


        public string SshAddress { get; set; }

        public int SshPort { get; set; }

        public string SshUserName { get; set; }

        public string SshPassword { get; set; }

        public string SshExeFileDirectory { get; set; }
        public string SshShellScriptFileName { get; set; }

        public string SshArgument { get; set; }

        public int SshAuthMode { get; set; }

        public string SshKeyPath { get; set; }

        public bool CanEncrypt => _encryptWrapper != null;

        #endregion


        private readonly IniLoader _iniLoader;
        private RijndaelWrapper _encryptWrapper;

        public SettingLoader(string filename)
        {
            _iniLoader = new IniLoader(filename);
            if (File.Exists(filename) && CheckOldFormat())
            {
                LoadOldFormat();
                File.WriteAllText(filename, "");
            }
            else
            {
                Load();
            }
        }

        private bool CheckOldFormat()
        {
            var value = _iniLoader.GetValue(MainClassName, "Version", "1.0");
            return value == "1.0";
        }

        private void LoadOldFormat()
        {
            Width = _iniLoader.GetValue("MAIN", "WIDTH", 900);
            Height = _iniLoader.GetValue("MAIN", "HEIGHT", 550);

            ExeFilePath = _iniLoader.GetValue("SERVER", "EXEPATH", string.Empty);
            ConfigFilePath = _iniLoader.GetValue("SERVER", "CONFIGPATH", string.Empty);
            AdminFilePath = _iniLoader.GetValue("SERVER", "ADMINPATH", string.Empty);

            Address = _iniLoader.GetValue("MAIN", "ADDRESS", "");

            Port = _iniLoader.GetValue("MAIN", "PORT", 8081);

            Password = _iniLoader.GetValue("MAIN", "PASSWORD", "");

            LocalMode = _iniLoader.GetValue("MAIN", "LOCALMODE", true);

            CultureName = _iniLoader.GetValue("MAIN", "CULTURE", ResourceService.English);

            ConsoleTextLength = _iniLoader.GetValue("SERVER", "CONSOLELOGLENGTH", ConsoleTextLength);

            TelnetWaitTime = _iniLoader.GetValue("SERVER", "TELNETWAITTIME", 2000);

            IsBetaMode = _iniLoader.GetValue("SERVER", "BETAMODE", false);

            IsLogGetter = _iniLoader.GetValue("SERVER", "LOGOUTPUT", true);

            IsFirstBoot = _iniLoader.GetValue("MAIN", "FIRSTBOOT", true);

            IsAutoUpdate = _iniLoader.GetValue("MAIN", "AUTOCHECK", true);

            BackupDirPath = _iniLoader.GetValue("BACKUP", "DIRPATH", "backup").UnifiedSystemPathSeparator();

            IsConsoleLogTextWrapping = _iniLoader.GetValue("MAIN", "CONSOLETEXTWRAPPING", false);
        }

        protected override void Load()
        {
            IsEncryptPassword = _iniLoader.GetValue(MainClassName, "IsEncryptPassword", false);
            if (!IsEncryptPassword)
            {
                Password = _iniLoader.GetValue(ServerClassName, "Password", "");
            }

            Width = _iniLoader.GetValue(MainClassName, "Width", 900);
            Height = _iniLoader.GetValue(MainClassName, "Height", 550);

            ExeFilePath = _iniLoader.GetValue(ServerClassName, "ExePath", string.Empty);
            ConfigFilePath = _iniLoader.GetValue(ServerClassName, "ConfigPath", string.Empty);
            AdminFilePath = _iniLoader.GetValue(ServerClassName, "AdminPath", string.Empty);

            Address = _iniLoader.GetValue(ServerClassName, "Address", "");

            Port = _iniLoader.GetValue(ServerClassName, "Port", 8081);

            LocalMode = _iniLoader.GetValue(MainClassName, "LocalServerMode", true);

            CultureName = _iniLoader.GetValue(MainClassName, "Culture", ResourceService.English);

            ConsoleTextLength = _iniLoader.GetValue(MainClassName, "ConsoleLogLength", ConsoleTextLength);

            TelnetWaitTime = _iniLoader.GetValue(MainClassName, "TelnetWaitTime", 2000);

            IsBetaMode = _iniLoader.GetValue(MainClassName, "BetaMode", false);

            IsLogGetter = _iniLoader.GetValue(MainClassName, "IsLogOutput", true);

            IsFirstBoot = _iniLoader.GetValue(MainClassName, "IsFirstBoot", true);

            IsAutoUpdate = _iniLoader.GetValue(MainClassName, "IsUpdateCheck", true);

            IntervalTime = _iniLoader.GetValue(AutoRestartClassName, "IntervalTime", 5);
            IntervalTimeMode = _iniLoader.GetValue(AutoRestartClassName, "IntervalTimeMode", 2);
            IsAutoRestartSendMessage = _iniLoader.GetValue(AutoRestartClassName, "IsSendMessage", false);
            AutoRestartSendingMessageStartTime =
                _iniLoader.GetValue(AutoRestartClassName, "SendingMessageStartTime", 1);
            AutoRestartSendingMessageStartTimeMode =
                _iniLoader.GetValue(AutoRestartClassName, "SendingMessageStartTimeMode", 1);
            AutoRestartSendingMessageIntervalTime =
                _iniLoader.GetValue(AutoRestartClassName, "SendingMessageIntervalTime", 10);
            AutoRestartSendingMessageIntervalTimeMode =
                _iniLoader.GetValue(AutoRestartClassName, "SendingMessageIntervalTimeMode", 0);
            AutoRestartSendingMessageFormat = _iniLoader.GetValue(MainClassName, "SendingMessageFormat", 
                    "Restart the server after {0} seconds.");

            BackupDirPath = _iniLoader.GetValue(BackupClassName, "DirPath", "backup").UnifiedSystemPathSeparator();
            RestoreDirPath = _iniLoader.GetValue(BackupClassName, "RestoreDirPath", "").UnifiedSystemPathSeparator();

            IsConsoleLogTextWrapping = _iniLoader.GetValue(MainClassName, "IsConsoleTextWrapping", false);

            SshAddress = _iniLoader.GetValue(ServerClassName, "SshAddress", "");
            SshPort = _iniLoader.GetValue(ServerClassName, "SshPort", 22);
            SshUserName = _iniLoader.GetValue(ServerClassName, "SshUserName", "");
            SshExeFileDirectory = _iniLoader.GetValue(ServerClassName, "SshExeFileDirectory", "");
            SshShellScriptFileName = _iniLoader.GetValue(ServerClassName, "SshShellScriptFileName", "");
            SshArgument = _iniLoader.GetValue(ServerClassName, "SshArgument", "-configfile=serverconfig.xml");
            SshAuthMode = _iniLoader.GetValue(ServerClassName, "SshAuthMode", AuthMode.Password.ToInt());
            SshKeyPath = _iniLoader.GetValue(ServerClassName, "SshKeyPath", "");
        }

        public void SetEncryptionPassword(string password, string salt)
        {
            salt ??= Environment.MachineName;
            _encryptWrapper = new RijndaelWrapper(password, CommonCoreLib.Crypto.Sha256.GetSha256(salt));
        }

        public void LoadEncryptionData()
        {
            var encryptedPassword = _iniLoader.GetValue(ServerClassName, "Password", "");
            var encryptedSshPassword = _iniLoader.GetValue(ServerClassName, "SshPassword", "");

            try
            {
                Password = _encryptWrapper.Decrypt(encryptedPassword);
                SshPassword = _encryptWrapper.Decrypt(encryptedSshPassword);
            }
            catch
            {
                Password = "";
                SshPassword = "";
                _encryptWrapper = null;
                throw;
            }
        }

        public void ApplyCulture()
        {
            ResourceService.Current.ChangeCulture(CultureName);
        }

        public override void Save()
        {
            _iniLoader.SetValue(MainClassName, "Version", "1.1");

            _iniLoader.SetValue(MainClassName, "IsEncryptPassword", IsEncryptPassword);
            if (!IsEncryptPassword)
            {
                _iniLoader.SetValue(ServerClassName, "Password", Password);
            }
            else
            {
                if (_encryptWrapper != null && !string.IsNullOrEmpty(Password))
                {
                    _iniLoader.SetValue(ServerClassName, "Password", _encryptWrapper.Encrypt(Password ?? string.Empty));
                    _iniLoader.SetValue(ServerClassName, "SshPassword", _encryptWrapper.Encrypt(SshPassword ?? string.Empty));
                }
            }

            _iniLoader.SetValue(MainClassName, "Width", Width);
            _iniLoader.SetValue(MainClassName, "Height", Height);
            _iniLoader.SetValue(ServerClassName, "ExePath", ExeFilePath);
            _iniLoader.SetValue(ServerClassName, "ConfigPath", ConfigFilePath);
            _iniLoader.SetValue(ServerClassName, "AdminPath", AdminFilePath);
            _iniLoader.SetValue(ServerClassName, "Address", Address);
            _iniLoader.SetValue(ServerClassName, "Port", Port);
            _iniLoader.SetValue(MainClassName, "LocalServerMode", LocalMode);
            _iniLoader.SetValue(MainClassName, "Culture", CultureName);
            _iniLoader.SetValue(MainClassName, "ConsoleLogLength", ConsoleTextLength);
            _iniLoader.SetValue(MainClassName, "TelnetWaitTime", TelnetWaitTime);
            _iniLoader.SetValue(MainClassName, "BetaMode", IsBetaMode);
            _iniLoader.SetValue(MainClassName, "IsLogOutput", IsLogGetter);
            _iniLoader.SetValue(MainClassName, "IsFirstBoot", IsFirstBoot);
            _iniLoader.SetValue(MainClassName, "IsUpdateCheck", IsAutoUpdate);
            _iniLoader.SetValue(AutoRestartClassName, "IntervalTime", IntervalTime);
            _iniLoader.SetValue(AutoRestartClassName, "IntervalTimeMode", IntervalTimeMode);
            _iniLoader.SetValue(AutoRestartClassName, "IsSendMessage", IsAutoRestartSendMessage);
            _iniLoader.SetValue(AutoRestartClassName, "SendingMessageStartTime", AutoRestartSendingMessageStartTime);
            _iniLoader.SetValue(AutoRestartClassName, "SendingMessageStartTimeMode", AutoRestartSendingMessageStartTimeMode);
            _iniLoader.SetValue(AutoRestartClassName, "SendingMessageIntervalTime", AutoRestartSendingMessageIntervalTime);
            _iniLoader.SetValue(AutoRestartClassName, "SendingMessageIntervalTimeMode", AutoRestartSendingMessageIntervalTimeMode);
            _iniLoader.SetValue(AutoRestartClassName, "SendingMessageFormat", AutoRestartSendingMessageFormat);
            _iniLoader.SetValue(BackupClassName, "DirPath", BackupDirPath);
            _iniLoader.SetValue(BackupClassName, "RestoreDirPath", RestoreDirPath);
            _iniLoader.SetValue(MainClassName, "IsConsoleTextWrapping", IsConsoleLogTextWrapping);
            _iniLoader.SetValue(ServerClassName, "SshAddress", SshAddress);
            _iniLoader.SetValue(ServerClassName, "SshPort", SshPort);
            _iniLoader.SetValue(ServerClassName, "SshUserName", SshUserName);
            _iniLoader.SetValue(ServerClassName, "SshShellScriptFileName", SshShellScriptFileName);
            _iniLoader.SetValue(ServerClassName, "SshExeFileDirectory", SshExeFileDirectory);
            _iniLoader.SetValue(ServerClassName, "SshArgument", SshArgument);
            _iniLoader.SetValue(ServerClassName, "SshAuthMode", SshAuthMode);
            _iniLoader.SetValue(ServerClassName, "SshKeyPath", SshKeyPath);
        }

        public void Dispose()
        {
            _encryptWrapper?.Dispose();
        }
    }
}
