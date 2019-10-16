using SvManagerLibrary.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SvManagerLibrary.Player.Tests
{
    [TestFixture]
    public class PlayerInfoConverterTests
    {
        [Test]
        public void ConvertPlayerDetailTest()
        {
            var text = "0. id=171, Aona Suzutsuki, pos=(-157.3, 61.1, -115.8), rot=(0.0, 350.2, 0.0), remote=True, health=32, deaths=18, zombies=0, players=0" +
                ", score=0, level=1, steamid=76561198010715714, ip=192.168.1.45, ping=2";
            var act = PlayerInfoConverter.ConvertPlayerDetail(text);
            var exp = new PlayerInfo
            {
                Id = "171",
                Name = "Aona Suzutsuki",
                Health = "32",
                Deaths = "18",
                ZombieKills = "0",
                PlayerKills = "0",
                Score = "0",
                Level = "1",
                SteamId = "76561198010715714",
                Coord = "(-157.3, 61.1, -115.8)",
            };

            Assert.AreEqual(exp, act);
        }
    }
}