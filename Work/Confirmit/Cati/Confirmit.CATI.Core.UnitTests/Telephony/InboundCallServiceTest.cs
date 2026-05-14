using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces.Fakes;
using Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Fakes;
using Confirmit.CATI.Core.Services.Interfaces.Fakes;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.Core.Services.TimeService.Fakes;
using Confirmit.CATI.Core.SystemSettings.Fakes;
using Confirmit.CATI.Core.Telephony.Inbound;
using Confirmit.CATI.Core.UnitTests.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.Telephony
{
    [TestClass]
    public class InboundCallServiceTest
    {
        [TestMethod, Owner(@"FIRM\alm")]
        public void SurveyIsClosed_CheckAndSearchInterview_DropInboundCallReasonIsSurveyIsNotOpened()
        {
            var stubIToggleSettings = new StubIToggleSettings
            {
                EnableInboundGet = () => true
            };

            var stubIInboundTelephoneNumberRepository = new StubIInboundTelephoneNumberRepository
            {
                TryGetByTelephoneNumberString = x => new BvInboundTelephoneNumberEntity { SurveyId = 0 }
            };

            var stubISurveyRepository = new StubISurveyRepository
            {
                TryGetByIdInt32 = x => new BvSurveyEntity { State = (int)SurveyState.Close }
            };

            var target = new InboundCallService(
                stubIToggleSettings,
                null,
                stubIInboundTelephoneNumberRepository,
                null,
                stubISurveyRepository,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            try
            {
                InterviewWithCall result = new InterviewWithCall();
                target.CheckAndSearchInterview("", "", result);

                Assert.Fail("InboundCallCantProceedException was expected but it is not thrown");
            }
            catch (InboundCallCantProceedException ex)
            {
                // The InboundCallCantProceedException exception is expected
                Assert.AreEqual(DropInboundCallReason.SurveyIsNotOpened, ex.DropInboundCallReason,
                    "DropInboundCallReason is not as expected");
            }
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void SurveyIsSoftDeleted_CheckAndSearchInterview_DropInboundCallReasonIsSurveyIsNotOpened()
        {
            var stubIToggleSettings = new StubIToggleSettings
            {
                EnableInboundGet = () => true
            };

            var stubIInboundTelephoneNumberRepository = new StubIInboundTelephoneNumberRepository
            {
                TryGetByTelephoneNumberString = x => new BvInboundTelephoneNumberEntity { SurveyId = 0 }
            };

            var stubISurveyRepository = new StubISurveyRepository
            {
                TryGetByIdInt32 = x => new BvSurveyEntity { State = (int)SurveyState.SoftDeleted }
            };

            var target = new InboundCallService(
                stubIToggleSettings,
                null,
                stubIInboundTelephoneNumberRepository,
                null,
                stubISurveyRepository,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            try
            {
                InterviewWithCall result = new InterviewWithCall();
                target.CheckAndSearchInterview("", "", result);

                Assert.Fail("InboundCallCantProceedException was expected but it is not thrown");
            }
            catch (InboundCallCantProceedException ex)
            {
                // The InboundCallCantProceedException exception is expected
                Assert.AreEqual(DropInboundCallReason.SurveyIsNotOpened, ex.DropInboundCallReason,
                    "DropInboundCallReason is not as expected");
            }
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void SurveyIsNotFound_CheckAndSearchInterview_DropInboundCallReasonIsSurveyIsNotFound()
        {
            var stubIToggleSettings = new StubIToggleSettings
            {
                EnableInboundGet = () => true
            };

            var stubIInboundTelephoneNumberRepository = new StubIInboundTelephoneNumberRepository
            {
                TryGetByTelephoneNumberString = x => new BvInboundTelephoneNumberEntity { SurveyId = 0 }
            };

            var stubISurveyRepository = new StubISurveyRepository
            {
                TryGetByIdInt32 = x => null
            };

            var target = new InboundCallService(
                stubIToggleSettings,
                null,
                stubIInboundTelephoneNumberRepository,
                null,
                stubISurveyRepository,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            try
            {
                InterviewWithCall result = new InterviewWithCall();
                target.CheckAndSearchInterview("", "", result);

                Assert.Fail("InboundCallCantProceedException was expected but it is not thrown");
            }
            catch (InboundCallCantProceedException ex)
            {
                // The InboundCallCantProceedException exception is expected
                Assert.AreEqual(DropInboundCallReason.SurveyIsNotFound, ex.DropInboundCallReason,
                    "DropInboundCallReason is not as expected");
            }
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void CheckAndSearchInterview_IsRespondentsDynamicCreationAllowedIsFalse_DropInboundCallReasonIsInterviewIsNotFound()
        {
            var stubIToggleSettings = new StubIToggleSettings
            {
                EnableInboundGet = () => true
            };

            var stubIInboundTelephoneNumberRepository = new StubIInboundTelephoneNumberRepository
            {
                TryGetByTelephoneNumberString = x => new BvInboundTelephoneNumberEntity { SurveyId = 0 }
            };

            var stubISurveyRepository = new StubISurveyRepository
            {
                TryGetByIdInt32 = x => new BvSurveyEntity { State = (int)SurveyState.Open, IsRespondentsDynamicCreationAllowed = false}
            };

            var stubIInterviewRepository = new StubIInterviewRepository
            {
                GetByTelephoneNumberInt32String = (i, s) => null
            };

            var target = new InboundCallService(
                stubIToggleSettings,
                null,
                stubIInboundTelephoneNumberRepository,
                stubIInterviewRepository,
                stubISurveyRepository,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            try
            {
                InterviewWithCall result = new InterviewWithCall();
                target.CheckAndSearchInterview("", "123", result);

                Assert.Fail("InboundCallCantProceedException was expected but it is not thrown");
            }
            catch (InboundCallCantProceedException ex)
            {
                Assert.AreEqual(DropInboundCallReason.InterviewIsNotFound, ex.DropInboundCallReason,
                    "DropInboundCallReason is not as expected");
            }
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void CheckAndSearchInterview_UseMatchAndCreateInboundBehaviorType_AddRespondentAndGetByTelephoneNumberMethodsWereCalled()
        {
            var stubIToggleSettings = new StubIToggleSettings
            {
                EnableInboundGet = () => true
            };

            var stubIInboundTelephoneNumberRepository = new StubIInboundTelephoneNumberRepository
            {
                TryGetByTelephoneNumberString = x => new BvInboundTelephoneNumberEntity { SurveyId = 0 }
            };

            var stubISurveyRepository = new StubISurveyRepository
            {
                TryGetByIdInt32 = x => new BvSurveyEntity
                    { State = (int)SurveyState.Open, InboundCallBehavior = (byte)InboundSurveyBehavior.MatchAndCreate }
            };

            int countCallsOfGetByTelephoneNumber = 0;
            var stubIInterviewRepository = new StubIInterviewRepository
            {
                GetByTelephoneNumberInt32String = (i, s) => { countCallsOfGetByTelephoneNumber++; return null; }
            };

            int countCallsOfAddRespondentInRespondentClient = 0;
            var stubIRespondentsClient = new StubIRespondentsClient
            {
                AddRespondentStringRespondentsInfo = (i, s) => { countCallsOfAddRespondentInRespondentClient++; return 0; }
            };

            int countCallsOfAddRespondentInInterviewService = 0;
            var stubIInterviewService = new StubIInterviewService
            {
                AddRespondentBvSurveyEntityInt32SchedulingScriptExecutionOptions = (s, resp, o) =>
                {
                    countCallsOfAddRespondentInInterviewService++;
                    return new BvInterviewWithOriginEntity(new BvInterviewEntity());
                }
            };

            var stubICallCenterRepository = new StubICallCenterRepository
            {
                DefaultGet = () => new BvCallCenterEntity { LocalTimezoneId = 0}
            };

            var stubIShiftServiceFactory = new StubIShiftServiceFactory
            {
                GetInt32 = i => new StubIShiftService
                { 
                    GetExactShiftDateTimeInt32 = (dt, tz) => new ShiftService.MatchingShift(new ShiftService.Shift(), dt)
                }
            };

            var stubICallQueueService = new StubICallQueueService
            {
                GetCallWithTryLockInt32Int32BooleanOut = 
                (int surveySid, int interviewId, out bool isCallLocked) => { isCallLocked = true; return new BvCallEntity(); }
            };

            var stubITimeService = new StubITimeService
            {
                GetUtcNow = () => new DateTime(2000, 1, 1)
            };

            var target = new InboundCallService(
                stubIToggleSettings,
                null,
                stubIInboundTelephoneNumberRepository,
                stubIInterviewRepository,
                stubISurveyRepository,
                stubIShiftServiceFactory,
                stubICallCenterRepository,
                stubIRespondentsClient,
                stubIInterviewService,
                stubITimeService,
                stubICallQueueService,
                new StubIContextInfoService());

            UnitTestsServiceLocatorInitializer.InitializeServiceLocator().RegisterInstance<ITimeService>(new TimeService());

            InterviewWithCall result = new InterviewWithCall();
            target.CheckAndSearchInterview("", "1234", result);

            Assert.AreEqual(1, countCallsOfGetByTelephoneNumber, "Method GetByTelephoneNumber was not called");
            Assert.AreEqual(1, countCallsOfAddRespondentInRespondentClient, "Method AddRespondent in RespondentClient class was not called");
            Assert.AreEqual(1, countCallsOfAddRespondentInInterviewService, "Method AddRespondent in InterviewService class was not called");
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void CheckAndSearchInterview_UseCreateOnlyInboundBehaviorType_OnlyAddRespondentMethodsWereCalled()
        {
            var stubIToggleSettings = new StubIToggleSettings
            {
                EnableInboundGet = () => true
            };

            var stubIInboundTelephoneNumberRepository = new StubIInboundTelephoneNumberRepository
            {
                TryGetByTelephoneNumberString = x => new BvInboundTelephoneNumberEntity { SurveyId = 0 }
            };

            var stubISurveyRepository = new StubISurveyRepository
            {
                TryGetByIdInt32 = x => new BvSurveyEntity
                { State = (int)SurveyState.Open, InboundCallBehavior = (byte)InboundSurveyBehavior.CreateOnly }
            };

            int countCallsOfGetByTelephoneNumber = 0;
            var stubIInterviewRepository = new StubIInterviewRepository
            {
                GetByTelephoneNumberInt32String = (i, s) => { countCallsOfGetByTelephoneNumber++; return null; }
            };

            int countCallsOfAddRespondentInRespondentClient = 0;
            var stubIRespondentsClient = new StubIRespondentsClient
            {
                AddRespondentStringRespondentsInfo = (i, s) => { countCallsOfAddRespondentInRespondentClient++; return 0; }
            };

            int countCallsOfAddRespondentInInterviewService = 0;
            var stubIInterviewService = new StubIInterviewService
            {
                AddRespondentBvSurveyEntityInt32SchedulingScriptExecutionOptions = (s, resp, o) =>
                {
                    countCallsOfAddRespondentInInterviewService++;
                    return new BvInterviewWithOriginEntity(new BvInterviewEntity());
                }
            };

            var stubICallCenterRepository = new StubICallCenterRepository
            {
                DefaultGet = () => new BvCallCenterEntity { LocalTimezoneId = 0 }
            };

            var stubIShiftServiceFactory = new StubIShiftServiceFactory
            {
                GetInt32 = i => new StubIShiftService
                {
                    GetExactShiftDateTimeInt32 = (dt, tz) => new ShiftService.MatchingShift(new ShiftService.Shift(), dt)
                }
            };

            var stubICallQueueService = new StubICallQueueService
            {
                GetCallWithTryLockInt32Int32BooleanOut =
                (int surveySid, int interviewId, out bool isCallLocked) => { isCallLocked = true; return new BvCallEntity(); }
            };

            var stubITimeService = new StubITimeService
            {
                GetUtcNow = () => new DateTime(2000, 1, 1)
            };

            var target = new InboundCallService(
                stubIToggleSettings,
                null,
                stubIInboundTelephoneNumberRepository,
                stubIInterviewRepository,
                stubISurveyRepository,
                stubIShiftServiceFactory,
                stubICallCenterRepository,
                stubIRespondentsClient,
                stubIInterviewService,
                stubITimeService,
                stubICallQueueService,
                new StubIContextInfoService());

            UnitTestsServiceLocatorInitializer.InitializeServiceLocator().RegisterInstance<ITimeService>(new TimeService());

            InterviewWithCall result = new InterviewWithCall();
            target.CheckAndSearchInterview("", "1234", result);

            Assert.AreEqual(0, countCallsOfGetByTelephoneNumber, "Method GetByTelephoneNumber has not to be called but it was called");
            Assert.AreEqual(1, countCallsOfAddRespondentInRespondentClient, "Method AddRespondent in RespondentClient class was not called");
            Assert.AreEqual(1, countCallsOfAddRespondentInInterviewService, "Method AddRespondent in InterviewService class was not called");
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]        
        public void CheckAndSearchInterview_UseMatchOnlyInboundBehaviorType_OnlyGetByTelephoneNumberWasCalled()
        {
            var stubIToggleSettings = new StubIToggleSettings
            {
                EnableInboundGet = () => true
            };

            var stubIInboundTelephoneNumberRepository = new StubIInboundTelephoneNumberRepository
            {
                TryGetByTelephoneNumberString = x => new BvInboundTelephoneNumberEntity { SurveyId = 0 }
            };

            var stubISurveyRepository = new StubISurveyRepository
            {
                TryGetByIdInt32 = x => new BvSurveyEntity
                { State = (int)SurveyState.Open, InboundCallBehavior = (byte)InboundSurveyBehavior.MatchOnly }
            };

            int countCallsOfGetByTelephoneNumber = 0;
            var stubIInterviewRepository = new StubIInterviewRepository
            {
                GetByTelephoneNumberInt32String = (i, s) => { countCallsOfGetByTelephoneNumber++; return null; }
            };

            int countCallsOfAddRespondentInRespondentClient = 0;
            var stubIRespondentsClient = new StubIRespondentsClient
            {
                AddRespondentStringRespondentsInfo = (i, s) => { countCallsOfAddRespondentInRespondentClient++; return 0; }
            };

            int countCallsOfAddRespondentInInterviewService = 0;
            var stubIInterviewService = new StubIInterviewService
            {
                AddRespondentBvSurveyEntityInt32Int32OperationTypeRoleNullableOfInt32 = (s, resp, c, o, role, p) =>
                {
                    countCallsOfAddRespondentInInterviewService++;
                    return new BvInterviewWithOriginEntity(new BvInterviewEntity());
                }
            };

            var stubICallCenterRepository = new StubICallCenterRepository
            {
                DefaultGet = () => new BvCallCenterEntity { LocalTimezoneId = 0 }
            };

            var stubIShiftServiceFactory = new StubIShiftServiceFactory
            {
                GetInt32 = i => new StubIShiftService
                {
                    GetExactShiftDateTimeInt32 = (dt, tz) => new ShiftService.MatchingShift(new ShiftService.Shift(), dt)
                }
            };

            var stubICallQueueService = new StubICallQueueService
            {
                GetCallWithTryLockInt32Int32BooleanOut =
                (int surveySid, int interviewId, out bool isCallLocked) => { isCallLocked = true; return new BvCallEntity(); }
            };

            var stubITimeService = new StubITimeService
            {
                GetUtcNow = () => new DateTime(2000, 1, 1)
            };

            var target = new InboundCallService(
                stubIToggleSettings,
                null,
                stubIInboundTelephoneNumberRepository,
                stubIInterviewRepository,
                stubISurveyRepository,
                stubIShiftServiceFactory,
                stubICallCenterRepository,
                stubIRespondentsClient,
                stubIInterviewService,
                stubITimeService,
                stubICallQueueService,
                new StubIContextInfoService());

            InterviewWithCall result = new InterviewWithCall();
            try
            {
                target.CheckAndSearchInterview("", "1234", result);
            }
            catch (InboundCallCantProceedException)
            {
                Assert.AreEqual(1, countCallsOfGetByTelephoneNumber, "Method GetByTelephoneNumber class was not called");
                Assert.AreEqual(0, countCallsOfAddRespondentInRespondentClient, "Method AddRespondent in RespondentClient has not to be called but it was called");
                Assert.AreEqual(0, countCallsOfAddRespondentInInterviewService, "Method AddRespondent in InterviewService has not to be called but it was called");
            }
        }
    }
}