using System;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.FilterServiceImplementation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tools;
using Confirmit.CATI.Core.DAL.Framework;
using SampleType = Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tools.FilterAndPagingTools.SampleType;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Repositories;
using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Classes.CallManagement;
using System.Data;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Data.Builders;
using Confirmit.Test.Common.Attributes;
using Microsoft.SqlServer.Management.Smo;

namespace Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tests
{
    [TestClass]
    public class CallListSearchTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private FilterAndPagingTools _filterAndPagingTools;
        private DatabaseEngine _confirmitSurveyDb;
        private int _timezoneId;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _filterAndPagingTools = new FilterAndPagingTools(_framework, new BackendTools(_framework));

            _confirmitSurveyDb = _filterAndPagingTools.CreateCFSurveyDatabaseEngine();
            _timezoneId = ServiceLocator.Resolve<ITimezoneService>().GetDefaultCallCenterTimezoneId();

            _framework.SetTestHttpContextCurrentWithSupervisorPrincipal();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.ClearTestHttpContextCurrent();

            new SqlObjectCreator(_framework).CleanTablesInSurveyDatabase(_framework.TestSurveyDatabaseName);

            _framework.TestCleanup();
        }

        private void FillSurveyData()
        {
            new SqlObjectCreator(_framework).CleanTablesInSurveyDatabase(_framework.TestSurveyDatabaseName);

            var formData = new[] 
            { 
                new FormData { Name = "q1" },
                new FormData { Name = "q2" },
                new FormData { Name = "key" }
            };

            var sdb = new SurveyDatabaseBuilder(_confirmitSurveyDb, formData);

            const int batchId = 1;
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "0", InterviewerId = "1", TelephoneNumber = "5550", ExtensionNumber = "0", LastChannelId = "1", TimeZoneId = "0", RespondentName = "0", DialMode = "1", Data = "q1=2,q2=3,key=9" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "1", InterviewerId = "2", TelephoneNumber = "5551", ExtensionNumber = "1", LastChannelId = "1", TimeZoneId = "1", RespondentName = "1", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "2", InterviewerId = "3", TelephoneNumber = "5552", ExtensionNumber = "2", LastChannelId = "1", TimeZoneId = "2", RespondentName = "2", DialMode = "1", Data = "q1=6,q2=9,key=9" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "3", InterviewerId = "4", TelephoneNumber = "5553", ExtensionNumber = "3", LastChannelId = "1", TimeZoneId = "3", RespondentName = "3", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "4", InterviewerId = "5", TelephoneNumber = "5554", ExtensionNumber = "4", LastChannelId = "1", TimeZoneId = "4", RespondentName = "4", DialMode = "1", Data = "q1=10,q2=15,key=9" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "5", InterviewerId = "6", TelephoneNumber = "5555", ExtensionNumber = "5", LastChannelId = "1", TimeZoneId = "5", RespondentName = "5", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "6", InterviewerId = "7", TelephoneNumber = "5556", ExtensionNumber = "6", LastChannelId = "1", TimeZoneId = "6", RespondentName = "6", DialMode = "1", Data = "q1=14,q2=21,key=9" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "7", InterviewerId = "8", TelephoneNumber = "5557", ExtensionNumber = "7", LastChannelId = "1", TimeZoneId = "0", RespondentName = "7", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "8", InterviewerId = "9", TelephoneNumber = "5558", ExtensionNumber = "8", LastChannelId = "1", TimeZoneId = "1", RespondentName = "8", DialMode = "1", Data = "q1=18,q2=27,key=9" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "9", InterviewerId = "10", TelephoneNumber = "5559", ExtensionNumber = "9", LastChannelId = "1", TimeZoneId = "2", RespondentName = "9", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "10", InterviewerId = "11", TelephoneNumber = "55510", ExtensionNumber = "10", LastChannelId = "1", TimeZoneId = "3", RespondentName = "10", DialMode = "1", Data = "q1=22,q2=33,key=9" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "11", InterviewerId = "12", TelephoneNumber = "55511", ExtensionNumber = "11", LastChannelId = "1", TimeZoneId = "4", RespondentName = "11", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "12", InterviewerId = "13", TelephoneNumber = "55512", ExtensionNumber = "12", LastChannelId = "1", TimeZoneId = "5", RespondentName = "12", DialMode = "1", Data = "q1=26,q2=39,key=9" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "13", InterviewerId = "14", TelephoneNumber = "55513", ExtensionNumber = "13", LastChannelId = "1", TimeZoneId = "6", RespondentName = "13", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "14", InterviewerId = "15", TelephoneNumber = "55514", ExtensionNumber = "14", LastChannelId = "1", TimeZoneId = "0", RespondentName = "14", DialMode = "1", Data = "q1=30,q2=45,key=9" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "15", InterviewerId = "16", TelephoneNumber = "55515", ExtensionNumber = "15", LastChannelId = "1", TimeZoneId = "1", RespondentName = "15", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "16", InterviewerId = "17", TelephoneNumber = "55516", ExtensionNumber = "16", LastChannelId = "1", TimeZoneId = "2", RespondentName = "16", DialMode = "1", Data = "q1=34,q2=51,key=9" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "17", InterviewerId = "18", TelephoneNumber = "55517", ExtensionNumber = "17", LastChannelId = "1", TimeZoneId = "3", RespondentName = "17", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "18", InterviewerId = "19", TelephoneNumber = "55518", ExtensionNumber = "18", LastChannelId = "1", TimeZoneId = "4", RespondentName = "18", DialMode = "1", Data = "q1=38,q2=57,key=9" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "19", InterviewerId = "20", TelephoneNumber = "55519", ExtensionNumber = "19", LastChannelId = "1", TimeZoneId = "5", RespondentName = "19", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "20", InterviewerId = "21", TelephoneNumber = "55520", ExtensionNumber = "20", LastChannelId = "1", TimeZoneId = "6", RespondentName = "20", DialMode = "1", Data = "q1=42,q2=63,key=9" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "21", InterviewerId = "22", TelephoneNumber = "55521", ExtensionNumber = "21", LastChannelId = "1", TimeZoneId = "0", RespondentName = "21", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "22", InterviewerId = "23", TelephoneNumber = "55522", ExtensionNumber = "22", LastChannelId = "1", TimeZoneId = "1", RespondentName = "22", DialMode = "1", Data = "q1=46,q2=69,key=9" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "23", InterviewerId = "24", TelephoneNumber = "55523", ExtensionNumber = "23", LastChannelId = "1", TimeZoneId = "2", RespondentName = "23", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "24", InterviewerId = "25", TelephoneNumber = "", ExtensionNumber = "24", LastChannelId = "1", TimeZoneId = "3", RespondentName = "24", DialMode = "1", Data = "q1=50,q2=75,key=9" });
        }

        /// <summary>
        /// Returns set with all filter ID's.
        /// </summary>
        /// <returns>Set of filter id's.</returns>
        private HashSet<int> GetAllFilters()
        {
            return new HashSet<int>(
                FilterRepository.GetAll().Select(c => c.SID)
            );
        }

        /// <summary>
        /// Returns set with all filter fields ID's.
        /// </summary>
        /// <returns>Set of filter fields id's.</returns>
        private HashSet<int> GetAllFields()
        {
            return new HashSet<int>(
                BvFilterFieldsAdapter.GetAll().Select(c => c.ID)
            );
        }

        /// <summary>
        /// Fills the respondent name of interviews of given survey in with needed values
        /// (without scheduling).
        /// </summary>
        /// <param name="surveyId">Survey identifier.</param>
        private void FillRespondentName(int surveyId)
        {
            int i = 0;
            List<BvInterviewEntity> interviews = BvInterviewAdapter.GetAll().Where(c => c.SurveySID == surveyId).ToList();
            // first 2 interviews respondent name will be "dump"
            for (; i < 2 && i < interviews.Count; i++)
            {
                BvInterviewEntity interview = interviews[i];
                interview.RespondentName = "dump";
                InterviewRepository.UpdateOnly(interview);
            }

            // next 2 interviews respondent name will be "auction"
            for (; i < 4 && i < interviews.Count; i++)
            {
                BvInterviewEntity interview = interviews[i];
                interview.RespondentName = "auction";
                InterviewRepository.UpdateOnly(interview);
            }

            // next 2 interviews respondent name will be "r_bel"
            for (; i < 6 && i < interviews.Count; i++)
            {
                BvInterviewEntity interview = interviews[i];
                interview.RespondentName = "r_bel";
                InterviewRepository.UpdateOnly(interview);
            }

            for (; i < interviews.Count; i++)
            {
                BvInterviewEntity interview = interviews[i];
                interview.RespondentName = "respondent";
                InterviewRepository.UpdateOnly(interview);
            }
        }

        [TestMethod, Owner(@"FIRM\VictorR")]
        public void GetCallsPage_QueryFromCustomFilterByLastInterviewerName_CallsWithSuitedLastInterviewerName()
        {
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        IsUseDb = true,
                        Interviews = new[]
                        {
                            new InterviewData { Tag = "S1.I1", Call = new CallData(), LastCallPerson = "P2" },     
                            new InterviewData { Tag = "S1.I2", Call = new CallData(), LastCallPerson = "P3" },     
                            new InterviewData { Tag = "S1.I3", Call = new CallData(), LastCallPerson = "P4" }
                        }
                    }
                },
                Persons = new[]
                {
                    new PersonData { Name = "user", Tag = "P1" }, 
                    new PersonData { Name = "user11", Tag = "P2" },
                    new PersonData { Name = "user12", Tag = "P3" }, 
                    new PersonData { Name = "user2", Tag = "P4" } 
                }
            }.Create();

            var filter = new BvFiltersEntity
            {
                Name = Guid.NewGuid().ToString(),
                AndOrOperator = (byte)AndOrOperator.And
            };
            var filterId = FilterRepository.Insert(filter);

            var fields = new List<BvFilterFieldsEntity>
            {
                new BvFilterFieldsEntity
                    {
                        Column = "Name",
                        Table = (int) TableTypes.Person,
                        Type = (int) VariableTypes.String,
                        Sign = (int) FilterOperator.Like,
                        Value = "user1"
                    }
            };
            FilterService.SetFields(filterId, fields);

            var args = new PagingArgs(1, 100, "InterviewID", true);
            int totalCount;

            // act  
            var result = CallHelper.GetCallsPage(context.GetSurvey("S1").Id, filterId, _timezoneId, CallStates.All,
                args, out totalCount, ShowTimeMode.Interviewer, false);

            var persons = (from DataRow row in result.Rows select (string)row["LastInterviewerName"]).ToList();

            // assert
            Assert.IsTrue(totalCount == 2);
            Assert.IsTrue(persons.Contains("user11") && persons.Contains("user12"));
        }
        
        [TestMethod, Owner(@"FIRM\EgorK")]
        public void GetCallsPage_QueryFromCustomFilterByLastInterviewerNameUsingInCondition_CallsWithSuitedLastInterviewerName()
        {
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        IsUseDb = true,
                        Interviews = new[]
                        {
                            new InterviewData { Tag = "S1.I1", Call = new CallData(), LastCallPerson = "P2" },     
                            new InterviewData { Tag = "S1.I2", Call = new CallData(), LastCallPerson = "P3" },     
                            new InterviewData { Tag = "S1.I3", Call = new CallData(), LastCallPerson = "P4" }
                        }
                    }
                },
                Persons = new[]
                {
                    new PersonData { Name = "user", Tag = "P1" }, 
                    new PersonData { Name = "user11", Tag = "P2" },
                    new PersonData { Name = "user12", Tag = "P3" }, 
                    new PersonData { Name = "user2", Tag = "P4" } 
                }
            }.Create();

            var filter = new BvFiltersEntity
            {
                Name = Guid.NewGuid().ToString(),
                AndOrOperator = (byte)AndOrOperator.And
            };
            var filterId = FilterRepository.Insert(filter);

            var fields = new List<BvFilterFieldsEntity>
            {
                new BvFilterFieldsEntity
                    {
                        Column = "Name",
                        Table = (int) TableTypes.Person,
                        Type = (int) VariableTypes.String,
                        Sign = (int) FilterOperator.In,
                        Value = "user11,user12"
                    }
            };
            FilterService.SetFields(filterId, fields);

            var args = new PagingArgs(1, 100, "InterviewID", true);
            int totalCount;

            // act  
            var result = CallHelper.GetCallsPage(context.GetSurvey("S1").Id, filterId, _timezoneId, CallStates.All,
                args, out totalCount, ShowTimeMode.Interviewer, false);

            var persons = (from DataRow row in result.Rows select (string)row["LastInterviewerName"]).ToList();

            // assert
            Assert.IsTrue(totalCount == 2);
            Assert.IsTrue(persons.Contains("user11") && persons.Contains("user12"));
        }
        
        [TestMethod, Owner(@"FIRM\EgorK")]
        public void GetCallsPage_QueryFromCustomFilterByLastInterviewerNameUsingNotInCondition_CallsWithSuitedLastInterviewerName()
        {
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        IsUseDb = true,
                        Interviews = new[]
                        {
                            new InterviewData { Tag = "S1.I1", Call = new CallData(), LastCallPerson = "P2" },     
                            new InterviewData { Tag = "S1.I2", Call = new CallData(), LastCallPerson = "P3" },     
                            new InterviewData { Tag = "S1.I3", Call = new CallData(), LastCallPerson = "P4" }
                        }
                    }
                },
                Persons = new[]
                {
                    new PersonData { Name = "user", Tag = "P1" }, 
                    new PersonData { Name = "user11", Tag = "P2" },
                    new PersonData { Name = "user12", Tag = "P3" }, 
                    new PersonData { Name = "user2", Tag = "P4" } 
                }
            }.Create();

            var filter = new BvFiltersEntity
            {
                Name = Guid.NewGuid().ToString(),
                AndOrOperator = (byte)AndOrOperator.And
            };
            var filterId = FilterRepository.Insert(filter);

            var fields = new List<BvFilterFieldsEntity>
            {
                new BvFilterFieldsEntity
                    {
                        Column = "Name",
                        Table = (int) TableTypes.Person,
                        Type = (int) VariableTypes.String,
                        Sign = (int) FilterOperator.NotIn,
                        Value = "user11,user12"
                    }
            };
            FilterService.SetFields(filterId, fields);

            var args = new PagingArgs(1, 100, "InterviewID", true);
            int totalCount;

            // act  
            var result = CallHelper.GetCallsPage(context.GetSurvey("S1").Id, filterId, _timezoneId, CallStates.All,
                args, out totalCount, ShowTimeMode.Interviewer, false);

            var persons = (from DataRow row in result.Rows select (string)row["LastInterviewerName"]).ToList();

            // assert
            Assert.IsTrue(totalCount == 1);
            Assert.IsTrue(persons.Contains("user2"));
        }


        [TestMethod, Owner(@"FIRM\VictorR")]
        public void GetCallsPage_QueryFromGridByLastInterviewerName_CallsWithSuitedLastInterviewerName()
        {
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        IsUseDb = true,
                        Interviews = new[]
                        {
                            new InterviewData { Tag = "S1.I1", Call = new CallData(), LastCallPerson = "P2" },     
                            new InterviewData { Tag = "S1.I2", Call = new CallData(), LastCallPerson = "P3" },     
                            new InterviewData { Tag = "S1.I3", Call = new CallData(), LastCallPerson = "P4" }
                        }
                    }
                },
                Persons = new[]
                {
                    new PersonData { Name = "user", Tag = "P1" }, 
                    new PersonData { Name = "user11", Tag = "P2" },
                    new PersonData { Name = "user12", Tag = "P3" }, 
                    new PersonData { Name = "user2", Tag = "P4" } 
                }
            }.Create();

            var search = new SearchParameterCollection
            {
                new SearchParameter
                {
                    ColumnName = "LastInterviewerName",
                    ColumnType = SearchColumnType.Text,
                    Operator = SearchOperator.Like,
                    Value = "user1"
                }
            };
            var args = new PagingArgs(1, 100, "InterviewID", true, search);
            int totalCount;

            // act  
            var result = CallHelper.GetCallsPage(context.GetSurvey("S1").Id, null, _timezoneId, CallStates.All,
                args, out totalCount, ShowTimeMode.Interviewer, false);

            var persons = (from DataRow row in result.Rows select (string)row["LastInterviewerName"]).ToList();

            // assert
            Assert.IsTrue(totalCount == 2);
            Assert.IsTrue(persons.Contains("user11") && persons.Contains("user12"));
        }

        [TestMethod, Owner(@"FIRM\VictorR")]
        public void GetCallsPage_QueryFromGridBySentForReviewStatus_SelectedReviewStatusCalls()
        {
            // arrange
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        IsUseDb = true,
                        Interviews = new[]
                        {          
                            new InterviewData { Tag = "ReviewStarted", Call = new CallData(), ReviewStatus = ReviewStatus.ReviewStarted },
                            new InterviewData { Tag = "NotSentToReview", Call = new CallData(), ReviewStatus = ReviewStatus.NotSentToReview },   
                            new InterviewData { Tag = "ReviewCompleted", Call = new CallData(), ReviewStatus = ReviewStatus.ReviewCompleted },
                            new InterviewData { Tag = "SentToReview", Call = new CallData(), ReviewStatus = ReviewStatus.SentToReview },
                        }
                    }
                }
            }.Create();

            var search = new SearchParameterCollection
            {
                new SearchParameter
                {
                    ColumnName = "ReviewStatus",
                    ColumnType = SearchColumnType.Number,
                    Operator = SearchOperator.Equal,
                    Value = (byte)ReviewStatus.ReviewStarted
                }
            };
            var args = new PagingArgs(1, 100, "InterviewID", true, search);
            int totalCount;

            // act  
            var result = CallHelper.GetCallsPage(context.GetSurvey("S1").Id, null, _timezoneId, CallStates.All,
                args, out totalCount, ShowTimeMode.Interviewer, false);

            // assert
            Assert.IsTrue(totalCount == 1);
            Assert.AreEqual((byte)ReviewStatus.ReviewStarted, (byte)result.Rows[0]["ReviewStatus"]);
            Assert.AreEqual(context.GetInterview("ReviewStarted").Id, result.Rows[0]["InterviewID"]);
        }

        [TestMethod, Owner(@"FIRM\VictorR")]
        public void GetCallsPage_QueryFromCustomFilterByForSentForReviewCalls_SelectedReviewStatusCalls()
        {
            // arrange
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        IsUseDb = true,
                        Interviews = new[]
                        {
                            new InterviewData { Tag = "ReviewStarted", Call = new CallData(), ReviewStatus = ReviewStatus.ReviewStarted },
                            new InterviewData { Tag = "NotSentToReview", Call = new CallData(), ReviewStatus = ReviewStatus.NotSentToReview },
                            new InterviewData { Tag = "ReviewCompleted", Call = new CallData(), ReviewStatus = ReviewStatus.ReviewCompleted },
                            new InterviewData { Tag = "SentToReview", Call = new CallData(), ReviewStatus = ReviewStatus.SentToReview },
                        }
                    }
                }
            }.Create();

            var filter = new BvFiltersEntity
            {
                Name = Guid.NewGuid().ToString(),
                AndOrOperator = (byte)AndOrOperator.And
            };
            var filterId = FilterRepository.Insert(filter);

            var fields = new List<BvFilterFieldsEntity>
            {
                new BvFilterFieldsEntity
                    {
                        Column = "ReviewStatus",
                        Table = (int) TableTypes.Interview,
                        Type = (int) VariableTypes.Integer,
                        Sign = (int) FilterOperator.Equal,
                        Value = ((byte)ReviewStatus.ReviewCompleted).ToString()
                    }
            };
            FilterService.SetFields(filterId, fields);

            var args = new PagingArgs(1, 100, "InterviewID", true);
            int totalCount;

            // act  
            var result = CallHelper.GetCallsPage(context.GetSurvey("S1").Id, filterId, _timezoneId, CallStates.All,
                args, out totalCount, ShowTimeMode.Interviewer, false);

            // assert
            Assert.IsTrue(totalCount == 1);
            Assert.AreEqual((byte)ReviewStatus.ReviewCompleted, (byte)result.Rows[0]["ReviewStatus"]);
            Assert.AreEqual(context.GetInterview("ReviewCompleted").Id, result.Rows[0]["InterviewID"]);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void CallHelper_CombineExistingFilter_AllCallsAreReturned()
        {
            FillSurveyData();

            int surveyId = _filterAndPagingTools.CreateSurveyWithSample("p0000001", SampleType.SmallSample);

            // create filter
            var filter = new BvFiltersEntity
            {
                Name = Guid.NewGuid().ToString(),
                AndOrOperator = (byte)AndOrOperator.And
            };
            int filterId = FilterRepository.Insert(filter);

            var fields = new List<BvFilterFieldsEntity>
            {
                new BvFilterFieldsEntity
                    {
                        Column = "ID",
                        Table = (int) TableTypes.Interview,
                        Type = (int) VariableTypes.Integer,
                        Sign = (int) FilterOperator.Bigger,
                        Value = "3"
                    }
            };
            FilterService.SetFields(filterId, fields);

            int totalCount;
            var search = new SearchParameterCollection
            {
                new SearchParameter
                    {
                        ColumnName = "InterviewID",
                        ColumnType = SearchColumnType.Number,
                        Operator = SearchOperator.LessThanOrEqual,
                        Value = 5
                    }
            };
            var args = new PagingArgs(
                1,
                100,
                "InterviewID",
                true,
                search
            );

            DataTable actual = CallHelper.GetCallsPage(
                surveyId,
                filterId,
                _timezoneId,
                CallStates.All,
                args,
                out totalCount,
                ShowTimeMode.Interviewer,
                false);

            const string expected = @"
InterviewID TelephoneNumber RespondentName LastInterviewerName StateName    LastCallTime             DialingMode ApptTime AttemptNumber ExpTime TimezoneName                                                  TimezoneID ReviewStatus DialTypeName DialTypeId Time CallID Shift_ID ShiftType   CallState Resource ExpireTime InterviewCallID TimeText TimeExportColumn ExpireTimeText ExpireTimeExportColumn LastCallTimeText LastCallTimeExportColumn ApptTimeText ApptTimeExportColumn ExpTimeText ExpTimeExportColumn CallStateText ReviewStatusText   Priority 
4           5553            3                                  Fresh sample 12/30/1899 00:00:00.0000 0           NULL     0             NULL    (GMT+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna  3          0            Landline     0          NULL 4      0        [Any Valid] 0                  NULL       4_4             Now      Now              Never          Never                                                                                                                              NULL          Not sent to review NULL     
5           5554            4                                  Fresh sample 12/30/1899 00:00:00.0000 0           NULL     0             NULL    (GMT+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague 4          0            Landline     0          NULL 5      0        [Any Valid] 0                  NULL       5_5             Now      Now              Never          Never                                                                                                                              NULL          Not sent to review NULL     ";

            Assert.AreEqual(expected, BackendTools.FormatDataTable(actual));
        }


        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void CallHelper_GetCallsWithEmptyTelNumberWhenStringFilterApplies_AllCallsAreReturned()
        {
            FillSurveyData();

            int surveyId = _filterAndPagingTools.CreateSurveyWithSample("p0000001", SampleType.SmallSample);

            // create filter
            var filter = new BvFiltersEntity
            {
                Name = Guid.NewGuid().ToString(),
                AndOrOperator = (byte)AndOrOperator.And
            };
            int filterId = FilterRepository.Insert(filter);

            var fields = new List<BvFilterFieldsEntity>
            {
                new BvFilterFieldsEntity
                {
                    Column = "ID",
                    Table = (int) TableTypes.Interview,
                    Type = (int) VariableTypes.Integer,
                    Sign = (int) FilterOperator.Bigger,
                    Value = "21"
                },
                new BvFilterFieldsEntity
                {
                    Column = "TelephoneNumber",
                    Table = (int) TableTypes.Interview,
                    Type = (int) VariableTypes.String,
                    Sign = (int) FilterOperator.NotEqual,
                    Value = "55521"
                },
                new BvFilterFieldsEntity
                {
                    Column = "TelephoneNumber",
                    Table = (int) TableTypes.Interview,
                    Type = (int) VariableTypes.String,
                    Sign = (int) FilterOperator.NotEqual,
                    Value = "55522"
                },
            };
            FilterService.SetFields(filterId, fields);

            var args = new PagingArgs(1, 100, "InterviewID", true, new SearchParameterCollection());
            DataTable actual = CallHelper.GetCallsPage(
                surveyId,
                filterId,
                _timezoneId,
                CallStates.All,
                args,
                out int totalCount,
                ShowTimeMode.Interviewer,
                false);

            const string expected = @"
InterviewID TelephoneNumber RespondentName LastInterviewerName StateName    LastCallTime             DialingMode ApptTime AttemptNumber ExpTime TimezoneName                                                 TimezoneID ReviewStatus DialTypeName DialTypeId Time CallID Shift_ID ShiftType   CallState Resource ExpireTime InterviewCallID TimeText TimeExportColumn ExpireTimeText ExpireTimeExportColumn LastCallTimeText LastCallTimeExportColumn ApptTimeText ApptTimeExportColumn ExpTimeText ExpTimeExportColumn CallStateText ReviewStatusText   Priority 
24          55523           23                                 Fresh sample 12/30/1899 00:00:00.0000 0           NULL     0             NULL    (GMT+00:00) Monrovia, Reykjavik                              2          0            Landline     0          NULL 24     0        [Any Valid] 0                  NULL       24_24           Now      Now              Never          Never                                                                                                                              NULL          Not sent to review NULL     
25                          24                                 Fresh sample 12/30/1899 00:00:00.0000 0           NULL     0             NULL    (GMT+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna 3          0            Landline     0          NULL 25     0        [Any Valid] 0                  NULL       25_25           Now      Now              Never          Never                                                                                                                              NULL          Not sent to review NULL     ";

            Assert.AreEqual(expected, BackendTools.FormatDataTable(actual));
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void CallHelper_FilterEmptyTelNumber_AllCallsAreReturned()
        {
            FillSurveyData();

            int surveyId = _filterAndPagingTools.CreateSurveyWithSample("p0000001", SampleType.SmallSample);

            // create filter
            var filter = new BvFiltersEntity
            {
                Name = Guid.NewGuid().ToString(),
                AndOrOperator = (byte)AndOrOperator.And
            };
            int filterId = FilterRepository.Insert(filter);

            var fields = new List<BvFilterFieldsEntity>
            {
                new BvFilterFieldsEntity
                {
                    Column = "TelephoneNumber",
                    Table = (int) TableTypes.Interview,
                    Type = (int) VariableTypes.String,
                    Sign = (int) FilterOperator.Equal,
                    Value = ""
                },
            };
            FilterService.SetFields(filterId, fields);

            var args = new PagingArgs(1, 100, "InterviewID", true, new SearchParameterCollection());
            DataTable actual = CallHelper.GetCallsPage(
                surveyId,
                filterId,
                _timezoneId,
                CallStates.All,
                args,
                out int totalCount,
                ShowTimeMode.Interviewer,
                false);

            const string expected = @"
InterviewID TelephoneNumber RespondentName LastInterviewerName StateName    LastCallTime             DialingMode ApptTime AttemptNumber ExpTime TimezoneName                                                 TimezoneID ReviewStatus DialTypeName DialTypeId Time CallID Shift_ID ShiftType   CallState Resource ExpireTime InterviewCallID TimeText TimeExportColumn ExpireTimeText ExpireTimeExportColumn LastCallTimeText LastCallTimeExportColumn ApptTimeText ApptTimeExportColumn ExpTimeText ExpTimeExportColumn CallStateText ReviewStatusText   Priority 
25                          24                                 Fresh sample 12/30/1899 00:00:00.0000 0           NULL     0             NULL    (GMT+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna 3          0            Landline     0          NULL 25     0        [Any Valid] 0                  NULL       25_25           Now      Now              Never          Never                                                                                                                              NULL          Not sent to review NULL     ";

            Assert.AreEqual(expected, BackendTools.FormatDataTable(actual));
        }


        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void CallHelper_CheckThatTemporaryFilterIsDeleted_DeletedSuccessfully()
        {
            int surveyId = _filterAndPagingTools.CreateSurveyWithSample("p00000001", SampleType.SmallSample);

            HashSet<int> oldFilters = GetAllFilters();
            HashSet<int> oldFields = GetAllFields();

            int totalCount;
            var search = new SearchParameterCollection
            {
                new SearchParameter
                    {
                        ColumnName = "InterviewID",
                        ColumnType = SearchColumnType.Number,
                        Operator = SearchOperator.LessThanOrEqual,
                        Value = 5
                    }
            };
            var args = new PagingArgs(
                1,
                100,
                "InterviewID",
                true,
                search
            );

            CallHelper.GetCallsPage(
                surveyId,
                null,
                _timezoneId,
                CallStates.All,
                args,
                out totalCount,
                ShowTimeMode.Interviewer,
                false);

            HashSet<int> newFilters = GetAllFilters();
            HashSet<int> newFields = GetAllFields();

            // checking that we do not add new filter
            Assert.IsTrue(oldFilters.SetEquals(newFilters));
            Assert.IsTrue(oldFields.SetEquals(newFields));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void CallHelper_ConfimitVariablesFilter_AllCallsAreReturned()
        {
            FillSurveyData();

            int surveyId = _filterAndPagingTools.CreateSurveyWithSample("p0000001", SampleType.SmallSample);

            int totalCount;
            var search = new SearchParameterCollection
            {
                new SearchParameter
                    {
                        ColumnName = "Varq1",
                        ColumnType = SearchColumnType.Number,
                        Operator = SearchOperator.LessThanOrEqual,
                        Value = 6
                    }
            };
            var args = new PagingArgs(
                1,
                100,
                "InterviewID",
                true,
                search
            );

            DataTable result = CallHelper.GetCallsPage(
                surveyId,
                null,
                _timezoneId,
                CallStates.All,
                args,
                out totalCount,
                ShowTimeMode.Interviewer,
                false);

            const string expected = @"
InterviewID TelephoneNumber RespondentName LastInterviewerName StateName    LastCallTime             DialingMode ApptTime AttemptNumber ExpTime TimezoneName                    TimezoneID ReviewStatus DialTypeName DialTypeId Time CallID Shift_ID ShiftType   CallState Resource ExpireTime InterviewCallID TimeText TimeExportColumn ExpireTimeText ExpireTimeExportColumn LastCallTimeText LastCallTimeExportColumn ApptTimeText ApptTimeExportColumn ExpTimeText ExpTimeExportColumn CallStateText ReviewStatusText   Priority 
1           5550            0                                  Fresh sample 12/30/1899 00:00:00.0000 0           NULL     0             NULL    NULL                            NULL       0            Landline     0          NULL 1      0        [Any Valid] 0                  NULL       1_1             Now      Now              Never          Never                                                                                                                              NULL          Not sent to review NULL     
3           5552            2                                  Fresh sample 12/30/1899 00:00:00.0000 0           NULL     0             NULL    (GMT+00:00) Monrovia, Reykjavik 2          0            Landline     0          NULL 3      0        [Any Valid] 0                  NULL       3_3             Now      Now              Never          Never                                                                                                                              NULL          Not sent to review NULL     ";

            Assert.AreEqual(expected, BackendTools.FormatDataTable(result));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [Bug(38263)]
        public void CallHelper_StringLikeWithSimplePattern_Success()
        {
            FillSurveyData();

            int surveyId = _filterAndPagingTools.CreateSurveyWithSample("p0000001", SampleType.SmallSample);
            FillRespondentName(surveyId);

            int totalCount;
            var search = new SearchParameterCollection
            {
                new SearchParameter
                    {
                        ColumnName = "RespondentName",
                        ColumnType = SearchColumnType.Text,
                        Operator = SearchOperator.Like,
                        Value = "d"
                    }
            };
            var args = new PagingArgs(
                1,
                100,
                "InterviewID",
                true,
                search
            );

            DataTable result = CallHelper.GetCallsPage(
                surveyId,
                null,
                _timezoneId,
                CallStates.All,
                args,
                out totalCount,
                ShowTimeMode.Interviewer,
                false);

            const string expected = @"
InterviewID TelephoneNumber RespondentName LastInterviewerName StateName    LastCallTime             DialingMode ApptTime AttemptNumber ExpTime TimezoneName                                  TimezoneID ReviewStatus DialTypeName DialTypeId Time CallID Shift_ID ShiftType   CallState Resource ExpireTime InterviewCallID TimeText TimeExportColumn ExpireTimeText ExpireTimeExportColumn LastCallTimeText LastCallTimeExportColumn ApptTimeText ApptTimeExportColumn ExpTimeText ExpTimeExportColumn CallStateText ReviewStatusText   Priority 
1           5550            dump                               Fresh sample 12/30/1899 00:00:00.0000 0           NULL     0             NULL    NULL                                          NULL       0            Landline     0          NULL 1      0        [Any Valid] 0                  NULL       1_1             Now      Now              Never          Never                                                                                                                              NULL          Not sent to review NULL     
2           5551            dump                               Fresh sample 12/30/1899 00:00:00.0000 0           NULL     0             NULL    (GMT+00:00) Dublin, Edinburgh, Lisbon, London 1          0            Landline     0          NULL 2      0        [Any Valid] 0                  NULL       2_2             Now      Now              Never          Never                                                                                                                              NULL          Not sent to review NULL     ";

            Assert.AreEqual(expected, BackendTools.FormatDataTable(result));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [Bug(38263)]
        public void CallHelper_StringLikeWithAsteriskPattern_Success()
        {
            FillSurveyData();

            int surveyId = _filterAndPagingTools.CreateSurveyWithSample("p0000001", SampleType.SmallSample);
            FillRespondentName(surveyId);

            int totalCount;
            var search = new SearchParameterCollection
            {
                new SearchParameter
                    {
                        ColumnName = "RespondentName",
                        ColumnType = SearchColumnType.Text,
                        Operator = SearchOperator.Like,
                        Value = "auc*on"
                    }
            };
            var args = new PagingArgs(
                1,
                100,
                "InterviewID",
                true,
                search
            );

            DataTable result = CallHelper.GetCallsPage(
                surveyId,
                null,
                _timezoneId,
                CallStates.All,
                args,
                out totalCount,
                ShowTimeMode.Interviewer,
                false);

            const string expected = @"
InterviewID TelephoneNumber RespondentName LastInterviewerName StateName    LastCallTime             DialingMode ApptTime AttemptNumber ExpTime TimezoneName                                                 TimezoneID ReviewStatus DialTypeName DialTypeId Time CallID Shift_ID ShiftType   CallState Resource ExpireTime InterviewCallID TimeText TimeExportColumn ExpireTimeText ExpireTimeExportColumn LastCallTimeText LastCallTimeExportColumn ApptTimeText ApptTimeExportColumn ExpTimeText ExpTimeExportColumn CallStateText ReviewStatusText   Priority 
3           5552            auction                            Fresh sample 12/30/1899 00:00:00.0000 0           NULL     0             NULL    (GMT+00:00) Monrovia, Reykjavik                              2          0            Landline     0          NULL 3      0        [Any Valid] 0                  NULL       3_3             Now      Now              Never          Never                                                                                                                              NULL          Not sent to review NULL     
4           5553            auction                            Fresh sample 12/30/1899 00:00:00.0000 0           NULL     0             NULL    (GMT+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna 3          0            Landline     0          NULL 4      0        [Any Valid] 0                  NULL       4_4             Now      Now              Never          Never                                                                                                                              NULL          Not sent to review NULL     ";

            Assert.AreEqual(expected, BackendTools.FormatDataTable(result));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [Bug(38263)]
        public void CallHelper_StringLikeWithUnderlinePattern_Success()
        {
            FillSurveyData();

            int surveyId = _filterAndPagingTools.CreateSurveyWithSample("p0000001", SampleType.SmallSample);
            FillRespondentName(surveyId);

            int totalCount;
            var search = new SearchParameterCollection
            {
                new SearchParameter
                    {
                        ColumnName = "RespondentName",
                        ColumnType = SearchColumnType.Text,
                        Operator = SearchOperator.Like,
                        Value = "r_"
                    }
            };
            var args = new PagingArgs(
                1,
                100,
                "InterviewID",
                true,
                search
            );

            DataTable result = CallHelper.GetCallsPage(
                surveyId,
                null,
                _timezoneId,
                CallStates.All,
                args,
                out totalCount,
                ShowTimeMode.Interviewer,
                false);

            const string expected = @"
InterviewID TelephoneNumber RespondentName LastInterviewerName StateName    LastCallTime             DialingMode ApptTime AttemptNumber ExpTime TimezoneName                                                  TimezoneID ReviewStatus DialTypeName DialTypeId Time CallID Shift_ID ShiftType   CallState Resource ExpireTime InterviewCallID TimeText TimeExportColumn ExpireTimeText ExpireTimeExportColumn LastCallTimeText LastCallTimeExportColumn ApptTimeText ApptTimeExportColumn ExpTimeText ExpTimeExportColumn CallStateText ReviewStatusText   Priority 
5           5554            r_bel                              Fresh sample 12/30/1899 00:00:00.0000 0           NULL     0             NULL    (GMT+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague 4          0            Landline     0          NULL 5      0        [Any Valid] 0                  NULL       5_5             Now      Now              Never          Never                                                                                                                              NULL          Not sent to review NULL     
6           5555            r_bel                              Fresh sample 12/30/1899 00:00:00.0000 0           NULL     0             NULL    (GMT+01:00) Brussels, Copenhagen, Madrid, Paris               5          0            Landline     0          NULL 6      0        [Any Valid] 0                  NULL       6_6             Now      Now              Never          Never                                                                                                                              NULL          Not sent to review NULL     ";

            Assert.AreEqual(expected, BackendTools.FormatDataTable(result));
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void Search_FilterByNumeric_FilteringIsOK()
        {
            FillSurveyData();

            _filterAndPagingTools.AddAdditionalColumnsToRespondentTable(_confirmitSurveyDb, new[] { "numeric" });

            var c1 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 32, Name = "CallAttemptCount", QuotaIds = null };
            var c2 = new ReplicationColumnInfo { DataType = SqlDataType.Numeric, NumericPrecision = 4, NumericScale = 2, Id = 4, Name = "numeric", QuotaIds = null };
            var p = new ColumnInfo { DataType = SqlDataType.Int, Name = "respid" };

            var t = new TableInfo { Name = "respondent", ReplicationColumns = new[] { c1, c2 }, PrimaryKeyColumns = new[] { p } };

            _confirmitSurveyDb.ExecuteNonQuery("UPDATE respondent SET [numeric] = CAST( [respid] AS FLOAT ) / 3 WHERE respId % 2 = 1", CommandType.Text);

            var surveyId = _filterAndPagingTools.CreateSurveyWithSample("p00012", new[] { t }, SampleType.SmallSample);

            var args = new PagingArgs(
                1,
                2,
                "numeric",
                true,
                new SearchParameterCollection(){new SearchParameter()
                                            {
                                                ColumnName = "Varnumeric", 
                                                ColumnType = SearchColumnType.Decimal, 
                                                Operator = SearchOperator.Greater, 
                                                Value = 1.4
                                            }}
            );

            int totalCount = 0;
            DataTable actual = CallHelper.GetCallsPage(
                surveyId,
                0,
                _timezoneId,
                CallStates.All,
                args,
                out totalCount,
                ShowTimeMode.Interviewer,
                false,
                "numeric");

            const string expected = @"
Varnumeric InterviewID TelephoneNumber RespondentName LastInterviewerName StateName    LastCallTime             DialingMode ApptTime AttemptNumber ExpTime TimezoneName                                                  TimezoneID ReviewStatus DialTypeName DialTypeId Time CallID Shift_ID ShiftType   CallState Resource ExpireTime InterviewCallID TimeText TimeExportColumn ExpireTimeText ExpireTimeExportColumn LastCallTimeText LastCallTimeExportColumn ApptTimeText ApptTimeExportColumn ExpTimeText ExpTimeExportColumn CallStateText ReviewStatusText   Priority 
1.66       5           5554            4                                  Fresh sample 12/30/1899 00:00:00.0000 0           NULL     0             NULL    (GMT+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague 4          0            Landline     0          NULL 5      0        [Any Valid] 0                  NULL       5_5             Now      Now              Never          Never                                                                                                                              NULL          Not sent to review NULL     
2.33       7           5556            6                                  Fresh sample 12/30/1899 00:00:00.0000 0           NULL     0             NULL    (GMT+01:00) Sarajevo, Skopje, Warsaw, Zagreb                  6          0            Landline     0          NULL 7      0        [Any Valid] 0                  NULL       7_7             Now      Now              Never          Never                                                                                                                              NULL          Not sent to review NULL     ";
            Assert.AreEqual(expected, BackendTools.FormatDataTable(actual));
        }

        [TestMethod, Owner(@"FIRM\VictorR")]
        public void GetCallsPage_FilterByDialTypeManual_Success()
        {
            // arrange
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        IsUseDb = true,
                        Interviews = new[]
                        {          
                            new InterviewData { Tag = "S1.I1", Call = new CallData(), DialType = DialType.Landline },
                            new InterviewData { Tag = "S1.I2", Call = new CallData(), DialType = DialType.Cellphone },   
                            new InterviewData { Tag = "S1.I3", Call = new CallData(), DialType = DialType.Landline }
                        }
                    }
                }
            }.Create();

            var search = new SearchParameterCollection
            {
                new SearchParameter
                {
                    ColumnName = "DialTypeId",
                    ColumnType = SearchColumnType.Number,
                    Operator = SearchOperator.Equal,
                    Value = (byte)DialType.Cellphone
                }
            };
            var args = new PagingArgs(1, 100, "InterviewID", true, search);
            int totalCount;

            // act  
            var result = CallHelper.GetCallsPage(context.GetSurvey("S1").Id, null, _timezoneId, CallStates.All,
                args, out totalCount, ShowTimeMode.Interviewer, false);

            // assert
            Assert.IsTrue(totalCount == 1);
            Assert.AreEqual((byte)DialType.Cellphone, (byte)result.Rows[0]["DialTypeId"]);
            Assert.AreEqual(context.GetInterview("S1.I2").Id, result.Rows[0]["InterviewID"]);
        }

        [TestMethod, Owner(@"FIRM\VictorR")]
        public void GetCallsPage_FilterByDialTypeAutomatic_Success()
        {
            // arrange
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        IsUseDb = true,
                        Interviews = new[]
                        {          
                            new InterviewData { Tag = "S1.I1", Call = new CallData(), DialType = DialType.Landline },
                            new InterviewData { Tag = "S1.I2", Call = new CallData(), DialType = DialType.Cellphone },   
                            new InterviewData { Tag = "S1.I3", Call = new CallData(), DialType = DialType.Landline }
                        }
                    }
                }
            }.Create();

            var search = new SearchParameterCollection
            {
                new SearchParameter
                {
                    ColumnName = "DialTypeId",
                    ColumnType = SearchColumnType.Number,
                    Operator = SearchOperator.Equal,
                    Value = (byte)DialType.Landline
                }
            };
            var args = new PagingArgs(1, 100, "InterviewID", true, search);
            int totalCount;

            // act  
            var result = CallHelper.GetCallsPage(context.GetSurvey("S1").Id, null, _timezoneId, CallStates.All,
                args, out totalCount, ShowTimeMode.Interviewer, false);

            // assert
            var expected = context.GetInterviews("S1.I1", "S1.I3").Select(x => x.Id).ToArray();
            var actual = result.Rows.Cast<DataRow>().Select(x => (int)(x["InterviewID"])).ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod, Owner(@"FIRM\VictorR")]
        public void GetCallsPage_OrderByDialType_Success()
        {
            // arrange
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        IsUseDb = true,
                        Interviews = new[]
                        {          
                            new InterviewData { Tag = "S1.I1", Call = new CallData(), DialType = DialType.Landline },
                            new InterviewData { Tag = "S1.I2", Call = new CallData(), DialType = DialType.Cellphone },   
                            new InterviewData { Tag = "S1.I3", Call = new CallData(), DialType = DialType.Landline }
                        }
                    }
                }
            }.Create();

            var args = new PagingArgs(1, 100, "DialTypeId", false);
            int totalCount;

            // act  
            var result = CallHelper.GetCallsPage(context.GetSurvey("S1").Id, null, _timezoneId, CallStates.All,
                args, out totalCount, ShowTimeMode.Interviewer, false);

            // assert

            var expected = context.GetInterviewsInOrder("S1.I2", "S1.I1", "S1.I3").Select(x => x.Id).ToArray();
            var actual = result.Rows.Cast<DataRow>().Select(x => (int)(x["InterviewID"])).ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
