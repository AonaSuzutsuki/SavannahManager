using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using SvManagerLibrary.Crypto;

namespace SvManagerLibraryTests2.Crypto
{
    [TestFixture]
    public class RijndaelWrapperTests
    {
        public RijndaelWrapper GetRijndaelWrapper()
        {
            return new RijndaelWrapper("pass", "salt");
        }

        [Test]
        public void EncryptStringTest()
        {
            using var wrapper = GetRijndaelWrapper();
            var text = "help";
            var exp = "u2j9DzevlOMDoMQcnSy1/A==";
            var result = wrapper.Encrypt(text);

            ClassicAssert.AreEqual(exp, result);
        }

        [Test]
        public void DecryptStringTest()
        {
            using var wrapper = GetRijndaelWrapper();
            var text = "u2j9DzevlOMDoMQcnSy1/A==";
            var exp = "help";
            var result = wrapper.Decrypt(text);

            ClassicAssert.AreEqual(exp, result);
        }
    }
}
