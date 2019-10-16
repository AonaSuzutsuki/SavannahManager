using Moq;
using NUnit.Framework;
using ITelnetClient = SvManagerLibrary.Telnet.ITelnetClient;

namespace SvManagerLibrary.Time.Tests
{
    [TestFixture]
    public class TimeTests
    {
        [Test]
        public void ConvertTimeTest()
        {
            var text = "Day 256, 11:23";
            var act = Time.ConvertTime(text);
            var exp = new TimeInfo()
            {
                Day = 256,
                Hour = 11,
                Minute = 23
            };

            Assert.AreEqual(exp, act);
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
            mock.Setup(x => x.Read()).Returns(text);
            mock.Setup(x => x.Connected).Returns(true);

            var act = Time.GetTimeFromTelnet(mock.Object);

            Assert.AreEqual(exp, act);
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
            Time.SendTime(obj, timeInfo);

            mock.Verify(m => m.WriteLine(exp), Times.Once);
        }
    }
}