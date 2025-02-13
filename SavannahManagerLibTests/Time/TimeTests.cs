using System.Linq;
using System.Net.Sockets;
using System.Text;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using SvManagerLibrary.Telnet;
using SvManagerLibrary.Time;
using SvManagerLibraryTests2.Telnet;
using ITelnetClient = SvManagerLibrary.Telnet.ITelnetClient;

namespace SvManagerLibraryTests2.Time
{
    [TestFixture]
    public class TimeTests
    {
        [Test]
        public void ConvertTimeTest()
        {
            var text = "Day 256, 11:23";
            var act = SvManagerLibrary.Time.Time.ConvertTime(text);
            var exp = new TimeInfo()
            {
                Day = 256,
                Hour = 11,
                Minute = 23
            };

            ClassicAssert.AreEqual(exp, act);
        }

        [Test]
        public void GetTimeFromTelnetTest()
        {
            var text = "Day 256, 11:23";
            var exp = new TimeInfo()
            {
                Day = 256,
                Hour = 11,
                Minute = 23
            };

            var mock = new Mock<ITelnetClient>();
            mock.Setup(x => x.DestructionEventRead(It.IsAny<string>(), It.IsAny<string>())).Returns(text);
            mock.Setup(x => x.Connected).Returns(true);

            var act = SvManagerLibrary.Time.Time.GetTimeFromTelnet(mock.Object);

            ClassicAssert.AreEqual(exp, act);
        }

        [Test]
        public void GetTimeFromTelnetTest2()
        {
            var text = "Day 256, 11:23";
            var exp = new TimeInfo()
            {
                Day = 256,
                Hour = 11,
                Minute = 23
            };

            var mock = TelnetTest.GetSocketMock();
            mock.Setup(m => m.Receive(It.IsAny<byte[]>(), It.IsAny<SocketFlags>())).Callback<byte[], SocketFlags>(
                (buffer, flag) => {
                    var data = Encoding.UTF8.GetBytes($"aaaaaa\n{text}\ntest\n");
                    foreach (var item in data.Select((v, i) => new { Index = i, Value = v }))
                    {
                        buffer[item.Index] = item.Value;
                    }
                });

            var telnetClient = new TelnetClient(mock.Object);
            var act = SvManagerLibrary.Time.Time.GetTimeFromTelnet(telnetClient);

            ClassicAssert.AreEqual(exp, act);
        }

        [Test]
        public void SendTimeTest()
        {
            var timeInfo = new TimeInfo
            {
                Day = 2,
                Hour = 11,
                Minute = 34
            };
            var exp = "st 35567";

            var mock = new Mock<ITelnetClient>();
            mock.Setup(x => x.Connected).Returns(true);

            var obj = mock.Object;
            SvManagerLibrary.Time.Time.SendTime(obj, timeInfo);

            mock.Verify(m => m.WriteLine(exp), Times.Once);
        }
    }
}