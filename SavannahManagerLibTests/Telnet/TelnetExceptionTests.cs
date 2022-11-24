using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Moq;
using NUnit.Framework;
using SvManagerLibrary.Telnet;

namespace SvManagerLibraryTests2.Telnet
{
    [TestFixture]
    public class TelnetExceptionTests
    {
        [Test]
        public void CheckTelnetClientNullTest()
        {
           Assert.Throws<NullReferenceException>(() => TelnetException.CheckTelnetClient(null));
        }

        [Test]
        public void CheckTelnetClientDisconnectedTest()
        {
            var mock = new Mock<ITelnetClient>();
            mock.Setup(x => x.Connected).Returns(false);
            var obj = mock.Object;
            Assert.Throws<SocketException>(() => TelnetException.CheckTelnetClient(obj));
        }
    }
}
