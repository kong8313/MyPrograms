using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DialerCommon.UnitTests
{
    [TestClass]
    public class RequestIdTest
    {
        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void RequestIdShouldBeChangedAfterNext()
        {
            var requestId = new RequestId();
            var id1 = requestId.Next();
            var id2 = requestId.Next();

            Assert.AreNotEqual(id1, id2);
        }
        
        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void RequestIdIsSetOnAThread()
        {
            var id = new RequestId().Next();

            var slot = Thread.GetNamedDataSlot("DialerRequestId");
            var data = (string) Thread.GetData(slot);

            Assert.AreEqual(id.ToString(), data);
        }
    }
}