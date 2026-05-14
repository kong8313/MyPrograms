using System.Collections.Generic;
using BvCallHandlerLibrary;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Core.Telephony.Fakes;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Telephony;
using Confirmit.CATI.Telephony.Fakes;
using ConfirmitDialerInterface;

using DialerCommon.DialerParameters;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.MultipleDialers
{
    [TestClass]
    public class SurveyOpenCloseAndConfigureTest
    {
        private IDialerSurveyParametersManager _dialerSurveyParametersManager;

        private const DialingMode PreviewDialingMode = DialingMode.Preview;

        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;

        

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _backendTools = new BackendTools(_framework);
            _dialerSurveyParametersManager = ServiceLocator.Resolve<IDialerSurveyParametersManager>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void OpenSurvey_StartCampaignIsCalledOnce()
        {
            var stubIDialer = new StubIDialerAPI
            {
                StartCampaignStringArrayOfInt32Int64StringDialingModeStringBooleanString =
                    (id, ids, campaignId, name, mode, type, interview, xml) => (int)DialerErrorCode.Success
            };

            PrepareTest(stubIDialer, 0);
            var surveySid = _backendTools.CreateSurvey("p10000001");

            SurveyService.SetDialingMode(surveySid, PreviewDialingMode);

            var surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            surveyStateService.Open(surveySid);
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void OpenSurvey_OneDialerIsUnavailable_StartCampaignCalledOnAllDialersAndProperExceptionIsThrown()
        {
            var stubIDialer = new StubIDialerAPI
            {
                StartCampaignStringArrayOfInt32Int64StringDialingModeStringBooleanString =
                    (id, ids, campaignId, name, mode, type, interview, xml) => (int)DialerErrorCode.Success
            };

            PrepareTest(stubIDialer, 1);

            var surveySid = _backendTools.CreateSurvey("p10000001");

            SurveyService.SetDialingMode(surveySid, PreviewDialingMode);

            var surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            TestAssert.InvokeMethodAndVerifyExceptionThrown<UserMessageException>(
                () => surveyStateService.Open(surveySid),
                exception => Assert.AreEqual(
                    "Warning: Survey 'p10000001' unavailable on dialers:  [id: 3, name: name of 3]",
                    exception.Message));
        }

        [TestMethod, Owner(@"FIRM\EgorK")]
        public void OpenSurvey_OneDialerIsAvailable_StartCampaignCalledOnAllDialers_SurveyOpen()
        {
            var stubIDialer = new StubIDialerAPI
            {
                StartCampaignStringArrayOfInt32Int64StringDialingModeStringBooleanString =
                    (id, ids, campaignId, name, mode, type, interview, xml) => (int)DialerErrorCode.Success
            };

            PrepareTest(stubIDialer, 0);

            var surveySid = _backendTools.CreateSurvey("p10000001");

            SurveyService.SetDialingMode(surveySid, PreviewDialingMode);

            var surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            surveyStateService.Open(surveySid);

            var survey = SurveyRepository.GetById(surveySid);
            Assert.IsTrue((SurveyState)survey.State == SurveyState.Open, "It is expected that the survey is Open");
        }

        [TestMethod, Owner(@"FIRM\EgorK")]
        public void OpenSurvey_ZeroDialersAvailable_StartCampaignCalledOnAllDialers_SurveyOpen()
        {

            _framework.BackendInitialize(true, null, 0);

            var surveySid = _backendTools.CreateSurvey("p10000001");

            SurveyService.SetDialingMode(surveySid, PreviewDialingMode);

            var surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            surveyStateService.Open(surveySid);

            var survey = SurveyRepository.GetById(surveySid);
            Assert.IsTrue((SurveyState)survey.State == SurveyState.Open, "It is expected that the survey is Open");
        }

        [TestMethod, Owner(@"FIRM\EgorK")]
        public void OpenSurvey_ThoDifferentDialersOneIsUnavailable_StartCampaignCalledOnAllDialers_SurveyOpen()
        {
            var stubIDialer1 = new StubIDialerAPI
            {
                StartCampaignStringArrayOfInt32Int64StringDialingModeStringBooleanString =
                   (id, ids, campaignId, name, mode, type, interview, xml) => (int)DialerErrorCode.Success
            };
            var stubIDialer2 = new StubIDialerAPI
            {
                StartCampaignStringArrayOfInt32Int64StringDialingModeStringBooleanString =
                  (id, ids, campaignId, name, mode, type, interview, xml) => (int)DialerErrorCode.Exception
            };
            int dialersInitialized = 0;
            var stubIDialerInitializer = new StubIDialerInitializer
            {
                InitializeDialerInt32IDialerAPIBooleanInt32OutStringOutDialTypeOut =
                   (int id, IDialerAPI api, bool b, out int tenantId, out string name, out DialType dialType) =>
                   {
                       if (dialersInitialized >= 2)
                           throw new InternalErrorException("Initialize of Dialer is failed.");

                       dialersInitialized++;

                       tenantId = 0;
                       name = "name of " + dialersInitialized;
                       dialType = dialersInitialized == 1 ? DialType.Landline : DialType.Cellphone;

                       return dialersInitialized == 1 ? stubIDialer2 : stubIDialer1;

                   }
            };

            ServiceLocator.RegisterInstance<IDialerInitializer>(stubIDialerInitializer);

            _framework.BackendInitialize(true, null, 2);

            var surveySid = _backendTools.CreateSurvey("p10000001");

            SurveyService.SetDialingMode(surveySid, PreviewDialingMode);

            try
            {
                var surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
                surveyStateService.Open(surveySid);
                Assert.Fail("SurveyService.Open did not throw exception while one dialer is unavailable.");
            }
            catch (UserMessageException ex)
            {
                Assert.AreEqual("Warning: Survey 'p10000001' unavailable on dialers:  [id: 1, name: name of 1]", ex.Message);
            }

            var survey = SurveyRepository.GetById(surveySid);
            Assert.IsTrue((SurveyState)survey.State == SurveyState.Open, "It is expected that the survey is Open");
        }
        
        [TestMethod, Owner(@"FIRM\EgorK")]
        public void OpenSurvey_ThoDifferentDialersOneIsUnavailable_StartCampaignCalledOnAllDialersIndividually_SurveyOpen()
        {
            ServiceLocator.Resolve<ISystemSettings>().Dialer.OpenSurveysOnDialersIndividually = true;
            var stubIDialer1 = new StubIDialerAPI
            {
                StartCampaignStringArrayOfInt32Int64StringDialingModeStringBooleanString =
                   (id, ids, campaignId, name, mode, type, interview, xml) => (int)DialerErrorCode.Success
            };
            var stubIDialer2 = new StubIDialerAPI
            {
                StartCampaignStringArrayOfInt32Int64StringDialingModeStringBooleanString =
                  (id, ids, campaignId, name, mode, type, interview, xml) => (int)DialerErrorCode.Exception
            };
            int dialersInitialized = 0;
            var stubIDialerInitializer = new StubIDialerInitializer
            {
                InitializeDialerInt32IDialerAPIBooleanInt32OutStringOutDialTypeOut =
                   (int id, IDialerAPI api, bool b, out int tenantId, out string name, out DialType dialType) =>
                   {
                       if (dialersInitialized >= 2)
                           throw new InternalErrorException("Initialize of Dialer is failed.");

                       dialersInitialized++;

                       tenantId = 0;
                       name = "name of " + dialersInitialized;
                       dialType = DialType.Landline;

                       return dialersInitialized == 1 ? stubIDialer2 : stubIDialer1;
                   }
            };

            ServiceLocator.RegisterInstance<IDialerInitializer>(stubIDialerInitializer);

            _framework.BackendInitialize(true, null, 2);

            var surveySid = _backendTools.CreateSurvey("p10000001");

            SurveyService.SetDialingMode(surveySid, PreviewDialingMode);

            try
            {
                var surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
                surveyStateService.Open(surveySid);
                Assert.Fail("SurveyService.Open did not throw exception while one dialer is unavailable.");
            }
            catch (UserMessageException ex)
            {
                Assert.AreEqual("Warning: Survey 'p10000001' unavailable on dialers:  [id: 1, name: name of 1]", ex.Message);
            }

            var survey = SurveyRepository.GetById(surveySid);
            Assert.IsTrue((SurveyState)survey.State == SurveyState.Open, "It is expected that the survey is Open");
        }


        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void OpenSurvey_AllDialersAreUnavailable_SurveyDidNotOpenedSuccessfullyAndProperExceptionIsThrown()
        {
            var stubIDialer = new StubIDialerAPI
            {
                StartCampaignStringArrayOfInt32Int64StringDialingModeStringBooleanString =
                    (id, ids, campaignId, name, mode, type, interview, xml) => (int)DialerErrorCode.Exception
            };

            PrepareTest(stubIDialer, 3);

            foreach (var dialerInstance in ServiceLocator.Resolve<IDialerCollection>().GetDialers())
            {
                dialerInstance.IsDialerInitialized = false;
            }

            var surveySid = _backendTools.CreateSurvey("p10000001");

            SurveyService.SetDialingMode(surveySid, PreviewDialingMode);
            try
            {
                var surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
                surveyStateService.Open(surveySid);
                Assert.Fail("SurveyService.Open did not throw exception while one dialer is unavailable.");
            }
            catch (UserMessageException ex)
            {
                Assert.AreEqual("Warning: Survey 'p10000001' unavailable on dialers:  [id: 1, name: name of 1] [id: 2, name: name of 2] [id: 3, name: name of 3]", ex.Message);
            }

            var survey = SurveyRepository.GetById(surveySid);
            Assert.IsTrue((SurveyState)survey.State == SurveyState.Close, "It is expected that the survey is closed");
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void CloseSurvey_StopCampaignIsCalledOnce()
        {
            var stubIDialer = new StubIDialerAPI
            {
                StartCampaignStringArrayOfInt32Int64StringDialingModeStringBooleanString =
                    (id, ids, campaignId, name, mode, type, interview, xml) => (int)DialerErrorCode.Success,
                StopCampaignStringArrayOfInt32Int64DialingMode =
                    (id, ids, campaignId, mode) => (int)DialerErrorCode.Success
            };

            PrepareTest(stubIDialer, 0);

            var surveySid = _backendTools.CreateSurvey("p10000001");

            SurveyService.SetDialingMode(surveySid, PreviewDialingMode);

            var surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            surveyStateService.Open(surveySid);
            surveyStateService.CloseSurvey(surveySid);
        }
        
        [TestMethod, Owner(@"FIRM\EgorK")]
        public void OpenSurveysOnDialersIndividuallyEnabled_AllDialerOperationsCalledIndividuallyForAllDialers()
        {
            ServiceLocator.Resolve<ISystemSettings>().Dialer.OpenSurveysOnDialersIndividually = true;
            int campaignsStarted = 0, campaignsStopped = 0, parametersChanged = 0, campaignsKilled = 0;
            
            var stubIDialer = new StubIDialerAPI
            {
                StartCampaignStringArrayOfInt32Int64StringDialingModeStringBooleanString =
                    (id, ids, campaignId, name, mode, type, interview, xml) =>
                    {
                        campaignsStarted++;
                        return (int)DialerErrorCode.Success;
                    },
                StopCampaignStringArrayOfInt32Int64DialingMode =
                    (id, ids, campaignId, mode) =>
                    {
                        campaignsStopped++;
                        return (int)DialerErrorCode.Success;
                    },
                SetCampaignParametersStringArrayOfInt32Int64DialingModeBooleanString =
                    (id, ids, campaignId, mode, interview, xml) =>
                    {
                        parametersChanged++;
                        return (int)DialerErrorCode.Success;
                    },
                KillCampaignStringArrayOfInt32Int64DialingMode = (id, ids, campaignId, mode) =>
                {
                    campaignsKilled++;
                    return (int)DialerErrorCode.Success;
                }
            };

            PrepareTest(stubIDialer, 0);

            var surveySid = _backendTools.CreateSurvey("p10000001");

            SurveyService.SetDialingMode(surveySid, PreviewDialingMode);

            var surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            surveyStateService.Open(surveySid);
            _dialerSurveyParametersManager.SetDialerSurveyParameters(surveySid, new List<DialerParameter>());
            surveyStateService.CloseSurvey(surveySid);
            
            Assert.AreEqual(3, parametersChanged);
            Assert.AreEqual(3, campaignsStarted);
            Assert.AreEqual(3, campaignsStopped);
            
            surveyStateService.Open(surveySid);
            surveyStateService.ShutdownSurvey(surveySid);
            
            Assert.AreEqual(3, campaignsKilled);
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void SurveyIsOpened_TheSurveyDialerParametersAreChanged_SetCampaignParametersIsCalledOnAllDialers()
        {
            var stubIDialer = new StubIDialerAPI
            {
                StartCampaignStringArrayOfInt32Int64StringDialingModeStringBooleanString =
                    (id, ids, campaignId, name, mode, type, interview, xml) => (int)DialerErrorCode.Success,
                SetCampaignParametersStringArrayOfInt32Int64DialingModeBooleanString =
                    (id, ids, campaignId, mode, interview, xml) => (int)DialerErrorCode.Success
            };

            PrepareTest(stubIDialer, 0);
            var surveySid = _backendTools.CreateSurvey("p10000001");

            SurveyService.SetDialingMode(surveySid, PreviewDialingMode);

            var surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            surveyStateService.Open(surveySid);

            _dialerSurveyParametersManager.SetDialerSurveyParameters(surveySid, new List<DialerParameter>());
        }

        private void PrepareTest(IDialerAPI dialer, int notAvailableDialersQuantity)
        {
            const int dialersQntity = 3;

            var initializeCallCount = 0;
            var initializeCallsToFinishSuccessfully = dialersQntity - notAvailableDialersQuantity;

            var stubIDialerInitializer = new StubIDialerInitializer
            {
                InitializeDialerInt32IDialerAPIBooleanInt32OutStringOutDialTypeOut =
                    (int id, IDialerAPI api, bool b, out int tenantId, out string name, out DialType dialType) =>
                    {
                        tenantId = 0;
                        name = "";
                        dialType = DialType.Landline;

                        ++initializeCallCount;

                        if (initializeCallCount <= initializeCallsToFinishSuccessfully)
                        {
                            return dialer;
                        }

                        throw new InternalErrorException("Initialize of Dialer is failed.");
                    }
            };

            ServiceLocator.RegisterInstance<IDialerInitializer>(stubIDialerInitializer);

            _framework.BackendInitialize(true, null, 3);
        }
    }
}
