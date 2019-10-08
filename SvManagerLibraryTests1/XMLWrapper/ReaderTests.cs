using CommonExtensionLib.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SvManagerLibrary.XMLWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvManagerLibrary.XMLWrapper.Tests
{
    [TestClass()]
    public class ReaderTests
    {
        public Reader GetReader()
        {
            var xmlPath = "{0}\\{1}".FormatString(AppDomain.CurrentDomain.BaseDirectory, "TestData\\Test.xml");
            var reader = new Reader(xmlPath);
            return reader;
        }

        [TestMethod()]
        public void GetAttributesTest()
        {
            var reader = GetReader();
            var attributes = reader.GetAttributes("name", "/ServerSettings/property");

            var exp = new List<string>
            {
                "ServerName",
                "ServerName2",
                "ServerDescription",
                "ServerWebsiteURL"
            };

            CollectionAssert.AreEqual(exp, attributes);
        }

        [TestMethod()]
        public void GetAttributeTest()
        {
            var reader = GetReader();
            var act = reader.GetAttribute("value", "/ServerSettings/property[@name='ServerName']");

            var exp = "My Game Host";

            Assert.AreEqual(exp, act);
        }

        [TestMethod()]
        public void GetValuesTest()
        {
            var reader = GetReader();
            var attributes = reader.GetValues("/ServerSettings/property");

            var exp = new List<string>
            {
                "サーバー名を設定します。サーバーリストにはこの名前で表示されます。",
                "サーバー名を設定します。サーバーリストにはこの名前で表示されます。\n    test",
                "サーバーの説明を設定します。",
                "サーバーのウェブサイトを設定します。"
            };

            CollectionAssert.AreEqual(exp, attributes);
        }

        [TestMethod()]
        public void GetValueTest()
        {
            var reader = GetReader();
            var act = reader.GetValue("/ServerSettings/property[@name='ServerName']", false);

            var exp = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。";

            Assert.AreEqual(exp, act);
        }

        [TestMethod()]
        public void GetValueTest2()
        {
            var reader = GetReader();
            var act = reader.GetValue("/ServerSettings/property[@name='ServerName2']");

            var exp = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。\n" +
                      "    test";

            Assert.AreEqual(exp, act);
        }
    }
}