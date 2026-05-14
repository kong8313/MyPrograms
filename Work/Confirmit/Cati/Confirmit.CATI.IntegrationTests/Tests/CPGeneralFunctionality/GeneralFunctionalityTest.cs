using System;
using System.Collections.Generic;
using System.Linq;

using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.FilterServiceImplementation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Data.Builders;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Tests.AsyncOperations;
using Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tools;
using Confirmit.CATI.Supervisor.Backend.Assignment;
using Confirmit.CATI.Supervisor.Classes.Filters;
using Confirmit.CATI.Supervisor.Core.Activity;
using Confirmit.CATI.Supervisor.Core.Assignment;
using Confirmit.CATI.Supervisor.Core.Persons;
using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using FilterData = Confirmit.CATI.IntegrationTests.Framework.Data.FilterData;

namespace Confirmit.CATI.IntegrationTests.Tests.CPGeneralFunctionality
{
    [TestClass]
    public class GeneralFunctionalityTest
    {
        const string SurveyName = "p000001";
        const string UserName = "grigoryk";
        const string PersonName = "TestPerson";

        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;
        private FilterAndPagingTools _filterAndPagingTools;

        private DatabaseEngine _confirmitSurveyDb;
        private int _timezoneId;
        private IAssignmentManager _assignmentManager;
        private IScheduleService _scheduleService;
        private IInterviewRepository _interviewRepository;
        private IActivityManager _activityManager;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);
            _filterAndPagingTools = new FilterAndPagingTools(_framework, _backendTools);
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();

            _confirmitSurveyDb = _filterAndPagingTools.CreateCFSurveyDatabaseEngine();
            _timezoneId = ServiceLocator.Resolve<ITimezoneService>().GetDefaultCallCenterTimezoneId();
            _assignmentManager = ServiceLocator.Resolve<IAssignmentManager>();
            _scheduleService = ServiceLocator.Resolve<IScheduleService>();
            _interviewRepository = ServiceLocator.Resolve<IInterviewRepository>();
            _activityManager = ServiceLocator.Resolve<IActivityManager>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            new SqlObjectCreator(_framework).CleanTablesInSurveyDatabase(_framework.TestSurveyDatabaseName);

            _framework.TestCleanup();
        }

        private ISurveyStateService _surveyStateService;

        private void FillSurveyData()
        {
            new SqlObjectCreator(_framework).CleanTablesInSurveyDatabase(_framework.TestSurveyDatabaseName);

            var sdb = new SurveyDatabaseBuilder(_confirmitSurveyDb);
            const int batchId = 1;
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "0", InterviewerId = "1", TelephoneNumber = "5550", ExtensionNumber = "0", LastChannelId = "1", TimeZoneId = "0", RespondentName = "0", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "1", InterviewerId = "2", TelephoneNumber = "5551", ExtensionNumber = "1", LastChannelId = "1", TimeZoneId = "1", RespondentName = "1", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "2", InterviewerId = "3", TelephoneNumber = "5552", ExtensionNumber = "2", LastChannelId = "1", TimeZoneId = "2", RespondentName = "2", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "3", InterviewerId = "4", TelephoneNumber = "5553", ExtensionNumber = "3", LastChannelId = "1", TimeZoneId = "3", RespondentName = "3", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "4", InterviewerId = "5", TelephoneNumber = "5554", ExtensionNumber = "4", LastChannelId = "1", TimeZoneId = "4", RespondentName = "4", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "5", InterviewerId = "6", TelephoneNumber = "5555", ExtensionNumber = "5", LastChannelId = "1", TimeZoneId = "5", RespondentName = "5", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "6", InterviewerId = "7", TelephoneNumber = "5556", ExtensionNumber = "6", LastChannelId = "1", TimeZoneId = "6", RespondentName = "6", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "7", InterviewerId = "8", TelephoneNumber = "5557", ExtensionNumber = "7", LastChannelId = "1", TimeZoneId = "0", RespondentName = "7", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "8", InterviewerId = "9", TelephoneNumber = "5558", ExtensionNumber = "8", LastChannelId = "1", TimeZoneId = "1", RespondentName = "8", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "9", InterviewerId = "10", TelephoneNumber = "5559", ExtensionNumber = "9", LastChannelId = "1", TimeZoneId = "2", RespondentName = "9", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "10", InterviewerId = "11", TelephoneNumber = "55510", ExtensionNumber = "10", LastChannelId = "1", TimeZoneId = "3", RespondentName = "10", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "11", InterviewerId = "12", TelephoneNumber = "55511", ExtensionNumber = "11", LastChannelId = "1", TimeZoneId = "4", RespondentName = "11", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "12", InterviewerId = "13", TelephoneNumber = "55512", ExtensionNumber = "12", LastChannelId = "1", TimeZoneId = "5", RespondentName = "12", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "13", InterviewerId = "14", TelephoneNumber = "55513", ExtensionNumber = "13", LastChannelId = "1", TimeZoneId = "6", RespondentName = "13", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "14", InterviewerId = "15", TelephoneNumber = "55514", ExtensionNumber = "14", LastChannelId = "1", TimeZoneId = "0", RespondentName = "14", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "15", InterviewerId = "16", TelephoneNumber = "55515", ExtensionNumber = "15", LastChannelId = "1", TimeZoneId = "1", RespondentName = "15", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "16", InterviewerId = "17", TelephoneNumber = "55516", ExtensionNumber = "16", LastChannelId = "1", TimeZoneId = "2", RespondentName = "16", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "17", InterviewerId = "18", TelephoneNumber = "55517", ExtensionNumber = "17", LastChannelId = "1", TimeZoneId = "3", RespondentName = "17", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "18", InterviewerId = "19", TelephoneNumber = "55518", ExtensionNumber = "18", LastChannelId = "1", TimeZoneId = "4", RespondentName = "18", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "19", InterviewerId = "20", TelephoneNumber = "55519", ExtensionNumber = "19", LastChannelId = "1", TimeZoneId = "5", RespondentName = "19", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "20", InterviewerId = "21", TelephoneNumber = "55520", ExtensionNumber = "20", LastChannelId = "1", TimeZoneId = "6", RespondentName = "20", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "21", InterviewerId = "22", TelephoneNumber = "55521", ExtensionNumber = "21", LastChannelId = "1", TimeZoneId = "0", RespondentName = "21", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "22", InterviewerId = "23", TelephoneNumber = "55522", ExtensionNumber = "22", LastChannelId = "1", TimeZoneId = "1", RespondentName = "22", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "23", InterviewerId = "24", TelephoneNumber = "55523", ExtensionNumber = "23", LastChannelId = "1", TimeZoneId = "2", RespondentName = "23", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "24", InterviewerId = "25", TelephoneNumber = "55524", ExtensionNumber = "24", LastChannelId = "1", TimeZoneId = "3", RespondentName = "24", DialMode = "1" });
        }

        /// <summary>
        /// 1. Create survey using BackendTools.CreateAndAssignSurvey method                
        /// 2. Set access for temp user using ManagementService().UpdateSurveyAccessList method
        /// 3. Get survey list using SurveyRepository().GetPage method
        /// 4. Check that method return one survey with correct name
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void SurveyRepositoryTest_GetPage_Successfully()
        {
            // Create survey
            _backendTools.CreateSurvey(SurveyName, true);

            // Set access            
            new ManagementService().UpdateSurveyAccessList(UserName, SurveyName, true);

            // Get survey list
            var pagingArgs = new PagingArgs(
                1 /*PageIndex*/,
                20 /*PageSize*/,
                "SID" /*SortedColumnKey*/,
                true /*SortIndicatorAsc*/);

            int totalCount;
            var dataList = SurveyRepository.GetPage(CallCenterTools.DefaultId, pagingArgs, UserName, out totalCount);

            Assert.AreEqual(1, dataList.Count, "GetPage return wrong survey count: " + dataList.Count);
            Assert.AreEqual(SurveyName, dataList[0].Name, "GetPage return wrong survey name: " + dataList[0].Name);
        }


        /// <summary>
        /// 1. Get default sheduling scripts using ScheduleRepository.GetPage method
        /// 2. Check that method return 2 scripts with correct names
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void ScheduleRepositoryTest_GetPage_Successfully()
        {
            // Get sheduling script list
            var pagingArgs = new PagingArgs(
                1 /*PageIndex*/,
                20 /*PageSize*/,
                "SID" /*SortedColumnKey*/,
                true /*SortIndicatorAsc*/);

            int totalCount;
            var dataList = ScheduleRepository.GetPage(pagingArgs, 1, out totalCount);

            Assert.AreEqual(2, dataList.Count, "GetPage return wrong sheduling script count: " + dataList.Count);
            Assert.AreEqual("Default Schedule", dataList[0].Name, "GetPage return wrong sheduling script name: " + dataList[0].Name);
            Assert.AreEqual((int)SchedulingScriptState.NotLaunched, dataList[0].State, "GetPage return wrong sheduling script state: " + dataList[0].Name);

            ScheduleService.Launch(_scheduleService.DefaultScheduleId);

            dataList = ScheduleRepository.GetPage(pagingArgs, 1, out totalCount);

            Assert.AreEqual(2, dataList.Count, "GetPage return wrong sheduling script count: " + dataList.Count);
            Assert.AreEqual("Default Schedule", dataList[0].Name, "GetPage return wrong sheduling script name: " + dataList[0].Name);
            Assert.AreEqual((int)SchedulingScriptState.Synchronized, dataList[0].State, "GetPage return wrong sheduling script state: " + dataList[0].Name);
        }

        /// <summary>
        /// 1. Create survey with sample using FilterAndPagingTools.CreateSurveyWithSample method                        
        /// 2. Assign 5 first interview to other ITS
        /// 3. Get information about interview using SurveyService.GetSampleStatusSummary method
        /// 4. Check that method return states with correct distribution of interview types
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void SurveyServiceTest_GetSampleStatusSummary_Successfully()
        {
            FillSurveyData();

            // Create survey
            int surveySid = _filterAndPagingTools.CreateSurveyWithSample(SurveyName, FilterAndPagingTools.SampleType.SmallSample);

            CallTools.MoveCalls(surveySid, new[] { 1, 2 }, 1);
            CallTools.MoveCalls(surveySid, new[] { 3, 4 }, 10);
            CallTools.MoveCalls(surveySid, new[] { 5 }, 119);

            var dataList = SurveyService.GetSampleStatusSummary(surveySid, null, null);

            var samples1 = Convert.ToInt32(dataList[0].count);
            var samples10 = Convert.ToInt32(dataList[1].count);
            var freshSamples = Convert.ToInt32(dataList[2].count);
            var samples119 = Convert.ToInt32(dataList[3].count);

            Assert.AreEqual(4, dataList.Count, "GetSampleStatusSummary return wrong all samples count");
            Assert.AreEqual(20, freshSamples, "GetSampleStatusSummary return wrong sample count for fresh samples");
            Assert.AreEqual(2, samples1, "GetSampleStatusSummary return wrong sample count for samples with id=1");
            Assert.AreEqual(2, samples10, "GetSampleStatusSummary return wrong sample count for samples with id=10");
            Assert.AreEqual(1, samples119, "GetSampleStatusSummary return wrong sample count for samples with id=119");
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void SurveyServiceTest_GetSampleStatusSummaryWithCallCount_Successfully()
        {
            var context = new TestData()
            {
                Surveys = new[]{new SurveyData(){ Tag="S1",
                    Interviews = new []
                    {
                        new InterviewData(1){ITS = CallOutcome.FreshSample, Call = new CallData()},
                        new InterviewData(2){ITS = CallOutcome.FreshSample, Call = new CallData(){CallState = 1}},
                        new InterviewData(3){ITS = CallOutcome.FreshSample, Call = new CallData(){CallState = 3}},
                        new InterviewData(4){ITS = CallOutcome.FreshSample},
                        new InterviewData(2){ITS = CallOutcome.Busy, Call = new CallData()},
                        new InterviewData(3){ITS = CallOutcome.Busy, Call = new CallData(){CallState = 1}},
                        new InterviewData(4){ITS = CallOutcome.Busy, Call = new CallData(){CallState = 3}},
                        new InterviewData(5){ITS = CallOutcome.Busy},
                    }
                }}
            }.Create();

            var dataList = SurveyService.GetSampleStatusSummary(context.GetSurvey("S1").Id, null, null);

            Assert.AreEqual(BackendTools.Format(dataList), @"
 id         name count fcd_disabled_call enabled_call user_disabled_call sample_size
  2         Busy    14                 3            2                  4          24
 16 Fresh sample    10                 2            1                  3          24");
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void SurveyServiceTest_GetSampleStatusSummaryWithExcludeFreshSample_Successfully()
        {
            var context = new TestData()
            {
                Surveys = new[]{new SurveyData(){ Tag="S1",
                    Interviews = new []
                    {
                        new InterviewData(1){ITS = CallOutcome.FreshSample, Call = new CallData()},
                        new InterviewData(2){ITS = CallOutcome.FreshSample, Call = new CallData(){CallState = 1}},
                        new InterviewData(3){ITS = CallOutcome.FreshSample, Call = new CallData(){CallState = 3}},
                        new InterviewData(4){ITS = CallOutcome.FreshSample},
                        new InterviewData(2){ITS = CallOutcome.Busy, Call = new CallData()},
                        new InterviewData(3){ITS = CallOutcome.Busy, Call = new CallData(){CallState = 1}},
                        new InterviewData(4){ITS = CallOutcome.Busy, Call = new CallData(){CallState = 3}},
                        new InterviewData(5){ITS = CallOutcome.Busy},
                    }
                }}
            }.Create();

            var dataList = SurveyService.GetSampleStatusSummary(context.GetSurvey("S1").Id, null, new int[] { (int)CallOutcome.FreshSample });

            Assert.AreEqual(BackendTools.Format(dataList), @"
 id name count fcd_disabled_call enabled_call user_disabled_call sample_size
  2 Busy    14                 3            2                  4          14");
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void SurveyServiceTest_GetSampleStatusSummaryByFilterWithExcludeFreshSample_Successfully()
        {
            var context = new TestData()
            {
                Surveys = new[]{new SurveyData(){ Tag="S1",
                    Interviews = new []
                    {
                        new InterviewData(1){ITS = CallOutcome.FreshSample, Call = new CallData()},
                        new InterviewData(2){ITS = CallOutcome.FreshSample, Call = new CallData(){CallState = 1}},
                        new InterviewData(3){ITS = CallOutcome.FreshSample, Call = new CallData(){CallState = 3}},
                        new InterviewData(4){ITS = CallOutcome.FreshSample},
                        new InterviewData(2){ITS = CallOutcome.Busy, Call = new CallData()},
                        new InterviewData(3){ITS = CallOutcome.Busy, Call = new CallData(){CallState = 1}},
                        new InterviewData(4){ITS = CallOutcome.Busy, Call = new CallData(){CallState = 3}},
                        new InterviewData(5){ITS = CallOutcome.Busy},
                        new InterviewData(3){ITS = CallOutcome.NoReply, Call = new CallData()},
                        new InterviewData(4){ITS = CallOutcome.NoReply, Call = new CallData(){CallState = 1}},
                        new InterviewData(5){ITS = CallOutcome.NoReply, Call = new CallData(){CallState = 3}},
                        new InterviewData(6){ITS = CallOutcome.NoReply},
                    }
                }},
                Filters = new[] {
                    new FilterData(){Tag="F1", Conditions = new[]{ "Interview.TransientState=2"}
                        }},
            }.Create();

            var filter = context.GetFilter("F1");

            var dataList = SurveyService.GetSampleStatusSummary(context.GetSurvey("S1").Id, filter.Id, new[] { (int)CallOutcome.FreshSample });

            Assert.AreEqual(BackendTools.Format(dataList), @"
 id name count fcd_disabled_call enabled_call user_disabled_call sample_size
  2 Busy    14                 3            2                  4          32");
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void SurveyServiceTest_GetSampleStatusSummaryByFilter_Successfully()
        {
            var context = new TestData()
            {
                Surveys = new[]{new SurveyData(){ Tag="S1",
                    Interviews = new []
                    {
                        new InterviewData(1){ITS = CallOutcome.FreshSample, Call = new CallData()},
                        new InterviewData(2){ITS = CallOutcome.FreshSample, Call = new CallData(){CallState = 1}},
                        new InterviewData(3){ITS = CallOutcome.FreshSample, Call = new CallData(){CallState = 3}},
                        new InterviewData(4){ITS = CallOutcome.FreshSample},
                        new InterviewData(2){ITS = CallOutcome.Busy, Call = new CallData()},
                        new InterviewData(3){ITS = CallOutcome.Busy, Call = new CallData(){CallState = 1}},
                        new InterviewData(4){ITS = CallOutcome.Busy, Call = new CallData(){CallState = 3}},
                        new InterviewData(5){ITS = CallOutcome.Busy},
                        new InterviewData(3){ITS = CallOutcome.NoReply, Call = new CallData()},
                        new InterviewData(4){ITS = CallOutcome.NoReply, Call = new CallData(){CallState = 1}},
                        new InterviewData(5){ITS = CallOutcome.NoReply, Call = new CallData(){CallState = 3}},
                        new InterviewData(6){ITS = CallOutcome.NoReply},
                    }
                }},
                Filters = new[] {
                    new FilterData(){Tag="F1", Join = FilterJoinType.Or, Conditions = new[]{ "Interview.TransientState=2", "Interview.TransientState=3"}
                    }},
            }.Create();

            var filter = context.GetFilter("F1");

            var dataList = SurveyService.GetSampleStatusSummary(context.GetSurvey("S1").Id, filter.Id, null);

            Assert.AreEqual(BackendTools.Format(dataList), @"
 id     name count fcd_disabled_call enabled_call user_disabled_call sample_size
  2     Busy    14                 3            2                  4          42
  3 No reply    18                 4            3                  5          42");
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void SurveyServiceTest_GetSampleStatusSummaryByOrFilterWithExcludeFreshSample_Successfully()
        {
            var context = new TestData()
            {
                Surveys = new[]{new SurveyData(){ Tag="S1",
                    Interviews = new []
                    {
                        new InterviewData(1){ITS = CallOutcome.FreshSample, Call = new CallData()},
                        new InterviewData(2){ITS = CallOutcome.FreshSample, Call = new CallData(){CallState = 1}},
                        new InterviewData(3){ITS = CallOutcome.FreshSample, Call = new CallData(){CallState = 3}},
                        new InterviewData(4){ITS = CallOutcome.FreshSample},
                        new InterviewData(2){ITS = CallOutcome.Busy, Call = new CallData()},
                        new InterviewData(3){ITS = CallOutcome.Busy, Call = new CallData(){CallState = 1}},
                        new InterviewData(4){ITS = CallOutcome.Busy, Call = new CallData(){CallState = 3}},
                        new InterviewData(5){ITS = CallOutcome.Busy},
                        new InterviewData(3){ITS = CallOutcome.NoReply, Call = new CallData()},
                        new InterviewData(4){ITS = CallOutcome.NoReply, Call = new CallData(){CallState = 1}},
                        new InterviewData(5){ITS = CallOutcome.NoReply, Call = new CallData(){CallState = 3}},
                        new InterviewData(6){ITS = CallOutcome.NoReply},
                    }
                }},
                Filters = new[] {
                    new FilterData(){Tag="F1", Join = FilterJoinType.Or, Conditions = new[]{ "Interview.TransientState=2", "Interview.TransientState=3"}
                    }},
            }.Create();

            var filter = context.GetFilter("F1");

            var dataList = SurveyService.GetSampleStatusSummary(context.GetSurvey("S1").Id, filter.Id, new[] { (int)CallOutcome.FreshSample });

            Assert.AreEqual(BackendTools.Format(dataList), @"
 id     name count fcd_disabled_call enabled_call user_disabled_call sample_size
  2     Busy    14                 3            2                  4          32
  3 No reply    18                 4            3                  5          32");
        }

        /// <summary>
        /// 1. Create survey with sample
        /// 2. Assign 5 first calls to different ITS
        /// 3. Create filter (Priority=3)
        /// 4. Get information about interviews states counters using SurveyService.GetSampleStatusSummary method
        /// 5. Check that method returns states with correct distribution of interviews ITSs
        /// </summary>
        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void SurveyServiceTest_GetSampleStatusSummaryWithFiltering_Successfully()
        {
            FillSurveyData();

            int surveySid = _filterAndPagingTools.CreateSurveyWithSample(SurveyName, FilterAndPagingTools.SampleType.SmallSample);

            CallTools.MoveCalls(surveySid, new[] { 1, 2 }, 1);
            CallTools.MoveCalls(surveySid, new[] { 3, 4 }, 10);
            CallTools.MoveCalls(surveySid, new[] { 5 }, 119);

            CallTools.ChangeCallsPriority(surveySid, new[] { 1, 2, 3, 6 }, CallStates.Scheduled, 3);

            int filterId = FilterRepository.Insert(
                new BvFiltersEntity
                {
                    Name = "Priority=3",
                    Description = "description",
                    SurveySID = surveySid
                });

            var fields = new List<BvFilterFieldsEntity>
            {
                new BvFilterFieldsEntity
                {
                    Column = "Priority",
                    Table = (int)TableTypes.Call,
                    Type = (int)VariableTypes.Integer,
                    Sign = (int)FilterOperator.Equal,
                    Value = "3"
                }
            };

            FilterService.SetFields(filterId, fields);

            var dataList = SurveyService.GetSampleStatusSummary(surveySid, filterId, null);

            var samples1 = Convert.ToInt32(dataList[0].count);
            var samples10 = Convert.ToInt32(dataList[1].count);
            var freshSamples = Convert.ToInt32(dataList[2].count);

            Assert.AreEqual(3, dataList.Count, "GetSampleStatusSummary return wrong all samples count");
            Assert.AreEqual(1, freshSamples, "GetSampleStatusSummary return wrong sample count for fresh samples");
            Assert.AreEqual(2, samples1, "GetSampleStatusSummary return wrong sample count for samples with id=1");
            Assert.AreEqual(1, samples10, "GetSampleStatusSummary return wrong sample count for samples with id=10");
        }

        /// <summary>
        /// 1. Create survey with sample
        /// 2. Create 2 interviews without calls (suspended calls)
        /// 3. Assign 5 first calls and 2 interviews to different ITS
        /// 4. Create filter (TransientState=10)
        /// 5. Get information about interviews states counters using SurveyService.GetSampleStatusSummary method
        /// 6. Check that method returns states with correct distribution of interviews ITSs
        /// </summary>
        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void SurveyServiceTest_GetSampleStatusSummaryWithFilteringAndSuspendedCalls_Successfully()
        {
            FillSurveyData();

            int surveySid = _filterAndPagingTools.CreateSurveyWithSample(SurveyName, FilterAndPagingTools.SampleType.SmallSample);

            // Create 2 interviews without call           
            var interview = new BvInterviewEntity
            {
                ID = 100,
                SurveySID = surveySid,
                TransientState = (int)CallOutcome.FreshSample
            };

            interview.TransientState = 10;
            BackendTools.CreateInterview(interview);

            interview = new BvInterviewEntity
            {
                ID = 101,
                SurveySID = surveySid,
                TransientState = (int)CallOutcome.FreshSample
            };
            interview.TransientState = 11;
            BackendTools.CreateInterview(interview);

            CallTools.MoveCalls(surveySid, new[] { 1, 2 }, 1);
            CallTools.MoveCalls(surveySid, new[] { 3, 4 }, 10);
            CallTools.MoveCalls(surveySid, new[] { 5 }, 119);

            int filterId = FilterRepository.Insert(new BvFiltersEntity
            {
                Name = "TransientState=10",
                Description = "description",
                SurveySID = surveySid
            });

            var fields = new List<BvFilterFieldsEntity>
                            {
                                new BvFilterFieldsEntity
                                    {
                                        Column = "TransientState",
                                        Table = (int)TableTypes.Interview,
                                        Type = (int)VariableTypes.Integer,
                                        Sign = (int)FilterOperator.Equal,
                                        Value = "10"
                                    }
                            };

            FilterService.SetFields(filterId, fields);

            var dataList = SurveyService.GetSampleStatusSummary(surveySid, filterId, null);

            var samples10 = Convert.ToInt32(dataList[0].count);

            Assert.AreEqual(1, dataList.Count, "GetSampleStatusSummary return wrong all samples count");
            Assert.AreEqual(3, samples10, "GetSampleStatusSummary return wrong sample count for samples with id=10");
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh"), Bug(38847)]
        public void FilterTest_GetSampleStatusSummaryWithFilterByAttemptNumber_Successfully()
        {
            FillSurveyData();

            int surveySid = _filterAndPagingTools.CreateSurveyWithSample(SurveyName, FilterAndPagingTools.SampleType.SmallSample);

            FilterAndPagingTools.UpdateCallAttemptCount(_confirmitSurveyDb);

            int filterId = FilterRepository.Insert(new BvFiltersEntity
            {
                Name = "Filter name",
                Description = "AttemptNumber has to less or equal to 5",
                SurveySID = surveySid
            });

            var fields = new List<BvFilterFieldsEntity>
            {
                new BvFilterFieldsEntity
                {
                    Column = "AttemptNumber",
                    Table = (int)TableTypes.Interview,
                    Type = (int)VariableTypes.Integer,
                    Sign = (int)FilterOperator.LessEqual,
                    Value = "5"
                }
            };

            FilterService.SetFields(filterId, fields);

            var dataList = SurveyService.GetSampleStatusSummary(surveySid, filterId, null);

            Assert.AreEqual(1, dataList.Count, "GetSampleStatusSummary returns incorrect row count");
            Assert.AreEqual(5, dataList[0].count, "GetSampleStatusSummary returns incorrect count value");
        }

        /// <summary>
        /// 1. Create survey using BackendTools.CreateAndAssignSurvey method
        /// 2. Create filter using FilterRepository.Insert method
        /// 3. Get filter list using FilterRepository.GetFiltersList method
        /// 4. Check that method return one filter with correct name
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void FilterRepositoryTest_GetFiltersList_Successfully()
        {
            const string filterName = "TestFilter";

            // Create survey
            int surveySid = _backendTools.CreateSurvey(SurveyName);

            // Create filter
            FilterRepository.Insert(
                new BvFiltersEntity
                {
                    Name = filterName,
                    Description = "description of " + filterName,
                    SurveySID = surveySid
                });

            var dataList = FilterRepository.GetFiltersList(true, surveySid);


            Assert.AreEqual(1, dataList.Count, "GetFiltersList return wrong filter count: " + dataList.Count);
            Assert.AreEqual(filterName, dataList[0].Name, "GetFiltersList return wrong filter name: " + dataList[0].Name);
        }

        /// <summary>
        /// 1. Create survey using BackendTools.CreateAndAssignSurvey method
        /// 2. Create person using PersonTools.CreatePerson method
        /// 3. Assign person to survey using BackendTools.AssignCatiPersonToSurvey method
        /// 4. Get person list using AssignmentManager.GetAssignedInterviewersAndGroupsList method
        /// 5. Check that method return one person with correct name
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void AssignmentManager_GetAssignedInterviewersAndGroupsList_Successfully()
        {
            // Create survey
            int surveySid = _backendTools.CreateSurvey(SurveyName);

            // Create person
            int personSid = PersonTools.CreatePerson(PersonName);

            // Assign person to survey
            BackendTools.AssignCatiPersonToSurvey(surveySid, personSid);

            var dataList = _assignmentManager.GetAssignedInterviewersAndGroupsList(surveySid);

            Assert.AreEqual(1, dataList.Count, "GetAssignedInterviewersAndGroupsList return wrong person count: " + dataList.Count);
            Assert.AreEqual(PersonName, dataList[0].Name, "GetAssignedInterviewersAndGroupsList return wrong person name: " + dataList[0].Name);
        }

        /// <summary>
        /// 1. Create survey using BackendTools.CreateAndAssignSurvey method
        /// 2. Create person using PersonTools.CreatePerson method
        /// 3. Create administrative and not administrative groups  
        /// 3. Get person and group list using AssignmentManager.GetNotAssignedInterviewersAndGroupsList method
        /// 4. Check that method return one person and 3 groups with correct ids
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void PersonManager_GetAllNotAssignedInterviewersAndGroups_Success()
        {
            // Create survey
            int surveySid = _backendTools.CreateSurvey(SurveyName);

            // Create person
            var personId = PersonTools.CreatePerson(PersonName);
            var group1 = PersonTools.CreatePersonGroup("PG1", false);
            var group2 = PersonTools.CreatePersonGroup("PG2", true);
            
            var dataList = PersonManager.GetAllNotAssignedPersonsAndGroups(surveySid);

            Assert.AreEqual(4, dataList.Count, "GetNotAssignedInterviewersAndGroupsList return wrong items count: " + dataList.Count);

            var expected = new[] { PersonGroupService.RootGroupId, personId, group1, group2 };

            Assert.IsTrue(expected.SequenceEqual(dataList.Select(x => x.Id)));
        }

        /// <summary>
        /// 1. Create survey using BackendTools.CreateAndAssignSurvey method
        /// 2. Create person using PersonTools.CreatePerson method
        /// 3. Assign person to survey using BackendTools.AssignCatiPersonToSurvey method
        /// 4. Set access for superuser usign new ManagementService().UpdateSurveyAccessList method
        /// 5. Get survey list using AssignmentManager.GetAssignedSurveyList method
        /// 6. Check that method return one survey with correct name
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void AssignmentManager_GetAssignedSurveyList_Successfully()
        {
            // Create survey
            int surveySid = _backendTools.CreateSurvey(SurveyName);

            // Create person
            int personSid = PersonTools.CreatePerson(PersonName);

            // Assign person to survey
            BackendTools.AssignCatiPersonToSurvey(surveySid, personSid);

            // Set access            
            new ManagementService().UpdateSurveyAccessList(UserName, SurveyName, true);

            var dataList = _assignmentManager.GetAssignedSurveyList(personSid, UserName);

            Assert.AreEqual(1, dataList.Count, "GetAssignedSurveyList return wrong survey count: " + dataList.Count);
            Assert.AreEqual(SurveyName, dataList[0].ProjectID, "GetAssignedSurveyList return wrong survey name: " + dataList[0].ProjectID);
        }

        /// <summary>
        /// 1. Create survey using BackendTools.CreateAndAssignSurvey method
        /// 2. Create person using PersonTools.CreatePerson method
        /// 3. Create 2 groups: group1 and group2.
        /// 4. Assign group1 and group2 to survey using BackendTools.AssignCatiPersonToSurvey method
        /// 5. Set access for superuser usign new ManagementService().UpdateSurveyAccessList method
        /// 6. Add person to both of groups.
        /// 7. Get survey list using AssignmentManager.GetAssignedSurveyList2 method
        /// 6. Check that method return record for each group (should be 2 records - for group1 and group2).
        /// </summary>
        [TestMethod, Owner(@"FIRM\SergeyC")]
        [Bug(38687)]
        public void AssignmentManager_GetAssignedSurveyListExtended_Successfully()
        {
            const string group1Name = "group1";
            const string group2Name = "group2";

            // Create survey
            int surveySid = _backendTools.CreateSurvey(SurveyName);

            // Create groups
            int group1 = PersonTools.CreatePersonGroup(group1Name);
            int group2 = PersonTools.CreatePersonGroup(group2Name);

            // Create person
            int personSid = PersonTools.CreatePerson(PersonName, "pass", AgentTaskChoiceMode.Manual, new[] { group1, group2 });

            // Assign groups to survey
            BackendTools.AssignCatiPersonToSurvey(surveySid, group1);
            BackendTools.AssignCatiPersonToSurvey(surveySid, group2);

            // Set access            
            new ManagementService().UpdateSurveyAccessList(UserName, SurveyName, true);

            var dataList = _assignmentManager.GetPersonAssignments(personSid, UserName, CallCenterTools.DefaultId);

            Assert.AreEqual(2, dataList.Count, "GetAssignedSurveyList2 returns wrong record count: " + dataList.Count);
            string g1 = (from c in dataList where c.ParentGroupName == group1Name select c.ParentGroupName).FirstOrDefault();
            Assert.IsFalse(String.IsNullOrEmpty(g1), "Function doesn't return data for group '" + group1Name + "'");
            string g2 = (from c in dataList where c.ParentGroupName == group2Name select c.ParentGroupName).FirstOrDefault();
            Assert.IsFalse(String.IsNullOrEmpty(g2), "Function doesn't return data for group '" + group2Name + "'");
        }

        /// <summary>
        /// 1. Create survey using BackendTools.CreateAndAssignSurvey method
        /// 2. Create person using PersonTools.CreatePerson method        
        /// 3. Set access for superuser usign new ManagementService().UpdateSurveyAccessList method
        /// 4. Get survey list using AssignmentManager.GetNotAssignedSurveysList method
        /// 5. Check that method return one survey with correct name
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void AssignmentManager_GetNotAssignedSurveysList_Successfully()
        {
            // Create survey
            _backendTools.CreateSurvey(SurveyName, true);

            // Create person
            int personSid = PersonTools.CreatePerson(PersonName);

            // Set access
            new ManagementService().UpdateSurveyAccessList(UserName, SurveyName, true);

            var dataList = _assignmentManager.GetNotAssignedSurveysList(personSid, UserName, false);

            Assert.AreEqual(1, dataList.Count, "GetNotAssignedSurveysList return wrong survey count: " + dataList.Count);
            Assert.AreEqual(SurveyName, dataList[0].ConfirmitID, "GetNotAssignedSurveysList return wrong survey name: " + dataList[0].ConfirmitID);
        }

        /// <summary>
        /// 1. Create survey using BackendTools.CreateAndAssignSurvey method
        /// 2. Create person using PersonTools.CreatePerson method        
        /// 3. Run AssignmentManager.IsPersonAssigned for not assigment person
        /// 4. Assign person to survey usign BackendTools.AssignCatiPersonToSurvey method
        /// 5. Run AssignmentManager.IsPersonAssigned for assigment person
        /// 6. Check that method return correct values
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void AssignmentManager_IsPersonAssigned_Successfully()
        {
            // Create survey
            int surveySid = _backendTools.CreateSurvey(SurveyName);

            // Create person
            int personSid = PersonTools.CreatePerson(PersonName);

            // Run IsPersonAssigned for not assigment person
            bool notAssignedPerson = _assignmentManager.IsPersonOrGroupAssigned(surveySid, personSid);

            // Assign person to survey
            BackendTools.AssignCatiPersonToSurvey(surveySid, personSid);

            // Run IsPersonAssigned for assigment person
            bool assignedPerson = _assignmentManager.IsPersonOrGroupAssigned(surveySid, personSid);

            Assert.IsTrue(assignedPerson, "IsPersonAssigned return false for assigned person");
            Assert.IsFalse(notAssignedPerson, "IsPersonAssigned return true for not assigned person");
        }


        /// <summary>
        /// 1. Create survey using BackendTools.CreateAndAssignSurvey method
        /// 2. Open survey using SurveyService.Open() method                
        /// 3. Get survey list using ActivityManager.GetSurveyActivityData method
        /// 4. Check that method return one survey with correct name
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void ActivityManager_GetSurveyActivityData_Successfully()
        {
            // Create survey
            int surveySid = _backendTools.CreateSurvey(SurveyName);

            // Open survey
            _surveyStateService.Open(surveySid);

            var dataList = _activityManager.GetSurveyActivityData(String.Empty, true, false, new[] { surveySid }, false);

            Assert.AreEqual(1, dataList.Count, "GetSurveyActivityData return wrong survey count: " + dataList.Count);
            Assert.AreEqual(SurveyName, dataList[0].Id, "GetSurveyActivityData return wrong survey name: " + dataList[0].Id);
        }

        /// <summary>
        /// 1. Create survey using BackendTools.CreateAndAssignSurvey method
        /// 2. Open survey using SurveyService.Open() method
        /// 3. Create person using PersonTools.CreatePerson method
        /// 4. Assign person to  survey using  BackendTools.AssignCatiPersonToSurvey method
        /// 5. Create interview using BackendTools.CreateInterview method
        /// 6. Create call using BackendTools.CreateCall method
        /// 7. Launch all hours script
        /// 8. Activate call using CallManager.StartAsyncActivateCalls method
        /// 9. Add appointment using BackendTools.AddAppointment method
        /// 10. Update interview using InterviewRepository.Update method
        /// 11. Insert data into BvTransferArrays table using BvTransferArraysAdapter.Insert method
        /// 12. Set thresholds values using BvSpThresholds_insertAdapter.ExecuteNonQuery method
        /// 13. Run aggregation for new objects
        /// 14. Get task list using ActivityManager.GetAppointmentActivityData method
        /// 15. Check that method return one appointment with correct survey name, person name and interview id
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void ActivityManager_GetAppointmentActivityData_Successfully()
        {
            // Launch script
            _backendTools.LaunchAllHoursScript();

            // Create survey
            int surveySid = _backendTools.CreateSurvey(SurveyName);

            // Open survey
            _surveyStateService.Open(surveySid);

            // Create person
            int personSid = PersonTools.CreatePerson(PersonName);

            // Assign person to survey
            BackendTools.AssignCatiPersonToSurvey(surveySid, personSid);

            // Create interview            
            var interview = BackendTools.NewInterview(surveySid);
            BackendTools.CreateInterview(interview);

            // Create call
            BvCallEntity call = BackendTools.NewCall(interview);
            BackendTools.CreateCall(call);

            // Activate call
            new TestCallManagementOperationFactory().CreateActivateCallsSelected(
                surveySid, new[] { interview.ID }, 1, personSid, -1, DateTime.FromOADate(0), CallStates.Scheduled, false);

            // Add appointment
            BackendTools.AddAppointment(interview.ID, surveySid, DateTime.Now.AddMinutes(-10).ToUniversalTime());

            // Update interview
            interview.TransientState = 1;
            InterviewRepository.Update(interview, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            // Insert data into BvTransferArrays table
            BvTransferArraysAdapter.Insert(
                new BvTransferArraysEntity
                {
                    BatchID = 1,
                    ItemID = surveySid
                });

            // Set thresholds values
            BvSpThresholds_insertAdapter.ExecuteNonQuery(0, 15, 1, 2);

            var systemSettings = ServiceLocator.Resolve<ISystemSettings>();
            // Run recalculation
            BvSpAlert_RecalculateAppointmentAdapter.ExecuteNonQuery(
                systemSettings.AppointmentAlert.ShortInterval,
                systemSettings.AppointmentAlert.LongInterval,
                _timezoneId);

            var dataList = ActivityManager.GetAppointmentActivityData(String.Empty, true, 0, new[] { surveySid });

            Assert.AreEqual(1, dataList.Count, "GetSurveyActivityData return wrong appointment count: " + dataList.Count);
            Assert.AreEqual(surveySid, dataList[0].SurveySID, "GetSurveyActivityData return appointment with wrong survey ID: " + dataList[0].SurveySID);
            Assert.AreEqual(interview.ID, dataList[0].InterviewID, "GetSurveyActivityData return appointment with wrong interview ID: " + dataList[0].InterviewID);
            Assert.AreEqual(call.CallID, dataList[0].CallID, "GetSurveyActivityData return appointment with wrong person ID: " + dataList[0].CallID);
        }


        /// <summary>
        /// 1. Create survey using BackendTools.CreateAndAssignSurvey method
        /// 2. Open survey using SurveyService.Open() method
        /// 3. Create person using PersonTools.CreatePerson method
        /// 4. Assign person to  survey using  BackendTools.AssignCatiPersonToSurvey method
        /// 5. Create interview using BackendTools.CreateInterview method
        /// 6. Create call using BackendTools.CreateCall method
        /// 7. Launch all hours script
        /// 8. Activate call using CallManager.StartAsyncActivateCalls method
        /// 9. Add appointment using BackendTools.AddAppointment method
        /// 10. Update interview using InterviewRepository.Update method        
        /// 11. Get appointment list using ActivityManager.GetSurveyAppointmentCountData method
        /// 12. Check that method return two appointments and second appointment has correct survey name
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void ActivityManager_GetSurveyAppointmentCountData_Successfully()
        {
            // Launch script
            _backendTools.LaunchAllHoursScript();

            // Create survey
            int surveySid = _backendTools.CreateSurvey(SurveyName);

            // Open survey
            _surveyStateService.Open(surveySid);

            // Create person
            int personSid = PersonTools.CreatePerson(PersonName);

            // Assign person to survey
            BackendTools.AssignCatiPersonToSurvey(surveySid, personSid);

            // Create interview            
            var interview = BackendTools.NewInterview(surveySid);
            BackendTools.CreateInterview(interview);

            // Create call
            BackendTools.CreateCall(BackendTools.NewCall(interview));

            // Activate call
            new TestCallManagementOperationFactory().CreateActivateCallsSelected(
                surveySid, new[] { interview.ID }, 1, personSid, -1, DateTime.FromOADate(0), CallStates.Scheduled, false);

            // Add appointment
            BackendTools.AddAppointment(interview.ID, surveySid, DateTime.Now.AddMinutes(10).ToUniversalTime());

            // Update interview
            interview.TransientState = 1;
            InterviewRepository.Update(interview, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false, opType = OperationType.MovedAndReschedule });

            var dataList = ActivityManager.GetSurveyAppointmentCountData(String.Empty, true, new[] { surveySid });

            Assert.AreEqual(2, dataList.Count, "GetSurveyAppointmentCountData return wrong appointment count: " + dataList.Count);
            Assert.AreEqual(SurveyName, dataList[1].ProjectId, "GetSurveyAppointmentCountData return wrong appointment name: " + dataList[1].ProjectId);
        }

        /// <summary>
        /// 1. Create survey using BackendTools.CreateAndAssignSurvey method
        /// 2. Open survey using SurveyService.Open() method
        /// 3. Get system wide info using ActivityManager.GetSystemWideInfo method
        /// 4. Check that method return info with correct opend survey count
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void ActivityManager_GetSystemWideInfo_Successfully()
        {
            // Create survey
            int surveySid = _backendTools.CreateSurvey(SurveyName);

            // Open survey
            _surveyStateService.Open(surveySid);

            var systemWideInfo = ActivityManager.GetSystemWideInfo(new List<int> { surveySid });

            Assert.AreEqual(1, systemWideInfo.OpenSurveysCount, "GetSystemWideInfo return wrong open survey count: " + systemWideInfo.OpenSurveysCount);
        }

        /// <summary>
        /// 1. Create survey using FilterAndPagingTools.CreateSurveyWithSample method
        /// 2. Open survey using SurveyService.Open() method
        /// 3. Get system wide info using ActivityManager.GetSystemWideInfo method
        /// 4. Check that method return info with correct opend survey count
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void ActivityManager_GetStatusBreakdown_Successfully()
        {
            FillSurveyData();

            // Create survey
            int surveySid = _filterAndPagingTools.CreateSurveyWithSample(SurveyName, FilterAndPagingTools.SampleType.SmallSample);

            CallTools.MoveCalls(surveySid, new[] { 1, 2 }, 1);

            var statusBreakdown = ActivityManager.GetStatusBreakdown(surveySid);

            Assert.AreEqual(2, statusBreakdown.Count, "GetStatusBreakdown return wrong status array count: " + statusBreakdown.Count);
            Assert.AreEqual(2, statusBreakdown[0].Value, "GetStatusBreakdown return wrong sample count for appointments: " + statusBreakdown[0].Value);
            Assert.AreEqual(1, statusBreakdown[0].Id, "GetStatusBreakdown return wrong id for appointments: " + statusBreakdown[0].Id);
            Assert.AreEqual(23, statusBreakdown[1].Value, "GetStatusBreakdown return wrong sample count for fresh samples: " + statusBreakdown[1].Value);
            Assert.AreEqual(16, statusBreakdown[1].Id, "GetStatusBreakdown return wrong id for appointments: " + statusBreakdown[1].Id);
        }


        /// <summary>
        /// 1. Create survey using FilterAndPagingTools.CreateSurveyWithSample method
        /// 2. Set access for superuser using new ManagementService().UpdateSurveyAccessList method
        /// 3. Create person using PersonTools.CreatePerson method
        /// 4. Insert data into BvPersonDeferredMonitoring table using BvPersonDeferredMonitoringAdapter.Insert method
        /// 5. Gets paged list of deferred monitoring records using DeferredMonitoringRepository.GetPage method
        /// 6. Check that method return one row with correct survey name
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void DeferredMonitoringRepository_GetPage_Successfully()
        {
            // Create survey
            int surveySid = _filterAndPagingTools.CreateSurveyWithSample(SurveyName, FilterAndPagingTools.SampleType.SmallSample);

            // Set access            
            new ManagementService().UpdateSurveyAccessList(UserName, SurveyName, true);

            // Create person
            int personSid = PersonTools.CreatePerson(PersonName);

            // Insert data into BvPersonDeferredMonitoring table
            BvPersonDeferredMonitoringAdapter.Insert(new BvPersonDeferredMonitoringEntity
            {
                EventsFile = new byte[0],
                HasAudio = false,
                ID = 1,
                InterviewID = 1,
                IsComplete = true,
                IsRecording = false,
                PersonSID = personSid,
                StartingFile = "1",
                SurveySID = surveySid,
                TimeStamp = DateTime.UtcNow,
                ServerTimeUtc = DateTime.UtcNow,
                ClientTimeUtc = DateTime.UtcNow,
                RecordCreationTime = DateTime.UtcNow
            });

            // Gets paged list
            var pagingArgs = new PagingArgs(
                1 /*PageIndex*/,
                20 /*PageSize*/,
                "ID" /*SortedColumnKey*/,
                true /*SortIndicatorAsc*/);

            int totalCount;
            var data = new DeferredMonitoringRepository().GetPages(
                    UserName,
                    pagingArgs,
                    _timezoneId,
                    out totalCount);

            Assert.AreEqual(1, data.Count, "GetPage return wrong count: " + data.Count);
            Assert.AreEqual(SurveyName, data[0].SurveyName, "GetPage return wrong survey name: " + data[0].SurveyName);
        }


        /// <summary>
        /// 1. Get state info using StateRepository.GetById method
        /// 2. Check that method return one state with correct stateSID and stateGroupSID
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void StateRepository_GetById_Successfully()
        {
            const int stateSid = 1;
            const int stateGroupSid = 27;

            var data = StateRepository.GetById(stateGroupSid, stateSid);

            Assert.AreEqual(stateGroupSid, data.StateGroupID, "GetById return wrong StateGroupID: " + data.StateGroupID);
            Assert.AreEqual(stateSid, data.StateID, "GetById return wrong StateID: " + data.StateID);
        }


        /// <summary>
        /// 1. Get state info using StateRepository.GetByItsAndStateGroupId method
        /// 2. Check that method return one state with correct stateSID and stateGroupSID
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void StateRepository_GetByItsAndStateGroupId_Successfully()
        {
            const int its = 1;
            const int stateGroupSid = 27;

            var data = StateRepository.GetByItsAndStateGroupId(its, stateGroupSid);

            Assert.AreEqual(stateGroupSid, data.StateGroupID, "GetById return wrong StateGroupID: " + data.StateGroupID);
            Assert.AreEqual(its, data.StateID, "GetById return wrong StateID: " + data.StateID);
        }


        /// <summary>
        /// 1. Get state info using StateRepository.GetById method
        /// 2. Cahgne state entity and update it using StateRepository.Update method
        /// 3. Get state info one more using StateRepository.GetById method
        /// 4. Check that method return one state with correct changed values
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void StateRepository_Update_Successfully()
        {
            const int stateSid = 1;
            const int stateGroupSid = 27;

            var data = StateRepository.GetById(stateGroupSid, stateSid);

            data.Priority++;
            data.Name += "_NewName";
            data.DA++;

            StateRepository.Update(data);

            var newData = StateRepository.GetById(stateGroupSid, stateSid);

            Assert.AreEqual(data.Priority, newData.Priority, "Update method didn't change a priority");
            Assert.AreEqual(data.Name, newData.Name, "Update method didn't change a name");
            Assert.AreEqual(data.DA, newData.DA, "Update method didn't change a DA parameter");
        }


        /// <summary>
        /// 1. Get state group list using StateGroupRepository.GetAll method
        /// 2. Check that method return one state group
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void StateGroupRepository_GetAll_Successfully()
        {
            var data = StateGroupRepository.GetAll();

            Assert.AreEqual(1, data.Count, "GetAll return wrong state group count: " + data.Count);
        }


        /// <summary>
        /// 1. Get state group info using StateGroupRepository.GetById method
        /// 2. Check that method return state group with correct ID
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void StateGroupRepository_GetById_Successfully()
        {
            const int id = 27;

            var data = StateGroupRepository.GetById(id);

            Assert.AreEqual(id, data.ID, "GetById return state group with wrong id: " + data.ID);
        }

        /// <summary>
        /// 1. Get state group info using StateGroupRepository.GetByName method
        /// 2. Check that method returns existing state group
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderM"), Bug(40702)]
        public void StateGroupRepository_GetDefault_ValidStateGroup()
        {
            var defaultStateGroup = StateGroupRepository.GetDefault();

            Assert.IsTrue(StateGroupRepository.GetAll().Select(x => x.ID).Contains(defaultStateGroup.ID));
        }

        /// <summary>
        /// 1. Add new state group using StateGroupRepository.Insert method
        /// 2. Get state group info using StateGroupRepository.GetById method
        /// 3. Check that method return one state group with correct name
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void StateGroupRepository_Insert_Successfully()
        {
            const int copyId = 0;

            var stateGroup = new BvStateGroupEntity
            {
                Name = "TestStateGroup"
            };

            int newId = StateGroupRepository.Insert(copyId, stateGroup);

            var data = StateGroupRepository.GetById(newId);

            Assert.AreEqual(stateGroup.Name, data.Name, "Insert method didn't add state group with name " + stateGroup.Name);
            Assert.AreEqual(true, StateRepository.GetById(newId, 1).FcdAction, "New group have wrong fcd action for appointment ITS");
            Assert.AreEqual(2, StateRepository.GetAll(newId).Count(x => x.FcdAction), "New group have wrong count of disabled ITS for FCD");
        }

        /// <summary>
        /// 1. Add new state group using StateGroupRepository.Insert method
        /// 2. Get state group info using StateGroupRepository.GetById method
        /// 3. Check that method return one state group with correct name
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void StateGroupRepository_Delete_Successfully()
        {
            const int copyId = 0;

            var stateGroup = new BvStateGroupEntity
            {
                Name = "TestStateGroup"
            };

            int newId = StateGroupRepository.Insert(copyId, stateGroup);

            StateGroupRepository.Delete(newId);

            var data = StateGroupRepository.GetAll();

            Assert.AreEqual(1, data.Count, "Delete method didn't delete state group");
            Assert.AreNotEqual(newId, data[0].ID, "Delete method has deleted wrong state group");
        }





        /// <summary>
        /// 1. Create survey using  method
        /// 2. Launch survey using  method
        /// 3. Open survey using  method
        /// 4. Create person using  method
        /// 5. Create interview using  method
        /// 6. Add call using  method
        /// 7. Assign call using  method
        /// 8. Get assignment interviewers list using  method
        /// 9. Deassign resource using  method
        /// 10. Get assignment interviewers list using  method        
        /// 11. Check that before the deassignment the list has one value and 
        ///     after the deassignment the list hasn't any values
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void AssignmentService_DeassignResourceFromSurveyCalls_Successfully()
        {
            const int interviewId = 1;

            // Launch and open survey
            _backendTools.LaunchAllHoursScript();

            // Create survey
            int surveySid = _backendTools.CreateSurvey(SurveyName);

            _surveyStateService.Open(surveySid);

            // Create person            
            int personSid = PersonTools.CreatePerson(PersonName);

            // Create interview
            _interviewRepository.InsertOnly(
                new BvInterviewEntity
                {
                    ID = interviewId,
                    SurveySID = surveySid,
                    TransientState = 1
                });

            // Add call
            CallQueueService.AddCall(
                new BvCallEntity
                {
                    InterviewID = interviewId,
                    SurveySID = surveySid,
                    CallState = 2,
                    ShiftID = (int)CallShiftType.None,
                    Priority = 5
                },
                0, 0);

            // Assign calls
            CallTools.AssignCalls(surveySid, new[] { interviewId }, personSid);

            var dataList = _assignmentManager.GetAssignedInterviewersAndGroupsList(surveySid);

            BackendTools.DeassignCatiPersonFromSurveyCalls(surveySid, personSid);

            var dataList1 = _assignmentManager.GetAssignedInterviewersAndGroupsList(surveySid);

            Assert.AreEqual(1, dataList.Count, "Not assigment objects for survey");
            Assert.AreEqual(0, dataList1.Count, "DeassignResourceFromSurveyCalls didn't delete assigment objects");
        }
    }
}
