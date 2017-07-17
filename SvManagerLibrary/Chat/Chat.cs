using SvManagerLibrary.Telnet;

namespace SvManagerLibrary.Chat
{
    public class Chat
    {
        public void SendChat(TelnetClient telnet, string message)
        {
            if (!telnet.Connected || telnet == null)
            {
                throw new System.NullReferenceException();
            }

            telnet.WriteLine("say " + message);
        }
    }
}
