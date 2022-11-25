using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using CommonCoreLib;
using NUnit.Framework;
using SvManagerLibrary.Telnet;

namespace SvManagerLibraryTests2.Telnet
{
    [TestFixture]
    public class LogStreamTest
    {
        private Stream GetStream(object obj)
        {
            var type = obj.GetType();
            var streamProperty = type.GetProperty("Stream", BindingFlags.NonPublic | BindingFlags.Instance);
            var getter = streamProperty?.GetGetMethod(nonPublic: true);
            return (Stream)getter?.Invoke(obj, null);
        }

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

            var stream = GetStream(logStream);

            stream.Position = 0;
            
            var data = new byte[logStream.Length];
            _ = stream.Read(data, 0, data.Length);
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

            var stream = GetStream(logStream);

            stream.Position = 0;

            var data = new byte[logStream.Length];
            _ = stream.Read(data, 0, data.Length);

            Assert.AreEqual(exp, data);
        }
    }
}
