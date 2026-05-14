using System;
using System.Threading;
using Confirmit.CATI.Backend.WebApiServices;
using Microsoft.Owin;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Backend.UnitTests.WebApiServices
{
    [TestClass]
    public class RateLimitingEngineTest
    {
        [TestMethod]
        public void Verify_NoIpInfo_ReturnedTrue()
        {
            var rle = new RateLimiter(1, TimeSpan.FromMinutes(1));
            IOwinContext context = new OwinContext();
            
            var result = rle.IsAllowed(context);
            Assert.IsTrue(result);
            
            result = rle.IsAllowed(context);
            Assert.IsTrue(result);
        }
        
        [TestMethod]
        public void Verify_XForwardedForIpInfoExist_WorkedCorrect()
        {
            var rle = new RateLimiter(1, TimeSpan.FromMinutes(1));
            IOwinContext context = new OwinContext();
            context.Request.Headers.Add("X-Forwarded-For", new[] { "127.0.0.1, 128.0.0.1" });
            
            var result = rle.IsAllowed(context);
            Assert.IsTrue(result);
            
            result = rle.IsAllowed(context);
            Assert.IsFalse(result);
        }
        
        [TestMethod]
        public void Verify_RemoteIpAddressInfoExist_WorkedCorrect()
        {
            var rle = new RateLimiter(1, TimeSpan.FromMinutes(1));
            IOwinContext context = new OwinContext();
            context.Request.RemoteIpAddress = "127.0.0.1";
            
            var result = rle.IsAllowed(context);
            Assert.IsTrue(result);
            
            result = rle.IsAllowed(context);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Verify_REMOTE_ADDRInfoExist_WorkedCorrect()
        {
            var rle = new RateLimiter(1, TimeSpan.FromMinutes(1));
            IOwinContext context = new OwinContext();
            context.Request.Headers.Add("REMOTE_ADDR", new[] { "127.0.0.1" });
            
            var result = rle.IsAllowed(context);
            Assert.IsTrue(result);
            
            result = rle.IsAllowed(context);
            Assert.IsFalse(result);
        }

        private IOwinContext CreateTestContext(string remoteIpAddress = "127.0.0.1")
        {
            IOwinContext context = new OwinContext();
            context.Request.RemoteIpAddress = remoteIpAddress;
            return context;
        }

        [TestMethod]
        public void Verify_TwoDifferentRequests_TwoRequestPerMinuteAllowed_WorkedCorrectForEachRequest()
        {
            var rle = new RateLimiter(2, TimeSpan.FromMinutes(1));
            IOwinContext context1 = CreateTestContext();
            IOwinContext context2 = CreateTestContext("128.0.0.1");
            
            var result = rle.IsAllowed(context1);
            Assert.IsTrue(result);
            
            result = rle.IsAllowed(context2);
            Assert.IsTrue(result);
            
            result = rle.IsAllowed(context1);
            Assert.IsTrue(result);
            
            result = rle.IsAllowed(context2);
            Assert.IsTrue(result);
            
            result = rle.IsAllowed(context1);
            Assert.IsFalse(result);
            
            result = rle.IsAllowed(context2);
            Assert.IsFalse(result);
        }
        
        [TestMethod]
        public void Verify_TwoRequestSecondsAllowed_CheckThatOldRequestsAreRemoved()
        {
            var rle = new RateLimiter(2, TimeSpan.FromSeconds(1));
            IOwinContext context1 = CreateTestContext();
            
            var result = rle.IsAllowed(context1);
            Assert.IsTrue(result);
            
            Thread.Sleep(500);
            
            result = rle.IsAllowed(context1);
            Assert.IsTrue(result);
            
            Thread.Sleep(200);
            
            result = rle.IsAllowed(context1);
            Assert.IsFalse(result);
            
            Thread.Sleep(500);
            
            result = rle.IsAllowed(context1);
            Assert.IsTrue(result);
            
            Thread.Sleep(100);
            
            result = rle.IsAllowed(context1);
            Assert.IsFalse(result);
            
            Thread.Sleep(1500);
            
            result = rle.IsAllowed(context1);
            Assert.IsTrue(result);
            
            result = rle.IsAllowed(context1);
            Assert.IsTrue(result);
            
            result = rle.IsAllowed(context1);
            Assert.IsFalse(result);
        }
    }
}