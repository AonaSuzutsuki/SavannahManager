using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7dtd_svmanager_fix_mvvm.Models.Interfaces
{
    public interface IMainWindowTelnet
    {
        bool SocTelnetSendNrt(string cmd);
        bool SocTelnetSendNrtNer(string cmd, bool isAsync = false);
    }
}
