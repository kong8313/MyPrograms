using BvCallHandlerLibrary;
using BvCallHandlerLibrary.Fakes;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Misc.Fakes;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Fakes;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Core.Telephony.Fakes;
using Confirmit.CATI.Core.UnitTests.ServiceLocation;
using Confirmit.CATI.Telephony.Fakes;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.Telephony
{
    [TestClass]
    public class DialersInstanceTest
    {
        [TestInitialize]
        public void TestInitialiaze()
        {
            UnitTestsServiceLocatorInitializer.InitializeServiceLocator();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            UnitTestsServiceLocatorInitializer.CleanupServiceLocator();
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void UninitializeWithRelease_ReleaseIsCalled()
        {
            var releaseIsCalled = false;

            var stubIDialerApi = new StubIDialerAPI
            {
                ReleaseInt32Int32 = (dialerId, companyId) =>
                {
                    releaseIsCalled = true;
                    return (int)DialerErrorCode.Success;
                }
            };
            ServiceLocator.RegisterInstance<IDialerInitializer>(new StubIDialerInitializer
            {
                CreateInstance = () => stubIDialerApi
            });


            var stubICompanyInfo = new StubICompanyInfo()
            {
                CompanyIdGet = () => { return 0; }
            };
            ServiceLocator.RegisterInstance<ICompanyInfo>(stubICompanyInfo);

            var target = ServiceLocator.Resolve<IDialerInstance>();

            target.Uninitialize(true);

            Assert.IsTrue(releaseIsCalled, "Release() was expected to be called but it is not called.");
            Assert.IsFalse(target.IsDialerInitialized, "IsDialerInitialized is expected to be 'false'");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void UninitializeWithoutRelease_ReleaseIsNotCalled()
        {
            var releaseIsCalled = false;

            var stubIDialerApi = new StubIDialerAPI
            {
                ReleaseInt32Int32 = (dialerId, companyId) =>
                {
                    releaseIsCalled = true;
                    return (int)DialerErrorCode.Success;
                }
            };
            ServiceLocator.RegisterInstance<IDialerInitializer>(new StubIDialerInitializer
            {
                CreateInstance = () => stubIDialerApi
            });

            var stubICompanyInfo = new StubICompanyInfo()
            {
                CompanyIdGet = () => { return 0; }
            };
            ServiceLocator.RegisterInstance<ICompanyInfo>(stubICompanyInfo);

            var target = ServiceLocator.Resolve<IDialerInstance>();

            target.Uninitialize(false);

            Assert.IsFalse(releaseIsCalled, "Release() was called but that is not expected.");
            Assert.IsFalse(target.IsDialerInitialized, "IsDialerInitialized is expected to be 'false'");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void OnDialerState_DialerServiceStarted_SendDialerWsStartedEmailNotification()
        {
            var sendDialerWsStartedEmailNotificationIsCalled = false;

            ServiceLocator.RegisterInstance<IDialerEmailNotificationService>(new StubIDialerEmailNotificationService
            {
                SendDialerWsStartedEmailNotificationInt32 = id => sendDialerWsStartedEmailNotificationIsCalled = true
            });

            var target = ServiceLocator.Resolve<IDialerInstance>();

            target.OnDialerState(DialerState.DialerServiceStarted);

            Assert.IsTrue(sendDialerWsStartedEmailNotificationIsCalled, "SendDialerWsStartedEmailNotification() was expected to be called but it is not called.");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void OnDialerState_Available_UpdateDialerStateNotificationTime()
        {
            var updateDialerStateNotificationTimeIsCalled = false;

            ServiceLocator.RegisterInstance<IDialerStateTools>(new StubIDialerStateTools
            {
                UpdateDialerStateNotificationTimeInt32 = id => updateDialerStateNotificationTimeIsCalled = true
            });

            var target = ServiceLocator.Resolve<IDialerInstance>();

            target.OnDialerState(DialerState.Available);

            Assert.IsTrue(updateDialerStateNotificationTimeIsCalled, "UpdateDialerStateNotificationTime() was expected to be called but it is not called.");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void OnDialerState_DialerLoggerProblem_SendDialerLoggerProblemEmailNotification()
        {
            var sendDialerLoggerProblemEmailNotificationIsCalled = false;

            ServiceLocator.RegisterInstance<IDialerEmailNotificationService>(new StubIDialerEmailNotificationService
            {
                SendDialerLoggerProblemEmailNotificationInt32 = id => sendDialerLoggerProblemEmailNotificationIsCalled = true
            });

            var target = ServiceLocator.Resolve<IDialerInstance>();

            target.OnDialerState(DialerState.DialerLoggerProblem);

            Assert.IsTrue(sendDialerLoggerProblemEmailNotificationIsCalled, "SendDialerLoggerProblemEmailNotification() was expected to be called but it is not called.");
        }
    }
}