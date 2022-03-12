using CommonCoreLib;

namespace _7dtd_XmlEditor
{
    public static class ConstantValues
    {
        /// <summary>
        /// Execution file absolutely path.
        /// </summary>
        public static readonly string AppDirectoryPath = AppInfo.GetAppPath();
        /// <summary>
        /// Path of file for settings.
        /// </summary>
        public static readonly string SettingFilePath = AppDirectoryPath + @"\settings.ini";
    }
}
