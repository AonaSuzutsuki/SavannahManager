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
    public class ChatInfoConverterTests
    {
        [Test]
        public void ConvertChatTest()
        {
            var text = "2019-01-19T16:14:21 140.048 INF Chat (from '-non-player-', entity id '-1', to 'Global'): 'Server': Hello, World.";
            var exp = new ChatInfo()
            {
                Name = "Server",
                Message = "Hello, World."
            };
            var act = ChatInfoConverter.ConvertChat(text);

            Assert.AreEqual(exp, act);
        }
    }
}