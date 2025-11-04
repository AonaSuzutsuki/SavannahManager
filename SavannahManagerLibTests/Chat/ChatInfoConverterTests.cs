using SvManagerLibrary.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using SvManagerLibrary.AnalyzerPlan.Console;

namespace SvManagerLibraryTests2.Chat
{
    [TestFixture]
    public class ChatInfoConverterTests
    {
        private IConsoleAnalyzer _analyzer = new OnePointTreeConsoleAnalyzer();

        [Test]
        public void ConvertChatTest()
        {
            var text = "2019-01-19T16:14:21 140.048 INF Chat (from '-non-player-', entity id '-1', to 'Global'): 'Server': Hello, World.";
            var exp = new ChatInfo()
            {
                Id = "-1",
                SteamId = "-non-player-",
                Name = "Server",
                Message = "Hello, World.",
                Date = "2019-01-19T16:14:21"
            };
            var act = ChatInfoConverter.ConvertChat(text, _analyzer);

            ClassicAssert.AreEqual(exp, act);
        }
    }
}