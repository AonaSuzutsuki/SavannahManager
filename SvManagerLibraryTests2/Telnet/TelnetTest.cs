
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
        public Mock<ITelnetSocket> GetSocketMock()
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

            return mock;
        }

        [Test]
        public void ReadTest()
        {
            var mock = GetSocketMock();
            var telnetClient = new TelnetClient(mock.Object);
            var log = telnetClient.Read().TrimEnd('\0');
            var exp = "test";

            Assert.AreEqual(exp, log);
        }

        [Test]
        public void DestructionEventReadSplitTest()
        {
            var mock = GetSocketMock();
            var cnt = 0;
            mock.Setup(m => m.Receive(It.IsAny<byte[]>(), It.IsAny<SocketFlags>())).Callback<byte[], SocketFlags>(
                (buffer, flag) => {
                    if (cnt == 0)
                    {
                        var data = Encoding.UTF8.GetBytes("test");
                        foreach (var item in data.Select((v, i) => new { Index = i, Value = v }))
                        {
                            buffer[item.Index] = item.Value;
                        }

                        cnt++;
                    }
                    else
                    {
                        var data = Encoding.UTF8.GetBytes(" message.\n");
                        foreach (var item in data.Select((v, i) => new { Index = i, Value = v }))
                        {
                            buffer[item.Index] = item.Value;
                        }
                    }
                });
            var telnetClient = new TelnetClient(mock.Object);

            var value = telnetClient.DestructionEventRead("");
            var exp = "test message.\n";
            Assert.AreEqual(exp, value);
        }

        [Test]
        public void DestructionEventReadSplitTest2()
        {
            var mock = GetSocketMock();
            var cnt = 0;
            mock.Setup(m => m.Receive(It.IsAny<byte[]>(), It.IsAny<SocketFlags>())).Callback<byte[], SocketFlags>(
                (buffer, flag) => {
                    if (cnt == 0)
                    {
                        var data = Encoding.UTF8.GetBytes("test\ntest2\ntest3");
                        foreach (var item in data.Select((v, i) => new { Index = i, Value = v }))
                        {
                            buffer[item.Index] = item.Value;
                        }

                        cnt++;
                    }
                    else
                    {
                        var data = Encoding.UTF8.GetBytes(" message.\n");
                        foreach (var item in data.Select((v, i) => new { Index = i, Value = v }))
                        {
                            buffer[item.Index] = item.Value;
                        }
                    }
                });
            var telnetClient = new TelnetClient(mock.Object);

            var value = telnetClient.DestructionEventRead("", "test3 message.");
            var exp = "test\ntest2\ntest3 message.\n";
            Assert.AreEqual(exp, value);
        }
    }
}
