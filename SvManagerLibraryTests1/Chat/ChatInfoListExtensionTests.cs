using Microsoft.VisualStudio.TestTools.UnitTesting;
using SvManagerLibrary.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvManagerLibrary.Chat.Tests
{
    [TestClass()]
    public class ChatInfoListExtensionTests
    {
        [TestMethod()]
        public void AddTest()
        {
            var text = "2019-01-19T16:14:21 140.048 INF Chat (from '-non-player-', entity id '-1', to 'Global'): 'Server': Hello, World.";
            var act = new List<ChatInfo> { text };
            var exp = new List<ChatInfo>
            {
                new ChatInfo()
                {
                    Name = "Server",
                    Message = "Hello, World."
                }
            };

            CollectionAssert.AreEqual(exp, act);
        }

        [TestMethod()]
        public void AddMultiLineTest()
        {
            var text = "2019-01-19T16:14:21 140.048 INF Chat (from '-non-player-', entity id '-1', to 'Global'): 'Server': Hello, World.\r\n";
            text += "2019-01-19T16:14:21 140.048 INF Chat (from '-non-player-', entity id '-1', to 'Global'): 'Server2': HogeHoge.\r\nasad9asc8z";
            var act = new List<ChatInfo>();
            act.AddMultiLine(text);
            var exp = new List<ChatInfo>
            {
                new ChatInfo()
                {
                    Name = "Server",
                    Message = "Hello, World."
                },
                new ChatInfo()
                {
                    Name = "Server2",
                    Message = "HogeHoge."
                }
            };

            CollectionAssert.AreEqual(exp, act);
        }

        [TestMethod()]
        public void GetLastTest()
        {
            var exp = new ChatInfo { Name = "Server", Message = "Hello, World." };
            var text = "2019-01-19T16:14:21 140.048 INF Chat (from '-non-player-', entity id '-1', to 'Global'): 'Server': Hello, World.";
            var list = new List<ChatInfo> { text };
            var act = list.GetLast();

            Assert.AreEqual(exp, act);
        }

        [TestMethod()]
        public void GetLastTestWithNull ()
        {
            ChatInfo exp = null;
            var list = new List<ChatInfo>();
            var act = list.GetLast();

            Assert.AreEqual(exp, act);
        }
    }
}