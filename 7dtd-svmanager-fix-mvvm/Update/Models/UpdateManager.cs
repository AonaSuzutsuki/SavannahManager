using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CommonLib.File;

namespace _7dtd_svmanager_fix_mvvm.Update.Models
{
    public class UpdateManager
    {
        public static bool CheckUpdate(string url)
        {
            string url_version = string.Empty;
            using (var wc = new WebClient())
            {
                byte[] data = wc.DownloadData(url);
                url_version = Encoding.UTF8.GetString(data);
            }

            string version = ConstantValues.Version;
            return !url_version.Equals(version);
        }

        public static async Task<bool> CheckUpdateAsync(string url)
        {
            string url_version = string.Empty;
            using (var wc = new WebClient())
            {
                byte[] data = await wc.DownloadDataTaskAsync(url);
                url_version = Encoding.UTF8.GetString(data);
            }

            string version = ConstantValues.Version;
            return !url_version.Equals(version);
        }
    }
}
