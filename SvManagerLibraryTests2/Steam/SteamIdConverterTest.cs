using System;
using NUnit.Framework;
using SvManagerLibrary.Steam;

namespace SvManagerLibraryTests2.Steam
{
    [TestFixture]
    public class SteamIdConverterTest
    {
        [Test]
        public void ToSteamIdTest()
        {
            var profileId = SteamIdConverter.ToSteamId(76561198010715714);
            var groupId = SteamIdConverter.ToSteamId(103582791437070330);

            Assert.AreEqual("STEAM_1:0:25224993", profileId);
            Assert.AreEqual("STEAM_1:0:3774461", groupId);
        }
    }
}
