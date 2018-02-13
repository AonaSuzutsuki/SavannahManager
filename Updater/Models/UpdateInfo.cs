using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Updater.Models
{
    public class UpdateInfo
    {
        public int Pid { get; }

        public string FileName { get; }

        public string Url { get; }

        public UpdateInfo(int pid, string fileName, string url)
        {
            Pid = pid;
            FileName = fileName;
            Url = url;
        }
    }
}
