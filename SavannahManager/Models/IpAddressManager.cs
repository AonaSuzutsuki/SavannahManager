using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SvManagerLibrary.Web;

namespace _7dtd_svmanager_fix_mvvm.Models
{
    public class IpAddressManager
    {
        public class IpAddressJsonInfo
        {
            [JsonProperty("External_Ip")]
            public string ExternalIp { get; set; }
        }

        public static async Task<string> GetExternalIpAddress(string url)
        {
            var json = await Downloader.DownloadStringAsync(url);
            var obj = JsonConvert.DeserializeObject<IpAddressJsonInfo>(json);
            return obj.ExternalIp;
        }

        public static string GetLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            return (from ip in host.AddressList where ip.AddressFamily == AddressFamily.InterNetwork select ip.ToString()).FirstOrDefault();
        }
    }
}
