using SvManagerLibrary.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Xml.Linq;
using NUnit.Framework.Legacy;

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
            ClassicAssert.AreEqual(exp, act);
        }

        [Test]
        public void IsNullOrEmptyTestWithEmpty()
        {
            var exp = true;
            var act = ChatInfo.IsNullOrEmpty(new ChatInfo());
            ClassicAssert.AreEqual(exp, act);
        }

        [Test]
        public void IsNullOrEmptyTestWithSingleElem()
        {
            var exp = false;
            var act = ChatInfo.IsNullOrEmpty(new ChatInfo() { Message = "test" });
            var act2 = ChatInfo.IsNullOrEmpty(new ChatInfo() { Name = "test" });

            ClassicAssert.AreEqual(exp, act);
            ClassicAssert.AreEqual(exp, act2);
        }

        [Test]
        public void IsNullOrEmptyTestWithDoubleElem()
        {
            var exp = false;
            var act = ChatInfo.IsNullOrEmpty(new ChatInfo() { Name = "Server", Message = "test" });

            ClassicAssert.AreEqual(exp, act);
        }

        [Test]
        public void GetMapTest()
        {
            var info = new ChatInfo
            {
                Id = "-1",
                SteamId = "-non-player-",
                Name = "Server",
                Message = "Hello, World.",
                Date = "2019-01-19T16:14:21"
            };
            var map = info.GetMap();
            var exp = new Dictionary<string, string>
            {
                { nameof(info.Date), info.Date },
                { nameof(info.Name), info.Name },
                { nameof(info.Id), info.Id },
                { nameof(info.SteamId), info.SteamId },
                { nameof(info.Message), info.Message }
            };

            CollectionAssert.AreEqual(exp, map);
        }

        [Test]
        public void NamesTest()
        {
            var info = new ChatInfo
            {
                Id = "-1",
                SteamId = "-non-player-",
                Name = "Server",
                Message = "Hello, World.",
                Date = "2019-01-19T16:14:21"
            };
            var names = ChatInfo.Names();
            var exp = new List<string>
            {
                nameof(info.Date),
                nameof(info.Name),
                nameof(info.Id),
                nameof(info.SteamId),
                nameof(info.Message)
            };

            CollectionAssert.AreEqual(exp, names);
        }
    }
}