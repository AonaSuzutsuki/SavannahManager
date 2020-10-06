using SvManagerLibrary.Telnet;
using System.Collections.Generic;

namespace SvManagerLibrary.Player
{
    public class Player
    {
        public static List<PlayerInfo> SetPlayerInfo(ITelnetClient telnet)
        {
            TelnetException.CheckTelnetClient(telnet);

            var players = new List<PlayerInfo>();
            var log = telnet.DestructionEventRead("lp");
            if (!string.IsNullOrEmpty(log))
                players.Add(log);

            return players;
        }
    }
}
