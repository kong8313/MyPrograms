using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Supervisor.Script.Classes;

namespace Confirmit.CATI.IntegrationTests.Tests.Scheduling
{
    [TestClass]
    public class ScheduleServiceTests
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;
        private IScheduleService _scheduleService;

        [TestInitialize]
        public void Init()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);
            _scheduleService = ServiceLocator.Resolve<IScheduleService>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _framework.TestCleanup();
        }

        

        [TestMethod, Owner(@"FIRM\LiubovK")]
        public void CopySchedulingScriptToDefault_CheckIfScriptCopiedSuccessfully()
        {
            var schedulingScript = new BvScheduleEntity
            {
                XmlUnderDev = "<?xml version=\"1.0\"?>\r\n<Schedule xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
                              "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  " +
                              "<Id xsi:nil=\"true\" />\r\n  <Name />\r\n  <Rules />\r\n" +
                              "  <ShiftTypes />\r\n  <Shifts />\r\n  <Exclusions />\r\n " +
                              " <CustomParameters />\r\n  <CustomScript />\r\n</Schedule>",
                Name = "newSchedulingScript"
            };
            var schedulingScriptId = ScheduleRepository.Insert(schedulingScript);

            var defaultXmlUnderDevBeforeCopyToDefault = ScheduleRepository.GetById(_scheduleService.DefaultScheduleId).XmlUnderDev;

            _scheduleService.CopySchedulingScriptToDefault(schedulingScriptId, DateTime.UtcNow);

            var backupSchedulingScript = ScheduleRepository.GetAll().SingleOrDefault(x =>
                x.Name.Contains("backup"));
            
            Assert.IsNotNull(backupSchedulingScript);

            Assert.AreEqual(defaultXmlUnderDevBeforeCopyToDefault, backupSchedulingScript.XmlUnderDev);

            Assert.AreEqual(schedulingScript.XmlUnderDev,
                ScheduleRepository.GetById(_scheduleService.DefaultScheduleId).XmlUnderDev, 
                "Scheduling script has not been copied to default!");
        }

        [TestMethod, Owner(@"FIRM\LiubovK"), ExpectedException(typeof(UserMessageException))]
        public void CopySchedulingScriptToDefault_TryCopySchedulingScriptIntoItself_CheckError()
        {
            _scheduleService.CopySchedulingScriptToDefault(_scheduleService.DefaultScheduleId, DateTime.UtcNow);
        }

        [TestMethod, Owner(@"FIRM\LiubovK")]
        public void DeleteSchedulingScript_CheckIfScriptDeletedSuccessfully()
        {
            var schedulingScript1 = ScheduleManager.AddSchedule("newSchedule1");
            var schedulingScript2 = ScheduleManager.AddSchedule("newSchedule2");

            _scheduleService.DeleteSchedulingScripts(new List<int> {schedulingScript1.ScheduleID, schedulingScript2.ScheduleID});

            Assert.IsNull(ScheduleRepository.GetById(schedulingScript1.ScheduleID), "Scheduling script has not been deleted!");
            Assert.IsNull(ScheduleRepository.GetById(schedulingScript2.ScheduleID), "Scheduling script has not been deleted!");
        }
    }
}
