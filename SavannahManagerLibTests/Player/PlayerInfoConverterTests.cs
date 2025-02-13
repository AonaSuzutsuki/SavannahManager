using SvManagerLibrary.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace SvManagerLibraryTests2.Player
{
    [TestFixture]
    public class PlayerInfoConverterTests
    {
        [Test]
        public void ConvertPlayerDetailTest()
        {
            var text = "0. id=171, Aona Suzutsuki, pos=(741.9, 45.1, 951.5), rot=(-11.2, -406.4, 0.0)" +
                ", remote=True, health=92, deaths=4, zombies=4, players=1, score=2, level=1, pltfmid=Steam_76561198010715714" +
                ", crossid=EOS_0002bfc568d6401ca9de387e0ae914c9, ip=192.168.0.81, ping=0";

            var act = PlayerInfoConverter.ConvertPlayerDetail(text);
            var exp = new PlayerInfo
            {
                Id = "171",
                Name = "Aona Suzutsuki",
                Health = "92",
                Deaths = "4",
                ZombieKills = "4",
                PlayerKills = "1",
                Score = "2",
                Level = "1",
                SteamId = "76561198010715714",
                Coord = "(741.9, 45.1, 951.5)",
            };

            ClassicAssert.AreEqual(exp, act);
        }
    }
}