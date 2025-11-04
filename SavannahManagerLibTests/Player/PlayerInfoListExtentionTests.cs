using SvManagerLibrary.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using SvManagerLibrary.AnalyzerPlan.Console;

namespace SvManagerLibraryTests2.Player
{
    [TestFixture]
    public class PlayerInfoListExtentionTests
    {
        private readonly IConsoleAnalyzer _analyzer = new OnePointTreeConsoleAnalyzer();

        [Test]
        public void AddTest()
        {
            var text = "0. id=171, Aona Suzutsuki, pos=(-157.3, 61.1, -115.8), rot=(0.0, 350.2, 0.0), remote=True, health=32, deaths=18, zombies=0, players=0" +
                ", score=0, level=1, steamid=76561198010715714, ip=192.168.1.45, ping=2";
            var text2 = "0. id=171, Aona Suzutsuki, pos=(-157.3, 61.1, -115.8), rot=(0.0, 350.2, 0.0), remote=True, health=32, deaths=18, zombies=0, players=0" +
                ", score=0, level=1, steamid=76561198010715714, ip=192.168.1.45, ping=2";
            var exp = new List<PlayerInfo>
            {
                PlayerInfoConverter.ConvertPlayerDetail(text, _analyzer),
                PlayerInfoConverter.ConvertPlayerDetail(text2, _analyzer),
            };

            var act = new List<PlayerInfo>().Add(
                "0. id=171, Aona Suzutsuki, pos=(-157.3, 61.1, -115.8), rot=(0.0, 350.2, 0.0), remote=True, health=32, deaths=18, zombies=0, players=0" +
                ", score=0, level=1, steamid=76561198010715714, ip=192.168.1.45, ping=2\r\n" +
                "aaaaasacasfvafklasdxckjasc0a" +
                "0. id=171, Aona Suzutsuki, pos=(-157.3, 61.1, -115.8), rot=(0.0, 350.2, 0.0), remote=True, health=32, deaths=18, zombies=0, players=0" +
                ", score=0, level=1, steamid=76561198010715714, ip=192.168.1.45, ping=2", _analyzer);

            CollectionAssert.AreEqual(exp, act);
        }
    }
}