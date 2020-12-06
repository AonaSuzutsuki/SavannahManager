using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7dtd_svmanager_fix_mvvm.Models
{
    public interface IMainWindowTelnet
    {
        bool SocTelnetSendNrt(string cmd);
    }
}
