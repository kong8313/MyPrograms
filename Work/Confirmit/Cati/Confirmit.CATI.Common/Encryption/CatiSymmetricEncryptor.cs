using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Confirmit.CATI.Common.Encryption
{
    public class CatiSymmetricEncryptor : ICatiSymmetricEncryptor
    {
        private readonly SymmetricAlgorithm _encryptor;
        
        public CatiSymmetricEncryptor()
        {
            _encryptor = new AesManaged {Padding = PaddingMode.PKCS7};
        }

        public void Dispose()
        {
            ((IDisposable)_encryptor).Dispose();
        }

        public byte[] Key
        {
            get { return _encryptor.Key; }
            set { _encryptor.Key = value; }
        }

        public byte[] IV
        {
            get { return _encryptor.IV; }
            set { _encryptor.IV = value; }
        }

        public string EncryptString(string text)
        {
            byte[] plainTextBytes = new UnicodeEncoding().GetBytes(text);

            using (var cipherTextStream = new MemoryStream())
            using (var cryptoTransform = _encryptor.CreateEncryptor())
            using (var cryptoStream = new CryptoStream(cipherTextStream, cryptoTransform, CryptoStreamMode.Write))
            {
                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                cryptoStream.FlushFinalBlock();

                return Convert.ToBase64String(cipherTextStream.ToArray());
            }
        }

        public string DecryptString(string cipherText)
        {
            var cipherTextBytes = Convert.FromBase64String(cipherText);

            using (var plainTextStream = new MemoryStream())
            using (var cryptoTransform = _encryptor.CreateDecryptor())
            using (var cryptoStream = new CryptoStream(plainTextStream, cryptoTransform, CryptoStreamMode.Write))
            {
                cryptoStream.Write(cipherTextBytes, 0, cipherTextBytes.Length);
                cryptoStream.FlushFinalBlock();

                return new UnicodeEncoding().GetString(plainTextStream.ToArray());
            }

        }

        public void Clear()
        {
            _encryptor.Clear();
        }
    }
}
