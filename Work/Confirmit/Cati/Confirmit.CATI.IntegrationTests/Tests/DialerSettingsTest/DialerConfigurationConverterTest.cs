using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Supervisor.Classes.DialerConfiguration;
using DialerCommon;
using DialerCommon.DialerParameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Confirmit.CATI.IntegrationTests.Tests.DialerSettingsTest
{
    [TestClass]
    public class DialerConfigurationConverterTest : BaseMockedIntegrationTest
    {
        private string _dialerConnectionParams = @"<?xml version=""1.0"" encoding=""UTF-8""?><DialerConnectionParameters><ServiceAddress>http://co-osl-devhv31.firmglobal.com/LTUSimulator(G)DialerService/DialerService.svc</ServiceAddress><!-- http --><ServiceEndpoint>DialerServiceEndpointHttp</ServiceEndpoint><!-- DialerServiceEndpointHttp --><AuthorizationKeyForOutgoingRequests>SWJ5//4DnIBk9PCSOTmmxY8u8zcXEgMZhPNjmh2NJSr/FYNnw2BmPy2hDF+YyYsjGAQYMKtGajgqevIHD8R3H3B+A1ALNLGSRlMeUFON9hrvGuxmOUSvfRTVnoo=</AuthorizationKeyForOutgoingRequests></DialerConnectionParameters>";
        private string _dialerConfigurationParams = @"<?xml version=""1.0"" encoding=""UTF-8""?><DialerConfigurationParameters><SupportedPersonModes>Manual,CampaignAssignment</SupportedPersonModes><IsReloginNeededOnCampaignChange>True</IsReloginNeededOnCampaignChange><IsHangUpSupported>True</IsHangUpSupported><IsPauseOrResumePlaybackSupported>False</IsPauseOrResumePlaybackSupported><IsToggleAgentListensToPlaybackOrRespondentSupported>False</IsToggleAgentListensToPlaybackOrRespondentSupported><IsDynamicExtensionNumberAllowedForLocalAgents>False</IsDynamicExtensionNumberAllowedForLocalAgents><IsDynamicExtensionNumberAllowedForRemoteAgents>False</IsDynamicExtensionNumberAllowedForRemoteAgents></DialerConfigurationParameters>";
        private DialerConfigurationConverter _converter;
        private DialerSettingTemplate _expectedTemplate;
        private string _authKeyForOutgoingRequests = "0275E046-7FFF-495B-ACFE-09B439DB4902";
        private IDialerAuthorizationKeyEncryptor _encryptor;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();

            _encryptor = ServiceLocator.Resolve<IDialerAuthorizationKeyEncryptor>();
            _converter = new DialerConfigurationConverter(ServiceLocator.Resolve<IDialerSettings>(), _encryptor);

            _expectedTemplate = new DialerSettingTemplate();
            _expectedTemplate.DialerConnectionParameters.Add(new DialerParameter { Id = "ServiceAddress", Value = "http://co-osl-devhv31.firmglobal.com/LTUSimulator(G)DialerService/DialerService.svc" });
            _expectedTemplate.DialerConnectionParameters.Add(new DialerParameter { Id = "ServiceEndpoint", Value = "DialerServiceEndpointHttp" });
            _expectedTemplate.DialerConnectionParameters.Add(new DialerParameter { Id = "AuthorizationKeyForOutgoingRequests", Value = _authKeyForOutgoingRequests });
            _expectedTemplate.DialerConfigurationParameters.Add(new DialerParameter { Id = "SupportedPersonModes", Value = "Manual,CampaignAssignment" });
            _expectedTemplate.DialerConfigurationParameters.Add(new DialerParameter { Id = "IsReloginNeededOnCampaignChange", Value = "True" });
            _expectedTemplate.DialerConfigurationParameters.Add(new DialerParameter { Id = "IsHangUpSupported", Value = "True" });
            _expectedTemplate.DialerConfigurationParameters.Add(new DialerParameter { Id = "IsPauseOrResumePlaybackSupported", Value = "False" });
            _expectedTemplate.DialerConfigurationParameters.Add(new DialerParameter { Id = "IsToggleAgentListensToPlaybackOrRespondentSupported", Value = "False" });
            _expectedTemplate.DialerConfigurationParameters.Add(new DialerParameter { Id = "IsDynamicExtensionNumberAllowedForLocalAgents", Value = "False" });
            _expectedTemplate.DialerConfigurationParameters.Add(new DialerParameter { Id = "IsDynamicExtensionNumberAllowedForRemoteAgents", Value = "False" });
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void SucceededToDetectDialerTemplate_MergeByType_Success()
        {
            var convertedFromXml = _converter.FromXmlToDialerSettingTemplate(_dialerConfigurationParams, _dialerConnectionParams);
            _converter.TryGetDialerType(convertedFromXml, out var type, out DialerConfigurationType? configurationType);

            Assert.IsNotNull(type);
            Assert.AreEqual(DiallerType.Generic, type.Value);
            Assert.AreEqual(DialerConfigurationType.Sytel, configurationType);

            var key = convertedFromXml.DialerConnectionParameters.FirstOrDefault(x => x.Id == "AuthorizationKeyForOutgoingRequests");
            key.Value = _encryptor.DecryptString(key.Value);

            _converter.MergeWithTemplate(convertedFromXml, type.Value);

            AssertIdAndValue(_expectedTemplate.DialerConnectionParameters, convertedFromXml.DialerConnectionParameters);
            AssertIdAndValue(_expectedTemplate.DialerConfigurationParameters, convertedFromXml.DialerConfigurationParameters);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void SucceededToDetectDialerTemplate_MergeByConfigurationType_Success()
        {
            var convertedFromXml = _converter.FromXmlToDialerSettingTemplate(_dialerConfigurationParams, _dialerConnectionParams);
            _converter.TryGetDialerType(convertedFromXml, out var type, out DialerConfigurationType? configurationType);

            Assert.IsNotNull(type);
            Assert.AreEqual(DiallerType.Generic, type.Value);
            Assert.IsNotNull(configurationType);
            Assert.AreEqual(DialerConfigurationType.Sytel, configurationType);

            var key = convertedFromXml.DialerConnectionParameters.FirstOrDefault(x => x.Id == "AuthorizationKeyForOutgoingRequests");
            key.Value = _encryptor.DecryptString(key.Value);

            _converter.MergeWithTemplate(convertedFromXml, configurationType.Value);

            AssertIdAndValue(_expectedTemplate.DialerConnectionParameters, convertedFromXml.DialerConnectionParameters);
            AssertIdAndValue(_expectedTemplate.DialerConfigurationParameters, convertedFromXml.DialerConfigurationParameters);
        }
        
        [TestMethod, Owner(@"FIRM\DenisM")]
        public void ConvertToXml_XmlToTemplateEntity_WithEncryption_Success()
        {

            var configParamsXml = _converter.GetDialerConfigurationParametersXml(_expectedTemplate);
            var connectionParamsXml = _converter.GetDialerConnectionParametersXml(_expectedTemplate, true);

            var convertedFromXml = _converter.FromXmlToDialerSettingTemplate(configParamsXml, connectionParamsXml);
            var key = convertedFromXml.DialerConnectionParameters.FirstOrDefault(x => x.Id == "AuthorizationKeyForOutgoingRequests");
            key.Value = _encryptor.DecryptString(key.Value);

            AssertIdAndValue(_expectedTemplate.DialerConnectionParameters, convertedFromXml.DialerConnectionParameters);
            AssertIdAndValue(_expectedTemplate.DialerConfigurationParameters, convertedFromXml.DialerConfigurationParameters);
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void ConvertToXml_XmlToTemplateEntity_WithoutEncryption_Success()
        {
            var configParamsXml = _converter.GetDialerConfigurationParametersXml(_expectedTemplate);
            var connectionParamsXml = _converter.GetDialerConnectionParametersXml(_expectedTemplate);

            var convertedFromXml = _converter.FromXmlToDialerSettingTemplate(configParamsXml, connectionParamsXml);
            
            AssertIdAndValue(_expectedTemplate.DialerConnectionParameters, convertedFromXml.DialerConnectionParameters);
            AssertIdAndValue(_expectedTemplate.DialerConfigurationParameters, convertedFromXml.DialerConfigurationParameters);
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void FailedToDetectDialerTemplate_MergeWithGeneric_Success()
        {
            var connectionParams = @"<?xml version=""1.0"" ?><DialerConnectionParameters><UnknownConnectionParam>1</UnknownConnectionParam><ServiceAddress>http://co-osl-devhv31.firmglobal.com/LTUSimulator(G)DialerService/DialerService.svc</ServiceAddress><!-- http --><ServiceEndpoint>DialerServiceEndpointHttp</ServiceEndpoint><!-- DialerServiceEndpointHttp --></DialerConnectionParameters>";
            var configParams = @"<?xml version=""1.0"" ?><DialerConfigurationParameters><UnknownConfigParam>2</UnknownConfigParam><SupportedPersonModes>Manual,CampaignAssignment</SupportedPersonModes><IsReloginNeededOnCampaignChange>True</IsReloginNeededOnCampaignChange><IsHangUpSupported>True</IsHangUpSupported><IsPauseOrResumePlaybackSupported>False</IsPauseOrResumePlaybackSupported><IsToggleAgentListensToPlaybackOrRespondentSupported>False</IsToggleAgentListensToPlaybackOrRespondentSupported><IsDynamicExtensionNumberAllowedForLocalAgents>False</IsDynamicExtensionNumberAllowedForLocalAgents></DialerConfigurationParameters>";

            var convertedFromXml = _converter.FromXmlToDialerSettingTemplate(configParams, connectionParams);
            _converter.TryGetDialerType(convertedFromXml, out var type, out DialerConfigurationType? configurationType);

            Assert.IsNull(type);
            Assert.IsNull(configurationType);

            _converter.MergeWithTemplate(convertedFromXml, DiallerType.Generic);

            _expectedTemplate.DialerConnectionParameters.Insert(0, new DialerParameter { Id = "UnknownConnectionParam", Value = "1" });
            _expectedTemplate.DialerConfigurationParameters.Insert(0, new DialerParameter { Id = "UnknownConfigParam", Value = "2" });
            _expectedTemplate.DialerConnectionParameters.Remove(
                _expectedTemplate.DialerConnectionParameters.FirstOrDefault(x =>
                    x.Id == "AuthorizationKeyForOutgoingRequests"));
            _expectedTemplate.DialerConnectionParameters.Insert(3, new DialerParameter
            { Id = "AuthorizationKeyForOutgoingRequests", Value = _authKeyForOutgoingRequests });

            AssertIdAndValue(_expectedTemplate.DialerConnectionParameters, convertedFromXml.DialerConnectionParameters);
            AssertIdAndValue(_expectedTemplate.DialerConfigurationParameters, convertedFromXml.DialerConfigurationParameters);
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void SystemSettings_OpenDialerConfig_ContainsExpectedProperties()
        {
            var settings = ServiceLocator.Resolve<ISystemSettings>().Dialer.SettingsTemplatesJson;
            var list = JsonConvert.DeserializeObject<DialerConfigurationList>(settings);
            var openDialerTemplate =
                list.DialerSettingTemplates.FirstOrDefault(x => x.DialerType == DiallerType.Generic);


            var expectedConnectionParams = new[]
            {
                "ServiceAddress","ServiceEndpoint","AuthorizationKeyForOutgoingRequests"
            };
            var expectedConfigParams = new[]
            {
                "SupportedPersonModes",
                "IsReloginNeededOnCampaignChange",
                "IsHangUpSupported",
                "IsPauseOrResumePlaybackSupported",
                "IsToggleAgentListensToPlaybackOrRespondentSupported",
                "IsDynamicExtensionNumberAllowedForLocalAgents",
                "IsDynamicExtensionNumberAllowedForRemoteAgents",
            };
            var expectedSurveyParams = new[]
            {
                "AbandonRate",
                "RNAtimeout",
                "AnsMachineDetect",
                "CallProgressToneDetection",
                "AbandonMessageName",
                "CTIName",
                "CLI",
                "AnsMachineAudioMessageUrl"
            };

            CollectionAssert.AreEqual(expectedSurveyParams, openDialerTemplate.DialerSurveyParameters.Select(x => x.Id).ToArray());
            CollectionAssert.AreEqual(expectedConnectionParams, openDialerTemplate.DialerConnectionParameters.Select(x => x.Id).ToArray());
            CollectionAssert.AreEqual(expectedConfigParams, openDialerTemplate.DialerConfigurationParameters.Select(x => x.Id).ToArray());
        }

        private static void AssertIdAndValue<T>
                   (IEnumerable<T> expectedParams,
                       IEnumerable<T> actualParams) where T : DialerParameter
        {
            TestAssert.AreEqual(
                expectedParams,
                actualParams,
                (x, y) => x.Id == y.Id && x.Value == y.Value);
        }
    }
}
