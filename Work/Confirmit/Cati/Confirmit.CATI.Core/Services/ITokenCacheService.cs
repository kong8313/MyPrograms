namespace Confirmit.CATI.Core.Services
{
    public interface ITokenCacheService
    {
        void Set(string key, string value);
        string Get(string key);
        void Remove(string key);
    }
}
