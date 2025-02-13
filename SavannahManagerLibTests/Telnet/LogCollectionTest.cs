using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using SvManagerLibrary.Telnet;

namespace SvManagerLibraryTests2.Telnet
{
    [TestFixture]
    public class LogCollectionTest
    {
        [Test]
        public void AppendTest()
        {
            var collection = new LogCollection();
            collection.Append("aaaaaaaaa\nbbbb");
            collection.Append("bbbb\n");
            collection.Append("ccccccc");
            collection.Append("ccccccc");
            collection.Append("ccccccc");
            collection.Append("\n");
            collection.Append("\n");
            collection.Append("\n");
            collection.Append("dddddd");
            collection.Append("\neeeeee\n");
            collection.Append("\nfffff\nggg\nhhhh\n");
            collection.Append("\n");
            collection.Append("iiiii");

            var expList = new []
            {
                "aaaaaaaaa",
                "bbbbbbbb",
                "ccccccccccccccccccccc",
                "",
                "",
                "dddddd",
                "eeeeee",
                "",
                "fffff",
                "ggg",
                "hhhh",
                "",
                null,
            };

            foreach (var exp in expList)
            {
                var act = collection.GetFirst();
                ClassicAssert.AreEqual(exp, act);
            }
        }
    }
}
