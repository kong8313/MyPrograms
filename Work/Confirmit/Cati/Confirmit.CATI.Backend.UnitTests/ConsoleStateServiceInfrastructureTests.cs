//using System;
//using System.ServiceModel;
//using System.ServiceModel.Channels;
//using System.ServiceModel.Dispatcher;
//using Confirmit.CATI.Backend.Properties;
//using Confirmit.CATI.Backend.WcfServices.External.ConsoleService;
//using Confirmit.CATI.Common.Exceptions;
//using Confirmit.CATI.Common.WcfTools.ConsoleMessageHeader;
//using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
//using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
//using Confirmit.CATI.Core.DAL.Handmade.Cache;
//using Confirmit.CATI.Core.Repositories;
//using Confirmit.CATI.Core.ServiceLocation;
//using Confirmit.CATI.Core.SystemSettings;

//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using TypeMock;
//using TypeMock.ArrangeActAssert;
//using Confirmit.CATI.Core.Misc;

//namespace Confirmit.CATI.Backend.UnitTests
//{
//    [TestClass]
//    public class ConsoleStateServiceInfrastructureTests
//    {
//        [TestInitialize]
//        public void TestInitialize()
//        {
//            MockManager.Init();

//            var backendInstance = new BackendInstance();
//            BackendInstance.Current = backendInstance;

//            var serviceLocator = new ServiceLocator();
//            serviceLocator.Cleanup();
//            serviceLocator.Initialize();
//            serviceLocator.RegisterSingleton<ISystemSettings, SystemSettings>();
//            serviceLocator.RegisterSingleton<ISystemSettingCache, MemorySystemSettingCache>();

//        }

//        [TestCleanup]
//        public void TestCleanup()
//        {
//            MockManager.ClearAll();

//            BackendInstance.Current = null;
//        }

//        [TestMethod, Owner(@"FIRM\AlexanderM"), Isolated]
//        public void InterviewingStateExtension_GetCurrentInterviewer_CorrectData()
//        {
//            var person = new BvPersonEntity { SID = new Random().Next() };

//            Isolate.WhenCalled(() => PersonRepository.GetByName(null)).WillReturn(person);

//            var current = new InterviewingStateExtension().Interviewer;

//            Assert.AreEqual(person.SID, current.SID);
//        }

//        [TestMethod, Owner(@"FIRM\AlexanderM"), Isolated]
//        public void InterviewingStateExtension_GetCurrentTask_CorrectData()
//        {
//            var person = new BvPersonEntity { SID = new Random().Next() };
//            var task = new BvTasksEntity { InterviewID = new Random().Next() };

//            Isolate.WhenCalled(() => PersonRepository.GetByName(null)).WillReturn(person);
//            Isolate.WhenCalled(() => TaskRepository.GetByPerson(0)).WillReturn(task);

//            var current = new InterviewingStateExtension().Task;

//            Assert.AreEqual(task.InterviewID, current.InterviewID);
//        }

//        [TestMethod, Owner(@"FIRM\AlexanderM"), Isolated]
//        public void InterviewingStateExtension_GetCurrentInterviewer2Times_CheckDalCalledOnce()
//        {
//            var person = new BvPersonEntity { SID = new Random().Next() };

//            Isolate.WhenCalled(() => PersonRepository.GetByName(null)).WillReturn(person);

//            var extension = new InterviewingStateExtension();

//            var current = extension.Interviewer;
//            Assert.AreEqual(person.SID, current.SID);

//            Isolate.WhenCalled(() => PersonRepository.GetByName(null)).WillThrow(new Exception());

//            current = extension.Interviewer;
//            Assert.AreEqual(person.SID, current.SID);
//        }

//        [TestMethod, Owner(@"FIRM\AlexanderM"), Isolated]
//        public void InterviewingStateExtension_GetCurrentTask2Times_CheckDalCalledOnce()
//        {
//            var person = new BvPersonEntity { SID = new Random().Next() };
//            var task = new BvTasksEntity { InterviewID = new Random().Next() };

//            Isolate.WhenCalled(() => PersonRepository.GetByName(null)).WillReturn(person);
//            Isolate.WhenCalled(() => TaskRepository.GetByPerson(0)).WillReturn(task);

//            var current = new InterviewingStateExtension().Task;
//            Assert.AreEqual(task.InterviewID, current.InterviewID);

//            Isolate.WhenCalled(() => PersonRepository.GetByName(null)).WillThrow(new Exception());
//            Isolate.WhenCalled(() => TaskRepository.GetByPerson(0)).WillThrow(new Exception());

//            current = new InterviewingStateExtension().Task;
//            Assert.AreEqual(task.InterviewID, current.InterviewID);
//        }

//        [TestMethod, Owner(@"FIRM\AlexanderM"), Isolated, ExpectedException(typeof(InterviewerNotLoggedInException))]
//        public void InterviewerValidationBehavior_InterNotLoggedIn_Exception()
//        {
//            var interviewingStateExtension = Isolate.Fake.Instance<InterviewingStateExtension>();

//            var logoutEvent = Isolate.Fake.Instance<ForcedLogoutEvent>();
//            Isolate.WhenCalled(() => new ForcedLogoutEvent()).WillReturn(logoutEvent);
//            Isolate.WhenCalled(() => logoutEvent.Save(0)).IgnoreCall();

//            Isolate.WhenCalled(() => InterviewingStateExtension.Current).WillReturn(interviewingStateExtension);
//            Isolate.WhenCalled(() => interviewingStateExtension.Task).WillReturn(null);

//            Message request = null;
//            ((IDispatchMessageInspector)(new InterviewerValidationBehaviorAttribute())).AfterReceiveRequest(ref request, null, null);
//        }

//        [TestMethod, Owner(@"FIRM\AlexanderM"), Isolated, ExpectedException(typeof(FaultException))]
//        public void InterviewerValidationBehavior_InterLoggedInButAuthKeyInvalid_Exception()
//        {
//            var interviewingStateExtension = Isolate.Fake.Instance<InterviewingStateExtension>();

//            var logoutEvent = Isolate.Fake.Instance<ForcedLogoutEvent>();
//            Isolate.WhenCalled(() => new ForcedLogoutEvent()).WillReturn(logoutEvent);
//            Isolate.WhenCalled(() => logoutEvent.Save(0)).IgnoreCall();

//            Isolate.WhenCalled(() => InterviewingStateExtension.Current).WillReturn(interviewingStateExtension);
//            Isolate.WhenCalled(() => interviewingStateExtension.Interviewer).WillReturn(new BvPersonEntity());
//            Isolate.WhenCalled(() => interviewingStateExtension.Task).WillReturn(
//                new BvTasksEntity { AuthenticationKey = new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11) });
//            Isolate.WhenCalled(() => ConsoleMessageHeaderBehavior.GetIncomingMessageKey()).WillReturn(
//                new Guid(1, 2, 3, 4, 5, 6, 1, 8, 9, 10, 11));
//            Message request = null;
//            ((IDispatchMessageInspector)(new InterviewerValidationBehaviorAttribute())).AfterReceiveRequest(ref request, null, null);
//        }

//        [TestMethod, Owner(@"FIRM\AlexanderM"), Isolated]
//        public void InterviewerValidationBehavior_InterLoggedInAndAuthKeyValid_Success()
//        {
//            var interviewingStateExtension = Isolate.Fake.Instance<InterviewingStateExtension>();

//            var logoutEvent = Isolate.Fake.Instance<ForcedLogoutEvent>();
//            Isolate.WhenCalled(() => new ForcedLogoutEvent()).WillReturn(logoutEvent);
//            Isolate.WhenCalled(() => logoutEvent.Save(0)).IgnoreCall();

//            Isolate.WhenCalled(() => InterviewingStateExtension.Current).WillReturn(interviewingStateExtension);
//            Isolate.WhenCalled(() => interviewingStateExtension.Interviewer).WillReturn(new BvPersonEntity());
//            Isolate.WhenCalled(() => interviewingStateExtension.Task).WillReturn(
//                new BvTasksEntity {AuthenticationKey = new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11)});
//            Isolate.WhenCalled(() => ConsoleMessageHeaderBehavior.GetIncomingMessageKey()).WillReturn(
//                new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11));
//            Isolate.WhenCalled(() => interviewingStateExtension.Task.StartSessionTime).WillReturn(DateTime.UtcNow);
//            Message request = null;
//            ((IDispatchMessageInspector)(new InterviewerValidationBehaviorAttribute())).AfterReceiveRequest(ref request, null, null);
//        }

//        [TestMethod, Owner(@"FIRM\SergeyC"), Isolated, ExpectedException(typeof(StateServiceSessionExpiredException))]
//        public void InterviewerValidationBehavior_InterLoggedInAndSessionIsExpired_ExceptionIsThrown()
//        {
//            var interviewingStateExtension = Isolate.Fake.Instance<InterviewingStateExtension>();
//            var sessionTimeout = ServiceLocator.Resolve<ISystemSettings>().Console.StateServiceSessionTimeoutInMinutes;

//            var logoutEvent = Isolate.Fake.Instance<ForcedLogoutEvent>();
//            Isolate.WhenCalled(() => new ForcedLogoutEvent()).WillReturn(logoutEvent);
//            Isolate.WhenCalled(() => logoutEvent.Save(0)).IgnoreCall();

//            Isolate.WhenCalled(() => InterviewingStateExtension.Current).WillReturn(interviewingStateExtension);
//            Isolate.WhenCalled(() => interviewingStateExtension.Interviewer).WillReturn(new BvPersonEntity());
//            Isolate.WhenCalled(() => interviewingStateExtension.Task).WillReturn(
//                new BvTasksEntity { AuthenticationKey = new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11) });
//            Isolate.WhenCalled(() => ConsoleMessageHeaderBehavior.GetIncomingMessageKey()).WillReturn(
//                new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11));
//            Isolate.WhenCalled(() => interviewingStateExtension.Task.StartSessionTime).WillReturn(DateTime.UtcNow - TimeSpan.FromMinutes(sessionTimeout + 5));
//            Message request = null;
//            ((IDispatchMessageInspector)(new InterviewerValidationBehaviorAttribute())).AfterReceiveRequest(ref request, null, null);
//        }
//    }
//}
