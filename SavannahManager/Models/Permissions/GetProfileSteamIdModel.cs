using System.IO;
using System.Net;
using System.Threading.Tasks;
using SavannahXmlLib.XmlWrapper;
using SvManagerLibrary.Web;

namespace _7dtd_svmanager_fix_mvvm.Models.Permissions
{
    public class GetProfileSteamIdModel : AbstractGetSteamIdModel
    {
        public override async Task Analyze(string url)
        {
            url += "?xml=1";

            try
            {
                var xml = await Downloader.DownloadDataAsync(url);
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
