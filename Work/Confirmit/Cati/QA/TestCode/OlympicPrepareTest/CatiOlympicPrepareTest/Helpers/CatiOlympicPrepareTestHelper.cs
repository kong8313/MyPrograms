using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using CatiOlympicPrepareTest.Constants;
using CatiOlympicPrepareTest.wsAuthoring;
using CatiOlympicPrepareTest.wsLogOn;
using CatiOlympicPrepareTest.wsSurveyData;
using CatiOlympicPrepareTest.wsSurveyDeployer;
using Confirmit.CATI.REST.SDK.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseType = CatiOlympicPrepareTest.wsSurveyDeployer.DatabaseType;


namespace CatiOlympicPrepareTest.Helpers
{
   
    public class CatiOlympicPrepareTestHelper
    {
        public static string ConnectionString { get; set; }

        public static InterviewerProperties CreateEmptyInterviewerEntity()
        {
            var interviewerProperties = new InterviewerProperties();

            return interviewerProperties;
        }
        public static string ImportSurvey(string server, string surveyLocation, string surveyName, string authKey)
        { 
            var authoring = GetAuthoringClient(server);
            using (var sr = new StreamReader(surveyLocation))
            {
                var surveyXml = sr.ReadToEnd();
                // update survey name in the template XML
                Regex reg = new Regex(@"Name=\""[a-zA-Z0-9!@#$%^*()_+-=]{1,65}\""");
                var ibool = reg.IsMatch(surveyXml);
                Assert.IsTrue(ibool,@"Can't find survey name for replace");

                var resultXml = reg.Replace(surveyXml, "Name =\"" + surveyName + "\"", 1);
                
                var surveyId = authoring.ImportSurvey(authKey, resultXml); 

                return surveyId;
            }
        }

        public static void LaunchSurvey(string server, string surveyId, string authKey)
        {
            var deployer = GetSurveyDeployer(server);
            var taskId = deployer.LaunchSurvey(authKey, surveyId, DatabaseType.Production, GenerateDbOptions.CreateNewDatabase, GenerateWiOptions.WiNet);
            var taskStatus = deployer.GetTaskStatus(authKey, taskId);
            var tries = 0;

            while (taskStatus != TaskStatus.Complete && taskStatus != TaskStatus.Error && tries < 60)
            {
                ++tries;
                Thread.Sleep(3000);
                taskStatus = deployer.GetTaskStatus(authKey, taskId);
            }

            var pct = deployer.GetTaskPercentageCompleted(authKey, taskId);
            if (pct != 100)
                throw new TimeoutException("The time(3min) allotted for a launch survey has expired");
        }

        public static void AddSampleAndUpdateSample(string dialMode, string surveyMode, string server, string authKey, string surveyId, string languagesFilePath, int amountOfRespondents)
        {
            var surveyData = GetSurveyData(server);
            var respondents = surveyData.GetRespondents(authKey, SurveyDataUtil.NewRespondentTransferDef(true, true, surveyId));
            var respondentTable = respondents.Tables[SurveyDataUtil.RespondentTableName];

            if ((string.Equals(surveyMode, SurveyMode.Predictive, StringComparison.OrdinalIgnoreCase)) 
                || (string.Equals(surveyMode, SurveyMode.PreviewInPredictive, StringComparison.OrdinalIgnoreCase)))
            {
                if (!respondentTable.Columns.Contains("language"))
                    respondentTable.Columns.Add(new DataColumn("language", typeof (string)));
            }

            if (!respondentTable.Columns.Contains("q1"))
                respondentTable.Columns.Add(new DataColumn("q1", typeof(string)));

            if (!respondentTable.Columns.Contains("q2"))
                respondentTable.Columns.Add(new DataColumn("q2", typeof(string)));

            if (!respondentTable.Columns.Contains("q3"))
                respondentTable.Columns.Add(new DataColumn("q3", typeof(string)));

            if (!respondentTable.Columns.Contains("q4"))
                respondentTable.Columns.Add(new DataColumn("q4", typeof(string)));
            
            if (!respondentTable.Columns.Contains("q5"))
                respondentTable.Columns.Add(new DataColumn("q5", typeof(string)));

            if (!respondentTable.Columns.Contains("q6"))
                respondentTable.Columns.Add(new DataColumn("q6", typeof(string)));

            using (var sr = new StreamReader(languagesFilePath))
            {
                for (var i = 1; i < amountOfRespondents + 1; i++)
                {     
                    var answersInfo = sr.ReadLine();
                    if (answersInfo != null)
                    {
                        var splitedAnswers = answersInfo.Split(' ');
                        var row = respondentTable.NewRow();

                        row["DialMode"] = dialMode;
                        row["language"] = splitedAnswers[0];
                        row["q5"] = splitedAnswers[1];
                        row["q6"] = splitedAnswers[2];
                        row["q1"] = "1";
                        row["q2"] = "2";
                        row["q3"] = "3";
                        row["q4"] = "hello world";
                        row["TelephoneNumber"] = string.Format("{0}{0}{0}", i);

                        respondentTable.Rows.Add(row);
                    }
                }
            }

            var errors = surveyData.UpdateRespondentsWithCatiScheduling(authKey, surveyId, respondents, true, false, string.Empty, false, -1, CatiScheduling.Full);
            if (errors.Length > 0)
                throw new Exception("Errors appeared during update respondents with CATI scheduling");

            // do modifications in respondents
            int j = 0;
            foreach (DataRow item in respondentTable.Rows)
            {
                item["respid"] = ++j;
                item["TelephoneNumber"] = string.Format("{0}-{0}-{0} update_1", j);
            }

            errors = surveyData.UpdateExistingRespondentsWithCatiScheduling(authKey, surveyId, respondents, true,
                "respid", false, -1, CatiScheduling.Full);

            if (errors.Length > 0)
                throw new Exception("Errors appeared during update respondents with CATI scheduling");

            var dataLevel = new DataLevel
            {
                LevelId = "respondent",
                Variables = new[]
                {
                    new wsSurveyData.Variable {Name = "respid", Type = VariableType.Integer  },
                    new wsSurveyData.Variable {Name = "TelephoneNumber", Type = VariableType.String}
                },


                Records = new[]
                {
                    new DataRecord {Values = new object[] {1, "111-111 update 2"}},
                    new DataRecord {Values = new object[] {2, "222-222 update 2"}},
                    new DataRecord {Values = new object[] {3, "333-333 update 2"}},
                    new DataRecord {Values = new object[] {4, "444-444 update 2"}},
                    new DataRecord {Values = new object[] {5, "555-555 update 2"}},
                    new DataRecord {Values = new object[] {6, "666-666 update 2"}},
                    new DataRecord {Values = new object[] {7, "777-777 update 2"}},
                    new DataRecord {Values = new object[] {8, "888-888 update 2"}},
                    new DataRecord {Values = new object[] {9, "999-999 update 2"}},
                    new DataRecord {Values = new object[] {10, "10-10-10 update 2"}},
                    new DataRecord {Values = new object[] {11, "11-11-11 update 2"}},
                    new DataRecord {Values = new object[] {12, "12-12-12 update 2"}}
                }
            };
            var data = new ConfirmitData
            {
                DataLevels = new[] { dataLevel }
            };

            errors = surveyData.UpdateExistingRespondentsGeneralWithCatiScheduling(authKey, surveyId, data, true,
                "respid", false, 0, CatiScheduling.Full);

            if (errors.Length > 0)
                throw new Exception("Errors appeared during update respondents with CATI scheduling");

        }

        public static Authoring GetAuthoringClient(string server)
        {
            return new Authoring
            {
                Url = "http://" + server + "/confirmit/webservices/18.0/authoring.asmx"
            };
        }

        public static LogOn GetLogonClient(string server)
        {
            return new LogOn
            {
                Url = "http://" + server + "/confirmit/webservices/18.0/logon.asmx"
            };
        }

        public static SurveyDeployer GetSurveyDeployer(string server)
        {
            return new SurveyDeployer
            {
                Url = "http://" + server + "/confirmit/webservices/18.0/surveydeployer.asmx"
            };
        }

        public static SurveyData GetSurveyData(string server)
        {
            return new SurveyData
            {
                Url = "http://" + server + "/confirmit/webservices/18.0/surveydata.asmx"
            };
        }

        public static int CheckFirstDialerStateAndEnableIfNeed(string companyId)
        {
            var dialerState = GetFirstDialerState(companyId, out var dialerId);
            if (!dialerState)
            {
                DialerEnable(ConnectionString, companyId, dialerId);
            }
            return dialerId;
        }

        public static void DialerEnable(string connectionString, string companyId, int dialerId)
        {
            SupervisorClientFactory.CreateClient(Convert.ToInt32(companyId)).EnableDialer(dialerId);

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command =
                    new SqlCommand($"UPDATE [ConfirmitCATIV15_{companyId}].[dbo].[BvDialers] SET [IsActive] = '1', [ExpectedState]=0, [DialerOperationalStateNotification]='1' WHERE [Id] = '{dialerId}'",
                        connection);

                command.ExecuteNonQuery();
            }
        }

        public static bool GetFirstDialerState(string companyId, out int dialerId)
        {
            // we return status of first configured dialer, regardless of it's ID
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command =
                    new SqlCommand(
                        "SELECT [Id], [DialerOperationalStateNotification], [IsActive]  FROM  [ConfirmitCATIV15_" +
                        companyId + "].[dbo].[BvDialers]", connection);

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                        throw new Exception("There are no dialers for company with id: " + companyId);

                    reader.Read();
                    var dialerState = (bool)reader["DialerOperationalStateNotification"];
                    var isActive = (bool)reader["IsActive"];
                    dialerId = (int)reader["Id"];

                    return dialerState && isActive;
                }
            }
        }

        public static void SetFcdBehaviorType(string companyId, string fcdBehaviorType)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand($"exec [ConfirmitCATIV15_{companyId}].[dbo].[BvSpSystemSetting_Update] 'FCD.BehaviorType', '{fcdBehaviorType}'", connection);
                command.ExecuteNonQuery();
            }
        }

        public static int GetSurveySid(string surveyDescription,  string companyId)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var sidCommand =
                    new SqlCommand(
                        "SELECT [SID]  FROM  [ConfirmitCATIV15_" + companyId + "].[dbo].[BvSurvey] WHERE [Description] = @SurveyDescription",
                        connection);
                sidCommand.Parameters.AddWithValue("@SurveyDescription", surveyDescription);

                using (var sidReader = sidCommand.ExecuteReader())
                {
                    if (!sidReader.HasRows)
                        throw new Exception(string.Format("No survey with [Description] = '{0}' in datadase", surveyDescription));

                    sidReader.Read();
                    return (int)sidReader["SID"];
                }
            }
        }

        public static List<int> GetActualInterviewStatuses(int surveySid, List<int> interviewIds, string companyId)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var actualInterviewStates = new List<int>();
                connection.Open();
                foreach (var interviewId in interviewIds)
                {
                    var checkTransientStateCommand =
                        new SqlCommand("SELECT [TransientState]  FROM  [ConfirmitCATIV15_" + companyId + "].[dbo].[BvInterview] WHERE [ID] = @ID AND [SurveySID] = @SurveySID",
                            connection);
                    checkTransientStateCommand.Parameters.AddWithValue("@ID", interviewId);
                    checkTransientStateCommand.Parameters.AddWithValue("@SurveySID", surveySid);

                    using (var readerTransientState = checkTransientStateCommand.ExecuteReader())
                    {
                        if (!readerTransientState.HasRows)
                        {
                            throw new Exception(
                                string.Format("No interview with [SurveySID] = '{0}' and [ID] = '{1}' in datadase",
                                    surveySid, interviewId));
                        }

                        readerTransientState.Read();
                        actualInterviewStates.Add((int)readerTransientState["TransientState"]);
                    }
                }

                return actualInterviewStates;
            }
        }

        public static List<int> GetActualCallStates(int surveySid, List<int> interviewIds, string companyId)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var actualCallStates = new List<int>();
                connection.Open();
                foreach (var interviewId in interviewIds)
                {
                    var checkCallStateCommand =
                        new SqlCommand("SELECT [CallState]  FROM  [ConfirmitCATIV15_" + companyId + "].[dbo].[BvSvySchedule] WHERE [InterviewID] = @InterviewID AND [SurveySID] = @SurveySID",
                            connection);
                    checkCallStateCommand.Parameters.AddWithValue("@InterviewID", interviewId);
                    checkCallStateCommand.Parameters.AddWithValue("@SurveySID", surveySid);

                    using (var readerCallState = checkCallStateCommand.ExecuteReader())
                    {
                        if (!readerCallState.HasRows)
                        {
                            continue;
                            //throw new Exception(
                            //    string.Format("No interview with [SurveySID] = '{0}' and [InterviewID] = '{1}' in datadase",
                            //        surveySid, interviewId));
                        }

                        readerCallState.Read();
                        actualCallStates.Add((int)readerCallState["CallState"]);
                    }
                }

                return actualCallStates;
            }
        }
    }
}
