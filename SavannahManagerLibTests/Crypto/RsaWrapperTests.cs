using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using SvManagerLibrary.Crypto;

namespace SvManagerLibraryTests2.Crypto
{
    [TestFixture]
    public class RsaWrapperTests
    {
        [Test]
        public void EncryptStringTest()
        {
            using var encryption = new RsaWrapper();
            using var decryption = new RsaWrapper(false)
            {
                PublicKey = encryption.PublicKey,
                PrivateKey = encryption.PrivateKey
            };
            
            var text = "help";
            var encrypted = encryption.Encrypt(text);
            var decrypted = decryption.Decrypt(encrypted);

            ClassicAssert.AreEqual(text, decrypted);
        }

        [Test]
        public void CreateKeyTest()
        {
            var keyCreator = new RsaWrapper();
            var decryption = new RsaWrapper(false)
            {
                PrivateKey = keyCreator.PrivateKey
            };
            decryption.CreatePublicKey();
            
            var encryption = new RsaWrapper(false)
            {
                PublicKey = decryption.PublicKey
            };

            var text = "help";
            var encrypted = encryption.Encrypt(text);
            var decrypted = decryption.Decrypt(encrypted);

            ClassicAssert.AreEqual(text, decrypted);
        }
    }
}
