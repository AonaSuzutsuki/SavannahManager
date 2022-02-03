using System;
using System.ComponentModel;
using CommonCoreLib.Ini;
using SavannahManagerStyleLib.Models;
using System.IO;

namespace ConfigEditor_mvvm.Models
{
    public sealed class SettingLoader : AbstractSettingLoader
    {
        private const string MainClassName = "Main";
        private const string SftpClassName = "Sftp";
        public const string DirectoryPath = @"C:\";

        private readonly IniLoader _iniLoader;

        #region Properties

        public string OpenDirectoryPath { get; set; } = DirectoryPath;

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
            if (File.Exists(fileName) && CheckOldFormat())
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
