using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Misc.Fakes;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Core.UnitTests.ServiceLocation;
using Confirmit.Test.Common.Attributes;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.Telephony
{
    [TestClass]
    public class InterviewRecordingManagerTest
    {
        [TestInitialize]
        public void TestInitialiaze()
        {
            UnitTestsServiceLocatorInitializer.InitializeServiceLocator();

            ServiceLocator.RegisterSingleton<IDbLibProvider>(new StubIDbLibProvider()
            {
                CatiDefaultConnectionStringGet = () => "Data Source=UnitTestSQL;Initial Catalog=UnitTestDb;"
            });
        }

        [TestCleanup]
        public void TestCleanup()
        {
            UnitTestsServiceLocatorInitializer.CleanupServiceLocator();
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Cr(72183)]
        public void ParseStopRecordingMode_DifferentCases_ParsingRulesAreInForce()
        {
            var interviewRecordingManager = ServiceLocator.Resolve<IInterviewRecordingManager>();
            StopRecordingMode typedStopRecordingMode;

            Assert.IsTrue(interviewRecordingManager.ParseStopRecordingMode("WholeInterview", out typedStopRecordingMode));
            Assert.AreEqual(StopRecordingMode.WholeInterview, typedStopRecordingMode, "Incorrect StopRecordingMode parsing result");
            Assert.IsTrue(interviewRecordingManager.ParseStopRecordingMode("1", out typedStopRecordingMode));
            Assert.AreEqual(StopRecordingMode.WholeInterview, typedStopRecordingMode, "Incorrect StopRecordingMode parsing result");

            Assert.IsTrue(interviewRecordingManager.ParseStopRecordingMode("Sectional", out typedStopRecordingMode));
            Assert.AreEqual(StopRecordingMode.Sectional, typedStopRecordingMode, "Incorrect StopRecordingMode parsing result");
            Assert.IsTrue(interviewRecordingManager.ParseStopRecordingMode("2", out typedStopRecordingMode));
            Assert.AreEqual(StopRecordingMode.Sectional, typedStopRecordingMode, "Incorrect StopRecordingMode parsing result");

            Assert.IsTrue(interviewRecordingManager.ParseStopRecordingMode("Both", out typedStopRecordingMode));
            Assert.AreEqual(StopRecordingMode.Both, typedStopRecordingMode, "Incorrect StopRecordingMode parsing result");
            Assert.IsTrue(interviewRecordingManager.ParseStopRecordingMode("3", out typedStopRecordingMode));
            Assert.AreEqual(StopRecordingMode.Both, typedStopRecordingMode, "Incorrect StopRecordingMode parsing result");

            // Test that Enum parsing is case insensitive
            Assert.IsTrue(interviewRecordingManager.ParseStopRecordingMode("wHoLeiNtervieW", out typedStopRecordingMode));
            Assert.AreEqual(StopRecordingMode.WholeInterview, typedStopRecordingMode, "Incorrect StopRecordingMode parsing result");

            // Out of range int must be converted to default value. Default value is StopRecordingMode.Both
            Assert.IsFalse(interviewRecordingManager.ParseStopRecordingMode("17", out typedStopRecordingMode));
            Assert.AreEqual(StopRecordingMode.Both, typedStopRecordingMode, "Incorrect StopRecordingMode parsing result");

            // Null must be converted to default value. Default value is StopRecordingMode.Both
            Assert.IsFalse(interviewRecordingManager.ParseStopRecordingMode(null, out typedStopRecordingMode));
            Assert.AreEqual(StopRecordingMode.Both, typedStopRecordingMode, "Incorrect StopRecordingMode parsing result");

            // Empty string must be converted to default value. Default value is StopRecordingMode.Both
            Assert.IsFalse(interviewRecordingManager.ParseStopRecordingMode(string.Empty, out typedStopRecordingMode));
            Assert.AreEqual(StopRecordingMode.Both, typedStopRecordingMode, "Incorrect StopRecordingMode parsing result");

            // Any unknown char sequence  must be converted to default value. Default value is StopRecordingMode.Both
            Assert.IsFalse(interviewRecordingManager.ParseStopRecordingMode("!^j3 htm", out typedStopRecordingMode));
            Assert.AreEqual(StopRecordingMode.Both, typedStopRecordingMode, "Incorrect StopRecordingMode parsing result");
        }
    }
}
