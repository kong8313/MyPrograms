using System;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.CATI.Core.UnitTests.ServiceLocation;
using Confirmit.CATI.Supervisor.Core.Activity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc.Fakes;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Repositories.Interfaces.Fakes;
using Confirmit.CATI.Core.SupervisorService.Fakes;

namespace Confirmit.CATI.Supervisor.Core.UnitTests
{
    [TestClass]
    public class ActivityManagerTest
    {
        [TestInitialize]
        public void Init()
        {
            var backendInstance = new BackendInstance();
            BackendInstance.Current = backendInstance;

            InitializeServiceLocator();
        }

        public void InitializeServiceLocator()
        {
            UnitTestsServiceLocatorInitializer.InitializeServiceLocator();

            ServiceLocator.Register<IActivityManager, ActivityManager>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            BackendInstance.Current = null;

            UnitTestsServiceLocatorInitializer.CleanupServiceLocator();
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void GetTZNameByBias_CallMethod_ReturnTZName()
        {
            Assert.AreEqual("(GMT)", ActivityManager.GetTZNameByBias(0));
            Assert.AreEqual("(GMT-0:30)", ActivityManager.GetTZNameByBias(30));
            Assert.AreEqual("(GMT+1)", ActivityManager.GetTZNameByBias(-60));
            Assert.AreEqual("(GMT+13)", ActivityManager.GetTZNameByBias(-780));
            Assert.AreEqual("(GMT-3:30)", ActivityManager.GetTZNameByBias(210));
            Assert.AreEqual("Incorrect timezone", ActivityManager.GetTZNameByBias(null));
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void TerminateTaskByPerson_TaskCanBeTerminated_Success()
        {
            var stubSupervisorServiceClient = new StubISupervisorServiceClient
            {
                TerminateTaskByPersonInt32NullableOfCallOutcome = (x, y) => new BvTasksEntity()
            };
            ServiceLocator.RegisterInstance<ISupervisorServiceClient>(stubSupervisorServiceClient);

            IConnectionStrings connectionStrings = new StubIConnectionStrings();
            ServiceLocator.RegisterInstance(connectionStrings);
            
            var person = new BvPersonEntity();

            IPersonRepository personRepositoryStub = new StubIPersonRepository
            {
                Inner = ServiceLocator.Resolve<IPersonRepository>(),
                GetByIdInt32 = sid => person
            };
            ServiceLocator.RegisterInstance(personRepositoryStub);

            ITaskRepository taskRepositoryStub = new StubITaskRepository
            {
                Inner = ServiceLocator.Resolve<ITaskRepository>(),
                GetByPersonInt32 = sid => new BvTasksEntity()
            };
            ServiceLocator.RegisterInstance(taskRepositoryStub);

            var activityManager = ServiceLocator.Resolve<IActivityManager>();
            activityManager.TerminateTaskByPerson(1);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        [ExpectedException(typeof(UserMessageException))]
        public void TerminateTaskByPerson_UnableToTerminateTask_ExceptionThrows()
        {
            var stubSupervisorServiceClient = new StubISupervisorServiceClient
            {
                TerminateTaskByPersonInt32NullableOfCallOutcome = (x, y) => null
            };
            ServiceLocator.RegisterInstance<ISupervisorServiceClient>(stubSupervisorServiceClient);

            IConnectionStrings connectionStrings = new StubIConnectionStrings();
            ServiceLocator.RegisterInstance(connectionStrings);

            var person = new BvPersonEntity();
            IPersonRepository personRepositoryStub = new StubIPersonRepository
            {
                Inner = ServiceLocator.Resolve<IPersonRepository>(),
                GetByIdInt32 = sid => person
            };
            ServiceLocator.RegisterInstance(personRepositoryStub);

            ITaskRepository taskRepositoryStub = new StubITaskRepository
            {
                Inner = ServiceLocator.Resolve<ITaskRepository>(),
                GetByPersonInt32 = sid => new BvTasksEntity()
            };
            ServiceLocator.RegisterInstance(taskRepositoryStub);

            var activityManager = ServiceLocator.Resolve<IActivityManager>();
            activityManager.TerminateTaskByPerson(1);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TerminateTaskByPerson_InvalidPersonID_ExceptionThrows()
        {
            ServiceLocator.Register<ISupervisorServiceClient, StubISupervisorServiceClient>();

            IConnectionStrings connectionStrings = new StubIConnectionStrings();
            ServiceLocator.RegisterInstance(connectionStrings);

            var activityManager = ServiceLocator.Resolve<IActivityManager>();
            var person = new BvPersonEntity();
            IPersonRepository personRepositoryStub = new StubIPersonRepository
            {
                Inner = ServiceLocator.Resolve<IPersonRepository>(),
                GetByIdInt32 = sid => person
            };
            ServiceLocator.RegisterInstance(personRepositoryStub);

            int invalidPersonID = -1;
            activityManager.TerminateTaskByPerson(invalidPersonID);
        }
    }
}