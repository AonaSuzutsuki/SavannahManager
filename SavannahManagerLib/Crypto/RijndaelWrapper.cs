using System;
using System.Security.Cryptography;
using System.Text;

namespace SvManagerLibrary.Crypto
{
    public class RijndaelWrapper : IDisposable
    {

        private readonly RijndaelManaged _rijndael;

        public RijndaelWrapper(string password, string salt = null)
        {
            salt ??= password;
            salt = CommonCoreLib.Crypto.Sha256.GetSha256(salt);
            _rijndael = CreateKey(password, salt);
        }

        #region Initialize Key
        private static RijndaelManaged CreateKey(string password, string salt)
        {
            var rijndael =  new RijndaelManaged();
            
            var (key, iv) = GenerateKeyFromPassword(password, rijndael.KeySize, rijndael.BlockSize, salt);
            rijndael.Key = key;
            rijndael.IV = iv;

            return rijndael;
        }

        private static (byte[] key, byte[] iv) GenerateKeyFromPassword(string password, int keySize, int blockSize, string salt)
        {
            var saltByte = Encoding.UTF8.GetBytes(salt);
            var deriveBytes = new Rfc2898DeriveBytes(password, saltByte)
            {
                IterationCount = 1000
            };

            var key = deriveBytes.GetBytes(keySize / 8);
            var iv = deriveBytes.GetBytes(blockSize / 8);

            return (key, iv);
        }
        #endregion

        #region Encrypter
        public string Encrypt(string text)
        {
            var strBytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(Encrypt(strBytes));
        }

        public byte[] Encrypt(byte[] data)
        {
            using var encryptor = _rijndael.CreateEncryptor();
            var encBytes = encryptor.TransformFinalBlock(data, 0, data.Length);
            return encBytes;
        }
        #endregion

        #region Decrypter
        public string Decrypt(string text)
        {
            var data = Convert.FromBase64String(text);
            return Encoding.UTF8.GetString(Decrypt(data));
        }

        public byte[] Decrypt(byte[] data)
        {
            using var decryptor = _rijndael.CreateDecryptor();
            var decBytes = decryptor.TransformFinalBlock(data, 0, data.Length);
            return decBytes;
        }
        #endregion


        public void Dispose()
        {
            _rijndael.Dispose();
        }
    }
}
