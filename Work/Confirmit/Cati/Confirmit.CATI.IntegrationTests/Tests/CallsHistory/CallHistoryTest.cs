using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Linq;
using DialType = Confirmit.CATI.Common.DialType;

namespace Confirmit.CATI.IntegrationTests.Tests.CallsHistory
{
    [TestClass]
    public class CallHistoryTest : BaseMockedIntegrationTest
    {

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void Perform3InterviewWithSaveHistoryOptimisation_DataIsWrittenToBvHistoryAndBvCallHistory()
        {
            var context = new TestData()
            {
                Surveys = new[]{ new SurveyData()
                {
                    Tag="S1", IsOpen = true,DialMode = DialingMode.Automatic, SchedulingScript = AllHoursSchedule.Name,
                    Interviews = new[] {
                        new InterviewData(){ Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData(){Resource = "P1"}},
                        new InterviewData(){ Tag="S1.I2", ITS=CallOutcome.FreshSample, Call = new CallData(){Resource = "P1"}},
                        new InterviewData(){ Tag="S1.I3", ITS=CallOutcome.FreshSample, Call = new CallData(){Resource = "P1"}}
                    },
                }},
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();

            console.ProcessAllInterviews(new CompletedInterviewDetails { Its = "13", Status = "Complete" });

            CheckHistoryRecordsAreInCallHistory(survey.Id, context.GetInterview("S1.I1").Id);
            CheckHistoryRecordsAreInCallHistory(survey.Id, context.GetInterview("S1.I2").Id);
            CheckHistoryRecordsAreInCallHistory(survey.Id, context.GetInterview("S1.I3").Id);
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void Perform3InterviewWithDialier_WithSaveHistoryOptimisation_DataIsWrittenToBvHistoryAndBvCallHistory()
        {
            var context = new TestData()
            {
                Surveys = new[]{ new SurveyData()
                {
                    Tag="S1", IsOpen = true,DialMode = DialingMode.Automatic, SchedulingScript = AllHoursSchedule.Name,
                    Interviews = new[] {
                        new InterviewData(){ Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData(){Resource = "P1"}},
                        new InterviewData(){ Tag="S1.I2", ITS=CallOutcome.FreshSample, Call = new CallData(){Resource = "P1"}},
                        new InterviewData(){ Tag="S1.I3", ITS=CallOutcome.FreshSample, Call = new CallData(){Resource = "P1"}}
                    },
                }},
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[] { new DialerData { Tag = "D1" } },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();
            console.LoginToDialer();

            dialer.SetOutcomes(CallOutcome.Connected, CallOutcome.NoReply, CallOutcome.Connected);

            console.ProcessAllInterviews(new CompletedInterviewDetails { Its = "13", Status = "Complete" });

            var history = CheckHistoryRecordsAreInCallHistory(survey.Id, context.GetInterview("S1.I1").Id);
            Assert.AreEqual((byte)CallOutcome.Completed, history.ITS);

            history = CheckHistoryRecordsAreInCallHistory(survey.Id, context.GetInterview("S1.I2").Id);
            Assert.AreEqual((byte)CallOutcome.NoReply, history.ITS);
            Assert.AreEqual((byte)OperationType.NotConnectedCall, history.OperationType);
            Assert.AreEqual((short)CallState.Scheduled, history.CallState);

            history = CheckHistoryRecordsAreInCallHistory(survey.Id, context.GetInterview("S1.I3").Id);
            Assert.AreEqual((byte)CallOutcome.Completed, history.ITS);
        }


        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void CloseCellWhileDoingInterview_TwoRecordsWrittenToCallHistory_BusyAndFilteredByFCD()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData()
                    {
                        SchedulingScript = AllHoursSchedule.Name,
                        Tag = "S1",
                        IsUseDb = true,
                        Forms = new[]
                        {
                            new SingleFormData() {Name = "q1", Precodes = new[] {"1", "2"}}
                        },
                        Quotas = new[]
                        {
                            new QuotaData()
                            {
                                Id = 1, Name = "quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData() {Id = 1, Values = "q1=1", Counter = 0, Limit = 1},
                                    new CellData() {Id = 2, Values = "q1=2", Counter = 0, Limit = 1},
                                }
                            }
                        },
                        Interviews = new[] {
                            new InterviewData() {Tag = "S1.I1",Data = "q1=1",ITS = CallOutcome.FreshSample,Call = new CallData()
                            },
                        },
                        Assigns = new[] {"P1"}
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("quota1");

            var console = context.GetPerson("P1").Login(survey).Start().Wait();
            var interview = console.Interview;

            quota.CloseCellById(1);

            console.WrapUp(new CompletedInterviewDetails { Its = "2", InterviewDuration = 10, Status = "Complete" }).Wait();

            Assert.IsNull(console.Interview);

            var callHistory = BvCallHistoryExAdapter.GetByCondition("SurveyId = @SurveyId AND InterviewID = @InterviewID ORDER BY ID",
                new SqlParameter("@SurveyId", survey.Id), new SqlParameter("@InterviewId", interview.Id));

            Assert.AreEqual(2, callHistory.Count);
            var record = callHistory.First();
            Assert.AreEqual((byte)CallOutcome.Busy, record.ITS);
            Assert.AreEqual((byte)OperationType.Interview, record.OperationType);

            record = callHistory.Last();
            Assert.IsNull(record.ITS);
            Assert.AreEqual((byte)OperationType.DeleteCallsByFcd, record.OperationType);
            Assert.AreEqual((byte)0, record.DialingMode);
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void PersonDialTypeIsManualDialingModeIsPredictive_OnlyDialTypeManualCallsAreDelivered_OnlyManualCallsRecordsAreInCallHistory()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                 {
                     Tag="S1", IsOpen = true, DialMode = DialingMode.Predictive,
                     SchedulingScript = AllHoursSchedule.Name,
                     Interviews = new[] {
                         new InterviewData { Tag="S1.I1", DialType = DialType.Cellphone, ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                         new InterviewData { Tag="S1.I2", DialType = DialType.Landline, ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                         new InterviewData { Tag="S1.I3", DialType = DialType.Cellphone, ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                         new InterviewData { Tag="S1.I4", DialType = DialType.Landline, ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }}
                     },
                 }},

                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment, DialType = DialType.Cellphone } },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();

            var actual = console.ProcessAllInterviews();

            Assert.AreEqual(2, actual.Count());
            var history = CheckHistoryRecordsAreInCallHistory(survey.Id, context.GetInterview("S1.I1").Id);
            Assert.AreEqual((byte)DialType.Cellphone, history.DialTypeId);
            history = CheckHistoryRecordsAreInCallHistory(survey.Id, context.GetInterview("S1.I3").Id);
            Assert.AreEqual((byte)DialType.Cellphone, history.DialTypeId);
        }

        private BvCallHistoryExEntity CheckHistoryRecordsAreInCallHistory(int surveyId, int interviewID)
        {
            var callHistory = BvCallHistoryExAdapter.GetByCondition("SurveyId = @SurveyId AND InterviewID = @InterviewID",
                new SqlParameter("@SurveyId", surveyId), new SqlParameter("@InterviewId", interviewID)).Single();

            var history = BvHistoryAdapter.GetByCondition("SurveyId = @SurveyId AND InterviewID = @InterviewID",
                new SqlParameter("@SurveyId", surveyId), new SqlParameter("@InterviewId", interviewID)).Single();

            return callHistory;
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void InterviewInPreviewMode_TransferInterviewToOtherAgent_CallHistoryAreCorrect()
        {
            var time = new DateTimeMocker("2018-12-07T08:00:00");
            var context = new TestData
            {
                Surveys = new[]{ 
                    new SurveyData{ Tag="S1", IsOpen = true, DialMode = DialingMode.Preview, SchedulingScript = AllHoursSchedule.Name,
                        Interviews = new[] {
                             new InterviewData { Tag = "S1.I1", Call = new CallData { Resource = "PI" }, 
                                 Data = "responsefield=1", },
                             new InterviewData { Tag = "S1.I2", Call = new CallData { Resource = "PI" }, 
                                 Data = "responsefield=2", }
                        }, 
                        Forms = new[]
                        {
                            new SingleFormData() { Name = "responsefield", Precodes = new[] {"1", "2"} },
                        },
                        IsUseDb = true,
                    }},
                PersonGroups = new[] { new PersonGroupData() { Tag = "PG1", Name = "PG1" } },
                Persons = new[]{
                    new PersonData { Tag = "PI", TaskChoice = TaskChoiceMode.SurveyAssignment },
                    new PersonData { Tag = "PT", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG1" }
                },

                Dialers = new[] { new DialerData { Tag = "D1" } },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            
            // Setup custom fields configuration in BvHistoryCustomFields (upsert)
            UpsertCustomField(1, CallHistoryCustomFieldSourceTable.Respondent, "RespondentName");
            UpsertCustomField(2, CallHistoryCustomFieldSourceTable.Respondent, "TelephoneNumber");
            UpsertCustomField(3, CallHistoryCustomFieldSourceTable.Response, "responsefield");

            
            var survey = context.GetSurvey("S1");
            var interview = context.GetInterview("S1.I1");

            // Use ISurveyDatabaseEngine for survey database operations
            var surveyDbEngine = ServiceLocator.Resolve<ISurveyDatabaseEngine>();

            // Set respondent field value
            var overridenRespName = "RespOverriden";
            var overridenTelNumber = "123123123";
            surveyDbEngine.ExecuteNonQuery(survey.Id,
                "UPDATE <Schema>.respondent SET RespondentName = @Value WHERE respID = @RespId",
                new SqlParameter("@Value", overridenRespName),
                new SqlParameter("@RespId", interview.Id));
            surveyDbEngine.ExecuteNonQuery(survey.Id,
                "UPDATE <Schema>.respondent SET TelephoneNumber = @Value WHERE respID = @RespId",
                new SqlParameter("@Value", overridenTelNumber),
                new SqlParameter("@RespId", interview.Id));

            var dialer = context.GetDialer("D1");

            var initiator = context.GetPerson("PI").Console.Login("S1").LoginToDialer();

            time.Pass("0:01:00");

            initiator.Start().Wait();

            time.Pass("0:01:00");

            initiator.Dial().Wait();

            time.Pass("0:01:00");

            var target = context.GetPerson("PT").Console.Login("S1").LoginToDialer().Start().Wait();

            time.Pass("0:03:00");

            initiator.StartTransfer(ConsoleTransferType.InternalCold, "PG1").CompleteTransfer().Wait().Do(console => Assert.AreEqual("S1.I2", console.Interview?.Tag));

            target.Start().Do(x => dialer.Connect(x, "S1.I1")).Wait();

            time.Pass("0:04:00");

            target.WrapUp();

            Assert.AreEqual(
                BackendTools.Format(context, @"
 ID SurveyId TelephoneNumber           FiredTime InterviewId  ITS AppointmentID WaitingTime ConfirmitDuration Duration BatchId PersonSID RoleID CallCenterID OpenEndReviewDuration LinkedInterviewSessionId DisplayTime PreviewTime ConnectedTime WrapTime DialTypeId CallAttemptNumber SessionId       Custom1   Custom2 Custom3 Custom4 Custom5
  1 {    S1}     01234567890 12/07/2018 08:06:00 {    S1.I1} 1010             0          60                 0      300  <NULL>        35      2            1                     0                   <NULL>      <NULL>           0             0        0     <NULL>                 0         0 RespOverriden 123123123       1  <NULL>  <NULL>
  2 {    S1}     01234567890 12/07/2018 08:10:00 {    S1.I1}   13             0         180                 0      240  <NULL>        36      2            1                     0                   <NULL>      <NULL>           0             0        0     <NULL>                 0         0 RespOverriden 123123123       1  <NULL>  <NULL>"),
                BackendTools.Format(BvHistoryAdapter.GetAll()));
        }



        private void UpsertCustomField(int id, CallHistoryCustomFieldSourceTable sourceTable, string sourceFieldName)
        {
            var existingField = BvHistoryCustomFieldsAdapter.GetByCondition("Id = @Id", new SqlParameter("@Id", id)).FirstOrDefault();
            
            if (existingField == null)
            {
                // Insert new record
                var customField = new BvHistoryCustomFieldsEntity
                {
                    Id = id,
                    SourceTable = (int)sourceTable,
                    SourceFieldName = sourceFieldName,
                    IsActive = true
                };
                BvHistoryCustomFieldsAdapter.Insert(customField);
            }
            else
            {
                // Update existing record
                existingField.SourceTable = (int)sourceTable;
                existingField.SourceFieldName = sourceFieldName;
                existingField.IsActive = true;
                BvHistoryCustomFieldsAdapter.Update(existingField);
            }
        }
    }
}
