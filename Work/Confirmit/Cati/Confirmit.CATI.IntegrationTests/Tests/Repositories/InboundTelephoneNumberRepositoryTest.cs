using System;
using System.Collections.Generic;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.Repositories
{
    [TestClass]
    public class InboundTelephoneNumberRepositoryTest : BaseMockedIntegrationTest
    {
        [TestMethod, Owner(@"firm\grigoryk")]
        public void GetValidByDialerId_DdiNumberWithoutSurveyId_BackendInstanceCurrentIsCacheEnabledFalse_GetDdiNumbersWithSurveyId()
        {
            // arrange
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

            var survey1 = context.GetSurvey("S1");
            var survey2 = context.GetSurvey("S2");
            BackendTools.DeleteSurvey(survey2.Model.ProjectId);

            BackendInstance.Current.IsCacheEnabled = false;
            var dialer = context.GetDialer("D1");

            var inboundTelephoneNumberRepository = ServiceLocator.Resolve<IInboundTelephoneNumberRepository>();
            var ddiNumbers = inboundTelephoneNumberRepository.GetValidByDialerId(dialer.Id);

            // assert            
            Assert.AreEqual(1, ddiNumbers.Count);
            Assert.AreEqual(survey1.Model.SID, ddiNumbers[0].SurveyId);
        }
    }
}