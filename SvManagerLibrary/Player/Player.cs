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

            var counter = new TelnetCounter();
            while (counter.CanLoop)
            {
                var log = telnet.Read().TrimEnd('\0');
                if (!string.IsNullOrEmpty(log))
                {
                    telnet.DestructionEvent = false;
                    players.Add(log);
                }

                counter.Next();
                System.Threading.Thread.Sleep(100);
            }

            return players;
        }
    }
}
