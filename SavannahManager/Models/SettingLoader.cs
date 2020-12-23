using _7dtd_svmanager_fix_mvvm.LangResources;
using CommonCoreLib.CommonPath;
using CommonCoreLib.Ini;

namespace _7dtd_svmanager_fix_mvvm.Models
{
    public sealed class SettingLoader
    {
        /// <summary>
        /// Singleton SettingLoader instance for all.
        /// </summary>
        public static SettingLoader SettingInstance { private set; get; }

        static SettingLoader()
        {
            SettingInstance = new SettingLoader(ConstantValues.SettingFilePath);
        }

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

        public string BackupDirPath { get; set; }

        public bool IsConsoleLogTextWrapping { get; set; }

        #endregion


        private readonly IniLoader _iniLoader;
        public SettingLoader(string filename)
        {
            _iniLoader = new IniLoader(filename);
            Load();
        }

        private void Load()
        {
            if (!int.TryParse(_iniLoader.GetValue("MAIN", "WIDTH", "900"), out var width)) { width = ConstantValues.Width; }
            if (!int.TryParse(_iniLoader.GetValue("MAIN", "HEIGHT", "550"), out var height)) { height = ConstantValues.Height; }
            Width = width;
            Height = height;

            ExeFilePath = _iniLoader.GetValue("SERVER", "EXEPATH", string.Empty);
            ConfigFilePath = _iniLoader.GetValue("SERVER", "CONFIGPATH", string.Empty);
            AdminFilePath = _iniLoader.GetValue("SERVER", "ADMINPATH", string.Empty);

            Address = _iniLoader.GetValue("MAIN", "ADDRESS", "");

            if (!int.TryParse(_iniLoader.GetValue("MAIN", "PORT", ""), out var port))
            {
                port = ConstantValues.DefaultPort;
            }
            Port = port;

            Password = _iniLoader.GetValue("MAIN", "PASSWORD", "");

            LocalMode = _iniLoader.GetValue("MAIN", "LOCALMODE", true);

            CultureName = _iniLoader.GetValue("MAIN", "CULTURE", ResourceService.Current.Culture);
            ResourceService.Current.ChangeCulture(CultureName);

            ConsoleTextLength = _iniLoader.GetValue("SERVER", "CONSOLELOGLENGTH", ConsoleTextLength);

            TelnetWaitTime = _iniLoader.GetValue("SERVER", "TELNETWAITTIME", 2000);

            IsBetaMode = _iniLoader.GetValue("SERVER", "BETAMODE", false);

            IsLogGetter = _iniLoader.GetValue("SERVER", "LOGOUTPUT", true);

            IsFirstBoot = _iniLoader.GetValue("MAIN", "FIRSTBOOT", true);

            IsAutoUpdate = _iniLoader.GetValue("MAIN", "AUTOCHECK", true);

            BackupDirPath = _iniLoader.GetValue("BACKUP", "DIRPATH", "backup").UnifiedSystemPathSeparator();

            IsConsoleLogTextWrapping = _iniLoader.GetValue("MAIN", "CONSOLETEXTWRAPPING", false);
        }

        public void ApplyCulture()
        {
            ResourceService.Current.ChangeCulture(CultureName);
        }

        public void Save()
        {
            _iniLoader.SetValue("MAIN", "WIDTH", Width);
            _iniLoader.SetValue("MAIN", "HEIGHT", Height);
            _iniLoader.SetValue("SERVER", "EXEPATH", ExeFilePath);
            _iniLoader.SetValue("SERVER", "CONFIGPATH", ConfigFilePath);
            _iniLoader.SetValue("SERVER", "ADMINPATH", AdminFilePath);
            _iniLoader.SetValue("MAIN", "ADDRESS", Address);
            _iniLoader.SetValue("MAIN", "PORT", Port);
            _iniLoader.SetValue("MAIN", "PASSWORD", Password);
            _iniLoader.SetValue("MAIN", "LOCALMODE", LocalMode);
            _iniLoader.SetValue("MAIN", "CULTURE", CultureName);
            _iniLoader.SetValue("SERVER", "CONSOLELOGLENGTH", ConsoleTextLength);
            _iniLoader.SetValue("SERVER", "TELNETWAITTIME", TelnetWaitTime);
            _iniLoader.SetValue("SERVER", "BETAMODE", IsBetaMode);
            _iniLoader.SetValue("SERVER", "LOGOUTPUT", IsLogGetter);
            _iniLoader.SetValue("MAIN", "FIRSTBOOT", IsFirstBoot);
            _iniLoader.SetValue("MAIN", "AUTOCHECK", IsAutoUpdate);
            _iniLoader.SetValue("BACKUP", "DIRPATH", BackupDirPath);
            _iniLoader.SetValue("MAIN", "CONSOLETEXTWRAPPING", IsConsoleLogTextWrapping);
        }
    }
}
