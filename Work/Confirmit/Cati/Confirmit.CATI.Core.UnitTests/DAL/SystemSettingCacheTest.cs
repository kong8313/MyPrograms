using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Cache;
using Confirmit.CATI.Core.IpLockDown.IPFilterInspectors;
using Confirmit.CATI.Core.Misc.Fakes;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.DAL
{
    [TestClass]
    public class SystemSettingCacheTest
    {
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void DetectChanges_NoChanges_NothingWasFound()
        {
            var systemSettingCache = new SystemSettingCache(new IpFilterCache(), new StubISqlTableUpdatedPublisher(), new StubICompanyInfo());

            var prevSettings = new Dictionary<string, BvSystemSettingsEntity>
            {
                { "1", new BvSystemSettingsEntity { SystemName = "1", Value = "1" } },
                { "2", new BvSystemSettingsEntity { SystemName = "2", Value = "2" } },
                { "3", new BvSystemSettingsEntity { SystemName = "3", Value = "3" } }
            };
            var curSettings = new Dictionary<string, BvSystemSettingsEntity>
            {
                { "1", new BvSystemSettingsEntity { SystemName = "1", Value = "1" } },
                { "2", new BvSystemSettingsEntity { SystemName = "2", Value = "2" } },
                { "3", new BvSystemSettingsEntity { SystemName = "3", Value = "3" } }
            };

            ChangedSettingsCollector changedSettingsCollector = systemSettingCache.DetectChanges(prevSettings, curSettings);
            Assert.AreEqual(0, changedSettingsCollector.TotalChangesCount, "DetectChanges method detect inexistent changes");
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void DetectChanges_TwoValuesWereChanged_CorrectInformationWasReturned()
        {
            var systemSettingCache = new SystemSettingCache(new IpFilterCache(), new StubISqlTableUpdatedPublisher(), new StubICompanyInfo());

            var prevSettings = new Dictionary<string, BvSystemSettingsEntity>
            {
                { "1", new BvSystemSettingsEntity { SystemName = "1", Value = "1" } },
                { "2", new BvSystemSettingsEntity { SystemName = "2", Value = "2" } },
                { "3", new BvSystemSettingsEntity { SystemName = "3", Value = "3" } }
            };
            var curSettings = new Dictionary<string, BvSystemSettingsEntity>
            {
                { "1", new BvSystemSettingsEntity { SystemName = "1", Value = "1" } },
                { "2", new BvSystemSettingsEntity { SystemName = "22", Value = "22" } },
                { "3", new BvSystemSettingsEntity { SystemName = "33", Value = "33" } }
            };

            ChangedSettingsCollector changedSettingsCollector = systemSettingCache.DetectChanges(prevSettings, curSettings);
            Assert.AreEqual(2, changedSettingsCollector.TotalChangesCount, "DetectChanges method detect incorrect count of total changes");
            Assert.AreEqual(2, changedSettingsCollector.ChangedSettings.Count, "DetectChanges method detect incorrect count of changed settings");
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void DetectChanges_TwoValuesWereAdded_CorrectInformationWasReturned()
        {
            var systemSettingCache = new SystemSettingCache(new IpFilterCache(), new StubISqlTableUpdatedPublisher(), new StubICompanyInfo());

            var prevSettings = new Dictionary<string, BvSystemSettingsEntity>
            {
                { "1", new BvSystemSettingsEntity { SystemName = "1", Value = "1" } },
                { "2", new BvSystemSettingsEntity { SystemName = "2", Value = "2" } }
            };
            var curSettings = new Dictionary<string, BvSystemSettingsEntity>
            {
                { "1", new BvSystemSettingsEntity { SystemName = "1", Value = "1" } },
                { "2", new BvSystemSettingsEntity { SystemName = "2", Value = "2" } },
                { "3", new BvSystemSettingsEntity { SystemName = "3", Value = "3" } },
                { "4", new BvSystemSettingsEntity { SystemName = "4", Value = "4" } }
            };

            ChangedSettingsCollector changedSettingsCollector = systemSettingCache.DetectChanges(prevSettings, curSettings);
            Assert.AreEqual(2, changedSettingsCollector.TotalChangesCount, "DetectChanges method detect incorrect count of total changes");
            Assert.AreEqual(2, changedSettingsCollector.AddedSettings.Count, "DetectChanges method detect incorrect count of added settings");
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void DetectChanges_TwoValuesWereRemoved_CorrectInformationWasReturned()
        {
            var systemSettingCache = new SystemSettingCache(new IpFilterCache(), new StubISqlTableUpdatedPublisher(), new StubICompanyInfo());

            var prevSettings = new Dictionary<string, BvSystemSettingsEntity>
            {
                { "1", new BvSystemSettingsEntity { SystemName = "1", Value = "1" } },
                { "2", new BvSystemSettingsEntity { SystemName = "2", Value = "2" } },
                { "3", new BvSystemSettingsEntity { SystemName = "3", Value = "3" } },
                { "4", new BvSystemSettingsEntity { SystemName = "4", Value = "4" } }
            };
            var curSettings = new Dictionary<string, BvSystemSettingsEntity>
            {
                { "1", new BvSystemSettingsEntity { SystemName = "1", Value = "1" } },
                { "2", new BvSystemSettingsEntity { SystemName = "2", Value = "2" } }
            };

            ChangedSettingsCollector changedSettingsCollector = systemSettingCache.DetectChanges(prevSettings, curSettings);
            Assert.AreEqual(2, changedSettingsCollector.TotalChangesCount, "DetectChanges method detect incorrect count of total changes");
            Assert.AreEqual(2, changedSettingsCollector.RemovedSettings.Count, "DetectChanges method detect incorrect count of removed settings");
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void DetectChanges_OneValueWasAddedOneWasRemovedAndOneWasChanged_CorrectInformationWasReturned()
        {
            var systemSettingCache = new SystemSettingCache(new IpFilterCache(), new StubISqlTableUpdatedPublisher(), new StubICompanyInfo());

            var prevSettings = new Dictionary<string, BvSystemSettingsEntity>
            {
                { "1", new BvSystemSettingsEntity { SystemName = "1", Value = "1" } },
                { "2", new BvSystemSettingsEntity { SystemName = "2", Value = "2" } },
                { "3", new BvSystemSettingsEntity { SystemName = "3", Value = "3" } }
            };
            var curSettings = new Dictionary<string, BvSystemSettingsEntity>
            {
                { "1", new BvSystemSettingsEntity { SystemName = "1", Value = "1" } },
                { "2", new BvSystemSettingsEntity { SystemName = "22", Value = "22" } },                
                { "4", new BvSystemSettingsEntity { SystemName = "4", Value = "4" } }
            };

            ChangedSettingsCollector changedSettingsCollector = systemSettingCache.DetectChanges(prevSettings, curSettings);
            Assert.AreEqual(3, changedSettingsCollector.TotalChangesCount, "DetectChanges method detect incorrect count of total changes");
            Assert.AreEqual(1, changedSettingsCollector.ChangedSettings.Count, "DetectChanges method detect incorrect count of changed settings");
            Assert.AreEqual(1, changedSettingsCollector.AddedSettings.Count, "DetectChanges method detect incorrect count of added settings");
            Assert.AreEqual(1, changedSettingsCollector.RemovedSettings.Count, "DetectChanges method detect incorrect count of removed settings");
        }
    }
}