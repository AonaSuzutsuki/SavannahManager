using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7dtd_svmanager_fix_mvvm.Update.Models
{
    public sealed class UpdateLink
    {
        public static UpdateLink GetInstance { get; } = new UpdateLink();

        public string VersionUrl { get; }
        public string XmlUrl { get; }
        public string UpdVersionUrl { get; }
        public string UpPath { get; }
        public string MainPath { get; }

        private UpdateLink()
        {
            VersionUrl = "http://kimamalab.azurewebsites.net/updates/SavannahManager3/version.txt";
            XmlUrl = "http://kimamalab.azurewebsites.net/updates/SavannahManager3/details/" + LangResources.UpdResources.Updater_XMLNAME;
            UpdVersionUrl = "http://kimamalab.azurewebsites.net/updates/SavannahManager3/version.updater.txt";
            UpPath = "http://kimamalab.azurewebsites.net/updates/SavannahManager3/update.zip";
            MainPath = "http://kimamalab.azurewebsites.net/updates/SavannahManager3/SavannahManager.zip";
        }
    }
}
