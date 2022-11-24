using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using NUnit.Framework;
using SvManagerLibrary.Crypto;

namespace SvManagerLibraryTests2.Crypto
{
    [TestFixture]
    public class RsaWrapperTests
    {
        [Test]
        public void EncryptStringTest()
        {
            var encryption = new RsaWrapper(RsaWrapper.KeyType.All);
            var decryption = new RsaWrapper()
            {
                PublicKey = encryption.PublicKey,
                PrivateKey = encryption.PrivateKey
            };
            
            var text = "help";
            var encrypted = encryption.Encrypt(text);
            var decrypted = decryption.Decrypt(encrypted);

            Assert.AreEqual(text, decrypted);
        }
    }
}
