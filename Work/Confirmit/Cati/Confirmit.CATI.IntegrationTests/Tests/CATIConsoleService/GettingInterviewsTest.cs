using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Tests.Replication;

using ConfirmitDialerInterface;

using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Supervisor.Core.ITSs;
using Confirmit.CATI.Core.Repositories;

namespace Confirmit.CATI.IntegrationTests.Tests.CATIConsoleService
{
    [TestClass]
    public class GettingInterviewsTest
    {
        #region Initialize and Cleanup methods

        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;

        const int PageSize = 20; //count of calls on Page in caticonsole
        // TODO: in v14 we get this value from GetRDProperty("Manual Count")
        // but in v15 we should use alternative method (if such one exists)
        const string GroupName = "group";
        const string OtherGroupName = "othergroup";
        const string User = "APerson";
        const string Password = "password";
        const string ProjectID = "p0123456";

        private CatiWsHelper _serviceHelper;

        private ISurveyStateService _surveyStateService;
        private OrderedSearchableFieldsService _orderedSearchableFieldsService;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            _orderedSearchableFieldsService = ServiceLocator.Resolve<OrderedSearchableFieldsService>();
            
            BackendTools.ResetInterviewId();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        #endregion


        /// <summary>
        /// Prepare data for test
        /// 1. Add survey, launch 'all hours' script, open survey
        /// 2. Create interview with required parameters
        /// 3. Create calls
        /// 4. If use group functionality - create person groups and assign interviews to groups
        /// 5. Login for user
        /// </summary>
        /// <param name="callsCount">Count of calls</param>
        /// <param name="respondentsName">Name of respondents</param>
        /// <param name="telephoneNumber">Telephone numbers</param>
        /// <param name="useGroupFunctionality">true - for use group functionality</param>
        /// <param name="allInterviews"></param>
        private int PrepareDataForTest(int callsCount,
                                        string[] respondentsName,
                                        string[] telephoneNumber,
                                        bool useGroupFunctionality,
                                        IList<BvInterviewEntity> allInterviews)
        {
            int personSID;

            int surveySID = PrepareSurveyWithCalls(callsCount, respondentsName, telephoneNumber, allInterviews);

            if (useGroupFunctionality)
            {
                Assert.IsTrue(callsCount > 7, "Here should be more than 7 calls for correct work");

                int groupID = PersonTools.CreatePersonGroup(GroupName);
                int otherGroupID = PersonTools.CreatePersonGroup(OtherGroupName);

                personSID = PersonTools.CreatePerson(User, Password, AgentTaskChoiceMode.Manual, new[] { groupID });

                BackendTools.AssignResourceToInterview(surveySID, allInterviews[callsCount - 2].ID, personSID);
                BackendTools.AssignResourceToInterview(surveySID, allInterviews[callsCount - 4].ID, groupID);
                BackendTools.AssignResourceToInterview(surveySID, allInterviews[callsCount - 6].ID, otherGroupID);
            }
            else
            {
                personSID = PersonTools.CreatePerson(User, Password, AgentTaskChoiceMode.Manual);
            }

            BackendTools.AssignCatiPersonToSurvey(surveySID, personSID);

            _serviceHelper = new CatiWsHelper(User, Password);

            PersonInfo personInfo;
            DiallerInfo diallerInfo;
            CatiConsolePropertiesContainer outProperties;
            var stationId = string.Empty;

            var consoleDescriptor = new ConsoleDescription();

            _serviceHelper.ConsoleService.Login(
                stationId,
                consoleDescriptor, 
                out personInfo,
                out diallerInfo,
                out outProperties);

            BackendTools.LoginPerson(personSID, "");

            // Set ManualCount from BvRDProperty table to PageSize (20)
            ServiceLocator.Resolve<ISystemSettings>().Console.InterviewsCountShownInManualMode = PageSize;

            return surveySID;
        }

        private int PrepareSurveyWithCalls(int callsCount, string[] respondentsName, string[] telephoneNumber, ICollection<BvInterviewEntity> allInterviews)
        {
            int surveySID = _backendTools.CreateSurvey(ProjectID);
            _backendTools.LaunchAllHoursScript();
            _surveyStateService.Open(surveySID);

            for (short i = 0; i < callsCount; ++i)
            {
                BvInterviewEntity interview = BackendTools.NewInterview(surveySID);
                if (respondentsName != null)
                    interview.RespondentName = respondentsName[i];
                if (telephoneNumber != null)
                    interview.TelephoneNumber = telephoneNumber[i];
                allInterviews.Add(interview);
                BackendTools.CreateInterview(interview);

                BvCallEntity call = BackendTools.NewCall(interview);
                call.CallState = 2;
                call.Priority = (short)(i + 1);
                BackendTools.CreateCall(call);
            }

            return surveySID;
        }

        //we havn't priority for InterviewControlData, but
        //id is the same as priority that is why we can use sorting by id. (see PrepareDataForTest method)
        private static bool IsDescOrdered(DataTable table)
        {
            for (int i = 1; i < table.Rows.Count; ++i)
            {
                if ((int)table.Rows[i][0] > (int)table.Rows[i - 1][0])
                    return false;
            }

            return true;
        }

        private void CreateAndFillCfResponceControl(int count)
        {
            _framework.DbEngine.ExecuteNonQuery("drop table response_control", CommandType.Text);

            _framework.DbEngine.CreateTableWithPrimaryKey("response_control", new[]
            {                
                new KeyValuePair<string, DataType>("respid", DataType.Int),
                new KeyValuePair<string, DataType>("responseid", DataType.Int),
                new KeyValuePair<string, DataType>("q1", DataType.NVarCharMax),
                new KeyValuePair<string, DataType>("q2", DataType.NVarCharMax),
                new KeyValuePair<string, DataType>("q3", DataType.Int),
                new KeyValuePair<string, DataType>("q4", DataType.Int),
                new KeyValuePair<string, DataType>("q5", DataType.NVarCharMax)
            },
            new[]
            {
                "responseid"
            });

            _framework.DbEngine.CreateTableWithPrimaryKey("respondent", new[]
            {                
                new KeyValuePair<string, DataType>("respid", DataType.Int),
                new KeyValuePair<string, DataType>("CallAttemptCount", DataType.Int)
            },
            new[]
            {
                "respid"
            });

            const string responseControlQuery = "INSERT INTO response_control" +
                "(respid, responseid, q1, q2, q3, q4, q5) VALUES" +
                "({0}, {0}, '{0}', '{0}', {0}, {0}, '{0}')";

            const string respondentQuery = "INSERT INTO respondent" +
                "(respid, CallAttemptCount) VALUES" +
                "({0}, {1})";

            for (int i = 1; i <= count; i++)
            {
                _framework.DbEngine.ExecuteNonQuery(String.Format(responseControlQuery, i), CommandType.Text);
                _framework.DbEngine.ExecuteNonQuery(String.Format(respondentQuery, i, 4 + i), CommandType.Text);
            }
        }


        /// <summary>
        /// 1. Add survey, launch 'all hours' script, open survey
        /// 2. Create 2 interviews where first interview has phone name '111111' next '222222'
        /// 3. Create 2 call where i-th call has prioritet i
        /// 4. Create person in manual mode
        /// 5. Assign person on Survey
        /// 6. Login person in caticonsole and BE
        /// 7. Call scheduling procedure
        /// 8. Call GetSurveyInterviews without filter information
        /// 
        /// We should take only 2 calls.
        /// We should take calls ordered by prioritet.
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void GettingInterviewsForSurvey_TryToGetFilteredByEmptyStringInterviews_AllInterviewsAreReturned()
        {
            const int callsCount = 2;
            var filter = new SearchParameter[] { };
            var telephoneNumber = new[] { "111111", "222222" };
            var allInterviews = new List<BvInterviewEntity>();

            PrepareDataForTest(callsCount, null, telephoneNumber, false, allInterviews);

            DataTable interviewsControlData = _serviceHelper.ConsoleService.GetSurveyInterviews(
                ProjectID,
                filter);

            Assert.AreEqual(2, interviewsControlData.Rows.Count, "Wrong interviews count");

            int curNum = 2;
            int i = 0;
            foreach (DataRow row in interviewsControlData.Rows)
            {
                Assert.AreEqual(allInterviews[curNum - 1].ID, row[0], "Interview #" + i + " have wrong id");
                curNum--;
                i++;
            }
        }



        /// <summary>
        /// 1.  Add survey, launch 'all hours' script, open survey
        /// 2.  Create 24 interview where first interview has respondent name 'aaz' next
        ///     9 of them have respondent name ('SPI'+ID), next 5 have name (ID+'SPI'+ID)
        ///     next 8 have name (ID+'sPI') and last interview has name ('SIP'+ID+'PIS'+ID+'IPS')
        /// 3.  Create 24 call where i-th call has prioritet i
        /// 4.  Create person in manual mode
        /// 5.  Create group of persons 'g1'
        /// 6.  Create group of persons 'g2'
        /// 7.  Assign person on 'g1'
        /// 8.  Assign person on Survey
        /// 9.  Assign person on 22-th call
        /// 10. Assign 'g1' on 20-th call
        /// 11. Assign 'g2' on 18-th call
        /// 12. Login person in caticonsole and BE
        /// 13. Call scheduling procedure
        /// 14. Call GetSurveyInterviews with filter by RespondentName and value 'SpI'
        /// 
        /// We should not take 22-th call because it assigned on another group.
        /// We should not take 23-th and 1-th calls because theese have name not mathes with 'SpI'
        /// We should take calls ordered by prioritet.
        /// Calls should be returned despite the fact that 'p' in filterd value is small.
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void GettingInterviewsForSurvey_TryToGetFilteredByRespondentNameInterviews_AllInterviewsForFirstPageAreReturned()
        {
            const int callsCount = 24;
            const string filteredValue = "SpI";
            var filter = new[] 
            { 
                new SearchParameter
                {
                    ColumnName="RespondentName", 
                    ColumnTypeName=typeof(string).FullName, 
                    Value=filteredValue 
                }
            };

            var respondentsName = new[] { "aaz", 
                                          "SPI2", "SPI3", "SPI4", "SPI5", "SPI6", "SPI7", "SPI8", "SPI9", "SPI10", 
                                          "11SPI11", "12SPI12", "13SPI13", "14SPI14", "15SPI15", 
                                          "16sPI", "17sPI", "18sPI", "19sPI", "20sPI", "21sPI", "22sPI", "23sPI", 
                                          "SIP24PIS24IPS"};

            var allInterviews = new List<BvInterviewEntity>();

            PrepareDataForTest(callsCount, respondentsName, null, true, allInterviews);

            DataTable interviewsControlData = _serviceHelper.ConsoleService.GetSurveyInterviews(
                ProjectID,
                filter
            );

            int expectedCount = Math.Min(PageSize, callsCount - 2 - 1); //2 records don't contain filteredValue. 1 belong to another person
            Assert.AreEqual(expectedCount, interviewsControlData.Rows.Count, "Count of records in the page is incorrect");

            int count = interviewsControlData.AsEnumerable().Count(
                                        r => r.Field<string>("RespondentName").IndexOf(filteredValue, StringComparison.OrdinalIgnoreCase) != -1 &&
                                        r.Field<int>("InterviewID") != callsCount - 5);

            Assert.AreEqual(expectedCount, count, "Count of obtained records is less then expected");

            Assert.IsTrue(IsDescOrdered(interviewsControlData), "Returned list is not sorted by priority");
        }


        /// <summary>
        /// 1.  Add survey, launch 'all hours' script, open survey
        /// 2.  Create 24 interview where first interview has phone name '111111' next
        ///     9 of them have respondent name ('1230'+ID), next 5 have name (ID+'01230'+ID)
        ///     next 8 have name (ID+'0123') and last interview has name ('132231')
        /// 3.  Create 24 call where i-th call has prioritet i
        /// 4.  Create person in manual mode
        /// 5.  Create group of persons 'g1'
        /// 6.  Create group of persons 'g2'
        /// 7.  Assign person on 'g1'
        /// 8.  Assign person on Survey
        /// 9.  Assign person on 22-th call
        /// 10. Assign 'g1' on 20-th call
        /// 11. Assign 'g2' on 18-th call
        /// 12. Login person in caticonsole and BE
        /// 13. Call scheduling procedure
        /// 14. Call GetSurveyInterviews with filter by RespondentName and value '123'
        /// 
        /// We should not take 22-th call because it assigned on another group.
        /// We should not take 23-th and 1-th calls because theese have name not mathes with '123'
        /// We should take calls ordered by prioritet.
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void GettingInterviewsForSurvey_TryToGetFilteredByTelephoneNumberInterviews_AllInterviewsForFirstPageAreReturned()
        {
            const int callsCount = 24;
            const string filterValue = "123";
            var filter = new SearchParameter
            {
                ColumnName = "TelephoneNumber",
                ColumnTypeName = typeof(string).FullName,
                Value = filterValue
            };

            var telephoneNumber = new[] { "111111", 
                                           "12302", "12303", "12304", "12305", "12306", "12307", "12308", "12309", "123010", 
                                           "110123011", "120123012", "130123013", "140123014", "150123015", 
                                           "160123", "170123", "180123", "190123", "200123", "210123", "220123", "230123", 
                                           "132231"};
            var allInterviews = new List<BvInterviewEntity>();

            PrepareDataForTest(callsCount, null, telephoneNumber, true, allInterviews);

            DataTable interviewsControlData = _serviceHelper.ConsoleService.GetSurveyInterviews(
                ProjectID,
                new[] { filter });

            int expectedCount = Math.Min(PageSize, callsCount - 2 - 1); //2 records don't contain filteredValue. 1 belong to another person
            Assert.AreEqual(expectedCount, interviewsControlData.Rows.Count, "Count of records in the page is incorrect");

            int count = interviewsControlData.AsEnumerable().Count(
                                        r => r.Field<string>("TelephoneNumber").IndexOf(filterValue, StringComparison.OrdinalIgnoreCase) != -1 &&
                                        r.Field<int>("InterviewID") != callsCount - 5);

            Assert.AreEqual(expectedCount, count, "Count of obtained records is less then expected");

            Assert.IsTrue(IsDescOrdered(interviewsControlData), "Returned list is not sorted by priority");
        }

        /// <summary>
        /// 1.  Add survey, launch 'all hours' script, open survey
        /// 2.  Create 24 interviews
        /// 3.  Create person in manual mode
        /// 4.  Assign person on Survey
        /// 5.  Login person in caticonsole and BE        
        /// 6.  Call GetSurveyInterviews 
        /// 7.  Check that 24 records are returned
        /// 8. Change task choice mode to "Automatic"
        /// 9. Check that 0 records are returned
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderZh"), Bug(40572)]
        public void GettingInterviewsForSurvey_TryToGetFilteredInterviewsForPersonInAutomaticMode_EmptyListReturned()
        {
            const int callsCount = 24;

            var allInterviews = new List<BvInterviewEntity>();

            int surveyId = PrepareSurveyWithCalls(callsCount, null, null, allInterviews);

            int personSID = PersonTools.CreatePerson(User, Password, AgentTaskChoiceMode.Manual);

            BackendTools.AssignCatiPersonToSurvey(surveyId, personSID);

            _serviceHelper = new CatiWsHelper(User, Password);

            PersonInfo personInfo;
            DiallerInfo diallerInfo;
            CatiConsolePropertiesContainer outProperties;
            var stationId = string.Empty;

            var consoleDescriptor = new ConsoleDescription();

            _serviceHelper.ConsoleService.Login(
                stationId,
                consoleDescriptor,
                out personInfo,
                out diallerInfo,
                out outProperties);

            DataTable interviewsControlData = _serviceHelper.ConsoleService.GetSurveyInterviews(
               ProjectID,
               new SearchParameter[] { });

            Assert.AreEqual(24, interviewsControlData.Rows.Count, "Wrong interviews count");

            PersonService.ChangeTaskChoice(new[] { personSID }, AgentTaskChoiceMode.Automatic, null, true);

            interviewsControlData = _serviceHelper.ConsoleService.GetSurveyInterviews(
               ProjectID,
               new SearchParameter[] { });

            Assert.AreEqual(0, interviewsControlData.Rows.Count, "Wrong interviews count");

        }


        /// <summary>
        /// 1.  Add survey, launch 'all hours' script, open survey
        /// 2.  Create pageSize+2 interview 
        /// 3.  Create pageSize+2 call where i-th call has prioritet i
        /// 4.  Create person in manual mode
        /// 5.  Create group of persons 'g1'
        /// 6.  Create group of persons 'g2'
        /// 7.  Assign person on 'g1'
        /// 8.  Assign person on Survey
        /// 9.  Assign person on pageSize-1-th call
        /// 10. Assign 'g1' on pageSize-3-th call
        /// 11. Assign 'g2' on pageSize-5-th call
        /// 12. Login person in caticonsole and BE
        /// 13. Call scheduling procedure
        /// 14. Call GetSurveyInterviews
        /// 
        /// We should take only pageSize calls (size of page in cati console).
        /// We should not take pageSize-6-th call because it assigned on another group.
        /// We should take calls ordered by prioritet.
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void GettingInterviewsForSurvey_TryToGetNotFilteredInterviews_AllInterviewsForFirstPageAreReturned()
        {
            const int callsCount = PageSize + 2;

            var filter = new SearchParameter
            {
                ColumnName = "RespondentName",
                ColumnTypeName = typeof(string).FullName,
                Value = ""
            };

            var allInterviews = new List<BvInterviewEntity>();

            PrepareDataForTest(callsCount, null, null, true, allInterviews);

            DataTable interviewsControlData = _serviceHelper.ConsoleService.GetSurveyInterviews(ProjectID,
                new[] { filter });

            //only pageSize calls are returned
            Assert.AreEqual(PageSize, interviewsControlData.Rows.Count, "Count of records in the page is incorrect");

            int count = interviewsControlData.AsEnumerable().Count(
                                        r => r.Field<int>("InterviewID") != callsCount - 5 && //except callsCount - 5. it is for another person
                                        r.Field<int>("InterviewID") <= callsCount &&
                                        r.Field<int>("InterviewID") > callsCount - PageSize - 1);

            Assert.AreEqual(interviewsControlData.Rows.Count, count, "Count of obtained records is less then expected");
        }

        /// <summary>
        /// Checking that filter by interview identifier works.
        /// 
        /// 1.  Add survey, launch 'all hours' script, open survey
        /// 2.  Create 10 interview 
        /// 3.  Create 10 call where i-th call has prioritet i
        /// 4.  Create person in manual mode
        /// 5.  Assign person on Survey
        /// 6.  Assign person on 10 calls
        /// 7. Login person in caticonsole and BE
        /// 8. Call scheduling procedure
        /// 9. Create filter by identifier of 6-th interview.
        /// 10. Call GetSurveyInterviews
        /// 
        /// Method should return single interview with identifier equal to 6-th interview.
        /// </summary>
        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GettingInterviewsForSurvey_TryToGetFilteredByInterviewId_AllInterviewsForFirstPageAreReturned()
        {
            const int callsCount = 10;
            var allInterviews = new List<BvInterviewEntity>();

            PrepareDataForTest(callsCount, null, null, false, allInterviews);
            int expectedId = allInterviews[5].ID;

            var filter = new SearchParameter
            {
                ColumnName = "InterviewID",
                ColumnTypeName = typeof(int).FullName,
                Value = expectedId.ToString(CultureInfo.InvariantCulture)
            };

            DataTable interviewsControlData = _serviceHelper.ConsoleService.GetSurveyInterviews(
                ProjectID,
                new[] { filter });

            Assert.AreEqual(1, interviewsControlData.Rows.Count);
            Assert.AreEqual(expectedId, interviewsControlData.Rows[0]["InterviewID"]);
        }

        /// <summary>
        /// Checking that filter by wrong column name works.
        /// 
        /// 1.  Add survey, launch 'all hours' script, open survey
        /// 2.  Create 10 interview 
        /// 3.  Create 10 call where i-th call has prioritet i
        /// 4.  Create person in manual mode
        /// 5.  Assign person on Survey
        /// 6.  Assign person on 10 calls
        /// 7. Login person in caticonsole and BE
        /// 8. Call scheduling procedure
        /// 9. Create filter with wrong column name.
        /// 10. Call GetSurveyInterviews
        /// 
        /// Method should return all interviews.
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GettingInterviewsForSurvey_TryToGetFilteredByWrongColumnName_AllInterviewsAreReturned()
        {
            const int callsCount = 10;
            var allInterviews = new List<BvInterviewEntity>();

            PrepareDataForTest(callsCount, null, null, false, allInterviews);
            int expectedId = allInterviews[5].ID;

            var filter = new SearchParameter
            {
                ColumnName = "WrongColumnName",
                ColumnTypeName = typeof(int).FullName,
                Value = expectedId.ToString(CultureInfo.InvariantCulture)
            };

            DataTable interviewsControlData = _serviceHelper.ConsoleService.GetSurveyInterviews(
                ProjectID,
                new[] { filter });

            Assert.AreEqual(callsCount, interviewsControlData.Rows.Count);

            Assert.IsTrue(IsDescOrdered(interviewsControlData), "Returned list is not sorted by priority");
        }

        /// <summary>
        /// Checking that interview ITS name is taken from ITS group, assigned to survey.
        /// </summary>
        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GettingInterviewsForSurvey_CheckInterviewITSName_ITSNameFromITSGroupAssignedToSurvey()
        {
            const string itsGroupName = "My group";
            const string myFreshSample = "My fresh sample";
            var allInterviews = new List<BvInterviewEntity>();

            int surveyId = PrepareDataForTest(1, null, null, false, allInterviews);

            // creating new ITS group and changing "Fresh sample" its name
            int stateGroupID = StateGroupsManager.AddStateGroup(itsGroupName);
            BvStateEntity freshSample = StateRepository.GetById(stateGroupID, 16);
            freshSample.Name = myFreshSample;
            StateRepository.Update(freshSample);

            // setting custom ITS group for survey
            BvSurveyEntity survey = SurveyRepository.GetById(surveyId);
            survey.StateGroupID = stateGroupID;
            SurveyRepository.Update(survey);

            DataTable interviewsControlData = _serviceHelper.ConsoleService.GetSurveyInterviews(ProjectID, new SearchParameter[] { });
            Assert.AreEqual(myFreshSample, interviewsControlData.Rows[0]["ITSName"]);
        }

        /// <summary>
        /// Checking that filter by confirmit variables works.
        /// 
        /// 1.  Add survey, launch 'all hours' script, open survey
        /// 2.  Create 3 interview        
        /// 3.  Create 3 call
        /// 4.  Create person in manual mode
        /// 5.  Assign person on Survey
        /// 6.  Assign person on 3 calls
        /// 7. Login person in caticonsole and BE
        /// 8. Call scheduling procedure
        /// 9. Create respondent and response_control tables and fill data to these tables
        /// 10. Enable change tracking
        /// 11. Update survey replication scheme
        /// 12. Fill SearchableFieldsInConsole table
        /// 13. Create filter by three confirmit variables
        /// 14. Call GetSurveyInterviews
        /// 
        /// Method should return single interview.
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GettingInterviewsForSurvey_TryToGetFilteredByConfirmitVariables_OnlyFilterdInterviewsAreReturned()
        {
            var allInterviews = new List<BvInterviewEntity>();
            int surveyId = PrepareDataForTest(3, null, null, false, allInterviews);
            TableInfo[] tableInfo = ReplicationTools.GetTestDataForSurveyInterview(new[] { 0, 1, 2 });

            CreateAndFillCfResponceControl(3);

            BackendTools.EnableChangeTracking(_framework.DbEngine, tableInfo);
            new ManagementService().UpdateSurveyReplicationScheme(ProjectID, tableInfo);

            _orderedSearchableFieldsService.RegenerateFields(surveyId);
            BvSearchableFieldsOrderedAdapter.Update(new BvSearchableFieldsOrderedEntity
                { FieldName = "q1", IsEnabled = true, OrderNumber = 4, IsSystem = false, SurveyId = surveyId });
            BvSearchableFieldsOrderedAdapter.Update(new BvSearchableFieldsOrderedEntity
                { FieldName = "q2", IsEnabled = true, OrderNumber = 5, IsSystem = false, SurveyId = surveyId });
            BvSearchableFieldsOrderedAdapter.Update(new BvSearchableFieldsOrderedEntity
                { FieldName = "q3", IsEnabled = true, OrderNumber = 6, IsSystem = false, SurveyId = surveyId });

            var filter = new[]
            { 
                new SearchParameter
                {
                    ColumnName = "var_q1",
                    ColumnTypeName = typeof(string).FullName,
                    Value = "2"
                },
                new SearchParameter
                {
                    ColumnName = "var_q2",
                    ColumnTypeName = typeof(string).FullName,
                    Value = "2"
                },
                new SearchParameter
                {
                    ColumnName = "var_q3",
                    ColumnTypeName = typeof(int).FullName,
                    Value = "2"
                }
            };

            DataTable interviewsControlData = _serviceHelper.ConsoleService.GetSurveyInterviews(
                ProjectID, filter);

            Assert.AreEqual(1, interviewsControlData.Rows.Count);

            Assert.AreEqual("2", interviewsControlData.Rows[0]["var_q1"]);
            Assert.AreEqual("2", interviewsControlData.Rows[0]["var_q2"]);
            Assert.AreEqual(2, (int)interviewsControlData.Rows[0]["var_q3"]);
        }

        /// <summary>
        /// Checking that filter by wrong confirmit variables works.
        /// 
        /// 1.  Add survey, launch 'all hours' script, open survey
        /// 2.  Create 3 interview        
        /// 3.  Create 3 call
        /// 4.  Create person in manual mode
        /// 5.  Assign person on Survey
        /// 6.  Assign person on 3 calls
        /// 7. Login person in caticonsole and BE
        /// 8. Call scheduling procedure
        /// 9. Create respondent and response_control tables and fill data to these tables
        /// 10. Enable change tracking
        /// 11. Update survey replication scheme
        /// 12. Fill SearchableFieldsInConsole table
        /// 13. Create filter by two wrong confirmit variables
        /// 14. Call GetSurveyInterviews
        /// 
        /// Method should return all interviews.
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GettingInterviewsForSurvey_TryToGetFilteredByWrongConfirmitVariables_AllInterviewsAreReturned()
        {
            var allInterviews = new List<BvInterviewEntity>();
            int surveyId = PrepareDataForTest(3, null, null, false, allInterviews);
            TableInfo[] tableInfo = ReplicationTools.GetTestDataForSurveyInterview(new[] { 0, 1, 2 });

            CreateAndFillCfResponceControl(3);

            BackendTools.EnableChangeTracking(_framework.DbEngine, tableInfo);
            new ManagementService().UpdateSurveyReplicationScheme(ProjectID, tableInfo);
            
            _orderedSearchableFieldsService.RegenerateFields(surveyId);
            BvSearchableFieldsOrderedAdapter.Update(new BvSearchableFieldsOrderedEntity
                { FieldName = "q1", IsEnabled = true, OrderNumber = 4, IsSystem = false, SurveyId = surveyId });
            BvSearchableFieldsOrderedAdapter.Update(new BvSearchableFieldsOrderedEntity
                { FieldName = "q2", IsEnabled = true, OrderNumber = 5, IsSystem = false, SurveyId = surveyId });
            BvSearchableFieldsOrderedAdapter.Update(new BvSearchableFieldsOrderedEntity
                { FieldName = "q3", IsEnabled = true, OrderNumber = 6, IsSystem = false, SurveyId = surveyId });
            
            var filter = new[]
            { 
                new SearchParameter
                {
                    ColumnName = "var_q10",
                    ColumnTypeName = typeof(string).FullName,
                    Value = "2"
                },
                new SearchParameter
                {
                    ColumnName = "var_q20",
                    ColumnTypeName = typeof(string).FullName,
                    Value = "2"
                }
            };

            DataTable interviewsControlData = _serviceHelper.ConsoleService.GetSurveyInterviews(
                ProjectID, filter);

            Assert.AreEqual(3, interviewsControlData.Rows.Count);
        }

        /// <summary>
        /// Checking that filter by confirmit variables and our variables (respondent name) works.
        /// 
        /// 1.  Add survey, launch 'all hours' script, open survey
        /// 2.  Create 3 interviews
        /// 3.  Create 3 calls for 3 respondent names
        /// 4.  Create person in manual mode
        /// 5.  Assign person on Survey
        /// 6.  Assign person on 3 calls
        /// 7. Login person in caticonsole and BE
        /// 8. Call scheduling procedure
        /// 9. Create respondent and response_control tables and fill data to these tables
        /// 10. Enable change tracking
        /// 11. Update survey replication scheme
        /// 12. Fill SearchableFieldsInConsole table
        /// 13. Create filter by one confirmit variable and repondent name
        /// 14. Call GetSurveyInterviews
        /// 
        /// Method should return single interview with correct confirmit variable and 
        /// correct respondent name.
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GettingInterviewsForSurvey_TryToGetFilteredByConfirmitVariableAndRespondentName_OnlyFilteredInterviewsAreReturned()
        {
            var allInterviews = new List<BvInterviewEntity>();
            var respondentsNames = new[] { "gri3", "gri2", "gri1" };
            int surveyId = PrepareDataForTest(3, respondentsNames, null, false, allInterviews);
            TableInfo[] tableInfo = ReplicationTools.GetTestDataForSurveyInterview(new[] { 0 });

            CreateAndFillCfResponceControl(3);

            BackendTools.EnableChangeTracking(_framework.DbEngine, tableInfo);
            new ManagementService().UpdateSurveyReplicationScheme(ProjectID, tableInfo);
            
            _orderedSearchableFieldsService.RegenerateFields(surveyId);
            BvSearchableFieldsOrderedAdapter.Update(new BvSearchableFieldsOrderedEntity
                { FieldName = "q1", IsEnabled = true, OrderNumber = 4, IsSystem = false, SurveyId = surveyId });
            
            var filter = new[]
            { 
                new SearchParameter
                {
                    ColumnName = "var_q1",
                    ColumnTypeName = typeof(string).FullName,
                    Value = "3"
                },
                new SearchParameter
                {
                    ColumnName="RespondentName", 
                    ColumnTypeName=typeof(string).FullName, 
                    Value="gri" 
                }
            };

            DataTable interviewsControlData = _serviceHelper.ConsoleService.GetSurveyInterviews(
                ProjectID, filter);

            Assert.AreEqual(1, interviewsControlData.Rows.Count);

            Assert.AreEqual("3", interviewsControlData.Rows[0]["var_q1"]);
            Assert.AreEqual("gri1", interviewsControlData.Rows[0]["RespondentName"]);
        }

        /// <summary>
        /// Checking that UpdateSurveyReplicationScheme function update 
        /// SearchableFieldsInConsole table correct
        /// 
        /// 1. Create survey
        /// 2. Create respondent and response_control tables and fill data to these tables
        /// 3. Enable change tracking
        /// 4. Update survey replication scheme for q1,q2,q3 columns
        /// 5. Fill SearchableFieldsInConsole table
        /// 6. Update survey replication scheme for q3, q4, q5 columns
        /// 7. Get columns from SearchableFieldsInConsole table for out survey id
        /// 
        /// Method should return one row from SearchableFieldsInConsole table with 
        /// correct column id.
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GettingInterviewsForSurvey_UpdateSchema_SearchableFieldsInConsoleAreChanged()
        {
            int surveyId = _backendTools.CreateSurvey(ProjectID);
            TableInfo[] tableInfo = ReplicationTools.GetTestDataForSurveyInterview(new[] { 0, 1, 2 });

            CreateAndFillCfResponceControl(3);

            BackendTools.EnableChangeTracking(_framework.DbEngine, tableInfo);
            new ManagementService().UpdateSurveyReplicationScheme(ProjectID, tableInfo);
            
            _orderedSearchableFieldsService.RegenerateFields(surveyId);
            BvSearchableFieldsOrderedAdapter.Update(new BvSearchableFieldsOrderedEntity
                { FieldName = "q1", IsEnabled = true, OrderNumber = 4, IsSystem = false, SurveyId = surveyId });
            BvSearchableFieldsOrderedAdapter.Update(new BvSearchableFieldsOrderedEntity
                { FieldName = "q2", IsEnabled = true, OrderNumber = 5, IsSystem = false, SurveyId = surveyId });
            BvSearchableFieldsOrderedAdapter.Update(new BvSearchableFieldsOrderedEntity
                { FieldName = "q3", IsEnabled = true, OrderNumber = 6, IsSystem = false, SurveyId = surveyId });
            
            tableInfo = ReplicationTools.GetTestDataForSurveyInterview(new[] { 2, 3, 4 });

            new ManagementService().UpdateSurveyReplicationScheme(ProjectID, tableInfo);

            _orderedSearchableFieldsService.RegenerateFields(surveyId);
            
            var result = new OrderedSearchableFieldsRepository().GetBySurveyId(surveyId).Where(x => x.IsEnabled && !x.IsSystem).ToList();

            Assert.AreEqual(1, result.Count);

            Assert.AreEqual("q3", result[0].FieldName);
        }
    }
}
