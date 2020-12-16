using SvManagerLibrary.Telnet;
using System.Collections.Generic;

namespace SvManagerLibrary.Player
{
    /// <summary>
    /// Provides a player service.
    /// </summary>
    public static class Player
    {
        /// <summary>
        /// Get player info list via telnet client.
        /// </summary>
        /// <param name="telnet">The telnet client.</param>
        /// <returns>A player info list.</returns>
        public static List<PlayerInfo> GetPlayerInfoList(ITelnetClient telnet)
        {
            TelnetException.CheckTelnetClient(telnet);

            var players = new List<PlayerInfo>();
            var log = telnet.DestructionEventRead("lp", "Total of [0-9]+ in the game");
            if (!string.IsNullOrEmpty(log))
                players.Add(log);

            return players;
        }
    }
}
