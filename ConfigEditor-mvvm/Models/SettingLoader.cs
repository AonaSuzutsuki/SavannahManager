using CommonCoreLib.Ini;

namespace ConfigEditor_mvvm.Models
{
    public class SettingLoader
    {
        public const string DirectoryPath = @"C:\";

        private string _openDirectoryPath = DirectoryPath;
        public string OpenDirectoryPath
        {
            get => _openDirectoryPath;
            set
            {
                _openDirectoryPath = value;
                iniLoader.SetValue("CONFIGEDITOR", "DIRPATH", value);
            }
        }

        IniLoader iniLoader;

        public SettingLoader(string fileName)
        {
            iniLoader = new IniLoader(fileName);
            OpenDirectoryPath = iniLoader.GetValue("CONFIGEDITOR", "DIRPATH", DirectoryPath);
        }
    }
}
