﻿using System;
using System.Diagnostics;
using NUnit.Framework;
using NUnit.Framework.Legacy;
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

            ClassicAssert.AreEqual(300, waiter.ElapsedSeconds);
        }

        [Test]
        public void NextText2()
        {
            var waiter = new TelnetWaiter
            {
                MaxMilliseconds = 0
            };

            while (waiter.CanLoop)
            {
                waiter.Next();
            }

            ClassicAssert.AreEqual(0, waiter.ElapsedSeconds);
        }
    }
}
