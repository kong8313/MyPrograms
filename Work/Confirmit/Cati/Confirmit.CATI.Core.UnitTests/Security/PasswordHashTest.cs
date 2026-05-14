using Confirmit.CATI.Core.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.Security
{
    [TestClass]
    public class PasswordHashTest
    {
        private string _modernHashString;
        private string _lecacyHashString;
        private string _legacySalt;
        private PasswordHash _hash;

        [TestInitialize]
        public void TestInitialize()
        {
            _hash = new PasswordHash();
            _lecacyHashString = "WilGsfbpiY/2sionlGQ3uQ==";
            _modernHashString = "wo7VglHWErm4LKptP1soVHv8a5t/F7+TRf+WzpgqxIxhdWmV9Pdp4b4ANYZdF0+PZ8ogCy4HVNgWb/XGujMj0g==";
            _legacySalt = "1b4y6A==";
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void IsLegacyHash_LegacyHash_ReturnsTrue()
        {
            Assert.IsTrue(_hash.IsLegacyHash(_lecacyHashString), "Provided hash {0} is not recognized as legacy", _lecacyHashString);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void IsLegacyHash_ModernHash_ReturnsFalse()
        {
            Assert.IsFalse(_hash.IsLegacyHash(_modernHashString), "Provided hash {0} is not recognized as modern", _modernHashString);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void ValidateLegacyHash_CorrectLegacyHash_ReturnsTrue()
        {
            const int personId = 38;
            const string password = "1";
            Assert.IsTrue(_hash.ValidateLegacyHash(personId, password, _legacySalt, _lecacyHashString));
        }
    }
}
