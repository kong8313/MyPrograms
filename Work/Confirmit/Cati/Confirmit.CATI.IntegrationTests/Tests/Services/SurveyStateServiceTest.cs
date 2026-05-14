using System;
using System.Collections.Generic;
using System.Reflection;
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
    public class SurveyStateServiceTest : BaseMockedIntegrationTest
    {
        [TestMethod, Owner(@"FIRM\alm")]
        public void ThereIsNoDialer_ShutdownSurveyWithNonManualDialingMode_NoExceptionIsThrown()
        {
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        IsOpen = true, DialMode = DialingMode.Predictive, Tag="S1"
                    }
                }
            }.Create();

            var surveyId = context.GetSurvey("S1").Id;

            var target = ServiceLocator.Resolve<ISurveyStateService>();
            target.ShutdownSurvey(surveyId);
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void ShutdownSurveyWithNonManualDialingMode_KillCampaignException_NoExceptionIsThrownToUpperLevel()
        {
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        IsOpen = true, DialMode = DialingMode.Predictive, Tag="S1"
                    }
                }
            }.Create();

            ServiceLocator.RegisterInstance<ITelephony>(new StubITelephony
            {
                KillCampaignInt64DialingMode = (id, mode) =>
                {
                    throw new Exception("Tests exception: " + MethodBase.GetCurrentMethod().Name);
                }
            });

            ServiceLocator.RegisterInstance<IMnTciTools>(new StubIMnTciTools
            {
                DoesCompanyUseTelephony = () => true
            });

            var surveyId = context.GetSurvey("S1").Id;

            var target = ServiceLocator.Resolve<ISurveyStateService>();
            target.ShutdownSurvey(surveyId);
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void CloseSurveyWithNonManualDialingMode_CompanyDoesNotUseTelephony_StopCampaignIsNotCalled()
        {
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        IsOpen = true, DialMode = DialingMode.Predictive, Tag="S1"
                    }
                }
            }.Create();

            var stopCampaignIsCalled = false;

            ServiceLocator.RegisterInstance<ITelephony>(new StubITelephony
            {
                StopCampaignInt64DialingMode = (id, mode) => { stopCampaignIsCalled = true; }
            });

            ServiceLocator.RegisterInstance<IMnTciTools>(new StubIMnTciTools
            {
                DoesCompanyUseTelephony = () => false
            });

            var surveyId = context.GetSurvey("S1").Id;

            var target = ServiceLocator.Resolve<ISurveyStateService>();
            target.CloseSurvey(surveyId);

            Assert.IsFalse(stopCampaignIsCalled, "StopCampaign is called but that is not expected.");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void CloseSurveyWithNonManualDialingMode_StopCampaignException_NoExceptionIsThrownToUpperLevel()
        {
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        IsOpen = true, DialMode = DialingMode.Predictive, Tag="S1"
                    }
                }
            }.Create();

            ServiceLocator.RegisterInstance<ITelephony>(new StubITelephony
            {
                StopCampaignInt64DialingMode = (id, mode) =>
                {
                    throw new Exception("Tests exception: " + MethodBase.GetCurrentMethod().Name);
                }
            });

            ServiceLocator.RegisterInstance<IMnTciTools>(new StubIMnTciTools
            {
                DoesCompanyUseTelephony = () => true
            });

            var surveyId = context.GetSurvey("S1").Id;

            var target = ServiceLocator.Resolve<ISurveyStateService>();
            target.CloseSurvey(surveyId);
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void OpenSurveyWithNonManualDialingMode_CompanyDoesNotUseTelephony_StartCampaignIsNotCalled()
        {
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        IsOpen = false, DialMode = DialingMode.Predictive, Tag="S1"
                    }
                }
            }.Create();

            var startCampaignIsCalled = false;

            ServiceLocator.RegisterInstance<ITelephony>(new StubITelephony
            {
                StartCampaignInt64StringDialingModeStringString = (id, name, mode, type, xml) =>
                {
                    startCampaignIsCalled = true;
                    return new List<DialerStartCampaignResult>();
                }
            });

            ServiceLocator.RegisterInstance<IMnTciTools>(new StubIMnTciTools
            {
                DoesCompanyUseTelephony = () => false
            });

            var surveyId = context.GetSurvey("S1").Id;

            var target = ServiceLocator.Resolve<ISurveyStateService>();
            target.Open(surveyId);

            Assert.IsFalse(startCampaignIsCalled, "StartCampaign is called but that is not expected.");
        }

    }
}