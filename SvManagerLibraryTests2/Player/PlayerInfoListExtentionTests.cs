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
    public class PlayerInfoListExtentionTests
    {
        [Test]
        public void AddTest()
        {
            var text = "0. id=171, Aona Suzutsuki, pos=(-157.3, 61.1, -115.8), rot=(0.0, 350.2, 0.0), remote=True, health=32, deaths=18, zombies=0, players=0" +
                ", score=0, level=1, steamid=76561198010715714, ip=192.168.1.45, ping=2";
            var text2 = "0. id=171, Aona Suzutsuki, pos=(-157.3, 61.1, -115.8), rot=(0.0, 350.2, 0.0), remote=True, health=32, deaths=18, zombies=0, players=0" +
                ", score=0, level=1, steamid=76561198010715714, ip=192.168.1.45, ping=2";
            var exp = new List<PlayerInfo>
            {
                PlayerInfoConverter.ConvertPlayerDetail(text),
                PlayerInfoConverter.ConvertPlayerDetail(text2),
            };

            var act = new List<PlayerInfo>()
            {
                "0. id=171, Aona Suzutsuki, pos=(-157.3, 61.1, -115.8), rot=(0.0, 350.2, 0.0), remote=True, health=32, deaths=18, zombies=0, players=0" +
                ", score=0, level=1, steamid=76561198010715714, ip=192.168.1.45, ping=2\r\n" +
                "aaaaasacasfvafklasdxckjasc0a" +
                "0. id=171, Aona Suzutsuki, pos=(-157.3, 61.1, -115.8), rot=(0.0, 350.2, 0.0), remote=True, health=32, deaths=18, zombies=0, players=0" +
                ", score=0, level=1, steamid=76561198010715714, ip=192.168.1.45, ping=2"
            };

            CollectionAssert.AreEqual(exp, act);
        }
    }
}