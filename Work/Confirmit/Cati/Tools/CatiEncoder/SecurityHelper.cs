using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CatiEncoder
{
    public static class SecurityHelper
    {
        private const string AlgorithemHashName = "MD5";
        private const string AlgorithemName = "RC2";
        private const int KeySize = 128;
        private const string PasswordPassword = "1#a2b&3c4d";

        private static string Decrypt(string password, string encryptedBase64)
        {
            var pdb = new PasswordDeriveBytes(password, new byte[0]);

            var des = new TripleDESCryptoServiceProvider
            {
                IV = new byte[8],
                Key = pdb.CryptDeriveKey(AlgorithemName, AlgorithemHashName, KeySize, new byte[8])
            };

            var encryptedBytes = Convert.FromBase64String(encryptedBase64);
            byte[] plainBytes;

            using (var ms = new MemoryStream(encryptedBase64.Length))
            {
                using (var decStream = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    decStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                    decStream.FlushFinalBlock();
                    plainBytes = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(plainBytes, 0, (int)ms.Length);
                }
            }

            return Encoding.UTF8.GetString(plainBytes);
        }

        public static string DecryptConfigPassword(string password)
        {
            return Decrypt(PasswordPassword, password);
        }

        private static string Encrypt(string password, string plainMessage)
        {
            var pdb = new PasswordDeriveBytes(password, new byte[0]);

            var des = new TripleDESCryptoServiceProvider
            {
                IV = new byte[8],
                Key = pdb.CryptDeriveKey(AlgorithemName, AlgorithemHashName, KeySize, new byte[8])
            };

            byte[] encryptedBytes;
            using (var ms = new MemoryStream(plainMessage.Length * 2))
            {
                using (var encStream = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    var plainBytes = Encoding.UTF8.GetBytes(plainMessage);
                    encStream.Write(plainBytes, 0, plainBytes.Length);
                    encStream.FlushFinalBlock();
                    encryptedBytes = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(encryptedBytes, 0, (int)ms.Length);
                }
            }

            return Convert.ToBase64String(encryptedBytes);
        }

        public static string EncryptConfigPassword(string password)
        {
            return Encrypt(PasswordPassword, password);
        }
    }
}