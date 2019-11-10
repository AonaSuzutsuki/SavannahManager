using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SvManagerLibrary.XmlWrapper;
using CommonStyleLib.File;

namespace _7dtd_svmanager_fix_mvvm.Update.Models
{
    public class UpdateManager
    {
        public Dictionary<string, string> Updates { get; }

        public bool IsUpdate = false;
        public bool IsUpdUpdate = false;
        public string Version = "1.0.0.0";

        public UpdateManager(UpdateLink updLink, string updFilePath)
        {
            var ret = CheckUpdate(updLink.VersionUrl, ConstantValues.Version);
            IsUpdate = ret.Item1;
            Version = ret.Item2;
            ret = CheckUpdate(updLink.UpdVersionUrl, CommonCoreLib.CommonFile.Version.GetVersion(updFilePath));
            IsUpdUpdate = ret.Item1;

            byte[] data;
            using (var wc = new WebClient())
            {
                data = wc.DownloadData(updLink.XmlUrl);
            }

            using var stream = new MemoryStream(data);
            var reader = new CommonXmlReader(stream);
            var nodes = reader.GetNodes("/updates/update");
            var items = (from node in nodes
                        let attr = node.GetAttribute("version")
                        let value = node.InnerText.Text
                        select new { Attribute = attr, Value = value }).ToList();

            var count = items.Count;
            Updates = new Dictionary<string, string>(count);
            foreach (var item in items)
            {
                Updates.Add(item.Attribute.Value, item.Value);
            }
        }

        public static (bool, string) CheckUpdate(string url, string version)
        {
            using var wc = new WebClient();
            var data = wc.DownloadData(url);
            var urlVersion = Encoding.UTF8.GetString(data);

            return (!urlVersion.Equals(version), urlVersion);
        }

        public static async Task<(bool, string)> CheckUpdateAsync(string url, string version)
        {
            using var wc = new WebClient();
            var data = await wc.DownloadDataTaskAsync(url);
            var urlVersion = Encoding.UTF8.GetString(data);

            return (!urlVersion.Equals(version), urlVersion);
        }
    }
}
