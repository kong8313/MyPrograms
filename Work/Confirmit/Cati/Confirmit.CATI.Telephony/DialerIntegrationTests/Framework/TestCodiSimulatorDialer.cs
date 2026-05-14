using Confirmit.CATI.Telephony;
using Confirmit.CATI.Telephony.DialerCommon;
using Confirmit.CATI.Telephony.DialerLibrary;
using Confirmit.CATI.Telephony.DialerService;
using ConfirmitDialerInterface;
using DialerCommon;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DialerIntegrationTests.Framework
{
    public class TestCodiSimulatorDialer : TestBaseCodiDialer, ITestDialer
    {
        public TestCodiSimulatorDialer()
            : base("CodiSimulator")
        {
        }

        public void Init()
        {
            Log.Info("TestCodiSimulatorDialer.Init", "Initialization ...");

            var dialerService = new DialerService("Confirmit.CATI.Telephony.SimulatorDialerDriver",
                                   "SimulatorDialerDriver",
                                   "SimulatorDialerDriver",
                                   new NotificationsSenderInitializer(new Logger("SimulatorDialerDriver", new RequestId())));

            StartServices(dialerService);

            _dialerLibrary = new DialerLibrary();

            var actualResultCode = _dialerLibrary.Initialize(
                1, 
                CompanyId,
                ConnectionParametersXml,
                ConfigurationParameters(),
                CampaignParameters()).DialerErrorCode;

            Assert.AreEqual(DialerErrorCode.Success, actualResultCode, "DialerLibrary initialization failed.");
        }

        protected override string ConfigurationParameters()
        {
            var configurationParameters = 
                @"<DialerConfigurationParameters>
                    <SupportedPersonModes>Manual,CampaignAssignment</SupportedPersonModes>
                    <IsReloginNeededOnCampaignChange>True</IsReloginNeededOnCampaignChange>
                    <IsHangUpSupported>True</IsHangUpSupported>
                    <IsPauseOrResumePlaybackSupported>False</IsPauseOrResumePlaybackSupported>
                    <IsToggleAgentListensToPlaybackOrRespondentSupported>False</IsToggleAgentListensToPlaybackOrRespondentSupported>
                    <IsDynamicExtensionNumberAllowedForLocalAgents>False</IsDynamicExtensionNumberAllowedForLocalAgents>
                    <IsDynamicExtensionNumberAllowedForRemoteAgents>False</IsDynamicExtensionNumberAllowedForRemoteAgents>
                </DialerConfigurationParameters>";

            return configurationParameters;
        }

        protected override string CampaignParameters()
        {
            return @"<DialerSurveyParameters>
                     <DialerParameter>
                     <Id>AbandonmentRate</Id>
                     <Name>Nuisance call abandonment rate</Name>
                     <Value>0</Value>
                     <Type>System.Int32</Type>
                     </DialerParameter>
                     <DialerParameter>
                     <Id>MaxRings</Id>
                     <Name>No reply timeout (sec)</Name>
                     <Value>5</Value>
                     <Type>System.Int32</Type>
                     </DialerParameter>
                     <DialerParameter>
                     <Id>AnsMachineDetect</Id>
                     <Name>Enable answer phone detection</Name>
                     <Value>False</Value>
                     <Type>System.Boolean</Type>
                     </DialerParameter>
                     <DialerParameter>
                     <Id>BillingCode</Id>
                     <Name>Billing Code</Name>
                     <Value>0</Value>
                     <Type>System.Int32</Type>
                     </DialerParameter>
                     </DialerSurveyParameters>";
        }

        public override int WaitRequestCallsNotification()
        {
            TraceInformation("TestProTsDialerConfirmitDialerInterface.WaitRequestCallsNotification", "Empty method");

            return ExpectedNumberOfSamples;
        }

        public override void StopSimulator()
        {
            // There is nothing to do here
        }
    }
}