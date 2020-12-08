using SvManagerLibrary.Player;
using SvManagerLibrary.Telnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using SvManagerLibraryTests2.Telnet;

namespace SvManagerLibraryTests2.Player
{
    [TestFixture]
    public class PlayerTests
    {
        [Test]
        public void SetPlayerInfoTest()
        {
            var text = "0. id=171, Aona Suzutsuki, pos=(-157.3, 61.1, -115.8), rot=(0.0, 350.2, 0.0), remote=True, health=32, deaths=18, zombies=0, players=0" +
                ", score=0, level=1, steamid=76561198010715714, ip=192.168.1.45, ping=2";
            var exp = new List<PlayerInfo>
            {
                new PlayerInfo
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
                }
            };

            var mock = new Moq.Mock<ITelnetClient>();
            mock.Setup(x => x.DestructionEventRead(It.IsAny<string>(), It.IsAny<string>())).Returns(text);
            mock.Setup(x => x.Connected).Returns(true);

            var act = SvManagerLibrary.Player.Player.SetPlayerInfo(mock.Object);

            CollectionAssert.AreEqual(exp, act);
        }

        [Test]
        public void SetPlayerInfoTest2()
        {
            var text = "0. id=171, Aona Suzutsuki, pos=(-157.3, 61.1, -115.8), rot=(0.0, 350.2, 0.0), remote=True, health=32, deaths=18, zombies=0, players=0" +
                       ", score=0, level=1, steamid=76561198010715714, ip=192.168.1.45, ping=2";
            var exp = new List<PlayerInfo>
            {
                new PlayerInfo
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
                }
            };

            var mock = TelnetTest.GetSocketMock();
            mock.Setup(m => m.ReceiveBufferSize).Returns(1024);
            mock.Setup(m => m.Receive(It.IsAny<byte[]>(), It.IsAny<SocketFlags>())).Callback<byte[], SocketFlags>(
                (buffer, flag) => {
                    var data = Encoding.UTF8.GetBytes("2020-11-09T11:52:26 1536.940 INF Executing command 'lp' by Telnet from 127.0.0.1:58077\n" +
                                                      $"{text}\n" +
                                                      "Total of 1 in the game\n");
                    foreach (var item in data.Select((v, i) => new { Index = i, Value = v }))
                    {
                        buffer[item.Index] = item.Value;
                    }
                });

            var telnetClient = new TelnetClient(mock.Object);
            var act = SvManagerLibrary.Player.Player.SetPlayerInfo(telnetClient);

            CollectionAssert.AreEqual(exp, act);
        }
    }
}