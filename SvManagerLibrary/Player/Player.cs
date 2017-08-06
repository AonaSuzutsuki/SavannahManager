﻿using SvManagerLibrary.Telnet;

namespace SvManagerLibrary.Player
{
    public class Player
    {
        public static PlayerInfoArray SetPlayerInfo(TelnetClient telnet)
        {
            TelnetException.CheckTelnetClient(telnet);

            var players = new PlayerInfoArray();
            telnet.WriteLine("lp");
            System.Threading.Thread.Sleep(200);
            string log = telnet.Read().TrimEnd('\0');
            players.Add(log);

            return players;
        }
    }
}