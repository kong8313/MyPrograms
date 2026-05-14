using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.IntegrationTests.Framework;
using System.Data.SqlClient;

using Microsoft.SqlServer.Management.Smo;

using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Common;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

namespace Confirmit.CATI.IntegrationTests.Tests.Scheduling
{
    [TestClass]
    public class SchedulingProcedureTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;

        private ISurveyStateService _surveyStateService;

        [TestInitialize]
        public void Init()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);

            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();

            WrapUpSchedulingProcedure();

            TimezoneManager.AddTimezone(51);

            var shifts = new object[]{ 
                new Shift(1, 1, new ShiftTimezone(null, "0.10:00:00", "0.11:00:00")),
                new Shift(2, 2, new ShiftTimezone(null, "0.12:00:00", "0.13:00:00")),
                new Shift(3, 3, new ShiftTimezone(null, "0.14:00:00", "0.15:00:00"), new ShiftTimezone(51, "0.06:00:00", "0.07:00:00")),
                new Shift(4, 4, new ShiftTimezone(null, "0.16:00:00", "0.17:00:00"))};

            var script = new TestScript(
                new SubRule(new Action(Action.Operation.SetNewITS, "24")){ ShiftTypeId = 5}, shifts);

            _surveyId = _backendTools.CreateSurvey(script, ProjectID);
            _surveyStateService.Open(_surveyId);
            _personId = PersonTools.CreatePerson("u1", "p1", AgentTaskChoiceMode.CampaignAssignment);
            BackendTools.AssignCatiPersonToSurvey(_surveyId, _personId);
            BackendTools.LoginPerson(_personId, "");
            PersonService.LoginPersonOnSurveyForSurveySelectionMode(_personId, _surveyId);

            //emulate CF table where data for sample is stored
            ConfirmitTools.CreateRespondentTable(_framework.DbEngine);
            const int startRespId = 1;
            const int count = 10;
            _backendTools.AddSample(ProjectID, 1, 2, startRespId, count, null);

            int dbShiftTypeID = SurveyManager.GetShiftTypes(_surveyId).Find(x => x.Id == 1).ObjectId;
            CallTools.ChangeCallsShiftType(_surveyId, new[] { 1, 2 }, CallStates.Scheduled, dbShiftTypeID);
            dbShiftTypeID = SurveyManager.GetShiftTypes(_surveyId).Find(x => x.Id == 2).ObjectId;
            CallTools.ChangeCallsShiftType(_surveyId, new[] { 3, 4 }, CallStates.Scheduled, dbShiftTypeID);
            dbShiftTypeID = SurveyManager.GetShiftTypes(_surveyId).Find(x => x.Id == 3).ObjectId;
            CallTools.ChangeCallsShiftType(_surveyId, new[] { 5, 6 }, CallStates.Scheduled, dbShiftTypeID);
            dbShiftTypeID = SurveyManager.GetShiftTypes(_surveyId).Find(x => x.Id == 4).ObjectId;
            CallTools.ChangeCallsShiftType(_surveyId, new[] { 7, 8 }, CallStates.Scheduled, dbShiftTypeID);
            CallTools.ChangeCallsShiftType(_surveyId, new[] { 9, 10 }, CallStates.Scheduled, (int)CallShiftType.AnyValid);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _framework.TestCleanup();
        }

        
        const string ProjectID = "p123921";
        int _surveyId;
        int _personId;

        private void WrapUpSchedulingProcedure()
        {
            const string additionalTableName = @"TransferTime";
            const string storedProcedureName = @"BvSpQueueUpSheduleTask3";
            string wrapUp = String.Format(@"CREATE PROCEDURE {0}
     @NowUTC           datetime,
     @DefaultTz        int,
     @TzBalancingThreshold INT=0
  AS
    DECLARE @date DATETIME
    SELECT @date = [time] FROM {1}
    SET @date = ISNULL(@date, @NowUTC)

    EXEC {0}_Source @date, @DefaultTz

    TRUNCATE TABLE {1}", storedProcedureName, additionalTableName);

            _framework.DbEngine.CreateTable(additionalTableName, new[]{ new KeyValuePair<string, DataType>("time", DataType.DateTime) });

            using (var connection = new SqlConnection(_framework.DbEngine.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(String.Format(
                    @"exec sp_rename '{0}', '{0}_Source'",
                    storedProcedureName), connection))
                {
                    command.ExecuteNonQuery();

                    command.CommandText = wrapUp;

                    command.ExecuteNonQuery();
                }
            }
        }

        private void PrepareTimeForScheduling(DateTime time)
        {
            using (var connection = new SqlConnection(_framework.DbEngine.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("INSERT INTO TransferTime VALUES(@time)", connection))
                {
                    command.Parameters.Add(new SqlParameter("@time", time));
                    command.ExecuteNonQuery();
                }
            }
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SchedulingProcedure_Test1()
        {
            var dt = new DateTime(2010, 2, 21, 10, 1, 0);
            PrepareTimeForScheduling(dt);

            BackendTools.RunSchedulingProcedure();

            var expectedCallIds = new[] { 1, 2, 9, 10 };

            TestAssert.AreEqual(expectedCallIds, expectedCallIds.Select(x => (int)TaskService.LookupByPersonSid(_personId, _surveyId).CallID));
            Assert.IsNull(TaskService.LookupByPersonSid(_personId, _surveyId));

            _surveyStateService.CloseSurvey(_surveyId);

            dt = new DateTime(2010, 2, 21, 12, 1, 0);
            PrepareTimeForScheduling(dt);

            BackendTools.RunSchedulingProcedure();
            Assert.IsNull(TaskService.LookupByPersonSid(_personId, _surveyId));

            _surveyStateService.Open(_surveyId);

            dt = new DateTime(2010, 2, 21, 12, 1, 0);
            PrepareTimeForScheduling(dt);

            BackendTools.RunSchedulingProcedure();

            expectedCallIds = new[] { 3, 4 };
            TestAssert.AreEqual(expectedCallIds, expectedCallIds.Select(x => (int)TaskService.LookupByPersonSid(_personId, _surveyId).CallID));
            Assert.IsNull(TaskService.LookupByPersonSid(_personId, _surveyId));

            var timeZones = new[] {51};
            const int count = 1;
            const int startRespId = 11;
            _backendTools.AddSample(ProjectID, 2, 2, startRespId, count, timeZones);
            int dbShiftTypeID = SurveyManager.GetShiftTypes(_surveyId).Find(x => x.Id == 3).ObjectId;
            CallTools.ChangeCallsShiftType(_surveyId, new[] { 11 }, CallStates.Scheduled, dbShiftTypeID);

            dt = new DateTime(2010, 2, 21, 7, 1, 0);
            PrepareTimeForScheduling(dt);

            BackendTools.RunSchedulingProcedure();

            expectedCallIds = new int[] { 11 };
            TestAssert.AreEqual(expectedCallIds, expectedCallIds.Select(x => (int)TaskService.LookupByPersonSid(_personId, _surveyId).CallID));
            Assert.IsNull(TaskService.LookupByPersonSid(_personId, _surveyId));
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SchedulingProcedure_Test2()
        {
            CallQueueService.DeleteCall(_surveyId, 9);
            CallQueueService.DeleteCall(_surveyId, 10);

            int dbShiftTypeID = SurveyManager.GetShiftTypes(_surveyId).Find(x => x.Id == 4).ObjectId;
            CallTools.ChangeCallsShiftType(_surveyId, new[] { 1 }, CallStates.Scheduled, dbShiftTypeID);

            var dt = new DateTime(2010, 2, 21, 10, 1, 0);
            PrepareTimeForScheduling(dt);

            BackendTools.RunSchedulingProcedure();

            var expectedCallIds = new[] { 2 };

            TestAssert.AreEqual(expectedCallIds, expectedCallIds.Select(x => (int)TaskService.LookupByPersonSid(_personId, _surveyId).CallID));
            Assert.IsNull(TaskService.LookupByPersonSid(_personId, _surveyId));

            dt = new DateTime(2010, 2, 21, 16, 1, 0);
            PrepareTimeForScheduling(dt);
            BackendTools.RunSchedulingProcedure();

            dt = new DateTime(2010, 2, 21, 14, 1, 0);
            PrepareTimeForScheduling(dt);
            BackendTools.RunSchedulingProcedure();

            expectedCallIds = new[] { 5, 6 };

            TestAssert.AreEqual(expectedCallIds, expectedCallIds.Select(x => (int)TaskService.LookupByPersonSid(_personId, _surveyId).CallID));
            Assert.IsNull(TaskService.LookupByPersonSid(_personId, _surveyId));

            const int startRespId = 11;
            const int count = 2;
            _backendTools.AddSample(ProjectID, 2, 2, startRespId, count, null);

            dt = new DateTime(2010, 2, 21, 10, 1, 0);
            PrepareTimeForScheduling(dt);
            BackendTools.RunSchedulingProcedure();

            dbShiftTypeID = SurveyManager.GetShiftTypes(_surveyId).Find(x => x.Id == 1).ObjectId;
            CallTools.ChangeCallsShiftType(_surveyId, new[] { 11, 12 }, CallStates.Scheduled, dbShiftTypeID);

            dt = new DateTime(2010, 2, 21, 16, 1, 0);
            PrepareTimeForScheduling(dt);
            BackendTools.RunSchedulingProcedure();

            expectedCallIds = new[] { 1, 7, 8 };

            TestAssert.AreEqual(expectedCallIds, expectedCallIds.Select(x => (int)TaskService.LookupByPersonSid(_personId, _surveyId).CallID));
            Assert.IsNull(TaskService.LookupByPersonSid(_personId, _surveyId));

            dt = new DateTime(2010, 2, 21, 10, 1, 0);
            PrepareTimeForScheduling(dt);
            BackendTools.RunSchedulingProcedure();

            expectedCallIds = new[] { 11, 12 };

            TestAssert.AreEqual(expectedCallIds, expectedCallIds.Select(x => (int)TaskService.LookupByPersonSid(_personId, _surveyId).CallID));
            Assert.IsNull(TaskService.LookupByPersonSid(_personId, _surveyId));
        }
    }
}
