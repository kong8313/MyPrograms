namespace DialerCommon
{
    public interface IDialerAuthorizationKeyEncryptor
    {
        string EncryptString(string text);

        string DecryptString(string cipherText);

        void Clear();
    }
}