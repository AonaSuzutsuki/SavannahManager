using SvManagerLibrary.Telnet;

namespace SvManagerLibrary.Chat
{
    /// <summary>
    /// Provides a chat service.
    /// </summary>
    public static class Chat
    {
        /// <summary>
        /// Send chat via telnet client.
        /// </summary>
        /// <param name="telnet">The telnet client.</param>
        /// <param name="message">A message to be sent.</param>
        public static void SendChat(ITelnetClient telnet, string message)
        {
            TelnetException.CheckTelnetClient(telnet);

            telnet.WriteLine("say " + message);
        }
    }
}
