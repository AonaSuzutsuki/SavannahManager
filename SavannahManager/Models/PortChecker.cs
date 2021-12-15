using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SvManagerLibrary.Web;

namespace _7dtd_svmanager_fix_mvvm.Models
{
    public class PortChecker
    {
        internal class PortCheckerInfo
        {
            [JsonProperty("Opened")]
            internal string OpenedString { get; set; }
        }

        public string Host { get; }
        public uint Port { get; }

        private string url = "https://aonsztk.xyz/api/?mode=portcheck&host={0}&port={1}";

        public PortChecker(string host, uint port)
        {
            Host = host;
            Port = port;
        }

        public async Task<bool> Search()
        {
            var json = await Downloader.DownloadStringAsync(string.Format(url, Host, Port.ToString()));
            var obj = JsonConvert.DeserializeObject<PortCheckerInfo>(json);
            bool.TryParse(obj.OpenedString, out var isOpened);
            return isOpened;
        }
    }
}
