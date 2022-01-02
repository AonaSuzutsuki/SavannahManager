using System;
using System.Diagnostics;
using System.IO;
using _7dtd_svmanager_fix_mvvm.LangResources;
using CommonCoreLib.CommonPath;
using CommonCoreLib.Ini;
using SvManagerLibrary.Crypto;

namespace _7dtd_svmanager_fix_mvvm.Models
{
    public sealed class SettingLoader : IDisposable
    {

        private const string MainClassName = "Main";
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

        public string BackupDirPath { get; set; }

        public string RestoreDirPath { get; set; }

        public bool IsConsoleLogTextWrapping { get; set; }


        public bool CanEncrypt => _encryptWrapper != null;

        #endregion


        private readonly IniLoader _iniLoader;
        private RijndaelWrapper _encryptWrapper;

        public SettingLoader(string filename)
        {
            _iniLoader = new IniLoader(filename);
            if (CheckOldFormat())
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
            var value = _iniLoader.GetValue(MainClassName, "Version", "1.1");
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

        private void Load()
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

            BackupDirPath = _iniLoader.GetValue(BackupClassName, "DirPath", "backup").UnifiedSystemPathSeparator();

            RestoreDirPath = _iniLoader.GetValue(BackupClassName, "RestoreDirPath", "").UnifiedSystemPathSeparator();

            IsConsoleLogTextWrapping = _iniLoader.GetValue(MainClassName, "IsConsoleTextWrapping", false);
        }

        public void SetEncryptionPassword(string password)
        {
            _encryptWrapper = new RijndaelWrapper(password, "9BBF8AA1-227C-4670-BF4B-DC279E254B03");
        }

        public void LoadEncryptionData()
        {
            var encryptedPassword = _iniLoader.GetValue(ServerClassName, "Password", "");

            try
            {
                Password = _encryptWrapper.Decrypt(encryptedPassword);
            }
            catch
            {
                Password = "";
                _encryptWrapper = null;
                throw;
            }
        }

        public void ApplyCulture()
        {
            ResourceService.Current.ChangeCulture(CultureName);
        }

        public void Save()
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
                    _iniLoader.SetValue(ServerClassName, "Password", _encryptWrapper.Encrypt(Password));
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
            _iniLoader.SetValue(BackupClassName, "DirPath", BackupDirPath);
            _iniLoader.SetValue(BackupClassName, "RestoreDirPath", RestoreDirPath);
            _iniLoader.SetValue(MainClassName, "IsConsoleTextWrapping", IsConsoleLogTextWrapping);
        }

        public void Dispose()
        {
            _encryptWrapper?.Dispose();
        }
    }
}
