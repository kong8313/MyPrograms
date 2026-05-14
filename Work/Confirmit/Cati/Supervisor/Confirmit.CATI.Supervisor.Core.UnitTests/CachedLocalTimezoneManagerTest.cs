using System.Collections.Generic;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.CallCenters;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Misc.Fakes;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated.Fakes;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Repositories.Interfaces.Fakes;
using Confirmit.CATI.Core.Services.Interfaces.Fakes;
using Confirmit.CATI.Core.UnitTests.ServiceLocation;
using Confirmit.CATI.Supervisor.Core.Activity;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Core.Timezone;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Supervisor.Core.UnitTests
{
    [TestClass]
    public class CachedLocalTimezoneManagerTest
    {
        [TestInitialize]
        public void Init()
        {
            InitializeServiceLocator();
        }
            
        private void InitializeServiceLocator()
        {
            UnitTestsServiceLocatorInitializer.InitializeServiceLocator();

            ServiceLocator.Register<ISqlTableUpdatedPublisher, StubISqlTableUpdatedPublisher>();
            ServiceLocator.RegisterSingleton<IConnectionStrings>(new StubIConnectionStrings());
            ServiceLocator.RegisterSingleton<IDbLibProvider>(new StubIDbLibProvider());
        }
        
        class TestChangeLocal : ICallCenterProvider, ICallCenterRepository
        {
            public BvCallCenterEntity CallCenter;

            public TestChangeLocal(BvCallCenterEntity callCenterForTest)
            {
                CallCenter = callCenterForTest;
            }

            public int GetCurrentId()
            {
                throw new System.NotImplementedException();
            }

            public BvCallCenterEntity GetCurrent()
            {
                return CallCenter;
            }

            public BvCallCenterEntity Get(int id)
            {
                throw new System.NotImplementedException();
            }

            public BvCallCenterEntity Default { get; private set; }
            public List<BvCallCenterEntity> GetAssignedToSurvey(int surveyId)
            {
                throw new System.NotImplementedException();
            }

            public List<BvCallCenterEntity> GetAll()
            {
                throw new System.NotImplementedException();
            }

            public void Insert(BvCallCenterEntity entity)
            {
                throw new System.NotImplementedException();
            }

            public void Update(BvCallCenterEntity entity)
            {
                CallCenter = entity;
            }

            public void Delete(int id, int moveToCallCenterId, InterviewerActionOnCallCenterDelete interviewerAction)
            {
                throw new System.NotImplementedException();
            }

            public BvCallCenterEntityWithDialerIds GetCallCenterWithDialers(int id)
            {
                throw new System.NotImplementedException();
            }

            public void Insert(BvCallCenterEntityWithDialerIds entity)
            {
                throw new System.NotImplementedException();
            }

            public void Update(BvCallCenterEntityWithDialerIds entity, int[] newDialerIds, int[] oldDialerIds)
            {
                throw new System.NotImplementedException();
            }

            public List<BvCallCenterEntityWithDialerIds> GetAllWithDialerIds()
            {
                throw new System.NotImplementedException();
            }
        }

        class TestGetLocalTimezone : ICallCenterProvider, ITimezoneRepository
        {
            public int Counter;

            public TestGetLocalTimezone(int counterInitialValue)
            {
                Counter = counterInitialValue;
            }

            public int GetCurrentId()
            {
                throw new System.NotImplementedException();
            }

            public BvCallCenterEntity GetCurrent()
            {
                return new BvCallCenterEntity {LocalTimezoneId = Counter++};
            }

            public BvTimezoneEntity Get(int timezoneId)
            {
                return new BvTimezoneEntity {ID = timezoneId};
            }

            public BvTimezoneEntity GetMasterTimezone(int timezoneId)
            {
                throw new System.NotImplementedException();
            }

            public List<BvTimezoneEntity> GetActiveList()
            {
                return new List<BvTimezoneEntity>();
            }

            public List<BvTimezoneEntity> GetMasterList()
            {
                throw new System.NotImplementedException();
            }

            public List<BvTimezoneEntity> GetMasterListFromDefaultDatabase()
            {
                throw new System.NotImplementedException();
            }

            public void InsertMasterEntity(BvTimezoneEntity entity)
            {
                throw new System.NotImplementedException();
            }

            public void UpdateMasterEntity(BvTimezoneEntity entity)
            {
                throw new System.NotImplementedException();
            }

            public void UpdateMasterEntity(BvTimezoneEntity entity, bool isActiveTimezone)
            {
                throw new System.NotImplementedException();
            }

            public List<BvTimezoneEntity> GetCustomTimezones(int parentTimezoneId)
            {
                throw new System.NotImplementedException();
            }

            public void InsertCustomTimezone(BvTimezoneEntity customTimezone)
            {
                throw new System.NotImplementedException();
            }

            public List<BvTimezoneEntity> GetAllCustomTimezones()
            {
                throw new System.NotImplementedException();
            }

            public void UpdateCustomTimezone(BvTimezoneEntity customTimezone)
            {
                throw new System.NotImplementedException();
            }

            public void DeleteCustomTimezone(int customTimezoneId)
            {
                throw new System.NotImplementedException();
            }

            public List<BvTimezoneEntity> GetActiveWithoutCustomList()
            {
                throw new System.NotImplementedException();
            }
        }

        class TestChangeLocalRenewCache : ICallCenterProvider, ICallCenterRepository, ITimezoneRepository
        {
            public int TimezoneId;

            public TestChangeLocalRenewCache(int initialTimezoneId)
            {
                TimezoneId = initialTimezoneId;
            }

            public int GetCurrentId()
            {
                throw new System.NotImplementedException();
            }

            public BvCallCenterEntity GetCurrent()
            {
                return new BvCallCenterEntity {LocalTimezoneId = TimezoneId};
            }

            BvCallCenterEntity ICallCenterRepository.Get(int id)
            {
                throw new System.NotImplementedException();
            }

            public BvTimezoneEntity GetMasterTimezone(int timezoneId)
            {
                throw new System.NotImplementedException();
            }

            List<BvTimezoneEntity> ITimezoneRepository.GetActiveList()
            {
                throw new System.NotImplementedException();
            }

            public List<BvTimezoneEntity> GetMasterList()
            {
                throw new System.NotImplementedException();
            }

            public List<BvTimezoneEntity> GetMasterListFromDefaultDatabase()
            {
                throw new System.NotImplementedException();
            }

            public void InsertMasterEntity(BvTimezoneEntity entity)
            {
                throw new System.NotImplementedException();
            }

            public void UpdateMasterEntity(BvTimezoneEntity entity)
            {
                throw new System.NotImplementedException();
            }

            public BvCallCenterEntity Default { get; private set; }

            public List<BvCallCenterEntity> GetAssignedToSurvey(int surveyId)
            {
                throw new System.NotImplementedException();
            }

            public List<BvCallCenterEntity> GetAll()
            {
                throw new System.NotImplementedException();
            }

            public void Insert(BvCallCenterEntity entity)
            {
                throw new System.NotImplementedException();
            }

            public void Update(BvCallCenterEntity entity)
            {
                TimezoneId = entity.LocalTimezoneId;
            }

            public void Delete(int id, int moveToCallCenterId, InterviewerActionOnCallCenterDelete interviewerAction)
            {
                throw new System.NotImplementedException();
            }

            BvTimezoneEntity ITimezoneRepository.Get(int timezoneId)
            {
                return new BvTimezoneEntity {ID = timezoneId};
            }

            public void UpdateMasterEntity(BvTimezoneEntity entity, bool isActiveTimezone)
            {
                throw new System.NotImplementedException();
            }

            public List<BvTimezoneEntity> GetCustomTimezones(int parentTimezoneId)
            {
                throw new System.NotImplementedException();
            }

            public void InsertCustomTimezone(BvTimezoneEntity customTimezone)
            {
                throw new System.NotImplementedException();
            }

            public List<BvTimezoneEntity> GetAllCustomTimezones()
            {
                throw new System.NotImplementedException();
            }

            public void UpdateCustomTimezone(BvTimezoneEntity customTimezone)
            {
                throw new System.NotImplementedException();
            }

            public void DeleteCustomTimezone(int customTimezoneId)
            {
                throw new System.NotImplementedException();
            }

            public List<BvTimezoneEntity> GetActiveWithoutCustomList()
            {
                throw new System.NotImplementedException();
            }

            public BvCallCenterEntityWithDialerIds GetCallCenterWithDialers(int id)
            {
                throw new System.NotImplementedException();
            }

            public void Insert(BvCallCenterEntityWithDialerIds entity)
            {
                throw new System.NotImplementedException();
            }

            public void Update(BvCallCenterEntityWithDialerIds entity, int[] newDialerIds, int[] oldDialerIds)
            {
                throw new System.NotImplementedException();
            }

            public List<BvCallCenterEntityWithDialerIds> GetAllWithDialerIds()
            {
                throw new System.NotImplementedException();
            }
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void ChangeLocal_CorrectTimezoneIsGiven_LocalTimezoneIsChanged()
        {
            var callCenter = new BvCallCenterEntity {ID = 2, Name = "Default", LocalTimezoneId = 1};
            var testChangeLocal = new TestChangeLocal(callCenter);
            var manager = new CachedLocalTimezoneManager(testChangeLocal, testChangeLocal, new StubITimezoneRepository(), new StubITimezoneService());
            const int expectedTimezone = 3;
            manager.ChangeLocal(expectedTimezone);

            Assert.AreEqual(callCenter.ID, testChangeLocal.CallCenter.ID);
            Assert.AreEqual(callCenter.Name, testChangeLocal.CallCenter.Name);
            Assert.AreEqual(expectedTimezone, testChangeLocal.CallCenter.LocalTimezoneId);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void ChangeLocal_CorrectTimezoneIsGiven_TimezoneCacheIsCleaned()
        {
            var test = new TestChangeLocalRenewCache(1);
            var manager = new CachedLocalTimezoneManager(test, test, test, new StubITimezoneService());

            var expectedTimezoneId = 5;
            manager.ChangeLocal(expectedTimezoneId);
            var tzAfterChange = manager.GetLocalTimezone();

            Assert.AreEqual(expectedTimezoneId, tzAfterChange.ID);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetLocalTimezone_FirstCall_TimezoneAreTakenFromProvider()
        {
            const int expectedLocalTimezoneId = 1;
            var testGetLocalTimezone = new TestGetLocalTimezone(expectedLocalTimezoneId);
            var manager = new CachedLocalTimezoneManager(testGetLocalTimezone, new CallCenterRepository(),
                                                         testGetLocalTimezone, new StubITimezoneService());
            var result = manager.GetLocalTimezone();
            Assert.AreEqual(expectedLocalTimezoneId, result.ID);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetLocalTimezone_TwoSequentialCalls_SecondCallReturnsTzFromCache()
        {
            const int expectedLocalTimezoneId = 1;
            var testGetLocalTimezone = new TestGetLocalTimezone(expectedLocalTimezoneId);
            var manager = new CachedLocalTimezoneManager(testGetLocalTimezone, new CallCenterRepository(),
                                                         testGetLocalTimezone, new StubITimezoneService());
            manager.GetLocalTimezone();
            Assert.AreEqual(expectedLocalTimezoneId + 1, testGetLocalTimezone.Counter, "Counter object is incorrect, seems test is broken.");
            var result = manager.GetLocalTimezone();

            Assert.AreEqual(expectedLocalTimezoneId, result.ID);
        }
    }
}
