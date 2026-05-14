using System;
using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.Telephony.Fakes;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.DialerInitializerTest
{
    [TestClass]
    public class DialerInitializerTests : BaseMockedIntegrationTest
    {
        private IDialerInitializer _dialerInitializer;
        private ICompanyInfo _companyInfo;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();

            _dialerInitializer = ServiceLocator.Resolve<IDialerInitializer>();
            _companyInfo = ServiceLocator.Resolve<ICompanyInfo>();
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void ConfigureDdiNumbers_TwoDialersOneIsSoftDeleted_OneCurrectDdiNumberIsSentToMethodConfigureInboundDdiNumbers()
        {
            var inboundCallNumberOpenSurvey = Guid.NewGuid().ToString();
            var inboundCallNumberDeletedSurvey = Guid.NewGuid().ToString();

            var context = new TestData
            {
                SystemSettings = new Dictionary<string, object>
                {
                    {SystemSettingConstants.Toggle.EnableInbound, true}
                },
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        IsOpen = true,
                        DialMode = DialingMode.Predictive,
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData { Dialer = "D1", TelephoneNumber = inboundCallNumberOpenSurvey}
                        }
                    },
                    new SurveyData
                    {
                        Tag = "S2",
                        IsOpen = true,
                        DialMode = DialingMode.Predictive,
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData { Dialer = "D1", TelephoneNumber = inboundCallNumberDeletedSurvey}
                        }
                    }
                },
                Dialers = new[]
                {
                    new DialerData {Tag = "D1"}
                }
            }.Create();

            var survey = context.GetSurvey("S2");
            survey.Model.State = (int)SurveyState.SoftDeleted;

            var dialer = context.GetDialer("D1");
            var result = new InboundDdiNumber[0];
            var stubDialerApi = new StubIDialerAPI
            {
                ConfigureInboundDdiNumbersInt32ArrayOfInboundDdiNumber = (companyId, inboundDdiNumbers) =>
                {
                    result = inboundDdiNumbers;
                    return new DialerErrorCode[0];
                }
            };

            // act
            ((DialerInitializer)_dialerInitializer).ConfigureDdiNumbers(dialer.Id, _companyInfo.CompanyId, stubDialerApi);
            // assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(inboundCallNumberOpenSurvey, result[0].Number);
        }
    }
}
