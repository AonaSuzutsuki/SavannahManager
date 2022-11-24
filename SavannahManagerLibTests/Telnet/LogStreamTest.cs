using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CommonCoreLib;
using NUnit.Framework;
using SvManagerLibrary.Telnet;

namespace SvManagerLibraryTests2.Telnet
{
    [TestFixture]
    public class LogStreamTest
    {
        [Test]
        public void WriteStringTest()
        {
            var exp = "TEST\nLOG.\n";
            
            using var logStream = new LogStream($"{AppInfo.GetAppPath()}\\logs")
            {
                AutoFlush = true,
                TextEncoding = Encoding.UTF8
            };

            logStream.Write("TEST");
            logStream.Write("\n");
            logStream.Write("LOG.\n");

            logStream.Stream.Position = 0;
            
            var data = new byte[logStream.Length];
            _ = logStream.Stream.Read(data, 0, data.Length);
            var act = logStream.TextEncoding.GetString(data);

            Assert.AreEqual(exp, act);
        }

        [Test]
        public void WriteByteTest()
        {
            using var logStream = new LogStream($"{AppInfo.GetAppPath()}\\logs")
            {
                AutoFlush = true,
                TextEncoding = Encoding.UTF8
            };

            var exp = logStream.TextEncoding.GetBytes("TEST\nLOG.\n");

            logStream.Write(exp, 0, exp.Length);
            
            logStream.Stream.Position = 0;

            var data = new byte[logStream.Length];
            _ = logStream.Stream.Read(data, 0, data.Length);

            Assert.AreEqual(exp, data);
        }
    }
}
