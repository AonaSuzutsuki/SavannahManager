using CommonExtensionLib.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SvManagerLibrary.XMLWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvManagerLibrary.XMLWrapper.Tests
{
    [TestClass()]
    public class WriterTests
    {
        [TestMethod()]
        public void WriteTest()
        {
            var xmlPath = "{0}\\{1}".FormatString(AppDomain.CurrentDomain.BaseDirectory, "TestData\\Test.xml");
            var exp = File.ReadAllText(xmlPath);

            var writer = new Writer();
            writer.SetRoot("ServerSettings");
            writer.AddElement("property", new AttributeInfo[]
                {
                    new AttributeInfo { Name = "name", Value = "ServerName" },
                    new AttributeInfo { Name = "value", Value = "My Game Host" }
                }, "\r\n    サーバー名を設定します。サーバーリストにはこの名前で表示されます。\r\n  ");
            writer.AddElement("property", new AttributeInfo[]
                {
                    new AttributeInfo { Name = "name", Value = "ServerDescription" },
                    new AttributeInfo { Name = "value", Value = "A 7 Days to Die server" }
                }, "サーバーの説明を設定します。");
            writer.AddElement("property", new AttributeInfo[]
                {
                    new AttributeInfo { Name = "name", Value = "ServerWebsiteURL" },
                    new AttributeInfo { Name = "value", Value = "" }
                }, "サーバーのウェブサイトを設定します。");

            using (var stream = new MemoryStream())
            {
                writer.Write(stream);
                stream.Seek(0, SeekOrigin.Begin);

                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                var act = Encoding.UTF8.GetString(buffer) + "\r\n";

                Assert.AreEqual(exp, act);
            }
        }

        [TestMethod()]
        public void AddElementTest()
        {
            var exp = "<?xml version=\"1.0\"?>\r\n" +
                      "<root>\r\n" +
                      "  <item attribute=\"attr\" />\r\n" +
                      "  <item attribute=\"attr2\">value</item>\r\n" +
                      "</root>\r\n";

            var writer = new Writer();
            writer.SetRoot("root");
            writer.AddElement("item", new AttributeInfo
            {
                Name = "attribute",
                Value = "attr"
            });
            writer.AddElement("item", new AttributeInfo
            {
                Name = "attribute",
                Value = "attr2"
            }, "value");

            using (var stream = new MemoryStream())
            {
                writer.Write(stream);
                stream.Seek(0, SeekOrigin.Begin);

                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                var act = Encoding.UTF8.GetString(buffer) + "\r\n";

                Assert.AreEqual(exp, act);
            }
        }

        [TestMethod()]
        public void AddElementNoAttributeTest()
        {
            var exp = "<?xml version=\"1.0\"?>\r\n" +
                      "<root>\r\n" +
                      "  <item />\r\n" +
                      "  <item>value</item>\r\n" +
                      "</root>\r\n";

            var writer = new Writer();
            writer.SetRoot("root");
            writer.AddElement("item");
            writer.AddElement("item", "value");

            using (var stream = new MemoryStream())
            {
                writer.Write(stream);
                stream.Seek(0, SeekOrigin.Begin);

                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                var act = Encoding.UTF8.GetString(buffer) + "\r\n";

                Assert.AreEqual(exp, act);
            }
        }
    }
}