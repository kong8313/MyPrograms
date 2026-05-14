using System;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Fakes;
using Confirmit.CATI.Core.Services.SchedulingScriptNotificationServiceImplementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.Scheduling.SchedulingScriptNotificatorTest
{
    [TestClass]
    public class SchedulingScriptNotificatorExceptionDescriptionTest : BaseTest
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
            RegistryStub<ITimezoneService, StubITimezoneService>().GetDefaultCallCenterTimezoneId = () => 1;
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void SchedulingScriptNotificatorExceptionDescription_NewInstance_InstanceContainsCorrectParameters()
        {
           

            var exception = new Exception("Test exception");
            int interviewId = 123;

            var exceptionDescription = new SchedulingScriptNotificatorExceptionDescription(interviewId, exception);

            Assert.AreEqual(interviewId, exceptionDescription.InterviewId, "Wrong RespId in SchedulingScriptNotificatorExceptionDescription");
            Assert.AreEqual(exception.Message, exceptionDescription.Message, "Wrong InnerException in SchedulingScriptNotificatorExceptionDescription");
        }

        [TestMethod, Owner(@"FIRM\grigoryk"), ExpectedException(typeof(ArgumentNullException))]
        public void SchedulingScriptNotificatorExceptionDescription_NewInstanceWithNullException_ExceptionOccured()
        {
            var exceptionDescription = new SchedulingScriptNotificatorExceptionDescription(0, null);
        }
    }
}