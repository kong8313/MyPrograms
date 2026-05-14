using System;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens;

namespace Confirmit.CATI.Core.Services
{
    public class TokenCacheService : ITokenCacheService
    {
        private readonly ConcurrentDictionary<string, string> Cache = new ConcurrentDictionary<string, string>();

        public void Set(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                Cache[key] = value;
            }
        }

        public string Get(string key)
        {
            if (Cache.TryGetValue(key, out var storedToken))
            {
                try
                {
                    var token = new JwtSecurityTokenHandler().ReadToken(storedToken);
                    return (token != null && token.ValidFrom <= DateTime.UtcNow.AddMinutes(1) &&
                            token.ValidTo > DateTime.UtcNow.AddMinutes(1))
                        ? storedToken
                        : null;
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        public void Remove(string key)
        {
            string tmp;
            Cache.TryRemove(key, out tmp);
        }
    }
}
