using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SvManagerLibrary.Web
{
    public class Downloader
    {
        /// <summary>
        /// Download content as string from url.
        /// </summary>
        /// <param name="url">Request url</param>
        /// <returns>The content downloaded as string</returns>
        public static async Task<string> DownloadStringAsync(string url)
        {
            var client = new HttpClient();

            using HttpResponseMessage response = await client.GetAsync(url);
            using HttpContent content = response.Content;
            var text = await content.ReadAsStringAsync();
            return text;
        }

        /// <summary>
        /// Download content as byte array from url.
        /// </summary>
        /// <param name="url">Request url</param>
        /// <returns>The content downloaded as byte array</returns>
        public static async Task<byte[]> DownloadDataAsync(string url)
        {
            var client = new HttpClient();

            using HttpResponseMessage response = await client.GetAsync(url);
            using HttpContent content = response.Content;
            var text = await content.ReadAsByteArrayAsync();
            return text;
        }
    }
}
