using CommonCoreLib.Ini;
using SavannahManagerStyleLib.Models;

namespace _7dtd_XmlEditor.Models
{
    public sealed class SettingLoader : AbstractSettingLoader
    {
        private const string MainClassName = "Main";
        private const string SftpClassName = "Sftp";

        private readonly IniLoader _iniLoader;

        #region Properties

        public string OpenDirectoryPath { get; set; }

        public string SftpAddress { get; set; }

        public int SftpPort { get; set; }

        public string SftpUserName { get; set; }

        public string SftpPassword { get; set; }

        public bool SftpIsPassword { get; set; }

        public string SftpKeyPath { get; set; }

        public string SftpDefaultWorkingDirectory { get; set; }

        #endregion

        public SettingLoader(string fileName)
        {
            _iniLoader = new IniLoader(fileName);
            Load();
        }

        protected override void Load()
        {
            OpenDirectoryPath = _iniLoader.GetValue(MainClassName, "DirectoryPath", "C:\\");

            SftpAddress = _iniLoader.GetValue(SftpClassName, nameof(SftpAddress), string.Empty);
            SftpPort = _iniLoader.GetValue(SftpClassName, nameof(SftpPort), 22);
            SftpUserName = _iniLoader.GetValue(SftpClassName, nameof(SftpUserName), string.Empty);
            SftpIsPassword = _iniLoader.GetValue(SftpClassName, nameof(SftpIsPassword), true);
            SftpKeyPath = _iniLoader.GetValue(SftpClassName, nameof(SftpKeyPath), string.Empty);
            SftpDefaultWorkingDirectory = _iniLoader.GetValue(SftpClassName, nameof(SftpDefaultWorkingDirectory), string.Empty);

#if DEBUG
            SftpPassword = _iniLoader.GetValue(SftpClassName, nameof(SftpPassword), string.Empty);
#endif
        }

        public override void Save()
        {
            _iniLoader.SetValue(MainClassName, "Version", "1.1");
            _iniLoader.SetValue(MainClassName, "DirectoryPath", OpenDirectoryPath);

            _iniLoader.SetValue(SftpClassName, nameof(SftpAddress), SftpAddress);
            _iniLoader.SetValue(SftpClassName, nameof(SftpPort), SftpPort);
            _iniLoader.SetValue(SftpClassName, nameof(SftpUserName), SftpUserName);
            _iniLoader.SetValue(SftpClassName, nameof(SftpIsPassword), SftpIsPassword);
            _iniLoader.SetValue(SftpClassName, nameof(SftpKeyPath), SftpKeyPath);
            _iniLoader.SetValue(SftpClassName, nameof(SftpDefaultWorkingDirectory), SftpDefaultWorkingDirectory);

#if DEBUG
            _iniLoader.SetValue(SftpClassName, nameof(SftpPassword), SftpPassword);
#endif
        }
    }
}
