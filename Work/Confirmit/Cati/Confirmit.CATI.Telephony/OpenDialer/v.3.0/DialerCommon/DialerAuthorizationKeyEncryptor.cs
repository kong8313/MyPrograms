using System.Text;
using Confirmit.CATI.Common.Encryption;
using Confirmit.CATI.Common.Random;

namespace DialerCommon
{
    public class DialerAuthorizationKeyEncryptor : CatiSymmetricEncryptor
    {
        //private const string AuthorizationKey = "{1FD74359-A908-4743-AFF1-4A58979CC6F7}";

        private readonly byte[] AuthorizationKey = new byte[32] {
            41, 194, 21, 235, 10, 25, 81, 104, 82, 127, 61, 29, 62, 224, 171, 252,
            52, 162, 93, 141, 19, 146, 84, 197, 91, 246, 248, 101, 166, 210, 252, 88
        } ;

        public DialerAuthorizationKeyEncryptor()
        {
            Key = AuthorizationKey;
        }

        public new string EncryptString(string text)
        {
            var ivString = RandomString(IV.Length);
            IV = Encoding.ASCII.GetBytes(ivString);

            return string.Format("{0}{1}", ivString, base.EncryptString(text));
        }

        public new string DecryptString(string cipherText)
        {
            var ivString = cipherText.Substring(0, IV.Length);
            IV = Encoding.ASCII.GetBytes(ivString);

            return base.DecryptString(cipherText.Substring(IV.Length));
        }

        public static string RandomString(int length)
        {
            const string charSet = 
                "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefjhijklmnopqrstuvwxyz0123456789+/";

            var builder = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                builder.Append(charSet[Randomizer.Next(charSet.Length)]);
            }

            return builder.ToString();
        }
    }
}
