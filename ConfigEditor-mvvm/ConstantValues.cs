using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigEditor_mvvm
{
    public static class ConstantValues
    {
        /// <summary>
        /// Execution file absolutely path.
        /// </summary>
        public static readonly string AppDirectoryPath = KimamaLib.AppInfo.GetAppPath();
        /// <summary>
        /// Path of managing versionList.
        /// </summary>
        public static readonly string VersionListPath = AppDirectoryPath + @"\lang\VersionList.xml";
        /// <summary>
        /// Path of file for settings.
        /// </summary>
        public static readonly string SettingFilePath = AppDirectoryPath + @"\settings.ini";

        /// <summary>
        /// File name of Server config.
        /// </summary>
        public const string ServerConfigFileName = "serverconfig.xml";
        /// <summary>
        /// File name template of Template file.
        /// </summary>
        public const string BaseTemplateFileName = @"{0}\{1}_ConfigData.xml";
    }
}
