using System;
using System.Diagnostics;
using NUnit.Framework;
using SvManagerLibrary.Telnet;

namespace SvManagerLibraryTests2.Telnet
{
    [TestFixture]
    public class TelnetWaiterTest
    {
        [Test]
        public void NextText()
        {
            var waiter = new TelnetWaiter
            {
                MaxMilliseconds = 300
            };

            while (waiter.CanLoop)
            {
                waiter.Next();
            }

            Assert.AreEqual(300, waiter.Count);
        }
    }
}
