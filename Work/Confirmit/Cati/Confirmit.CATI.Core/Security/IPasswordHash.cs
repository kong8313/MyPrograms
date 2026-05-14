namespace Confirmit.CATI.Core.Security
{
    public interface IPasswordHash
    {
        string ComputeHash(string password, string salt);
        string GenerateSaltValue();
        bool ValidateHash(string password, string salt, string hash);
        bool IsLegacyHash(string hash);
        bool ValidateLegacyHash(int personId, string password, string salt, string hash);
    }
}
