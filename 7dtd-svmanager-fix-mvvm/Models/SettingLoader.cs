using KimamaLib;
using KimamaLib.Ini;
using LanguageEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7dtd_svmanager_fix_mvvm.Models
{
    public class SettingLoader
    {
        /// <summary>
        /// For sharing instance.
        /// </summary>
        public static SettingLoader Setting { private set; get; }

        static SettingLoader()
        {
            Setting = new SettingLoader(ConstantValues.SettingFilePath);
        }

        #region Properties
        private int width = CommonLib.StaticData.Width;
        public int Width
        {
            get => width;
            set
            {
                width = value;
                iniLoader.SetValue("MAIN", "WIDTH", value);
            }
        }
        private int height = CommonLib.StaticData.Height;
        public int Height
        {
            get => height;
            set
            {
                height = value;
                iniLoader.SetValue("MAIN", "HEIGHT", value);
            }
        }

        private string exeFilePath = string.Empty;
        public string ExeFilePath
        {
            get => exeFilePath;
            set
            {
                exeFilePath = value;
                iniLoader.SetValue("SERVER", "EXEPATH", value);
            }
        }
        private string configFilePath = string.Empty;
        public string ConfigFilePath
        {
            get => configFilePath;
            set
            {
                configFilePath = value;
                iniLoader.SetValue("SERVER", "CONFIGPATH", value);
            }
        }
        private string adminFilePath = string.Empty;
        public string AdminFilePath
        {
            get => adminFilePath;
            set
            {
                adminFilePath = value;
                iniLoader.SetValue("SERVER", "ADMINPATH", value);
            }
        }

        private string address = string.Empty;
        public string Address
        {
            get => address;
            set
            {
                address = value;
                iniLoader.SetValue("MAIN", "ADDRESS", value);
            }
        }
        private int port = ConstantValues.DefaultPort;
        public int Port
        {
            get => port;
            set
            {
                port = value;
                iniLoader.SetValue("MAIN", "PORT", value);
            }
        }
        private string password = string.Empty;
        public string Password
        {
            get => password;
            set
            {
                password = value;
                iniLoader.SetValue("MAIN", "PASSWORD", value);
            }
        }

        private bool localMode = true;
        public bool LocalMode
        {
            get => localMode;
            set
            {
                localMode = value;
                iniLoader.SetValue("MAIN", "LOCALMODE", value);
            }
        }

        private string cultureName = string.Empty;
        public string CultureName
        {
            get => cultureName;
            set
            {
                cultureName = value;
                ResourceService.Current.ChangeCulture(value);
                iniLoader.SetValue("MAIN", "CULTURE", value);
            }
        }

        private int consoleTextLength = 32768;
        public int ConsoleTextLength
        {
            get => consoleTextLength;
            set
            {
                consoleTextLength = value;
                iniLoader.SetValue("SERVER", "CONSOLELOGLENGTH", value);
            }
        }

        private bool isBetaMode = true;
        public bool IsBetaMode
        {
            get => isBetaMode;
            set
            {
                isBetaMode = value;
                iniLoader.SetValue("SERVER", "BETAMODE", value);
            }
        }

        private bool isLogGetter;
        public bool IsLogGetter
        {
            get => isLogGetter;
            set
            {
                isLogGetter = value;
                iniLoader.SetValue("SERVER", "LOGOUTPUT", value);
            }
        }

        private bool isFirstBoot = true;
        public bool IsFirstBoot
        {
            get => isFirstBoot;
            set
            {
                isFirstBoot = value;
                iniLoader.SetValue("MAIN", "FIRSTBOOT", value);
            }
        }

        private bool isAutoUpdate = true;
        public bool IsAutoUpdate
        {
            get => isAutoUpdate;
            set
            {
                isAutoUpdate = value;
                iniLoader.SetValue("MAIN", "AUTOCHECK", value);
            }
        }
        #endregion


        private IniLoader iniLoader;
        public SettingLoader(string filename)
        {
            iniLoader = new IniLoader(filename);
            Load();
        }

        private void Load()
        {
            if (!int.TryParse(iniLoader.GetValue("MAIN", "WIDTH", "900"), out int width)) { width = CommonLib.StaticData.Width; }
            if (!int.TryParse(iniLoader.GetValue("MAIN", "HEIGHT", "550"), out int height)) { height = CommonLib.StaticData.Height; }
            this.width = width;
            this.height = height;

            exeFilePath = iniLoader.GetValue("SERVER", "EXEPATH", string.Empty);
            configFilePath = iniLoader.GetValue("SERVER", "CONFIGPATH", string.Empty);
            adminFilePath = iniLoader.GetValue("SERVER", "ADMINPATH", string.Empty);

            address = iniLoader.GetValue("MAIN", "ADDRESS", "");

            if (!int.TryParse(iniLoader.GetValue("MAIN", "PORT", ""), out int port))
            {
                port = ConstantValues.DefaultPort;
            }
            this.port = port;

            password = iniLoader.GetValue("MAIN", "PASSWORD", "");

            localMode = iniLoader.GetValue("MAIN", "LOCALMODE", true);

            cultureName = iniLoader.GetValue("MAIN", "CULTURE", ResourceService.Current.Culture);
            ResourceService.Current.ChangeCulture(cultureName);

            consoleTextLength = iniLoader.GetValue("SERVER", "CONSOLELOGLENGTH", consoleTextLength);

            isBetaMode = iniLoader.GetValue("SERVER", "BETAMODE", false);

            isLogGetter = iniLoader.GetValue("SERVER", "LOGOUTPUT", true);

            isFirstBoot = iniLoader.GetValue("MAIN", "FIRSTBOOT", true);

            isAutoUpdate = iniLoader.GetValue("SERVER", "AUTOCHECK", true);
        }
    }
}
