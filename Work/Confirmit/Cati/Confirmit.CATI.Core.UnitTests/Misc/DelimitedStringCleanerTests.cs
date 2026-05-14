using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Core.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.Misc
{
    [TestClass]
    public class DelimitedStringCleanerTests
    {
        private static DelimitedStringCleaner _cleaner;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _cleaner = new DelimitedStringCleaner();   
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void ParseString_CommaSeparatedString_ReturnsArrayOfString()
        {
            const string input = "q1,q2,q3,q44,qwertty";

            var result = _cleaner.ParseString(input).ToArray();

            Assert.IsTrue(result.Count() == 5);
            CollectionAssert.AreEqual(new[] { "q1", "q2", "q3", "q44", "qwertty" }, result);
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void ParseString_CommaAndSemicolonSeparatedString_ReturnsArrayOfString()
        {
            const string input = "q1,q2;q3,q44;qwertty";

            var result = _cleaner.ParseString(input).ToArray();

            Assert.IsTrue(result.Count() == 5);
            CollectionAssert.AreEqual(new[] { "q1", "q2", "q3", "q44", "qwertty" }, result);
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void ParseString_CommaSeparatedStringWithEmptyValues_ReturnsArrayOfString()
        {
            const string input = ",q1,q2,q3,q44,,qwertty";

            var result = _cleaner.ParseString(input).ToArray();

            Assert.IsTrue(result.Count() == 5);
            CollectionAssert.AreEqual(new[] { "q1", "q2", "q3", "q44", "qwertty" }, result);
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void ParseString_CommaSeparatedStringWithWhitespacesArountValues_ReturnsArrayOfString()
        {
            const string input = "q1,q2 ,q3 , q44,, qwertty";

            var result = _cleaner.ParseString(input).ToArray();

            Assert.IsTrue(result.Count() == 5);
            CollectionAssert.AreEqual(new[] { "q1", "q2", "q3", "q44", "qwertty" }, result);
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void CleanString_CommaAndSemicolonSeparatedString_ReturnsCleanString()
        {
            const string input = "q1 , q2; q3,q44;,qwertty";

            var result = _cleaner.CleanString(input);

            Assert.AreEqual("q1;q2;q3;q44;qwertty", result);
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void ParseString_NullString_ReturnsEmptyArray()
        {
            const string input = null;

            var result = _cleaner.ParseString(input).ToArray();

            CollectionAssert.AreEqual(new string[] { }, result);
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void CleanString_NullString_ReturnsEmptyString()
        {
            const string input = null;

            var result = _cleaner.CleanString(input);

            Assert.AreEqual(string.Empty, result);
        }
    }
}
