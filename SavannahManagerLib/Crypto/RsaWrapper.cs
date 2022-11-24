using System;
using System.Security.Cryptography;
using System.Text;

namespace SvManagerLibrary.Crypto
{
    public class RsaWrapper : IDisposable
    {

        public enum KeyType
        {
            All,
            Public,
            Private
        }

        private readonly RSACryptoServiceProvider _rsaCryptoService = new RSACryptoServiceProvider();

        public string PublicKey { get; set; }

        public string PrivateKey { get; set; }

        public RsaWrapper(bool isCreateAllKey = true)
        {
            if (isCreateAllKey)
                CreateKey();
        }

        public void CreatePublicKey()
        {
            Initialize(KeyType.Private);

            PublicKey = _rsaCryptoService.ToXmlString(false);
        }

        private void CreateKey()
        {
            using var rsa = new RSACryptoServiceProvider();

            PrivateKey = rsa.ToXmlString(true);
            PublicKey = rsa.ToXmlString(false);
        }

        private void Initialize(KeyType keyType)
        {
            _rsaCryptoService.FromXmlString(keyType == KeyType.Private ? PrivateKey : PublicKey);
        }
        
        #region Encrypt
        public byte[] Encrypt(byte[] data)
        {
            Initialize(KeyType.Public);
            var encrypted = _rsaCryptoService.Encrypt(data, true);
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
            Initialize(KeyType.Private);
            var decrypted = _rsaCryptoService.Decrypt(data, true);
            return decrypted;
        }

        public string Decrypt(string text)
        {
            var encrypted = Convert.FromBase64String(text);
            var data = Decrypt(encrypted);
            return Encoding.UTF8.GetString(data);
        }
        #endregion

        public void Dispose()
        {
            _rsaCryptoService?.Dispose();
        }
    }
}
