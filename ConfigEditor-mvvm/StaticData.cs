using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigEditor_mvvm
{
    public static class StaticData
    {
        public static readonly string AppDirectoryPath = KimamaLib.AppInfo.GetAppPath();
        public static readonly string VersionListPath = AppDirectoryPath + @"\lang\VersionList.xml";
        public static readonly string SettingFilePath = AppDirectoryPath + @"\settings.ini";
        
        public const string ServerConfigFileName = "serverconfig.xml";
        public const string BaseTemplateFileName = @"{0}\{1}_ConfigData.xml";
    }
}
