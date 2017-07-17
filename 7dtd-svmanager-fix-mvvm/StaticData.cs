using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace _7dtd_svmanager_fix_mvvm
{
    public static class StaticData
    {
        private static Color GetColor(string hexCode)
        {
            return (Color)ColorConverter.ConvertFromString(hexCode);
        }

        public const int Width = 900;
        public const int Height = 550;
        public const int Port = 8081;

        public static SolidColorBrush ActivatedBorderColor { get; } = new SolidColorBrush(GetColor("#4090ff"));
        public static SolidColorBrush ActivatedBorderColor2 { get; } = new SolidColorBrush(GetColor("#ff6b00"));
        public static SolidColorBrush DeactivatedBorderColor { get; } = new SolidColorBrush(GetColor("#4090ff66"));
        
        public const string RegSteamPath = "SOFTWARE\\Valve\\Steam";
        public const string RegSteamKey = "SteamPath";
        public const string GameClientName = "7DaysToDie.exe";
        public const string ServerClientName = "7DaysToDieServer.exe";
        public const string ServerConfigName = "serverconfig.xml";
        public const string GameClientPath = @"\steamapps\common\7 Days to Die";
        public const string ServerClientPath = @"\steamapps\common\7 Days to Die Dedicated Server";
        public const string SteamLibraryPath = @"\steamapps\libraryfolders.vdf";

        public const string DirectoryPath = @"C:\";

    }
}
