using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CommonLib.File;

namespace _7dtd_svmanager_fix_mvvm.Update.Models
{
    public class UpdateManager
    {
        public Dictionary<string, string> Updates { get; private set; }

        public bool IsUpdate = false;
        public bool IsUpdUpdate = false;
        public string Version = "1.0.0.0";

        public UpdateManager(UpdateLink updLink, string updFilePath)
        {
            (bool, string) ret = CheckUpdate(updLink.VersionUrl, ConstantValues.Version);
            IsUpdate = ret.Item1;
            Version = ret.Item2;
            ret = CheckUpdate(updLink.UpdVersionUrl, CommonLib.File.Version.GetVersion(updFilePath));
            IsUpdUpdate = ret.Item1;

            byte[] data;
            using (var wc = new WebClient())
            {
                data = wc.DownloadData(updLink.XmlUrl);
            }

            List<string> versions;
            List<string> details;
            using (var stream = new MemoryStream(data))
            {
                var reader = new SvManagerLibrary.XMLWrapper.Reader(stream);
                versions = reader.GetAttributes("version", "/updates/update");
                details = reader.GetValues("/updates/update");
            }

            int count = versions.Count >= details.Count ? details.Count : versions.Count;
            Updates = new Dictionary<string, string>(count);
            for (int i = 0; i < count; ++i)
            {
                var version = versions[i];
                var detail = details[i];
                Updates.Add(version, detail);
            }
        }

        public static (bool, string) CheckUpdate(string url, string version)
        {
            string url_version = string.Empty;
            using (var wc = new WebClient())
            {
                byte[] data = wc.DownloadData(url);
                url_version = Encoding.UTF8.GetString(data);
            }
            
            return (!url_version.Equals(version), url_version);
        }

        public static async Task<(bool, string)> CheckUpdateAsync(string url, string version)
        {
            string url_version = string.Empty;
            using (var wc = new WebClient())
            {
                byte[] data = await wc.DownloadDataTaskAsync(url);
                url_version = Encoding.UTF8.GetString(data);
            }

            return (!url_version.Equals(version), url_version);
        }
    }
}
