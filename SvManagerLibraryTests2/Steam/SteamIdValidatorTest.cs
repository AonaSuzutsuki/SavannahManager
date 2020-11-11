using System;
using NUnit.Framework;
using SvManagerLibrary.Steam;

namespace SvManagerLibraryTests2.Steam
{
    [TestFixture]
    public class SteamIdValidatorTest
    {
        [Test]
        public void ValidateSteamIdTest()
        {
            var validator = new SteamIdValidator();
            var isValidated = SteamIdValidator.ValidateSteamId(76561198010715714);
            var isValidated2 = SteamIdValidator.ValidateSteamId(413435657435212879042);

            Assert.AreEqual(true, isValidated);
            Assert.AreEqual(false, isValidated2);
        }
    }
}
