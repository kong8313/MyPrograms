using System.Data.SqlClient;
using System.Linq;
using BvDotNetEngine;
using BvDotNetEngine.Fakes;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Fakes;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.Test.Common.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class RegenerateScriptTest : BaseMockedIntegrationTest
    {
        private readonly BvInterviewWithOriginEntity _interview;
        private readonly ISurveyStateService _surveyStateService;
        private readonly IScheduleService _scheduleService;
        private readonly IScriptAssembly _scriptAssembly;
        private readonly IInterviewRepository _interviewRepository;
        
        public RegenerateScriptTest()
        {
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            _scheduleService = ServiceLocator.Resolve<IScheduleService>();
            _scriptAssembly = ServiceLocator.Resolve<IScriptAssembly>();
            _interviewRepository = ServiceLocator.Resolve<IInterviewRepository>();

            // Create an interview, schedule it
            BackendToolsObject.LaunchAllHoursScript();
            var surveySid = BackendToolsObject.CreateSurvey("p0000001");
            _surveyStateService.Open(surveySid);
            _interview = new BvInterviewWithOriginEntity(new BvInterviewEntity() { ID = 1, SurveySID = surveySid });
            _interviewRepository.Insert(_interview, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });
        }

        [Theory, Owner(@"FIRM\MikhailT"), Cr(48039)]
        [ClassData(typeof(TestDataGenerator))]
        public void RegenerateIsRequiredFlagIsOn_ExecuteSchedulingScript_ScriptIsRegeneratedAndRecompiled(SecurityMode mode)
        {
            SetSecurityMode(mode);

            // Set RegenerateIsRequired flag
            var scheduleEntity = ScheduleRepository.GetById(BackendTools.GetAllHoursID());
            scheduleEntity.RegenerateIsRequired = true;
            BvScheduleAdapter.Update(scheduleEntity); // we do not use ScheduleRepository.Update here in order not to change scheduleEntity.ModifyDate

            bool reGenerateScriptCalled = false;
            var stubIScheduleService = new StubIScheduleService
            {
                Inner = _scheduleService,
                ReGenerateScriptBvScheduleEntity = entity =>
                {
                    reGenerateScriptCalled = true;
                    _scheduleService.ReGenerateScript(entity);
                }
            };
            ServiceLocator.RegisterInstance<IScheduleService>(stubIScheduleService);

            bool compileCalled = false;
            var stubIScriptAssembly = new StubIScriptAssembly
            {
                Inner = _scriptAssembly,
                CompileScriptAssemblyFileInfoDnScript = (fileInfo, script) =>
                {
                    compileCalled = true;
                    return _scriptAssembly.Compile(fileInfo, script);
                }
            };
            ServiceLocator.RegisterInstance<IScriptAssembly>(stubIScriptAssembly);

            // Schedule the interview again
            var options = new SchedulingScriptExecutionOptions
            {
                ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled,
                IsLogToHistory = false
            };
            new ScheduleScriptExecutor().ScheduleInterview(_interview, options);

            // Ensure Regenerate and compile methods were called
            Assert.IsTrue(reGenerateScriptCalled);
            Assert.IsTrue(compileCalled);
        }

        [Theory, Owner(@"FIRM\MikhailT"), Cr(48039)]
        [ClassData(typeof(TestDataGenerator))]
        public void RegenerateIsRequiredFlagIsOff_ExecuteSchedulingScript_ScriptIsNotRegeneratedAndNotRecompiled(SecurityMode mode)
        {
            SetSecurityMode(mode);

            bool reGenerateScriptCalled = false;
            var stubIScheduleService = new StubIScheduleService
            {
                Inner = _scheduleService,
                ReGenerateScriptBvScheduleEntity = entity =>
                {
                    reGenerateScriptCalled = true;
                    _scheduleService.ReGenerateScript(entity);
                }
            };
            ServiceLocator.RegisterInstance<IScheduleService>(stubIScheduleService);

            bool compileCalled = false;
            var stubIScriptAssembly = new StubIScriptAssembly
            {
                Inner = _scriptAssembly,
                CompileScriptAssemblyFileInfoDnScript = (fileInfo, script) =>
                {
                    compileCalled = true;
                    return _scriptAssembly.Compile(fileInfo, script);
                }
            };
            ServiceLocator.RegisterInstance<IScriptAssembly>(stubIScriptAssembly);

            // Schedule the interview again
            var options = new SchedulingScriptExecutionOptions
            {
                ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled,
                IsLogToHistory = false
            };
            new ScheduleScriptExecutor().ScheduleInterview(_interview, options);

            // // Ensure Regenerate and compile methods were not called
            Assert.IsFalse(reGenerateScriptCalled);
            Assert.IsFalse(compileCalled);
        }

        [Theory, Owner(@"FIRM\MikhailT"), Cr(48039)]
        [ClassData(typeof(TestDataGenerator))]
        public void RegenerateIsRequiredFlagIsOnForTwoSchedulingScripts_ExecuteSchedulingScripts1_Script1IsRegeneratedAndRecompiledScript2Not(SecurityMode mode)
        {
            SetSecurityMode(mode);

            // Let AllHours script be script 1

            // Prepare script 2
            var script2 = new TestScript(
                new Action(Action.Operation.SuspendTheInterview),
                new Shift(1, 1, "0.00:00:00", "1.00:00:00"),
                new Shift(2, 1, "1.00:00:00", "0.00:00:00"));
            script2.Create(null);
            ScheduleService.Launch(script2.ScheduleID);

            // Create a survey and an interview for the second script and schedule it
            var surveySid2 = BackendToolsObject.CreateSurvey("p0000002");
            ScheduleService.Launch(script2.ScheduleID);
            _surveyStateService.Open(surveySid2);
            var interview2 = new BvInterviewWithOriginEntity(new BvInterviewEntity() { ID = 2, SurveySID = surveySid2 });
            _interviewRepository.Insert(interview2, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            // Set RegenerateIsRequired flag for the scripts
            var schedule1Entity = ScheduleRepository.GetById(BackendTools.GetAllHoursID());
            var schedule2Entity = ScheduleRepository.GetById(script2.ScheduleID);
            schedule1Entity.RegenerateIsRequired = true;
            schedule2Entity.RegenerateIsRequired = true;
            BvScheduleAdapter.Update(schedule1Entity); // we do not use ScheduleRepository.Update here in order not to change scheduleEntity.ModifyDate
            BvScheduleAdapter.Update(schedule2Entity);

            // Keep ModifyDate for both schedules
            var schedule1ModifyDateOld = schedule1Entity.ModifyDate;
            var schedule2ModifyDateOld = schedule2Entity.ModifyDate;

            // Schedule the first interview again
            var options = new SchedulingScriptExecutionOptions
            {
                IsLogToHistory = false
            };
            new ScheduleScriptExecutor().ScheduleInterview(_interview, options);

            // Ensure Regenerate was called but only the first schedule was changed.
            schedule1Entity = BvScheduleAdapter.GetByCondition(
                "[ScheduleID] = @ScheduleID\r\n",
                new SqlParameter("@ScheduleID", BackendTools.GetAllHoursID())).FirstOrDefault();

            schedule2Entity = BvScheduleAdapter.GetByCondition(
                "[ScheduleID] = @ScheduleID\r\n",
                new SqlParameter("@ScheduleID", script2.ScheduleID)).FirstOrDefault();

            Assert.IsNotNull(schedule1Entity);
            Assert.IsNotNull(schedule2Entity);
            Assert.AreNotEqual(schedule1ModifyDateOld, schedule1Entity.ModifyDate, "Script1 (AllHours) was not regenerated.");
            Assert.AreEqual(false, schedule1Entity.RegenerateIsRequired, "RegenerateIsRequired was not reset for Script1.");
            Assert.AreEqual(schedule2ModifyDateOld, schedule2Entity.ModifyDate, "Script2 was regenerated.");
            Assert.AreEqual(true, schedule2Entity.RegenerateIsRequired, "RegenerateIsRequired was reset for Script2.");
        }
    }
}
