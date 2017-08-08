using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigEditor_mvvm.Models
{
    public class SettingLoader
    {
        public const string DirectoryPath = @"C:\";

        private string openDirectoryPath = DirectoryPath;
        public string OpenDirectoryPath
        {
            get => openDirectoryPath;
            set
            {
                openDirectoryPath = value;
                iniLoader.SetValue("CONFIGEDITOR", "DIRPATH", value);
            }
        }

        KimamaLib.Ini.IniLoader iniLoader;

        public SettingLoader(string fileName)
        {
            iniLoader = new KimamaLib.Ini.IniLoader(fileName);
            OpenDirectoryPath = iniLoader.GetValue("CONFIGEDITOR", "DIRPATH", DirectoryPath);
        }
    }
}
