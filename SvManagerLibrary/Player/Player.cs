using SvManagerLibrary.Telnet;
using System.Collections.Generic;

namespace SvManagerLibrary.Player
{
    public class Player
    {
        public static List<PlayerInfo> SetPlayerInfo(ITelnetClient telnet)
        {
            TelnetException.CheckTelnetClient(telnet);

            telnet.DestructionEvent = true;
            var players = new List<PlayerInfo>();
            telnet.WriteLine("lp");
            System.Threading.Thread.Sleep(200);
            string log = telnet.Read().TrimEnd('\0');
            telnet.DestructionEvent = false;
            players.Add(log);

            return players;
        }
    }
}
