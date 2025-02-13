
#define TELNET_TEST

using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using SvManagerLibrary.Telnet;

namespace SvManagerLibraryTests2.Telnet
{
    [TestFixture]
    public class TelnetTest
    {
        public static Mock<ITelnetSocket> GetSocketMock()
        {
            var mock = new Mock<ITelnetSocket>();
            mock.Setup(m => m.Poll(It.IsAny<int>(), It.IsAny<SelectMode>())).Returns(true);
            mock.Setup(m => m.Available).Returns(1);
            mock.Setup(m => m.ReceiveBufferSize).Returns(64);
            mock.Setup(m => m.Receive(It.IsAny<byte[]>(), It.IsAny<SocketFlags>())).Callback<byte[], SocketFlags>(
                (buffer, flag) => {
                    var data = Encoding.UTF8.GetBytes("test\n");
                    foreach (var item in data.Select((v, i) => new { Index = i, Value = v }))
                    {
                        buffer[item.Index] = item.Value;
                    }
                });
            mock.Setup(m => m.Connect(It.IsAny<string>(), It.IsAny<int>()));
            mock.Setup(m => m.RemoteEndPoint).Returns(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 26900));
            mock.Setup(m => m.Send(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<SocketFlags>())).Returns(4);

            return mock;
        }

        public static TelnetClient GetTelnetClient(ITelnetSocket socket)
        {
            var telnetClient = new TelnetClient(socket)
            {
                Encoding = Encoding.UTF8,
                TelnetEventWaitTime = 200,
                ReceiveTimeout = 500,
                ReceiveBufferSize = 1024
            };

            return telnetClient;
        }

        [Test]
        public void ConnectTest()
        {
            var mock = GetSocketMock();
            using var telnetClient = GetTelnetClient(mock.Object);
            var connected = telnetClient.Connect("localhost", 26900);
            var exp = true;

            ClassicAssert.AreEqual(exp, connected);
        }

        [Test]
        public void ReadTest()
        {
            var mock = GetSocketMock();
            using var telnetClient = GetTelnetClient(mock.Object);
            var log = telnetClient.Read().TrimEnd('\0');
            var exp = "test\n";

            ClassicAssert.AreEqual(exp, log);
        }

        [Test]
        public void DestructionEventReadTest()
        {
            var mock = GetSocketMock();
            using var telnetClient = GetTelnetClient(mock.Object);
            var log = telnetClient.DestructionEventRead("lp");
            var exp = "test\n";

            ClassicAssert.AreEqual(exp, log);
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
            using var telnetClient = GetTelnetClient(mock.Object);

            var value = telnetClient.DestructionEventRead("");
            var exp = "test message.\n";
            ClassicAssert.AreEqual(exp, value);
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
            using var telnetClient = GetTelnetClient(mock.Object);

            var value = telnetClient.DestructionEventRead("", "test3 message.");
            var exp = "test\ntest2\ntest3 message.\n";
            ClassicAssert.AreEqual(exp, value);
        }

        [Test]
        public void CalculateWaitTimeTest()
        {
            var mock = GetSocketMock();
            var cnt = 0;
            mock.Setup(m => m.Receive(It.IsAny<byte[]>(), It.IsAny<SocketFlags>())).Callback<byte[], SocketFlags>(
                (buffer, flag) => {
                    if (cnt == 0)
                    {
                        var data = Encoding.UTF8.GetBytes("");
                        foreach (var item in data.Select((v, i) => new { Index = i, Value = v }))
                        {
                            buffer[item.Index] = item.Value;
                        }
                        cnt++;
                    }
                    else
                    {
                        var data = Encoding.UTF8.GetBytes("test\n");
                        foreach (var item in data.Select((v, i) => new { Index = i, Value = v }))
                        {
                            buffer[item.Index] = item.Value;
                        }
                    }
                });

            using var telnetClient = GetTelnetClient(mock.Object);
            var waitTime = telnetClient.CalculateWaitTime();
            var exp = 100;

            ClassicAssert.AreEqual(exp, waitTime);
        }

        [Test]
        public void WriteLineTest()
        {
            var mock = GetSocketMock();
            using var telnetClient = GetTelnetClient(mock.Object);

            var cmd = "help";

            telnetClient.WriteLine(cmd);

            var data = telnetClient.Encoding.GetBytes(cmd);
            var concat = data.Concat(telnetClient.BreakLineData).ToArray();
            mock.Verify(x => x.Send(concat, concat.Length, SocketFlags.None));
        }

        [Test]
        public void WriteStringTest()
        {
            var mock = GetSocketMock();
            using var telnetClient = GetTelnetClient(mock.Object);

            var cmd = "help";

            telnetClient.Write(cmd);

            var data = telnetClient.Encoding.GetBytes(cmd);
            mock.Verify(x => x.Send(data, data.Length, SocketFlags.None));
        }

        [Test]
        public void WriteByteTest()
        {
            var mock = GetSocketMock();
            using var telnetClient = GetTelnetClient(mock.Object);

            var cmd = telnetClient.Encoding.GetBytes("help");

            telnetClient.Write(cmd);
            
            mock.Verify(x => x.Send(cmd, cmd.Length, SocketFlags.None));
        }

        [Test]
        public void BreakLineTest()
        {
            var mock = GetSocketMock();
            using var telnetClient = GetTelnetClient(mock.Object);

            telnetClient.BreakLine = TelnetClient.BreakLineType.Cr;
            var breakLine1 = telnetClient.BreakLineData;

            telnetClient.BreakLine = TelnetClient.BreakLineType.Lf;
            var breakLine2 = telnetClient.BreakLineData;

            telnetClient.BreakLine = TelnetClient.BreakLineType.CrLf;
            var breakLine3 = telnetClient.BreakLineData;

            CollectionAssert.AreEqual(TelnetClient.Cr, breakLine1);
            CollectionAssert.AreEqual(TelnetClient.Lf, breakLine2);
            CollectionAssert.AreEqual(TelnetClient.Crlf, breakLine3);
        }

        [Test]
        public void HandleTcpTest()
        {
            var manualEvent = new ManualResetEvent(false);

            var mock = GetSocketMock();
            using var telnetClient = GetTelnetClient(mock.Object);
            
            var result = "";
            telnetClient.ReadEvent += (_, args) =>
            {
                result = args.Log;
                manualEvent.Set();
            };
            telnetClient.Connect("", 0);

            telnetClient.WriteLine("help");

            manualEvent.WaitOne(1000, false);

            ClassicAssert.AreEqual("test\n", result);
        }
    }
}
