using SvManagerLibrary.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace SvManagerLibraryTests2.Chat
{
    [TestFixture]
    public class ChatInfoListExtensionTests
    {
        [Test]
        public void AddTest()
        {
            var text = "2019-01-19T16:14:21 140.048 INF Chat (from '-non-player-', entity id '-1', to 'Global'): 'Server': Hello, World.";
            var act = new List<ChatInfo> { text };
            var exp = new List<ChatInfo>
            {
                new ChatInfo()
                {
                    Id = "-1",
                    SteamId = "-non-player-",
                    Name = "Server",
                    Message = "Hello, World.",
                    Date = "2019-01-19T16:14:21"
                }
            };

            CollectionAssert.AreEqual(exp, act);
        }

        [Test]
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
                    Id = "-1",
                    SteamId = "-non-player-",
                    Name = "Server",
                    Message = "Hello, World.",
                    Date = "2019-01-19T16:14:21"
                },
                new ChatInfo()
                {
                    Id = "-1",
                    SteamId = "-non-player-",
                    Name = "Server2",
                    Message = "HogeHoge.",
                    Date = "2019-01-19T16:14:21"
                }
            };

            CollectionAssert.AreEqual(exp, act);
        }
    }
}