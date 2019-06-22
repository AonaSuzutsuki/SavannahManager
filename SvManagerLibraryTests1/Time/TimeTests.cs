using Microsoft.VisualStudio.TestTools.UnitTesting;
using SvManagerLibrary.Telnet;
using SvManagerLibrary.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            var mock = new Moq.Mock<ITelnetClient>();
            mock.Setup(x => x.Read()).Returns(text);

            var act = Time.ConvertTime(text);

            Assert.AreEqual(exp, act);
        }
    }
}