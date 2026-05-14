using BvCallHandlerLibrary;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.CATI
{
    [TestClass]
    public class AudioMonitoringTest : BaseMockedIntegrationTest
    {
        [TestMethod, Owner(@"FIRM\MikhailT")]
        [ExpectedException(typeof(UserMessageException))]
        public void DialerIsUnavailabe_StopPlayback_StopPlaybackIsNotCalledOnDialer()
        {
            var test = new TestCati2(true, false, BackendToolsObject);
            const string user = "testUser1";
            const string password = "password";
            const string extensionNumber = "101010";

            test.CreateSurveyWithPerson(DialingMode.Preview, user, password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(1);

            test.Login(user, password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer(extensionNumber);

            test.StartInterview_ManualOrPreview(null, 0);

            var audioMonitoring = ServiceLocator.Resolve<IAudioMonitoring>();

            audioMonitoring.StartAudioMonitor("super1", test.PersonSID, "tel1");

            const string user2 = "testUser2";
            const string extensionNumber2 = "202020";
            test.CreatePerson(user2, password, AgentTaskChoiceMode.CampaignAssignment);
            test.Login(user2, password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer(extensionNumber2);

            try
            {
                // Note: test.PersonSID is now id of the second person.
                audioMonitoring.StartAudioMonitor("super2", test.PersonSID, "tel1");
            }
            catch (UserMessageException ex)
            {
                Assert.AreEqual("The monitoring resource 'tel1' is currently being used by another supervisor: 'super1'.", ex.Message);
                throw;
            }
        }
    }
}
