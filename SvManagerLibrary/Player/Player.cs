using SvManagerLibrary.Telnet;

namespace SvManagerLibrary.Player
{
    public class Player
    {
        PlayerInfoArray players = new PlayerInfoArray();
        public PlayerInfoArray Players
        {
            get
            {
                return players;
            }
        }

        public void SetPlayerInfo(TelnetClient telnet)
        {
            if (!telnet.Connected || telnet == null)
            {
                throw new System.NullReferenceException();
            }

            telnet.WriteLine("lp");
            System.Threading.Thread.Sleep(200);
            string log = telnet.Read().TrimEnd('\0');
            Players.Clear();
            Players.Add(log);
        }
    }
}
