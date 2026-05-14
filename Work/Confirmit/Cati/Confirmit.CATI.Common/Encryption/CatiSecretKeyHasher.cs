using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Confirmit.CATI.Common.Encryption
{
    public class CatiSecretKeyHasher : ICatiSecretKeyHasher
    {
        public byte[] ComputeHash(byte[] secretKey, string companyAlias, ClientErrorSource source, string message)
        {
             using (var cryptProvider = new MD5CryptoServiceProvider())
             {
                 var hash = cryptProvider.ComputeHash(
                    secretKey.Concat(
                        new ASCIIEncoding().GetBytes(
                            companyAlias + source + message))
                   .ToArray());

                 return hash;
             }
        }

        public bool VerifyComputedHash(byte[] secretKey, byte[] expectedHash, string companyAlias, ClientErrorSource source, string message)
        {
            return ComputeHash(secretKey, companyAlias, source, message).SequenceEqual(expectedHash);
        }
    }
}
