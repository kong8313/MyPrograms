using System;

namespace Confirmit.CATI.Common.Encryption
{
    public interface ICatiSymmetricEncryptor : IDisposable
    {
        byte[] Key { get; set; }
        byte[] IV { get; set; }

        string EncryptString(string text);
        string DecryptString(string cipherText);

        void Clear();
    }
}
