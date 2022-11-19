using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;
using SvManagerLibrary.Telnet;

namespace SvManagerLibraryTests2.Chat
{
    [TestFixture]
    public class ChatTests
    {
        [Test]
        public void SendChatTest()
        {
            var mock = new Mock<ITelnetClient>();
            mock.Setup(x => x.Connected).Returns(true);

            var obj = mock.Object;
            SvManagerLibrary.Chat.Chat.SendChat(obj, "message");

            mock.Verify(x => x.WriteLine("say message"), Times.Once);
        }
    }
}
