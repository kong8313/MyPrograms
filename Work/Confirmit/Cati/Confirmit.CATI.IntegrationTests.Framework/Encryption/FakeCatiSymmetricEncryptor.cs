using Confirmit.CATI.Common.Encryption;

namespace Confirmit.CATI.IntegrationTests.Framework.Encryption
{
    public class FakeCatiSymmetricEncryptor : ICatiSymmetricEncryptor
    {
        public void Dispose()
        {
        }

        public byte[] Key
        {
            get { return new byte[0]; }
            set { }
        }

        public byte[] IV
        {
            get { return new byte[0]; }
            set { }
        }

        public string EncryptString(string text)
        {
            return text;
        }

        public string DecryptString(string cipherText)
        {
            return cipherText;
        }

        public void Clear()
        {
        }
    }
}
