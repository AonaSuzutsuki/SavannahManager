using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using _7dtd_svmanager_fix_mvvm.Models;
using Newtonsoft.Json;
using SavannahXmlLib.XmlWrapper;

namespace _7dtd_svmanager_fix_mvvm.Permissions.Models
{
    public class GetProfileSteamIdModel : AbstractGetSteamIdModel
    {
        public class ProfileApiInfo
        {
            public string SteamId { get; set; }
        }
        public override async Task Analyze(string username)
        {
            //76561198010715714
            //https://aonsztk.xyz/api/?mode=SteamId&username=rvv-jp
            var url = $"https://aonsztk.xyz/api/?mode=SteamId&username={username}";

            try
            {
                using var webClient = new WebClient();
                var json = await webClient.DownloadStringTaskAsync(url);
                var obj = JsonConvert.DeserializeObject<ProfileApiInfo>(json);
                var steamId = obj.SteamId;

                CanWrite = !string.IsNullOrEmpty(steamId);
                Steam64Id = steamId;
            }
            catch (WebException)
            {
                CanWrite = false;
                Steam64Id = "Invalid URL.";
            }
        }
    }
}
