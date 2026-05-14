using Confirmit.CATI.Supervisor.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Supervisor.UnitTests
{
    [TestClass]
    public class EscapeHelperTest
    {
        private readonly EscapeHelper _escaper = new EscapeHelper();

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void EscapeString_Letters()
        {
            var original = "AaBbYyZz";
            var expected = original;

            var actual = _escaper.EscapeString(original);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void EscapeString_Digits()
        {
            var original = "0123456789";
            var expected = original;

            var actual = _escaper.EscapeString(original);

            Assert.AreEqual(expected, actual);
        }
        
        [TestMethod, Owner(@"FIRM\KirillV")]
        public void EscapeString_ReservedCharecters()
        {
            var original = "-_.!*";
            var expected = original;

            var actual = _escaper.EscapeString(original);

            Assert.AreEqual(expected, actual);
        }
        
        [TestMethod, Owner(@"FIRM\KirillV")]
        public void EscapeString_DangerousCharacters_Brackets()
        {
            var original = "()[]{}<>";
            var expected = "%28%29%5B%5D%7B%7D%3C%3E";

            var actual = _escaper.EscapeString(original);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void EscapeString_DangerousCharacters_Misc()
        {
            var original = " \"#$%&'+,/:;=?@\\^`|~";
            var expected = "+%22%23%24%25%26%27%2B%2C%2F%3A%3B%3D%3F%40%5C%5E%60%7C%7E";

            var actual = _escaper.EscapeString(original);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void EscapeString_NullArgument()
        {
            string original = null;
            var expected = string.Empty;

            var actual = _escaper.EscapeString(original);

            Assert.AreEqual(expected, actual);
        }
    }
}