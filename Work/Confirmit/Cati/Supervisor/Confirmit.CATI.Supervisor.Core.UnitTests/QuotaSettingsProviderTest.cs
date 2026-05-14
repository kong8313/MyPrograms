using System.Collections.Generic;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Core.Misc.CP.Fakes;
using Confirmit.CATI.Supervisor.Core.Quotas;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings.Fakes;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings.Quota;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Supervisor.Core.UnitTests
{
    [TestClass]
    public class QuotaSettingsProviderTest
    {
        private IServiceRegistrator _serviceRegistrator;

        [TestInitialize]
        public void Init()
        {
            var sl = new ServiceLocator();
            sl.Cleanup();
            sl.Initialize();

            _serviceRegistrator = ServiceLocator.Resolve<IServiceRegistrator>();
            _serviceRegistrator.RegisterInstance<ISupervisorNameProvider>(new StubISupervisorNameProvider());
            _serviceRegistrator.Register<IQuotaSettingsProvider, QuotaSettingsProvider>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            new ServiceLocator().Cleanup();
        }

        [TestMethod, Owner(@"FIRM\VictorR")]
        public void UpdateAndGetSettings_AddNewQuota_SettingsWereUpdated()
        {
            var settings = new QuotaPageViewSettings()
            {
                QuotasOrder = new List<string> {"quota1", "quota2", "quota3"},
                QuotasExclusion = new List<string> {"quota2"},
                NumberOfColumns = 3
            };

            var stubRepository = new StubISupervisorSettingsRepository
            {
                ReadQuotaSettingsInt32 = delegate(int i) { return settings; },
                WriteQuotaSettingsInt32QuotaPageViewSettings = (id, viewSettings) => { settings = viewSettings; }
            };

            var stubProvider = new StubIQuotaNameProvider
            {
                Quotas = new List<string> { "quota1", "quota2", "quota3", "newQuota" }
            };

            _serviceRegistrator.RegisterInstance<ISupervisorSettingsRepository>(stubRepository);
            _serviceRegistrator.RegisterInstance<IQuotaNameProvider>(stubProvider);

            // act
            ServiceLocator.Resolve<IQuotaSettingsProvider>().UpdateAndGetSettings(12);

            //assert
            Assert.IsTrue(settings.QuotasOrder.Count == 4);
            Assert.AreEqual("quota1", settings.QuotasOrder[0]);
            Assert.AreEqual("quota2", settings.QuotasOrder[1]);
            Assert.AreEqual("quota3", settings.QuotasOrder[2]);
            Assert.AreEqual("newQuota", settings.QuotasOrder[3]);
        }     
        
        [TestMethod, Owner(@"FIRM\VictorR")]
        public void UpdateAndGetSettings_SettingIsActual_SettingsWereNotUpdated()
        {
            var settings = new QuotaPageViewSettings
            {
                QuotasOrder = new List<string> {"quota1", "quota2", "quota3"},
                QuotasExclusion = new List<string> {"quota2"},
                NumberOfColumns = 3
            };

            var stubRepository = new StubISupervisorSettingsRepository
            {
                ReadQuotaSettingsInt32 = delegate (int i) { return settings; },
                WriteQuotaSettingsInt32QuotaPageViewSettings = (id, viewSettings) => { settings = viewSettings; }
            };

            var stubProvider = new StubIQuotaNameProvider
            {
                Quotas = new List<string> {"quota1", "quota2", "quota3"}
            };

            _serviceRegistrator.RegisterInstance<ISupervisorSettingsRepository>(stubRepository);
            _serviceRegistrator.RegisterInstance<IQuotaNameProvider>(stubProvider);

            // act
            ServiceLocator.Resolve<IQuotaSettingsProvider>().UpdateAndGetSettings(12);

            // assert
            Assert.AreEqual("quota1", settings.QuotasOrder[0]);
            Assert.AreEqual("quota2", settings.QuotasOrder[1]);
            Assert.AreEqual("quota3", settings.QuotasOrder[2]);
        }  
        
        [TestMethod, Owner(@"FIRM\VictorR")]
        public void NeedToUpdate_AddNewQuota_True()
        {
            // arrange
            var currentSettings = new QuotaPageViewSettings
            {
                QuotasOrder = new List<string> { "quota1", "quota2", "quota3" },
                QuotasExclusion = new List<string> { "quota2" },
                NumberOfColumns = 3
            };
            var actualQuotaList = new List<string> { "quota1", "quota2", "quota3", "newQuota" };

            // act
            var needToUpdate = QuotaSettingsProvider.NeedToUpdate(currentSettings, actualQuotaList);

            // assert
            Assert.IsTrue(needToUpdate);
        }    
        
        [TestMethod, Owner(@"FIRM\VictorR")]
        public void NeedToUpdate_RemoveQuota_True()
        {        
            // arrange
            var currentSettings = new QuotaPageViewSettings
            {
                QuotasOrder = new List<string> { "quota1", "quota2", "quota3" },
                QuotasExclusion = new List<string> { "quota2" },
                NumberOfColumns = 3
            };
            var actualQuotaList = new List<string> { "quota1", "quota2"};

            // act
            var needToUpdate = QuotaSettingsProvider.NeedToUpdate(currentSettings, actualQuotaList);

            // assert
            Assert.IsTrue(needToUpdate);
        }    
        
        [TestMethod, Owner(@"FIRM\VictorR")]
        public void NeedToUpdate_ChangedOrderingOnly_False()
        {
            // arrange
            var currentSettings = new QuotaPageViewSettings
            {
                QuotasOrder = new List<string> { "quota1", "quota3", "quota2" },
                QuotasExclusion = new List<string> { "quota2" },
                NumberOfColumns = 3
            };
            var actualQuotaList = new List<string> { "quota1", "quota2", "quota3" };

            // act
            var needToUpdate = QuotaSettingsProvider.NeedToUpdate(currentSettings, actualQuotaList);

            // assert
            Assert.IsFalse(needToUpdate);
        }    
        
        [TestMethod, Owner(@"FIRM\VictorR")]
        public void SetActualSettings_AddNewQuota_SettingsBecameActual()
        {
            // arrange
            var settings = new QuotaPageViewSettings
            {
                QuotasOrder = new List<string> { "quota1", "quota3" },
                QuotasExclusion = new List<string> { "quota2" },
                NumberOfColumns = 3
            };
            var actualQuotaList = new List<string> { "quota1", "quota2", "quota3" };

            // act
            QuotaSettingsProvider.SetActualSettings(settings, actualQuotaList);

            // assert
            Assert.IsTrue(settings.QuotasOrder.Count == 3);
            Assert.AreEqual("quota1", settings.QuotasOrder[0]);
            Assert.AreEqual("quota3", settings.QuotasOrder[1]);
            Assert.AreEqual("quota2", settings.QuotasOrder[2]);
        }    
    
        [TestMethod, Owner(@"FIRM\VictorR")]
        public void SetActualSettings_RemoveQuota_SettingsBecameActual()
        {      
            // arrange
            var settings = new QuotaPageViewSettings
            {
                QuotasOrder = new List<string> { "quota1", "quota3", "quota2" },
                QuotasExclusion = new List<string> { "quota2" },
                NumberOfColumns = 3
            };
            var actualQuotaList = new List<string> { "quota2", "quota1" };

            // act
            QuotaSettingsProvider.SetActualSettings(settings, actualQuotaList);

            // assert
            Assert.IsTrue(settings.QuotasOrder.Count == 2);
            Assert.AreEqual("quota1", settings.QuotasOrder[0]);
            Assert.AreEqual("quota2", settings.QuotasOrder[1]);
        }

        private class StubIQuotaNameProvider : IQuotaNameProvider
        {
            public IEnumerable<string> Quotas { get; set; }

            public IEnumerable<string> GetQuotaNames(int surveySid)
            {
                return Quotas;
            }
        }
    }
}
