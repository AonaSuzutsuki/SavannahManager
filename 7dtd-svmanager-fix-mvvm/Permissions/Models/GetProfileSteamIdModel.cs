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
        public override async Task Analyze(string url)
        {
            url += "?xml=1";

            try
            {
                using var webClient = new WebClient();
                var xml = await webClient.DownloadDataTaskAsync(url);
                using var ms = new MemoryStream(xml);

                var reader = new SavannahXmlReader(ms);
                var steamId = reader.GetNode("/profile/steamID64").InnerText;

                CanWrite = !string.IsNullOrEmpty(steamId);
                Steam64Id = steamId;
            }
            catch (WebException)
            {
                CanWrite = false;
                Steam64Id = "Invalid URL.";
            }
            catch (System.Xml.XmlException)
            {
                CanWrite = false;
                Steam64Id = "Invalid URL.";
            }
        }
    }
}
