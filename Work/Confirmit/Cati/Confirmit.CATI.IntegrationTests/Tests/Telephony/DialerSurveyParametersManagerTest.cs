using System.Collections.Generic;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Core.Telephony.Fakes;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using ConfirmitDialerInterface;
using DialerCommon.DialerParameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.Telephony
{
    [TestClass]
    public class DialerSurveyParametersManagerTest : BaseMockedIntegrationTest
    {
        [TestMethod, Owner(@"FIRM\alm")]
        public void OpenedSurvey_SetDialerSurveyParameters_SetCampaignParametersIsCalled()
        {
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        IsOpen = true,
                        DialMode = DialingMode.Predictive,
                        Tag = "S1"
                    }
                }
            }.Create();

            var surveyId = context.GetSurvey("S1").Id;

            var setCampaignParametersIsCalled = false;

            ServiceLocator.RegisterInstance<ITelephony>(new StubITelephony
            {
                SetCampaignParametersInt64DialingModeString = (id, mode, xml) =>
                {
                    setCampaignParametersIsCalled = true;
                }
            });

            var target = ServiceLocator.Resolve<IDialerSurveyParametersManager>();

            target.SetDialerSurveyParameters(surveyId, new List<DialerParameter>());

            Assert.IsTrue(setCampaignParametersIsCalled, "SetCampaignParameters was expected to be called but it is not called.");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void ClosedSurvey_SetDialerSurveyParameters_SetCampaignParametersIsCalled()
        {
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        IsOpen = false,
                        DialMode = DialingMode.Predictive,
                        Tag = "S1"
                    }
                }
            }.Create();

            var surveyId = context.GetSurvey("S1").Id;

            var setCampaignParametersIsCalled = false;

            ServiceLocator.RegisterInstance<ITelephony>(new StubITelephony
            {
                SetCampaignParametersInt64DialingModeString = (id, mode, xml) =>
                {
                    setCampaignParametersIsCalled = true;
                }
            });

            var target = ServiceLocator.Resolve<IDialerSurveyParametersManager>();

            target.SetDialerSurveyParameters(surveyId, new List<DialerParameter>());

            Assert.IsFalse(setCampaignParametersIsCalled, "SetCampaignParameters is called but that is not expected.");
        }
    }
}