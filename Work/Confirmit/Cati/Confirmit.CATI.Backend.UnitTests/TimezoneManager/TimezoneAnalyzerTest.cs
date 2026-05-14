using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Backend.TimezoneManager;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Backend.UnitTests.TimezoneManager
{
    [TestClass]
    public class TimezoneAnalyzerTest
    {
        [TestMethod, Owner(@"FIRM\LiubovK")]
        public void GenerateIdForSystemTimezones_CheckGenerateCorrectness()
        {
            var timezoneAnalyzer = new TimeZoneAnalyzer();
            var testSystemTimezones = new List<BvTimezoneEntity>()
            {
                new BvTimezoneEntity {StandardName = "timezone1", Name = "(UTC+00:00) test1"}, 
                new BvTimezoneEntity {StandardName = "timezone2", Name = "(UTC+00:00) test2"},
            };

            var testMasterTimezones = new List<BvTimezoneEntity>{ new BvTimezoneEntity { ID = 1, StandardName = "timezone1" } };

            timezoneAnalyzer.GenerateIdForSystemTimezones(testSystemTimezones, testMasterTimezones);

            var firstTimezone = testSystemTimezones.FirstOrDefault(x => x.StandardName == "timezone1");
            var secondTimezone = testSystemTimezones.FirstOrDefault(x => x.StandardName == "timezone2");
            Assert.AreEqual(firstTimezone.ID, 1);
            Assert.AreEqual(secondTimezone.ID, 2);
            Assert.AreEqual(firstTimezone.Name, "(GMT+00:00) test1");
            Assert.AreEqual(secondTimezone.Name, "(GMT+00:00) test2");
        }
    }
}
