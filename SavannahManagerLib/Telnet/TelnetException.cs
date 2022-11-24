using System;

namespace SvManagerLibrary.Telnet
{
    /// <summary>
    /// Provided exception code for telnet.
    /// </summary>
    public class TelnetException
    {
        /// <summary>
        /// Check if the Telnet Client is enabled.
        /// If null or unconnected, raise an exception.
        /// </summary>
        /// <param name="telnet">The telnet client to be checked.</param>
        public static void CheckTelnetClient(ITelnetClient telnet)
        {
            if (telnet == null)
                throw new NullReferenceException();
            if (!telnet.Connected)
                throw new System.Net.Sockets.SocketException((int)System.Net.Sockets.SocketError.NotConnected);
        }
    }
}
