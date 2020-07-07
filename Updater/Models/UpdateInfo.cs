using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateLib.Update;

namespace Updater.Models
{
    public class UpdateInfo
    {
        public int Pid { get; }

        public string FileName { get; }

        public UpdateClient Client { get; }

        public UpdateInfo(int pid, string fileName, UpdateClient updateClient)
        {
            Pid = pid;
            FileName = fileName;
            Client = updateClient;
        }
    }
}
