using Confirmit.CATI.Common.Encoding;
using Confirmit.Test.Common.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Common.UnitTests
{
    [TestClass]
    public class HttpUtilitySafeTest
    {       
        [TestMethod, Owner(@"FIRM\AlexanderZh"), Bug(61559)]
        public void HtmlEncode_StringContainsSpecialCharacters_Correct()
        {                        
            Assert.AreEqual("^%&amp;", HttpUtilitySafe.HtmlEncode("^%&"));
            Assert.AreEqual("test&amp;message", HttpUtilitySafe.HtmlEncode("test&message"));
        }     
    }
}
