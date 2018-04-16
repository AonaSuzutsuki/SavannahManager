using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7dtd_svmanager_fix_mvvm.Models
{
    public class PortChecker
    {
        public string Host { get; }
        public uint Port { get; }

        private string url = "https://aonsztk.xyz/api/?mode=portcheck&host={0}&port={1}";

        public PortChecker(string host, uint port)
        {
            this.Host = host;
            this.Port = port;
        }

        public bool Search()
        {
            var jsonLoader = new JsonLoader(string.Format(url, Host, Port.ToString()));
            var opened = jsonLoader.GetValue(0, "Opened");
            bool.TryParse(opened, out bool isOpened);
            return isOpened;
        }
    }
}
