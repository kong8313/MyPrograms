using System;
using System.Globalization;
using System.IdentityModel.Tokens;
using Confirmit.CATI.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.Services
{
    [TestClass]
    public class TokenCacheServiceTests
    {
        private readonly Random _random = new Random();
        private TokenCacheService _tokenCacheService;

        [TestInitialize()]
        public void Initialize()
        {
            _tokenCacheService = new TokenCacheService();
        }

        [TestMethod, Owner(@"FIRM\DmitryS")]
        public void SetTokenEmptyString_ReadTokenNull()
        {
            _tokenCacheService.Set("token", "");
            var cachedToken = _tokenCacheService.Get("token");
            Assert.IsNull(cachedToken);
        }

        [TestMethod, Owner(@"FIRM\DmitryS")]
        public void SetTokenNull_ReadTokenNull()
        {
            _tokenCacheService.Set("token", null);
            var cachedToken = _tokenCacheService.Get("token");
            Assert.IsNull(cachedToken);
        }

        [TestMethod, Owner(@"FIRM\DmitryS")]
        public void SetTokenNotValid_ReadTokenNull()
        {
            _tokenCacheService.Set("token", null);
            var cachedToken = _tokenCacheService.Get("NotValid");
            Assert.IsNull(cachedToken);
        }

        [TestMethod, Owner(@"FIRM\DmitryS")]
        public void SetBadToken_ReadTokenNull()
        {
            _tokenCacheService.Set("token", "bad token");
            var cachedToken = _tokenCacheService.Get("token");
            Assert.IsNull(cachedToken);
        }

        [TestMethod, Owner(@"FIRM\DmitryS")]
        public void SetToken_CanReadToken_TokensEqual()
        {
            var token = GenerateToken();
            _tokenCacheService.Set("token", token);
            var cachedToken = _tokenCacheService.Get("token");
            Assert.AreEqual(token, cachedToken);
        }

        [TestMethod, Owner(@"FIRM\DmitryS")]
        public void SetToken_CanReadToken_OverrideToken()
        {
            var token = GenerateToken();
            _tokenCacheService.Set("token", token);
            var cachedToken = _tokenCacheService.Get("token");
            Assert.AreEqual(token, cachedToken);

            var token2 = GenerateToken();
            _tokenCacheService.Set("token", token2);
            var cachedToken2 = _tokenCacheService.Get("token");
            Assert.AreEqual(token2, cachedToken2);
            Assert.AreNotEqual(cachedToken, cachedToken2);
        }

        [TestMethod, Owner(@"FIRM\DmitryS")]
        public void SetTokens_CanReadTokens_TokensAreEqual()
        {
            var token = GenerateToken();
            var token2 = GenerateToken();

            _tokenCacheService.Set("token", token);
            _tokenCacheService.Set("token2", token2);

            var cachedToken = _tokenCacheService.Get("token");
            var cachedToken2 = _tokenCacheService.Get("token2");

            Assert.AreEqual(token, cachedToken);
            Assert.AreEqual(token2, cachedToken2);
            Assert.AreNotEqual(cachedToken, cachedToken2);
        }

        [TestMethod, Owner(@"FIRM\DmitryS")]
        public void SetToken_TimeExpired_GetNull()
        {
            var token = GenerateToken(-1, -2);
            _tokenCacheService.Set("token", token);
            var cachedToken = _tokenCacheService.Get("token");
            Assert.IsNull(cachedToken);
        }

        [TestMethod, Owner(@"FIRM\DmitryS")]
        public void SetToken_TimeBeforeNotValid_GetNull()
        {
            var token = GenerateToken(2, 1);
            _tokenCacheService.Set("token", token);
            var cachedToken = _tokenCacheService.Get("token");
            Assert.IsNull(cachedToken);
        }

        private string GenerateToken(double expiresIn = 1, double notBefore = 0)
        {
            JwtSecurityToken jwt =
                new JwtSecurityToken(
                    issuer: _random.NextDouble().ToString(CultureInfo.InvariantCulture),
                    expires: DateTime.UtcNow + TimeSpan.FromHours(expiresIn),
                    notBefore: DateTime.UtcNow + TimeSpan.FromHours(notBefore));

            JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();
            return jwtHandler.WriteToken(jwt);
        }
    }
}
