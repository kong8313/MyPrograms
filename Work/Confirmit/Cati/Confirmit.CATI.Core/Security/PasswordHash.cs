using System;
using System.Security.Cryptography;
using System.Text;

namespace Confirmit.CATI.Core.Security
{
    public class PasswordHash : IPasswordHash
    {
        private const int MD5HashLenghtInBase64Encoding = 24;

        public string ComputeHash(string password, string salt)
        {
            string value = password + salt;
            using (var hashFunction = new SHA512Managed())
            {
                var hash = hashFunction.ComputeHash(Encoding.Unicode.GetBytes(value));
                return Convert.ToBase64String(hash);
            }
        }

        public string ComputeLegacyHash(int personId, string password, string salt)
        {
            string value = salt + password + personId;
            using (var hashFunction = new MD5CryptoServiceProvider())
            {
                return Convert.ToBase64String(hashFunction.ComputeHash(Encoding.Unicode.GetBytes(value)));
            }
        }

        public string GenerateSaltValue()
        {
            var saltBytes = new byte[64];
            using (var rgn = new RNGCryptoServiceProvider())
            {
                rgn.GetBytes(saltBytes);
                return Convert.ToBase64String(saltBytes);
            }
        }

        public bool ValidateHash(string password, string salt, string hash)
        {
            return StringComparer.InvariantCulture.Equals(ComputeHash(password, salt), hash);
        }

        public bool IsLegacyHash(string hash)
        {
            return hash.Length <= MD5HashLenghtInBase64Encoding;
        }

        public bool ValidateLegacyHash(int personId, string password, string salt, string hash)
        {
            return StringComparer.InvariantCulture.Equals(ComputeLegacyHash(personId, password, salt), hash);
        }
    }
}