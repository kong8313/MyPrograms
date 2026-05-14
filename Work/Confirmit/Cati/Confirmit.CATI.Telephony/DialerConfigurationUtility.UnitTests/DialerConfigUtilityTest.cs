using System;
using System.Collections.Generic;
using System.Xml;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework.Fakes;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated.Fakes;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Fakes;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Fakes;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Telephony;
using Confirmit.Test.Common.Attributes;
using DialerCommon.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DialerConfigurationUtility.UnitTests
{
    [TestClass]
    public class DialerConfigUtilityTest
    {
        private ServiceLocator _serviceLocator;

        [TestInitialize]
        public void TestInitialize()
        {
            _serviceLocator = new ServiceLocator();
            _serviceLocator.Cleanup();
            _serviceLocator.Initialize();
            new SystemSettingUnitTestRegistrator().RegisterTypes(_serviceLocator);
            ServiceLocator.Register<IDialerType, DialerType>();
            ServiceLocator.Register<ISqlTableUpdatedPublisher, StubISqlTableUpdatedPublisher>();
            ServiceLocator.Register<IDialerApiClient, StubIDialerApiClient>();
            new SystemSettingUnitTestRegistrator().RegisterTypes(_serviceLocator);
            var backendInstance = new BackendInstance();
            BackendInstance.Current = backendInstance;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _serviceLocator.Cleanup();
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Bug(59435)]
        public void NonGenericDialerTypeIsPointedInDialerConfig_UpdateDialerConfiguration_DialerTypeIsSetCorrectly()
        {
            var dialerType = "PROTS";
            var fakeConfig = GetFakeConfig(dialerType);
            var stubIDatabaseTransactionScope = new StubIDatabaseTransactionScope();

            ServiceLocator.Resolve<ISystemSettings>().Dialer.DialerType = "Initial value of checkStr";

            var dialersConfigurator = new DialersConfigurator(new StubIDialerAuthorizationKeyEncryptor());

            dialersConfigurator.UpdateDialerConfigurationParametersFromConfigurationFile(fakeConfig, stubIDatabaseTransactionScope, 1, new List<int> { 1 }, 1, false);

            Assert.AreEqual(dialerType, ServiceLocator.Resolve<ISystemSettings>().Dialer.DialerType);
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Bug(60583)]
        public void NonGenericDialerTypeIsPointedInDialerConfig_UpdateDialerConfiguration_EncryptAuthorizationKeyForOutgoingRequestsIsNotProcessed()
        {
            var fakeConfig = GetFakeConfig("PROTS");
            var fakeTransactionScope = new StubIDatabaseTransactionScope();
            var isEncryptAuthorizationKeyForOutgoingRequestsCalled = false;
            var stubIDialerAuthorizationKeyEncryptor = new StubIDialerAuthorizationKeyEncryptor
            {
                EncryptStringString = text =>
                {
                    isEncryptAuthorizationKeyForOutgoingRequestsCalled = true;
                    return "test";
                }
            };

            var dialersConfigurator = new DialersConfigurator(stubIDialerAuthorizationKeyEncryptor);

            dialersConfigurator.UpdateDialerConfigurationParametersFromConfigurationFile(fakeConfig, fakeTransactionScope, 1, new List<int> { 1 }, 1, false);

            Assert.IsFalse(isEncryptAuthorizationKeyForOutgoingRequestsCalled, "EncryptAuthorizationKeyForOutgoingRequestsCalled should not be called");
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Bug(60583)]
        public void GenericDialerTypeIsPointedInDialerConfig_UpdateDialerConfiguration_EncryptAuthorizationKeyForOutgoingRequestsIsProcessed()
        {
            var fakeConfig = GetFakeConfig("Generic");
            var fakeTransactionScope = new StubIDatabaseTransactionScope();
            var isEncryptAuthorizationKeyForOutgoingRequestsCalled = false;
            var stubIDialerAuthorizationKeyEncryptor = new StubIDialerAuthorizationKeyEncryptor
            {
                EncryptStringString = text =>
                {
                    isEncryptAuthorizationKeyForOutgoingRequestsCalled = true;
                    return "test";
                }
            };

            var dialersConfigurator = new DialersConfigurator(stubIDialerAuthorizationKeyEncryptor);

            dialersConfigurator.UpdateDialerConfigurationParametersFromConfigurationFile(fakeConfig, fakeTransactionScope, 1, new List<int> { 1 }, 1, false);

            Assert.IsTrue(isEncryptAuthorizationKeyForOutgoingRequestsCalled, "EncryptAuthorizationKeyForOutgoingRequestsCalled should be called");
        }

        private XmlDocument GetFakeConfig(string dialerType)
        {
            var fakeConfig = new XmlDocument();
            string dialerXmlSection;
            if (dialerType.Equals("PROTS"))
            {
                dialerXmlSection = ProtsDialerXmlSection;
            }
            else if (dialerType.Equals("Generic"))
            {
                dialerXmlSection = GenericDialerXmlSection;
            }
            else
            {
                dialerXmlSection = string.Empty;
            }

            fakeConfig.LoadXml(("<?xml version=\"1.0\" encoding=\"utf-8\"?><DialersConfiguration><DialerType>" +
                dialerType + "</DialerType>" +
                dialerXmlSection + " </DialersConfiguration>"));

            return fakeConfig;
        }

        private const string ProtsDialerXmlSection =
                            "  <PROTS>" +
                            "    <Dialer>" +
                            "      <Id>1</Id>" +
                            "      <Name>dialer 1 name</Name>" +
                            "      <DialerConnectionParameters>" +
                            "        <HostNameOrIp>protsdialer</HostNameOrIp>" +
                            "        <OutgoingTcpPort>1810</OutgoingTcpPort>" +
                            "        <IncomingTcpPort>1811</IncomingTcpPort>" +
                            "        <ServiceAddress>http://localhost/ProtsDialerService/ProtsDialerService.svc</ServiceAddress>" +
                            "        <ServiceEndpoint>PROTSDialerServiceEndpoint</ServiceEndpoint>" +
                            "        <OperationsTimeout>7000</OperationsTimeout>" +
                            "      </DialerConnectionParameters>" +
                            "      <DialerConfigurationParameters>" +
                            "        <RootDirectoryForAudioRecords>C:\\DSM</RootDirectoryForAudioRecords>" +
                            "      </DialerConfigurationParameters>" +
                            "    </Dialer>" +
                            "    <DialerSurveyParameters>" +
                            "      <DialerParameter>" +
                            "        <Id>AbandonmentRate</Id>" +
                            "        <Name>Nuisance call abandonment rate</Name>" +
                            "        <Value>0</Value>" +
                            "        <Type>System.Int32</Type>" +
                            "      </DialerParameter>" +
                            "      <DialerParameter>" +
                            "        <Id>MaxRings</Id>" +
                            "        <Name>No reply timeout (no. of rings)</Name>" +
                            "        <Value>5</Value>" +
                            "        <Type>System.Int32</Type>" +
                            "      </DialerParameter>" +
                            "      <DialerParameter>" +
                            "        <Id>AnsMachineDetect</Id>" +
                            "        <Name>Enable answer phone detection</Name>" +
                            "        <Value>False</Value>" +
                            "        <Type>System.Boolean</Type>" +
                            "      </DialerParameter>" +
                            "      <DialerParameter>" +
                            "        <Id>BillingCode</Id>" +
                            "        <Name>Billing Code</Name>" +
                            "       <Value>0</Value>" +
                            "        <Type>System.Int32</Type>" +
                            "      </DialerParameter>" +
                            "    </DialerSurveyParameters>" +
                            "  </PROTS>";

        private const string GenericDialerXmlSection =
                            "  <Generic>" +
                            "    <Dialer>" +
                            "      <Id>1</Id>" +
                            "      <Name>dialer 1 name</Name>" +
                            "      <DialerConnectionParameters>" +
                            "        <ServiceAddress>http://localhost/DialerService/DialerService.svc</ServiceAddress>" +
                            "        <ServiceEndpoint>DialerServiceEndpoint</ServiceEndpoint>" +
                            "        <AuthorizationKeyForOutgoingRequests>{0275E046-7FFF-495B-ACFE-09B439DB4902}</AuthorizationKeyForOutgoingRequests>" +
                            "      </DialerConnectionParameters>" +
                            "      <DialerConfigurationParameters>" +
                            "        <SupportedPersonModes>Manual,CampaignAssignment</SupportedPersonModes>" +
                            "        <IsReloginNeededOnCampaignChange>True</IsReloginNeededOnCampaignChange>" +
                            "        <IsHangUpSupported>True</IsHangUpSupported>" +
                            "        <IsPauseOrResumePlaybackSupported>False</IsPauseOrResumePlaybackSupported>" +
                            "        <IsToggleAgentListensToPlaybackOrRespondentSupported>False</IsToggleAgentListensToPlaybackOrRespondentSupported>" +
                            "        <IsDynamicExtensionNumberAllowedForLocalAgents>False</IsDynamicExtensionNumberAllowedForLocalAgents>" +
                            "        <IsDynamicExtensionNumberAllowedForRemoteAgents>False</IsDynamicExtensionNumberAllowedForRemoteAgents>" +
                            "      </DialerConfigurationParameters>" +
                            "    </Dialer>" +
                            "    <DialerSurveyParameters>" +
                            "      <DialerParameter>" +
                            "        <Id>AbandonmentRate</Id>" +
                            "        <Name>Nuisance call abandonment rate</Name>" +
                            "        <Value>0</Value>" +
                            "       <Type>System.Int32</Type>" +
                            "      </DialerParameter>" +
                            "      <DialerParameter>" +
                            "        <Id>MaxRings</Id>" +
                            "        <Name>No reply timeout (no. of rings)</Name>" +
                            "        <Value>5</Value>" +
                            "        <Type>System.Int32</Type>" +
                            "      </DialerParameter>" +
                            "      <DialerParameter>" +
                            "        <Id>AnsMachineDetect</Id>" +
                            "        <Name>Enable answer phone detection</Name>" +
                            "        <Value>False</Value>" +
                            "        <Type>System.Boolean</Type>" +
                            "      </DialerParameter>" +
                            "      <DialerParameter>" +
                            "        <Id>BillingCode</Id>" +
                            "        <Name>Billing Code</Name>" +
                            "        <Value>0</Value>" +
                            "        <Type>System.Int32</Type>" +
                            "      </DialerParameter>" +
                            "    </DialerSurveyParameters>" +
                            "  </Generic>";
    }
}
