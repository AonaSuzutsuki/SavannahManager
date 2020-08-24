using CommonCoreLib.CommonPath;
using CommonCoreLib.Ini;
using LanguageEx;

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

        public bool IsBetaMode { get; set; } = true;

        public bool IsLogGetter { get; set; }

        public bool IsFirstBoot { get; set; } = true;

        public bool IsAutoUpdate { get; set; }

        public string BackupDirPath { get; set; }

        #endregion


        private readonly IniLoader iniLoader;
        public SettingLoader(string filename)
        {
            iniLoader = new IniLoader(filename);
            Load();
        }

        private void Load()
        {
            if (!int.TryParse(iniLoader.GetValue("MAIN", "WIDTH", "900"), out int width)) { width = ConstantValues.Width; }
            if (!int.TryParse(iniLoader.GetValue("MAIN", "HEIGHT", "550"), out int height)) { height = ConstantValues.Height; }
            this.Width = width;
            this.Height = height;

            ExeFilePath = iniLoader.GetValue("SERVER", "EXEPATH", string.Empty);
            ConfigFilePath = iniLoader.GetValue("SERVER", "CONFIGPATH", string.Empty);
            AdminFilePath = iniLoader.GetValue("SERVER", "ADMINPATH", string.Empty);

            Address = iniLoader.GetValue("MAIN", "ADDRESS", "");

            if (!int.TryParse(iniLoader.GetValue("MAIN", "PORT", ""), out int port))
            {
                port = ConstantValues.DefaultPort;
            }
            this.Port = port;

            Password = iniLoader.GetValue("MAIN", "PASSWORD", "");

            LocalMode = iniLoader.GetValue("MAIN", "LOCALMODE", true);

            CultureName = iniLoader.GetValue("MAIN", "CULTURE", ResourceService.Current.Culture);
            ResourceService.Current.ChangeCulture(CultureName);

            ConsoleTextLength = iniLoader.GetValue("SERVER", "CONSOLELOGLENGTH", ConsoleTextLength);

            IsBetaMode = iniLoader.GetValue("SERVER", "BETAMODE", false);

            IsLogGetter = iniLoader.GetValue("SERVER", "LOGOUTPUT", true);

            IsFirstBoot = iniLoader.GetValue("MAIN", "FIRSTBOOT", true);

            IsAutoUpdate = iniLoader.GetValue("MAIN", "AUTOCHECK", true);

            BackupDirPath = iniLoader.GetValue("BACKUP", "DIRPATH", "backup").UnifiedSystemPathSeparator();
        }

        public void ApplyCulture()
        {
            ResourceService.Current.ChangeCulture(CultureName);
        }

        public void Save()
        {
            iniLoader.SetValue("MAIN", "WIDTH", Width);
            iniLoader.SetValue("MAIN", "HEIGHT", Height);
            iniLoader.SetValue("SERVER", "EXEPATH", ExeFilePath);
            iniLoader.SetValue("SERVER", "CONFIGPATH", ConfigFilePath);
            iniLoader.SetValue("SERVER", "ADMINPATH", AdminFilePath);
            iniLoader.SetValue("MAIN", "ADDRESS", Address);
            iniLoader.SetValue("MAIN", "PORT", Port);
            iniLoader.SetValue("MAIN", "PASSWORD", Password);
            iniLoader.SetValue("MAIN", "LOCALMODE", LocalMode);
            iniLoader.SetValue("MAIN", "CULTURE", CultureName);
            iniLoader.SetValue("SERVER", "CONSOLELOGLENGTH", ConsoleTextLength);
            iniLoader.SetValue("SERVER", "BETAMODE", IsBetaMode);
            iniLoader.SetValue("SERVER", "LOGOUTPUT", IsLogGetter);
            iniLoader.SetValue("MAIN", "FIRSTBOOT", IsFirstBoot);
            iniLoader.SetValue("MAIN", "AUTOCHECK", IsAutoUpdate);
            iniLoader.SetValue("BACKUP", "DIRPATH", BackupDirPath);
        }
    }
}
