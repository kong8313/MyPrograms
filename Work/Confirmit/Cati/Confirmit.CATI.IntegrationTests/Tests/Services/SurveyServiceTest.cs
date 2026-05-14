using BvCallHandlerLibrary.Tools;
using BvCallHandlerLibrary.Tools.Fakes;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Core.Telephony.Fakes;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.Services
{
    [TestClass]
    public class SurveyServiceTest : BaseMockedIntegrationTest
    {
        [TestMethod, Owner(@"FIRM\alm")]
        public void SurveyIsOpenedAndTelephonyIsEnabled_OnLaunchSurvey_SetCampaignParametersIsCalled()
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

            ServiceLocator.RegisterInstance<IMnTciTools>(new StubIMnTciTools
            {
                DoesCompanyUseTelephony = () => true
            });

            var target = ServiceLocator.Resolve<ISurveyService>();

            target.OnLaunchSurvey(surveyId);

            Assert.IsTrue(setCampaignParametersIsCalled, "SetCampaignParameters was expected to be called but it is not called.");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void SurveyIsClosedAndTelephonyIsEnabled_OnLaunchSurvey_SetCampaignParametersIsNotCalled()
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

            ServiceLocator.RegisterInstance<IMnTciTools>(new StubIMnTciTools
            {
                DoesCompanyUseTelephony = () => true
            });

            var target = ServiceLocator.Resolve<ISurveyService>();

            target.OnLaunchSurvey(surveyId);

            Assert.IsFalse(setCampaignParametersIsCalled, "SetCampaignParameters is called but that is not expected.");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void SurveyIsOpenedAndTelephonyIsNotEnabled_OnLaunchSurvey_SetCampaignParametersIsNotCalled()
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

            ServiceLocator.RegisterInstance<IMnTciTools>(new StubIMnTciTools
            {
                DoesCompanyUseTelephony = () => false
            });

            var target = ServiceLocator.Resolve<ISurveyService>();

            target.OnLaunchSurvey(surveyId);

            Assert.IsFalse(setCampaignParametersIsCalled, "SetCampaignParameters is called but that is not expected.");
        }
    }
}