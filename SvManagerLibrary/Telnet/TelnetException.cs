using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvManagerLibrary.Telnet
{
    public class TelnetException
    {
        public static void CheckTelnetClient(TelnetClient telnet)
        {
            if (telnet == null)
                throw new NullReferenceException();
            if (!telnet.Connected)
                throw new System.Net.Sockets.SocketException((int)System.Net.Sockets.SocketError.NotConnected);
        }
    }
}
