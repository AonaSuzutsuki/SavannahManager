using SvManagerLibrary.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SvManagerLibraryTests2.Chat
{
    [TestFixture]
    public class ChatInfoTests
    {
        [Test]
        public void IsNullOrEmptyTestWithNull()
        {
            var exp = true;
            var act = ChatInfo.IsNullOrEmpty(null);
            Assert.AreEqual(exp, act);
        }

        [Test]
        public void IsNullOrEmptyTestWithEmpty()
        {
            var exp = true;
            var act = ChatInfo.IsNullOrEmpty(new ChatInfo());
            Assert.AreEqual(exp, act);
        }

        [Test]
        public void IsNullOrEmptyTestWithSingleElem()
        {
            var exp = false;
            var act = ChatInfo.IsNullOrEmpty(new ChatInfo() { Message = "test" });
            var act2 = ChatInfo.IsNullOrEmpty(new ChatInfo() { Name = "test" });

            Assert.AreEqual(exp, act);
            Assert.AreEqual(exp, act2);
        }

        [Test]
        public void IsNullOrEmptyTestWithDoubleElem()
        {
            var exp = false;
            var act = ChatInfo.IsNullOrEmpty(new ChatInfo() { Name = "Server", Message = "test" });

            Assert.AreEqual(exp, act);
        }
    }
}