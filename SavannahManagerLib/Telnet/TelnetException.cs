using System;

namespace SvManagerLibrary.Telnet
{
    public class TelnetException
    {
        public static void CheckTelnetClient(ITelnetClient telnet)
        {
            if (telnet == null)
                throw new NullReferenceException();
            if (!telnet.Connected)
                throw new System.Net.Sockets.SocketException((int)System.Net.Sockets.SocketError.NotConnected);
        }
    }
}
