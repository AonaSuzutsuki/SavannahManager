using System;
using System.Security.Cryptography;
using System.Text;

namespace SvManagerLibrary.Crypto
{
    public class RsaWrapper
    {

        public enum KeyType
        {
            All,
            Public,
            Private
        }

        public string PublicKey { get; set; }

        public string PrivateKey { get; set; }

        public RsaWrapper() { }

        public RsaWrapper(KeyType keyType)
        {
            CreateKey(keyType);
        }

        private void CreateKey(KeyType keyType = KeyType.All)
        {
            var rsa = new RSACryptoServiceProvider();

            switch (keyType)
            {
                case KeyType.Private:
                    PrivateKey = rsa.ToXmlString(true);
                    break;
                case KeyType.Public:
                    PublicKey = rsa.ToXmlString(false);
                    break;
                default:
                    PrivateKey = rsa.ToXmlString(true);
                    PublicKey = rsa.ToXmlString(false);
                    break;
            }
        }

        #region Encrypt
        public byte[] Encrypt(byte[] data)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(PublicKey);

            var encrypted = rsa.Encrypt(data, true);
            return encrypted;
        }

        public string Encrypt(string text)
        {
            var data = Encoding.UTF8.GetBytes(text);
            var encrypted = Encrypt(data);
            var base64 = Convert.ToBase64String(encrypted);
            return base64;
        }
        #endregion

        #region Decrypt
        public byte[] Decrypt(byte[] data)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(PrivateKey);

            var decrypted = rsa.Decrypt(data, true);
            return decrypted;
        }

        public string Decrypt(string text)
        {
            var encrypted = Convert.FromBase64String(text);
            var data = Decrypt(encrypted);
            return Encoding.UTF8.GetString(data);
        }
        #endregion
    }
}
