using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using SvManagerLibrary.Web;

namespace SvManagerLibraryTests2.Web
{
    [TestFixture]
    public class DownloaderTests
    {
        [Test]
        public async Task DownloadStringTest()
        {
            var url = "https://aonsztk.xyz/KimamaLab/Tests/download.txt";
            var exp = "SUCCESS.";
            var result = await Downloader.DownloadStringAsync(url);

            ClassicAssert.AreEqual(exp, result);
        }

        [Test]
        public async Task DownloadDataTest()
        {
            var url = "https://aonsztk.xyz/KimamaLab/Tests/download.txt";
            var exp = new byte[]
            {
                0x53, 0x55, 0x43, 0x43, 0x45, 0x53, 0x53, 0x2E
            };
            var result = await Downloader.DownloadDataAsync(url);

            CollectionAssert.AreEqual(exp, result);
        }
    }
}
