using CommonCoreLib.Ini;
using SavannahManagerStyleLib.Models;
using System.IO;

namespace ConfigEditor_mvvm.Models
{
    public sealed class SettingLoader : AbstractSettingLoader
    {
        private const string MainClassName = "Main";
        public const string DirectoryPath = @"C:\";

        private readonly IniLoader _iniLoader;

        public string OpenDirectoryPath { get; set; } = DirectoryPath;

        public SettingLoader(string fileName)
        {
            _iniLoader = new IniLoader(fileName);
            if (CheckOldFormat())
            {
                LoadOldFormat();
                File.WriteAllText(fileName, "");
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
            OpenDirectoryPath = _iniLoader.GetValue("CONFIGEDITOR", "DIRPATH", DirectoryPath);
        }

        protected override void Load()
        {
            OpenDirectoryPath = _iniLoader.GetValue(MainClassName, "DirectoryPath", DirectoryPath);
        }

        public override void Save()
        {
            _iniLoader.SetValue(MainClassName, "Version", "1.1");
            _iniLoader.SetValue(MainClassName, "DirectoryPath", OpenDirectoryPath);
        }
    }
}
