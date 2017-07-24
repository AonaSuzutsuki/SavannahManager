using SvManagerLibrary.Telnet;

namespace SvManagerLibrary.Chat
{
    public class Chat
    {
        public static void SendChat(TelnetClient telnet, string message)
        {
            TelnetException.CheckTelnetClient(telnet);

            telnet.WriteLine("say " + message);
        }
    }
}
