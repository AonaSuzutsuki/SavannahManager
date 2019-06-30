using Microsoft.VisualStudio.TestTools.UnitTesting;
using SvManagerLibrary.Telnet;
using SvManagerLibrary.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using ITelnetClient = SvManagerLibrary.Telnet.ITelnetClient;

namespace SvManagerLibrary.Time.Tests
{
    [TestClass()]
    public class TimeTests
    {
        [TestMethod()]
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

        [TestMethod()]
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

        [TestMethod()]
        public void SendTimeTest()
        {
            var mock = new Mock<ITelnetClient>();
            string actCmd = string.Empty;
            var timeInfo = new TimeInfo
            {
                Day = 2,
                Hour = 11,
                Minute = 34
            };
            var exp = "st 35567";

            mock.Setup(x => x.Connected).Returns(true);
            mock.Setup(mr => mr.WriteLine(It.IsAny<string>())).Callback((string cmd) => actCmd = cmd);

            var obj = mock.Object;
            Time.SendTime(obj, timeInfo);

            Assert.AreEqual(exp, actCmd);
        }
    }
}