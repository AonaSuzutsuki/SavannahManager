
#define TELNET_TEST

using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Moq;
using NUnit.Framework;
using SvManagerLibrary.Telnet;

namespace SvManagerLibraryTests2.Telnet
{
    [TestFixture]
    public class TelnetTest
    {
        public ITelnetSocket GetSocketMock()
        {
            var mock = new Mock<ITelnetSocket>();
            mock.Setup(m => m.Poll(It.IsAny<int>(), It.IsAny<SelectMode>())).Returns(true);
            mock.Setup(m => m.Available).Returns(1);
            mock.Setup(m => m.ReceiveBufferSize).Returns(64);
            mock.Setup(m => m.Receive(It.IsAny<byte[]>(), It.IsAny<SocketFlags>())).Callback<byte[], SocketFlags>(
                (buffer, flag) => {
                    var data = Encoding.UTF8.GetBytes("test");
                    foreach (var item in data.Select((v, i) => new { Index = i, Value = v }))
                    {
                        buffer[item.Index] = item.Value;
                    }
                });

            return mock.Object;
        }

        [Test]
        public void ReadTest()
        {
            var mock = GetSocketMock();
            var telnetClient = new TelnetClient(mock);
            var log = telnetClient.Read().TrimEnd('\0');
            var exp = "test";

            Assert.AreEqual(exp, log);
        }
    }
}
