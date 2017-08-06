﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace _7dtd_svmanager_fix_mvvm
{
    public static class StaticData
    {
        public const int Port = 8081;

        public const string RegSteamPath = "SOFTWARE\\Valve\\Steam";
        public const string RegSteamKey = "SteamPath";
        public const string GameClientName = "7DaysToDie.exe";
        public const string ServerClientName = "7DaysToDieServer.exe";
        public const string ServerConfigName = "serverconfig.xml";
        public const string GameClientPath = @"\steamapps\common\7 Days to Die";
        public const string ServerClientPath = @"\steamapps\common\7 Days to Die Dedicated Server";
        public const string SteamLibraryPath = @"\steamapps\libraryfolders.vdf";

        public const string DirectoryPath = @"C:\";
        public static readonly string AppDirectoryPath = KimamaLib.AppInfo.GetAppPath();
        public static readonly string LogDirectoryPath = AppDirectoryPath + @"\logs\";
    }
}