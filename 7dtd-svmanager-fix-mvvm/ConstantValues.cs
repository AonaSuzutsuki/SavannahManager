using CommonCoreLib;

namespace _7dtd_svmanager_fix_mvvm
{
    public static class ConstantValues
    {
        public const int DefaultPort = 8081;
        public const int Width = 900;
        public const int Height = 550;

        public const string SteamLibraryPath = @"\steamapps\libraryfolders.vdf";
        public const string RegSteamPath = "SOFTWARE\\Valve\\Steam";
        public const string RegSteamKey = "SteamPath";

        public const string GameClientName = "7DaysToDie.exe";
        public const string ServerClientName = "7DaysToDieServer.exe";
        public const string ServerConfigName = "serverconfig.xml";
        public const string GameClientPath = @"\steamapps\common\7 Days to Die";
        public const string ServerClientPath = @"\steamapps\common\7 Days to Die Dedicated Server";

        public const string DefaultDirectoryPath = @"C:\";

        public const string ExternalIpUrl = "https://aonsztk.xyz/api/?mode=externalip";

        public static readonly string AppDirectoryPath = AppInfo.GetAppPath();
        public static readonly string LogDirectoryPath = AppDirectoryPath + @"\logs\";
        public static readonly string SettingFilePath = AppDirectoryPath + @"\settings.ini";
        public static readonly string ConfigEditorFilePath = AppDirectoryPath + @"\ConfigEditor.exe";
        public static readonly string UpdaterFilePath = AppDirectoryPath + @"\Updater\update.exe";

        public static string Version => CommonCoreLib.CommonFile.Version.GetVersion(); /* + "b";*/
    }
}
