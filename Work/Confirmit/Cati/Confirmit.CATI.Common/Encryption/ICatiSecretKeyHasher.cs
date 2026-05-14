namespace Confirmit.CATI.Common.Encryption
{
    public interface ICatiSecretKeyHasher
    {
        byte[] ComputeHash(byte[] secretKey, string companyAlias, ClientErrorSource source, string message);
        bool VerifyComputedHash(byte[] secretKey, byte[] expectedHash, string companyAlias, ClientErrorSource source, string message);
    }
}