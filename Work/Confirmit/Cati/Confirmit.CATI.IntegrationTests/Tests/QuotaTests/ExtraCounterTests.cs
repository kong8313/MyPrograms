using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.DAL.Handmade.Entity;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.InterviewServiceImplementation;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Supervisor.Core.Confirmit;
using Confirmit.CATI.Supervisor.Core.Confirmit.QuotaViewExtension;
using ConfirmitDialerInterface;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.QuotaTests
{
    /// <summary>
    /// Summary description for ExtraCounterTests
    /// </summary>
    [TestClass]
    public class ExtraCounterTests : BaseMockedIntegrationTest
    {
        enum InterviewCallState
        {
            None,
            Normal,
            Disabled,
            InProgress,
            Delete
        }

        private IQuotaInfoService _quotaInfoService;

        public override void OnPostTestInitialize()
        {
            _quotaInfoService = ServiceLocator.Resolve<IQuotaInfoService>();
        }
        
        private void CreateInterview(int surveySid, CallOutcome its, InterviewCallState callState, TestQuota quota, params string[] quotaAnswers)
        {
            var interview = BackendTools.NewInterview(surveySid);
            interview.TransientState = (int)its;
            BackendTools.CreateInterview(interview);
            quota.PutInterviewInCell(interview.ID, quotaAnswers);
            
            switch( callState )
            {
                case InterviewCallState.None:
                    break;
                case InterviewCallState.Normal:
                    {
                        var call = BackendTools.NewCall(interview);
                        BackendTools.CreateCall(call);
                    }
                    break;
                case InterviewCallState.InProgress:
                    {
                        var call = BackendTools.NewCall(interview);
                        BackendTools.CreateCall(call);
                        BvSpCall_GetAdapter.ExecuteNonQuery(call.SurveySID, call.InterviewID, 1, 1);
                    }
                    break;
                case InterviewCallState.Delete:
                    {
                        var call = BackendTools.NewCall(interview);
                        BackendTools.CreateCall(call);
                        CallQueueService.DeleteCall(call, 0);
                    }
                    break;
                case InterviewCallState.Disabled:
                    {
                        var call = BackendTools.NewCall(interview);
                        call.CallState = 1;
                        BackendTools.CreateCall(call);
                    }
                    break;
            }
        }

        private void CreateInterviewWithHistoryRecord(int surveySid, CallOutcome its, TestQuota quota, DateTime firedTime, params string[] quotaAnswers)
        {
            var interview = BackendTools.NewInterview(surveySid);
            interview.TransientState = (int) its;
            BackendTools.CreateInterview(interview);
            quota.PutInterviewInCell(interview.ID, quotaAnswers);

            new BackendTools(TestingFramework).CreateHistoryRecords(surveySid, 1, new[] {firedTime}, interview.ID, 100, 5, (byte) its);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetCellCounterOfScheduledCalls_6InterviewWith4Scheduled_ResultCorrect()
        {
            var surveySid = BackendToolsObject.CreateSurvey(null, null, null);
            
            var quota = TestQuota.Create(TestingFramework.DbEngine,
                surveySid,
                1,
                new[] { "q1", "q2" },
                new [] { 2, 2 });

            quota.MockWs();

            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.None, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "1", "2");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "2", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.None, quota, "2", "2");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "1", null);

            var fields = _quotaInfoService.GetQuotaFields(surveySid, quota.QuotaId);
            var parameters = new CallsCounterParameter(surveySid, quota.QuotaId, true, null, fields);
            var calculator = ExtraQuotaCounterService.Create(parameters);
            var result = calculator.GetCellCounter();

            TestAssert.AreEqual(
                (IEnumerable<QuotaCellCounter>)result.OrderBy(x => x.Descriptor).ToArray(),
                new[]
                    {
                        new QuotaCellCounter { Descriptor = "1,1", Value = 1 },
                        new QuotaCellCounter { Descriptor = "1,2", Value = 1 },
                        new QuotaCellCounter { Descriptor = "2,1", Value = 1 }
                    }.OrderBy(x => x.Descriptor).ToArray());

            CheckIts(quota, calculator, new[] { "1", "1" }, new Dictionary<int, int> { { (int)CallOutcome.FreshSample, 1 } });
            CheckIts(quota, calculator, new[] { "1", "2" }, new Dictionary<int, int> { { (int)CallOutcome.FreshSample, 1 } });
            CheckIts(quota, calculator, new[] { "2", "1" }, new Dictionary<int, int> { { (int)CallOutcome.FreshSample, 1 } });
            CheckIts(quota, calculator, new[] { "2", "2" }, new Dictionary<int, int>());

            BackendTools.ForceProcessingAsyncTriggers();
            TestAssert.AreEqual(CallQueueService.GetCountOfScheduledInterviews(surveySid), 4);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetCellCounterOfScheduledCallsWithoutDisabled_6InterviewWith3ScheduledAnd1Disabled_ResultCorrect()
        {
            var surveySid = BackendToolsObject.CreateSurvey(null, null, null);

            var quota = TestQuota.Create(TestingFramework.DbEngine,
                surveySid,
                1,
                new[] { "q1", "q2" },
                new[] { 2, 2 });

            quota.MockWs();

            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.None, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "1", "2");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "2", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.None, quota, "2", "2");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Disabled, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "1", null);

            var fields = _quotaInfoService.GetQuotaFields(surveySid, quota.QuotaId);
            var parameters = new CallsCounterParameter(surveySid, quota.QuotaId, false, null, fields);
            var calculator = ExtraQuotaCounterService.Create(parameters);
            var result = calculator.GetCellCounter();

            TestAssert.AreEqual(
                (IEnumerable<QuotaCellCounter>)result.OrderBy(x => x.Descriptor).ToArray(),
                new[]
                    {
                        new QuotaCellCounter { Descriptor = "1,2", Value = 1 },
                        new QuotaCellCounter { Descriptor = "2,1", Value = 1 }
                    }.OrderBy(x => x.Descriptor).ToArray());

            CheckIts(quota, calculator, new[] { "1", "1" }, new Dictionary<int, int>());
            CheckIts(quota, calculator, new[] { "1", "2" }, new Dictionary<int, int> { { (int)CallOutcome.FreshSample, 1 } });
            CheckIts(quota, calculator, new[] { "2", "1" }, new Dictionary<int, int> { { (int)CallOutcome.FreshSample, 1 } });
            CheckIts(quota, calculator, new[] { "2", "2" }, new Dictionary<int, int>());

            BackendTools.ForceProcessingAsyncTriggers();
            TestAssert.AreEqual(CallQueueService.GetCountOfScheduledInterviews(surveySid), 3);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetCellCounterOfScheduledCallsWithDisabled_6InterviewWith3ScheduledAnd1Disabled_ResultCorrect()
        {
            var surveySid = BackendToolsObject.CreateSurvey(null, null, null);

            var quota = TestQuota.Create(TestingFramework.DbEngine,
                surveySid,
                1,
                new[] { "q1", "q2" },
                new[] { 2, 2 });

            quota.MockWs();

            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.None, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "1", "2");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "2", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.None, quota, "2", "2");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Disabled, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "1", null);

            var fields = _quotaInfoService.GetQuotaFields(surveySid, quota.QuotaId);
            var parameters = new CallsCounterParameter(surveySid, quota.QuotaId, true, null, fields);
            var calculator = ExtraQuotaCounterService.Create(parameters);
            var result = calculator.GetCellCounter();

            TestAssert.AreEqual(
                (IEnumerable<QuotaCellCounter>)result.OrderBy(x => x.Descriptor).ToArray(),
                new[]
                    {
                        new QuotaCellCounter { Descriptor = "1,1", Value = 1 },
                        new QuotaCellCounter { Descriptor = "1,2", Value = 1 },
                        new QuotaCellCounter { Descriptor = "2,1", Value = 1 }
                    }.OrderBy(x => x.Descriptor).ToArray());

            CheckIts(quota, calculator, new[] { "1", "1" }, new Dictionary<int, int> { { (int)CallOutcome.FreshSample, 1 } });
            CheckIts(quota, calculator, new[] { "1", "2" }, new Dictionary<int, int> { { (int)CallOutcome.FreshSample, 1 } });
            CheckIts(quota, calculator, new[] { "2", "1" }, new Dictionary<int, int> { { (int)CallOutcome.FreshSample, 1 } });
            CheckIts(quota, calculator, new[] { "2", "2" }, new Dictionary<int, int>());

            BackendTools.ForceProcessingAsyncTriggers();
            TestAssert.AreEqual(CallQueueService.GetCountOfScheduledInterviews(surveySid), 3);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetCellCounterOfScheduledCalls_7nterviewWith3ScheduledAnd2Deleted_ResultCorrect()
        {
            var surveySid = BackendToolsObject.CreateSurvey(null, null, null);

            var quota = TestQuota.Create(TestingFramework.DbEngine,
                surveySid,
                1,
                new[] { "q1", "q2" },
                new[] { 2, 2 });

            quota.MockWs();

            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.None, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "1", "2");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Delete, quota, "2", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.None, quota, "2", "2");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "1", null);
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Delete, quota, "1", null);

            var fields = _quotaInfoService.GetQuotaFields(surveySid, quota.QuotaId);
            var parameters = new CallsCounterParameter(surveySid, quota.QuotaId, false, null, fields);
            var calculator = ExtraQuotaCounterService.Create(parameters);
            var result = calculator.GetCellCounter();

            TestAssert.AreEqual(
                (IEnumerable<QuotaCellCounter>)result.OrderBy(x => x.Descriptor).ToArray(),
                new[]
                    {
                        new QuotaCellCounter { Descriptor = "1,1", Value = 1 },
                        new QuotaCellCounter { Descriptor = "1,2", Value = 1 }
                    }.OrderBy(x => x.Descriptor).ToArray());

            CheckIts(quota, calculator, new[] { "1", "1" }, new Dictionary<int, int> { { (int)CallOutcome.FreshSample, 1 } });
            CheckIts(quota, calculator, new[] { "1", "2" }, new Dictionary<int, int> { { (int)CallOutcome.FreshSample, 1 } });
            CheckIts(quota, calculator, new[] { "2", "1" }, new Dictionary<int, int>());
            CheckIts(quota, calculator, new[] { "2", "2" }, new Dictionary<int, int>());

            BackendTools.ForceProcessingAsyncTriggers(); 
            TestAssert.AreEqual(CallQueueService.GetCountOfScheduledInterviews(surveySid), 3);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetCellCounterOfScheduledCallsWithDiffDbFieldOrder_7nterviewWith3ScheduledAnd2Deleted_ResultCorrect()
        {
            var context = new TestData
            {
                Surveys = new[]{ 
                    new SurveyData { Tag="S1", IsUseDb = true,
                        Forms = new []
                        {
                            new SingleFormData{Name="q1", Precodes = new []{"1", "2"}},
                            new SingleFormData{Name="q2", Precodes = new []{"A", "B"}, SqlType = SqlDataType.Char},
                        },
                        Quotas = new[]{ 
                            new QuotaData(){ Id = 1, Name="Q1", Fields = new[] {"q1", "q2"}, 
                                Cells = new[] 
                                {
                                    new CellData(){Id = 1, Values="q1=1,q2=A", Counter=0, Limit=10},
                                    new CellData(){Id = 2, Values="q1=1,q2=B", Counter=0, Limit=10},
                                    new CellData(){Id = 3, Values="q1=2,q2=A", Counter=0, Limit=10},
                                    new CellData(){Id = 4, Values="q1=2,q2=B", Counter=0, Limit=10},
                                }
                            }},
                        Interviews = new []
                        {
                            new InterviewData(){Tag="S1.I1", Data="q1=1,q2=A", ITS=CallOutcome.FreshSample},
                            new InterviewData(){Tag="S1.I1", Data="q1=1,q2=B", ITS=CallOutcome.FreshSample, Call = new CallData(){CallState = 2}},
                            new InterviewData(){Tag="S1.I1", Data="q1=2,q2=A", ITS=CallOutcome.FreshSample, Call = new CallData(){CallState = 0}},
                            new InterviewData(){Tag="S1.I1", Data="q1=2,q2=B", ITS=CallOutcome.FreshSample},
                            new InterviewData(){Tag="S1.I1", Data="q1=1,q2=A", ITS=CallOutcome.FreshSample, Call = new CallData(){CallState = 2}},
                            new InterviewData(){Tag="S1.I1", Data="q1=1", ITS=CallOutcome.FreshSample, Call = new CallData(){CallState = 2}},
                            new InterviewData(){Tag="S1.I1", Data="q1=2", ITS=CallOutcome.FreshSample, Call = new CallData(){CallState = 0}},
                        }
                    
                }}
            }.Create();

            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("Q1");

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            var fields = _quotaInfoService.GetQuotaFields(survey.Id, quota.Data.Id);
            var parameters = new CallsCounterParameter(survey.Id, quota.Data.Id, false, null, fields);

            var builder = new AdditionalColumnsBuilderFactory().Create(false, false, false, parameters);

            var forms = new List<SingleForm>(new[]
            {
                new SingleForm() {Name = "Q2", FormTexts = new[] {new FormText {Title = "title"}}},
                new SingleForm() {Name = "Q1", FormTexts = new[] {new FormText {Title = "title"}}}
            });

            var result = QuotaManager.CreateQuotaDataTable(forms, builder);
            var row = result.NewRow();
            var quotaList = new QuotaList() {FieldNames = new[] {"q2", "q1"}};

            CheckCellValue(builder, row, quotaList, new[] { "A", "1" }, 1);
            CheckCellValue(builder, row, quotaList, new[] { "B", "1" }, 1);
            CheckCellValue(builder, row, quotaList, new[] { "A", "2" }, 0);
            CheckCellValue(builder, row, quotaList, new[] { "B", "2" }, 0);
            
            BackendTools.ForceProcessingAsyncTriggers();

            TestAssert.AreEqual(CallQueueService.GetCountOfScheduledInterviews(survey.Id), 3);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetCellCounterOfScheduledCallsWithDiffPrecodesAndDbFieldOrder_7nterviewWith3ScheduledAnd2Deleted_ResultCorrect()
        {
            var context = new TestData
            {
                Surveys = new[]{ 
                    new SurveyData { Tag="S1", IsUseDb = true,
                        Forms = new []
                        {
                            new SingleFormData{Name="q1", Precodes = new []{"2", "4"}},
                            new SingleFormData{Name="q2", Precodes = new []{"3", "8"}, SqlType = SqlDataType.Char},
                        },
                        Quotas = new[]{ 
                            new QuotaData(){ Id = 1, Name="Q1", Fields = new[] {"q1", "q2"}, 
                                Cells = new[] 
                                {
                                    new CellData(){Id = 1, Values="q1=2,q2=3", Counter=0, Limit=10},
                                    new CellData(){Id = 2, Values="q1=2,q2=8", Counter=0, Limit=10},
                                    new CellData(){Id = 3, Values="q1=4,q2=3", Counter=0, Limit=10},
                                    new CellData(){Id = 4, Values="q1=4,q2=8", Counter=0, Limit=10},
                                }
                            }},
                        Interviews = new []
                        {
                            new InterviewData(){Tag="S1.I1", Data="q1=2,q2=8", ITS=CallOutcome.FreshSample, Call = new CallData(){CallState = 2}},
                            new InterviewData(){Tag="S1.I1", Data="q1=2,q2=3", ITS=CallOutcome.FreshSample, Call = new CallData(){CallState = 2}},
                            new InterviewData(){Tag="S1.I1", Data="q1=2,q2=3", ITS=CallOutcome.FreshSample, Call = new CallData(){CallState = 2}},
                            new InterviewData(){Tag="S1.I1", Data="q1=4,q2=3", ITS=CallOutcome.FreshSample, Call = new CallData(){CallState = 2}},
                            new InterviewData(){Tag="S1.I1", Data="q1=4,q2=3", ITS=CallOutcome.FreshSample, Call = new CallData(){CallState = 2}},
                            new InterviewData(){Tag="S1.I1", Data="q1=4,q2=3", ITS=CallOutcome.FreshSample, Call = new CallData(){CallState = 2}},
                            new InterviewData(){Tag="S1.I1", Data="q1=4,q2=8", ITS=CallOutcome.FreshSample, Call = new CallData(){CallState = 2}},
                            new InterviewData(){Tag="S1.I1", Data="q1=4,q2=8", ITS=CallOutcome.FreshSample, Call = new CallData(){CallState = 2}},
                            new InterviewData(){Tag="S1.I1", Data="q1=4,q2=8", ITS=CallOutcome.FreshSample, Call = new CallData(){CallState = 2}},
                            new InterviewData(){Tag="S1.I1", Data="q1=4,q2=8", ITS=CallOutcome.FreshSample, Call = new CallData(){CallState = 2}},
                            new InterviewData(){Tag="S1.I1", Data="q1=2", ITS=CallOutcome.FreshSample, Call = new CallData(){CallState = 2}},
                        }
                    
                }}
            }.Create();

            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("Q1");

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            var fields = _quotaInfoService.GetQuotaFields(survey.Id, quota.Data.Id);
            var parameters = new CallsCounterParameter(survey.Id, quota.Data.Id, false, null, fields);

            var builder = new AdditionalColumnsBuilderFactory().Create(false, false, false, parameters);

            var forms = new List<SingleForm>(new[]
            {
                new SingleForm() {Name = "Q2", FormTexts = new[] {new FormText {Title = "title"}}},
                new SingleForm() {Name = "Q1", FormTexts = new[] {new FormText {Title = "title"}}}
            });

            var result = QuotaManager.CreateQuotaDataTable(forms, builder);
            var row = result.NewRow();
            var quotaList = new QuotaList() { FieldNames = new[] { "q2", "q1" } };

            CheckCellValue(builder, row, quotaList, new[] { "3", "2" }, 2);
            CheckCellValue(builder, row, quotaList, new[] { "8", "2" }, 1);
            CheckCellValue(builder, row, quotaList, new[] { "3", "4" }, 3);
            CheckCellValue(builder, row, quotaList, new[] { "8", "4" }, 4);

            BackendTools.ForceProcessingAsyncTriggers();

            TestAssert.AreEqual(CallQueueService.GetCountOfScheduledInterviews(survey.Id), 11);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetCellCounterOfScheduledCallsWithSameDbFieldOrder_7nterviewWith3ScheduledAnd2Deleted_ResultCorrect()
        {
            var context = new TestData
            {
                Surveys = new[]{ 
                    new SurveyData { Tag="S1", IsUseDb = true,
                        Forms = new []
                        {
                            new SingleFormData{Name="q1", Precodes = new []{"1", "2"}},
                            new SingleFormData{Name="q2", Precodes = new []{"A", "B"}, SqlType = SqlDataType.Char},
                        },
                        Quotas = new[]{ 
                            new QuotaData(){ Id = 1, Name="Q1", Fields = new[] {"q1", "q2"}, 
                                Cells = new[] 
                                {
                                    new CellData(){Id = 1, Values="q1=1,q2=A", Counter=0, Limit=10},
                                    new CellData(){Id = 2, Values="q1=1,q2=B", Counter=0, Limit=10},
                                    new CellData(){Id = 3, Values="q1=2,q2=A", Counter=0, Limit=10},
                                    new CellData(){Id = 4, Values="q1=2,q2=B", Counter=0, Limit=10},
                                }
                            }},
                        Interviews = new []
                        {
                            new InterviewData(){Tag="S1.I1", Data="q1=1,q2=A", ITS=CallOutcome.FreshSample},
                            new InterviewData(){Tag="S1.I1", Data="q1=1,q2=B", ITS=CallOutcome.FreshSample, Call = new CallData(){CallState = 2}},
                            new InterviewData(){Tag="S1.I1", Data="q1=2,q2=A", ITS=CallOutcome.FreshSample, Call = new CallData(){CallState = 0}},
                            new InterviewData(){Tag="S1.I1", Data="q1=2,q2=B", ITS=CallOutcome.FreshSample},
                            new InterviewData(){Tag="S1.I1", Data="q1=1,q2=A", ITS=CallOutcome.FreshSample, Call = new CallData(){CallState = 2}},
                            new InterviewData(){Tag="S1.I1", Data="q1=1", ITS=CallOutcome.FreshSample, Call = new CallData(){CallState = 2}},
                            new InterviewData(){Tag="S1.I1", Data="q1=2", ITS=CallOutcome.FreshSample, Call = new CallData(){CallState = 0}},
                        }
                    
                }}
            }.Create();

            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("Q1");

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            var fields = _quotaInfoService.GetQuotaFields(survey.Id, quota.Data.Id).OrderByDescending(x => x).ToArray();
            var parameters = new CallsCounterParameter(survey.Id, quota.Data.Id, false, null, fields);

            var builder = new AdditionalColumnsBuilderFactory().Create(false, false, false, parameters);

            var forms = new List<SingleForm>(new[]
            {
                new SingleForm() {Name = "Q2", FormTexts = new[] {new FormText {Title = "title"}}},
                new SingleForm() {Name = "Q1", FormTexts = new[] {new FormText {Title = "title"}}}
            });

            var result = QuotaManager.CreateQuotaDataTable(forms, builder);
            var row = result.NewRow();
            var quotaList = new QuotaList() { FieldNames = new[] { "q2", "q1" } };

            CheckCellValue(builder, row, quotaList, new[] { "A", "1" }, 1);
            CheckCellValue(builder, row, quotaList, new[] { "B", "1" }, 1);
            CheckCellValue(builder, row, quotaList, new[] { "A", "2" }, 0);
            CheckCellValue(builder, row, quotaList, new[] { "B", "2" }, 0);

            BackendTools.ForceProcessingAsyncTriggers();

            TestAssert.AreEqual(CallQueueService.GetCountOfScheduledInterviews(survey.Id), 3);
        }

        private void CheckCellValue(IQuotaViewAdditionalColumnsBuilder builder, DataRow row, QuotaList quotaList, string[] precodes, int value)
        {
            builder.FillRow(row, quotaList, new QuotaRow() { FieldPrecodes = precodes });

            Assert.AreEqual(row[QuotaManager.ExtraCounter], value);
           
        }

        


        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetCellCounterOfScheduledCalls_7InterviewWith3ScheduledAnd2InProgress_ResultCorrect()
        {
            var surveySid = BackendToolsObject.CreateSurvey(null, null, null);

            var quota = TestQuota.Create(TestingFramework.DbEngine,
                surveySid,
                1,
                new[] { "q1", "q2" },
                new[] { 2, 2 });

            quota.MockWs();

            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.None, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.InProgress, quota, "1", "2");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "2", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.None, quota, "2", "2");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "1", null);
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.InProgress, quota, "1", null);

            var fields = _quotaInfoService.GetQuotaFields(surveySid, quota.QuotaId);
            var parameters = new CallsCounterParameter(surveySid, quota.QuotaId, false, null, fields);
            var calculator = ExtraQuotaCounterService.Create(parameters);
            var result = calculator.GetCellCounter();

            TestAssert.AreEqual(
                (IEnumerable<QuotaCellCounter>)result.OrderBy(x => x.Descriptor).ToArray(),
                new[]
                    {
                        new QuotaCellCounter { Descriptor = "1,1", Value = 1 },
                        new QuotaCellCounter { Descriptor = "2,1", Value = 1 }
                    }.OrderBy(x => x.Descriptor).ToArray());

            CheckIts(quota, calculator, new[] { "1", "1" }, new Dictionary<int, int> { { (int)CallOutcome.FreshSample, 1 } });
            CheckIts(quota, calculator, new[] { "1", "2" }, new Dictionary<int, int>());
            CheckIts(quota, calculator, new[] { "2", "1" }, new Dictionary<int, int> { { (int)CallOutcome.FreshSample, 1 } });
            CheckIts(quota, calculator, new[] { "2", "2" }, new Dictionary<int, int>());

            BackendTools.ForceProcessingAsyncTriggers();
            TestAssert.AreEqual(CallQueueService.GetCountOfScheduledInterviews(surveySid), 3);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetCellCounterOfScheduledCalls_12InterviewWith7Scheduled2InProgress2Deleted_ResultCorrect()
        {
            var surveySid = BackendToolsObject.CreateSurvey(null, null, null);

            var quota = TestQuota.Create(TestingFramework.DbEngine,
                surveySid,
                1,
                new[] { "q1", "q2" },
                new[] { 2, 2 });

            quota.MockWs();

            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.None, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.InProgress, quota, "1", "2");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "2", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "2", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "2", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Delete, quota, "2", "2");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.InProgress, quota, "1", null);
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, null, "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Delete, quota, null, null);
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, null, "1");

            var fields = _quotaInfoService.GetQuotaFields(surveySid, quota.QuotaId);
            var parameters = new CallsCounterParameter(surveySid, quota.QuotaId, false, null, fields);
            var calculator = ExtraQuotaCounterService.Create(parameters);
            var result = calculator.GetCellCounter();

            TestAssert.AreEqual(
                (IEnumerable<QuotaCellCounter>)result.OrderBy(x => x.Descriptor).ToArray(),
                new[]
                    {
                        new QuotaCellCounter { Descriptor = "1,1", Value = 2 },
                        new QuotaCellCounter { Descriptor = "2,1", Value = 3 }
                    }.OrderBy(x => x.Descriptor).ToArray());

            CheckIts(quota, calculator, new[] { "1", "1" }, new Dictionary<int, int> { { (int)CallOutcome.FreshSample, 2 } });
            CheckIts(quota, calculator, new[] { "1", "2" }, new Dictionary<int, int>());
            CheckIts(quota, calculator, new[] { "2", "1" }, new Dictionary<int, int> { { (int)CallOutcome.FreshSample, 3 } });
            CheckIts(quota, calculator, new[] { "2", "2" }, new Dictionary<int, int>());

            BackendTools.ForceProcessingAsyncTriggers();
            TestAssert.AreEqual(CallQueueService.GetCountOfScheduledInterviews(surveySid), 7);
        }


        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetCellCounterOfScheduledCallsWithSpecificITS_12InterviewWith8Scheduled_ResultCorrect()
        {
            var surveySid = BackendToolsObject.CreateSurvey(null, null, null);

            var quota = TestQuota.Create(TestingFramework.DbEngine,
                surveySid,
                1,
                new[] { "q1", "q2" },
                new[] { 2, 2 });

            quota.MockWs();
            
            var itsList = new[] { (int)CallOutcome.FreshSample };

            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.None, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "1", "2");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "2", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.None, quota, "2", "2");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "1", null);

            CreateInterview(surveySid, CallOutcome.NoReply, InterviewCallState.None, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.NoReply, InterviewCallState.Normal, quota, "1", "2");
            CreateInterview(surveySid, CallOutcome.NoReply, InterviewCallState.Normal, quota, "2", "1");
            CreateInterview(surveySid, CallOutcome.NoReply, InterviewCallState.None, quota, "2", "2");
            CreateInterview(surveySid, CallOutcome.NoReply, InterviewCallState.Normal, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.NoReply, InterviewCallState.Normal, quota, "1", null);

            var fields = _quotaInfoService.GetQuotaFields(surveySid, quota.QuotaId);
            var parameters = new CallsCounterParameter(surveySid, quota.QuotaId, false, itsList, fields);
            var calculator = ExtraQuotaCounterService.Create(parameters);
            var result = calculator.GetCellCounter();

            TestAssert.AreEqual(
                (IEnumerable<QuotaCellCounter>)result.OrderBy(x => x.Descriptor).ToArray(),
                new[]
                    {
                        new QuotaCellCounter { Descriptor = "1,1", Value = 1 },
                        new QuotaCellCounter { Descriptor = "1,2", Value = 1 },
                        new QuotaCellCounter { Descriptor = "2,1", Value = 1 }
                    }.OrderBy(x => x.Descriptor).ToArray());

            CheckIts(quota, calculator, new[] { "1", "1" }, new Dictionary<int, int> { { (int)CallOutcome.FreshSample, 1 } });
            CheckIts(quota, calculator, new[] { "1", "2" }, new Dictionary<int, int> { { (int)CallOutcome.FreshSample, 1 } });
            CheckIts(quota, calculator, new[] { "2", "1" }, new Dictionary<int, int> { { (int)CallOutcome.FreshSample, 1 } });
            CheckIts(quota, calculator, new[] { "2", "2" }, new Dictionary<int, int>());

            BackendTools.ForceProcessingAsyncTriggers();
            TestAssert.AreEqual(CallQueueService.GetCountOfScheduledInterviewsWithSpecificIts(surveySid, itsList), 4);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetCellCounterOfScheduledCallsWithSpecificITS_14nterviewWith3ScheduledAnd2Deleted_ResultCorrect()
        {
            var surveySid = BackendToolsObject.CreateSurvey(null, null, null);

            var quota = TestQuota.Create(TestingFramework.DbEngine,
                surveySid,
                1,
                new[] { "q1", "q2" },
                new[] { 2, 2 });

            quota.MockWs();

            var itsList = new[] { (int)CallOutcome.FreshSample, (int)CallOutcome.NoReply };

            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.None, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "1", "2");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Delete, quota, "2", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.None, quota, "2", "2");
            CreateInterview(surveySid, CallOutcome.NoReply, InterviewCallState.Normal, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "1", null);
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Delete, quota, "1", null);

            CreateInterview(surveySid, CallOutcome.Busy, InterviewCallState.None, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.Busy, InterviewCallState.Normal, quota, "1", "2");
            CreateInterview(surveySid, CallOutcome.Busy, InterviewCallState.Delete, quota, "2", "1");
            CreateInterview(surveySid, CallOutcome.Busy, InterviewCallState.None, quota, "2", "2");
            CreateInterview(surveySid, CallOutcome.Busy, InterviewCallState.Normal, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.Busy, InterviewCallState.Normal, quota, "1", null);
            CreateInterview(surveySid, CallOutcome.Busy, InterviewCallState.Delete, quota, "1", null);

            var fields = _quotaInfoService.GetQuotaFields(surveySid, quota.QuotaId);
            var parameters = new CallsCounterParameter(surveySid, quota.QuotaId, false, itsList, fields);
            var calculator = ExtraQuotaCounterService.Create(parameters);
            var result = calculator.GetCellCounter();

            TestAssert.AreEqual(
                (IEnumerable<QuotaCellCounter>)result.OrderBy(x => x.Descriptor).ToArray(),
                new[]
                    {
                        new QuotaCellCounter { Descriptor = "1,1", Value = 1 },
                        new QuotaCellCounter { Descriptor = "1,2", Value = 1 }
                    }.OrderBy(x => x.Descriptor).ToArray());

            CheckIts(quota, calculator, new[] { "1", "1" }, new Dictionary<int, int> { { (int)CallOutcome.NoReply, 1 } });
            CheckIts(quota, calculator, new[] { "1", "2" }, new Dictionary<int, int> { { (int)CallOutcome.FreshSample, 1 } });
            CheckIts(quota, calculator, new[] { "2", "1" }, new Dictionary<int, int>());
            CheckIts(quota, calculator, new[] { "2", "2" }, new Dictionary<int, int>());

            BackendTools.ForceProcessingAsyncTriggers();
            TestAssert.AreEqual(CallQueueService.GetCountOfScheduledInterviewsWithSpecificIts(surveySid, itsList), 3);


        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetCellCounterOfScheduledCallsWithSpecificITS_9InterviewWith5ScheduledAnd2InProgress_ResultCorrect()
        {
            var surveySid = BackendToolsObject.CreateSurvey(null, null, null);

            var quota = TestQuota.Create(TestingFramework.DbEngine,
                surveySid,
                1,
                new[] { "q1", "q2" },
                new[] { 2, 2 });

            quota.MockWs();

            var itsList = new[] { (int)CallOutcome.FreshSample, (int)CallOutcome.Busy };

            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.None, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.InProgress, quota, "1", "2");
            CreateInterview(surveySid, CallOutcome.Busy, InterviewCallState.Normal, quota, "2", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "2", "1");
            CreateInterview(surveySid, CallOutcome.NoReply, InterviewCallState.Normal, quota, "2", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.None, quota, "2", "2");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "1", null);
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.InProgress, quota, "1", null);

            var fields = _quotaInfoService.GetQuotaFields(surveySid, quota.QuotaId);
            var parameters = new CallsCounterParameter(surveySid, quota.QuotaId, false, itsList, fields);
            var calculator = ExtraQuotaCounterService.Create(parameters);
            var result = calculator.GetCellCounter();

            TestAssert.AreEqual(
                (IEnumerable<QuotaCellCounter>)result.OrderBy(x => x.Descriptor).ToArray(),
                new[]
                    {
                        new QuotaCellCounter { Descriptor = "1,1", Value = 1 },
                        new QuotaCellCounter { Descriptor = "2,1", Value = 2 }
                    }.OrderBy(x => x.Descriptor).ToArray());

            CheckIts(quota, calculator, new[] { "1", "1" }, new Dictionary<int, int> { { (int)CallOutcome.FreshSample, 1 } });
            CheckIts(quota, calculator, new[] { "1", "2" }, new Dictionary<int, int>());
            CheckIts(quota, calculator, new[] { "2", "1" }, new Dictionary<int, int> { { (int)CallOutcome.FreshSample, 1 }, { (int)CallOutcome.Busy, 1 }});
            CheckIts(quota, calculator, new[] { "2", "2" }, new Dictionary<int, int>());

            BackendTools.ForceProcessingAsyncTriggers();
            TestAssert.AreEqual(CallQueueService.GetCountOfScheduledInterviewsWithSpecificIts(surveySid, itsList), 4);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetCellCounterOfScheduledCallsWithSpecificITS_14InterviewWith7Scheduled2InProgress2Deleted_ResultCorrect()
        {
            var surveySid = BackendToolsObject.CreateSurvey(null, null, null);

            var quota = TestQuota.Create(TestingFramework.DbEngine,
                surveySid,
                1,
                new[] { "q1", "q2" },
                new[] { 2, 2 });

            quota.MockWs();

            var itsList = new[] { (int)CallOutcome.FreshSample };

            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.None, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.NoReply, InterviewCallState.Normal, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.InProgress, quota, "1", "2");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "2", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "2", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "2", "1");
            CreateInterview(surveySid, CallOutcome.NoReply, InterviewCallState.Normal, quota, "2", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Delete, quota, "2", "2");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.InProgress, quota, "1", null);
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, null, "1");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Delete, quota, null, null);
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, null, "1");

            var fields = _quotaInfoService.GetQuotaFields(surveySid, quota.QuotaId);
            var parameters = new CallsCounterParameter(surveySid, quota.QuotaId, false, itsList, fields);
            var calculator = ExtraQuotaCounterService.Create(parameters);
            var result = calculator.GetCellCounter();

            TestAssert.AreEqual(
                (IEnumerable<QuotaCellCounter>)result.OrderBy(x => x.Descriptor).ToArray(),
                new[]
                    {
                        new QuotaCellCounter { Descriptor = "1,1", Value = 2 },
                        new QuotaCellCounter { Descriptor = "2,1", Value = 3 }
                    }.OrderBy(x => x.Descriptor).ToArray());

            CheckIts(quota, calculator, new[] { "1", "1" }, new Dictionary<int, int> { { (int)CallOutcome.FreshSample, 2 } });
            CheckIts(quota, calculator, new[] { "1", "2" }, new Dictionary<int, int>());
            CheckIts(quota, calculator, new[] { "2", "1" }, new Dictionary<int, int> { { (int)CallOutcome.FreshSample, 3 }});
            CheckIts(quota, calculator, new[] { "2", "2" }, new Dictionary<int, int>());

            BackendTools.ForceProcessingAsyncTriggers();
            TestAssert.AreEqual(CallQueueService.GetCountOfScheduledInterviewsWithSpecificIts(surveySid, itsList), 7);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetCellCounterOfInterviewsWithSprcificITS_9InterviewWith4FreshSample_ResultCorrect()
        {
            var surveySid = BackendToolsObject.CreateSurvey(null, null, null);

            var quota = TestQuota.Create(TestingFramework.DbEngine,
                surveySid,
                1,
                new[] { "q1", "q2" },
                new[] { 2, 2 });

            quota.MockWs();

            var itsList = new[] { (int)CallOutcome.FreshSample };

            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.Appointment, InterviewCallState.InProgress, quota, "1", "2");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Delete, quota, "2", "1");
            CreateInterview(surveySid, CallOutcome.Appointment, InterviewCallState.None, quota, "2", "2");
            CreateInterview(surveySid, CallOutcome.Busy, InterviewCallState.Normal, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.Appointment, InterviewCallState.InProgress, quota, "1", null);
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Delete, quota, null, "1");
            CreateInterview(surveySid, CallOutcome.Appointment, InterviewCallState.None, quota, null, null);
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, null, "1");

            var fields = _quotaInfoService.GetQuotaFields(surveySid, quota.QuotaId);
            var parameters = new InterviewsCounterParameter(surveySid, quota.QuotaId, itsList, fields);
            var calculator = ExtraQuotaCounterService.Create(parameters);
            var result = calculator.GetCellCounter();

            TestAssert.AreEqual(
                (IEnumerable<QuotaCellCounter>)result.OrderBy(x => x.Descriptor).ToArray(),
                new[]
                    {
                        new QuotaCellCounter { Descriptor = "1,1", Value = 1 },
                        new QuotaCellCounter { Descriptor = "2,1", Value = 1 }
                    }.OrderBy(x => x.Descriptor).ToArray());

            CheckIts(quota, calculator, new[] { "1", "1" }, new Dictionary<int, int> { { (int)CallOutcome.FreshSample, 1 } });
            CheckIts(quota, calculator, new[] { "1", "2" }, new Dictionary<int, int>());
            CheckIts(quota, calculator, new[] { "2", "1" }, new Dictionary<int, int> { { (int)CallOutcome.FreshSample, 1 }});
            CheckIts(quota, calculator, new[] { "2", "2" }, new Dictionary<int, int>());

            BackendTools.ForceProcessingAsyncTriggers();
            TestAssert.AreEqual(InterviewService.GetCountOfInterviewsWithSpecificITSs(surveySid, itsList), 4);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetCellCounterOfInterviewsWithSprcificITS_9InterviewWith4FreshSampleAnd4Appointment_ResultCorrect()
        {
            var surveySid = BackendToolsObject.CreateSurvey(null, null, null);

            var quota = TestQuota.Create(TestingFramework.DbEngine,
                surveySid,
                1,
                new[] { "q1", "q2" },
                new[] { 2, 2 });

            quota.MockWs();

            var itsList = new[] { (int)CallOutcome.FreshSample, (int)CallOutcome.Appointment };

            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.Appointment, InterviewCallState.InProgress, quota, "1", "2");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Delete, quota, "2", "1");
            CreateInterview(surveySid, CallOutcome.Appointment, InterviewCallState.None, quota, "2", "1");
            CreateInterview(surveySid, CallOutcome.Busy, InterviewCallState.Normal, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.Appointment, InterviewCallState.InProgress, quota, "1", null);
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Delete, quota, null, "1");
            CreateInterview(surveySid, CallOutcome.Appointment, InterviewCallState.None, quota, null, null);
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, null, "1");

            var fields = _quotaInfoService.GetQuotaFields(surveySid, quota.QuotaId);
            var parameters = new InterviewsCounterParameter(surveySid, quota.QuotaId, itsList, fields);
            var calculator = ExtraQuotaCounterService.Create(parameters);
            var result = calculator.GetCellCounter();

            CheckIts(quota, calculator, new[] { "1", "1" }, new Dictionary<int, int> { {(int)CallOutcome.FreshSample, 1}});
            CheckIts(quota, calculator, new[] { "1", "2" }, new Dictionary<int, int> { { (int)CallOutcome.Appointment, 1 } });
            CheckIts(quota, calculator, new[] { "2", "1" }, new Dictionary<int, int> { { (int)CallOutcome.FreshSample, 1 }, { (int)CallOutcome.Appointment, 1 } });
            CheckIts(quota, calculator, new[] { "2", "2" }, new Dictionary<int, int>());

            TestAssert.AreEqual(
                (IEnumerable<QuotaCellCounter>)result.OrderBy(x => x.Descriptor).ToArray(),
                new[]
                    {
                        new QuotaCellCounter { Descriptor = "1,1", Value = 1 },
                        new QuotaCellCounter { Descriptor = "1,2", Value = 1 },
                        new QuotaCellCounter { Descriptor = "2,1", Value = 2 }
                    }.OrderBy(x => x.Descriptor).ToArray());

            BackendTools.ForceProcessingAsyncTriggers();
            TestAssert.AreEqual(InterviewService.GetCountOfInterviewsWithSpecificITSs(surveySid, itsList), 8);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetCellCounterOfInterviewsWithSprcificITS_RequestWithoutITS_ResultEmpty()
        {
            var surveySid = BackendToolsObject.CreateSurvey(null, null, null);

            var quota = TestQuota.Create(TestingFramework.DbEngine,
                surveySid,
                1,
                new[] { "q1", "q2" },
                new[] { 2, 2 });

            quota.MockWs();

            var itsList = new int[] { };

            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.Appointment, InterviewCallState.InProgress, quota, "1", "2");
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Delete, quota, "2", "1");
            CreateInterview(surveySid, CallOutcome.Appointment, InterviewCallState.None, quota, "2", "1");
            CreateInterview(surveySid, CallOutcome.Busy, InterviewCallState.Normal, quota, "1", "1");
            CreateInterview(surveySid, CallOutcome.Appointment, InterviewCallState.InProgress, quota, "1", null);
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Delete, quota, null, "1");
            CreateInterview(surveySid, CallOutcome.Appointment, InterviewCallState.None, quota, null, null);
            CreateInterview(surveySid, CallOutcome.FreshSample, InterviewCallState.Normal, quota, null, "1");

            var fields = _quotaInfoService.GetQuotaFields(surveySid, quota.QuotaId);
            var parameters = new InterviewsCounterParameter(surveySid, quota.QuotaId, itsList, fields);
            var calculator = ExtraQuotaCounterService.Create(parameters);
            var result = calculator.GetCellCounter();

            CheckIts(quota, calculator, new[] { "1", "1" }, new Dictionary<int, int>());
            CheckIts(quota, calculator, new[] { "1", "2" }, new Dictionary<int, int>());
            CheckIts(quota, calculator, new[] { "2", "1" }, new Dictionary<int, int>());
            CheckIts(quota, calculator, new[] { "2", "2" }, new Dictionary<int, int>());

            TestAssert.AreEqual(
                (IEnumerable<QuotaCellCounter>)result.OrderBy(x => x.Descriptor).ToArray(),
                new QuotaCellCounter[]
                    {
                    }.OrderBy(x => x.Descriptor).ToArray());

            BackendTools.ForceProcessingAsyncTriggers();
            TestAssert.AreEqual(InterviewService.GetCountOfInterviewsWithSpecificITSs(surveySid, itsList), 0);
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void GetDailyQuotaCounters_CompletedCallsWithDiffItss_FirstCellHasCounterEqual3_SecondAndThirdEqualOne_LastEqualZero()
        {
            var surveySid = BackendToolsObject.CreateSurvey(null, null, null);

            var quota = TestQuota.Create(TestingFramework.DbEngine,
                surveySid,
                1,
                new[] { "q1", "q2" },
                new[] { 2, 2 });

            quota.MockWs();

            var time = new DateTime(2016, 11, 23, 0, 0, 0);
            new DateTimeMocker(TestingFramework).MockDate(time);

            CreateInterviewWithHistoryRecord(surveySid, CallOutcome.Completed, quota, time.AddHours(2), "1", "1");
            CreateInterviewWithHistoryRecord(surveySid, CallOutcome.Completed, quota, time.AddHours(2), "1", "1");
            CreateInterviewWithHistoryRecord(surveySid, (CallOutcome)31, quota, time.AddHours(2), "1", "1");
            CreateInterviewWithHistoryRecord(surveySid, (CallOutcome)32, quota, time.AddHours(2), "1", "1");
            CreateInterviewWithHistoryRecord(surveySid, CallOutcome.Completed, quota, time.AddHours(2), "1", "2");
            CreateInterviewWithHistoryRecord(surveySid, CallOutcome.Completed, quota, time.AddHours(2), "2", "1");
            CreateInterviewWithHistoryRecord(surveySid, CallOutcome.Completed, quota, time.AddDays(300), "2", "2");
       
            var fields = _quotaInfoService.GetQuotaFields(surveySid, quota.QuotaId);
            var parameters = new DailyCounterParameter(surveySid, quota.QuotaId, new[] {13,31}, fields);
            var calculator = ExtraQuotaCounterService.Create(parameters);
            var result = calculator.GetCellCounter();

            TestAssert.AreEqual(
                (IEnumerable<QuotaCellCounter>)result.OrderBy(x => x.Descriptor).ToArray(),
                new[]
                    {
                        new QuotaCellCounter { Descriptor = "1,1", Value = 3 },
                        new QuotaCellCounter { Descriptor = "1,2", Value = 1 },
                        new QuotaCellCounter { Descriptor = "2,1", Value = 1 }
                    }.OrderBy(x => x.Descriptor).ToArray());

            CheckIts(quota, calculator, new[] { "1", "1" }, new Dictionary<int, int> { { (int)CallOutcome.Completed, 2 }, {31,1} });
            CheckIts(quota, calculator, new[] { "1", "2" }, new Dictionary<int, int> { { (int)CallOutcome.Completed, 1 } });
            CheckIts(quota, calculator, new[] { "2", "1" }, new Dictionary<int, int> { { (int)CallOutcome.Completed, 1 } });
            CheckIts(quota, calculator, new[] { "2", "2" }, new Dictionary<int, int>());
            //TODO:need to modify test to work with different TZ
            /*CallCenterTools.SetCallCenterTimeZone(55);  //-3 hours
            result = calculator.GetCellCounter();
            Assert.AreEqual( 0, result.Count());
             */

        }

        [TestMethod, Owner(@"FIRM\EvgeniiL")]
        public void GetDailyQuotaCountersForPeriod_default_lastWeek_lastYear()
        {
            var surveySid = BackendToolsObject.CreateSurvey(null, null, null);

            var quota = TestQuota.Create(TestingFramework.DbEngine,
                surveySid,
                1,
                new[] { "q1", "q2" },
                new[] { 2, 2 });

            quota.MockWs();

            var time = new DateTime(2021, 12, 20, 0, 0, 0);
            new DateTimeMocker(TestingFramework).MockDate(time);

            CreateInterviewWithHistoryRecord(surveySid, CallOutcome.Completed, quota, time.AddHours(2), "1", "1");
            CreateInterviewWithHistoryRecord(surveySid, CallOutcome.Completed, quota, time.AddHours(2), "1", "1");
            CreateInterviewWithHistoryRecord(surveySid, (CallOutcome)31, quota, time.AddHours(2), "1", "1");
            CreateInterviewWithHistoryRecord(surveySid, (CallOutcome)32, quota, time.AddHours(2), "1", "1");
            CreateInterviewWithHistoryRecord(surveySid, CallOutcome.Completed, quota, time.AddHours(2), "1", "2");
            CreateInterviewWithHistoryRecord(surveySid, CallOutcome.Completed, quota, time.AddHours(2), "2", "1");
            CreateInterviewWithHistoryRecord(surveySid, CallOutcome.Completed, quota, time.AddDays(300), "2", "2");

            CreateInterviewWithHistoryRecord(surveySid, CallOutcome.Completed, quota, time.AddDays(-7), "1", "1");
            CreateInterviewWithHistoryRecord(surveySid, CallOutcome.Completed, quota, time.AddDays(-7), "1", "1");
            CreateInterviewWithHistoryRecord(surveySid, (CallOutcome)31, quota, time.AddDays(-7), "1", "1");
            CreateInterviewWithHistoryRecord(surveySid, (CallOutcome)32, quota, time.AddDays(-7), "1", "1");
            CreateInterviewWithHistoryRecord(surveySid, CallOutcome.Completed, quota, time.AddDays(-7), "1", "2");
            CreateInterviewWithHistoryRecord(surveySid, CallOutcome.Completed, quota, time.AddDays(-7), "2", "1");
            CreateInterviewWithHistoryRecord(surveySid, CallOutcome.Completed, quota, time.AddDays(-7), "2", "2");


            CreateInterviewWithHistoryRecord(surveySid, CallOutcome.Completed, quota, time.AddDays(-45), "1", "1");
            CreateInterviewWithHistoryRecord(surveySid, CallOutcome.Completed, quota, time.AddDays(-45), "1", "1");
            CreateInterviewWithHistoryRecord(surveySid, (CallOutcome)31, quota, time.AddDays(-45), "1", "1");
            CreateInterviewWithHistoryRecord(surveySid, (CallOutcome)32, quota, time.AddDays(-60), "1", "1");
            CreateInterviewWithHistoryRecord(surveySid, CallOutcome.Completed, quota, time.AddDays(-70), "1", "2");
            CreateInterviewWithHistoryRecord(surveySid, CallOutcome.Completed, quota, time.AddDays(-80), "2", "1");
            CreateInterviewWithHistoryRecord(surveySid, CallOutcome.Completed, quota, time.AddDays(-90), "2", "2");

            var fields = _quotaInfoService.GetQuotaFields(surveySid, quota.QuotaId);

            var parameters = new DailyCounterParameter(surveySid, quota.QuotaId, new[] { 13, 31 }, fields);
            var calculator = ExtraQuotaCounterService.Create(parameters);
            var result = calculator.GetCellCounter();

            TestAssert.AreEqual(
                (IEnumerable<QuotaCellCounter>)result.OrderBy(x => x.Descriptor).ToArray(),
                new[]
                    {
                        new QuotaCellCounter { Descriptor = "1,1", Value = 3 },
                        new QuotaCellCounter { Descriptor = "1,2", Value = 1 },
                        new QuotaCellCounter { Descriptor = "2,1", Value = 1 }
                    }.OrderBy(x => x.Descriptor).ToArray());

             parameters = new DailyCounterParameter(surveySid, quota.QuotaId, new[] { 13, 31 }, fields, (time.AddDays(-7), time.AddDays(1)));
             calculator = ExtraQuotaCounterService.Create(parameters);
             result = calculator.GetCellCounter();

            TestAssert.AreEqual(
                (IEnumerable<QuotaCellCounter>)result.OrderBy(x => x.Descriptor).ToArray(),
                new[]
                {
                    new QuotaCellCounter { Descriptor = "1,1", Value = 6 },
                    new QuotaCellCounter { Descriptor = "1,2", Value = 2 },
                    new QuotaCellCounter { Descriptor = "2,1", Value = 2 },
                    new QuotaCellCounter { Descriptor = "2,2", Value = 1 }
                }.OrderBy(x => x.Descriptor).ToArray());

            parameters = new DailyCounterParameter(surveySid, quota.QuotaId, new[] { 13, 31 }, fields, (time.AddYears(-1), time.AddDays(1)));
            calculator = ExtraQuotaCounterService.Create(parameters);
            result = calculator.GetCellCounter();

            TestAssert.AreEqual(
                (IEnumerable<QuotaCellCounter>)result.OrderBy(x => x.Descriptor).ToArray(),
                new[]
                {
                    new QuotaCellCounter { Descriptor = "1,1", Value = 9 },
                    new QuotaCellCounter { Descriptor = "1,2", Value = 3 },
                    new QuotaCellCounter { Descriptor = "2,1", Value = 3 },
                    new QuotaCellCounter { Descriptor = "2,2", Value = 2 }
                }.OrderBy(x => x.Descriptor).ToArray());
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void GetDailyQuotaCounters_ItsNotDefined_FirstCellHasCounterEqual4_SecondAndThirdEqualOne_LastEqualZero()
        {
            var surveySid = BackendToolsObject.CreateSurvey(null, null, null);

            var quota = TestQuota.Create(TestingFramework.DbEngine,
                surveySid,
                1,
                new[] { "q1", "q2" },
                new[] { 2, 2 });

            quota.MockWs();

            var time = new DateTime(2016, 11, 23, 0, 0, 0);
            new DateTimeMocker(TestingFramework).MockDate(time);

            CreateInterviewWithHistoryRecord(surveySid, CallOutcome.Completed, quota, time.AddHours(2), "1", "1");
            CreateInterviewWithHistoryRecord(surveySid, CallOutcome.Completed, quota, time.AddHours(2), "1", "1");
            CreateInterviewWithHistoryRecord(surveySid, (CallOutcome)31, quota, time.AddHours(2), "1", "1");
            CreateInterviewWithHistoryRecord(surveySid, (CallOutcome)32, quota, time.AddHours(2), "1", "1");
            CreateInterviewWithHistoryRecord(surveySid, CallOutcome.Completed, quota, time.AddHours(2), "1", "2");
            CreateInterviewWithHistoryRecord(surveySid, CallOutcome.Completed, quota, time.AddHours(2), "2", "1");
            CreateInterviewWithHistoryRecord(surveySid, CallOutcome.Completed, quota, time.AddDays(300), "2", "2");

            var fields = _quotaInfoService.GetQuotaFields(surveySid, quota.QuotaId);
           
            var parameters = new DailyCounterParameter(surveySid, quota.QuotaId, new int[] { }, fields);
            var calculator = ExtraQuotaCounterService.Create(parameters);
            var result = calculator.GetCellCounter();

            TestAssert.AreEqual(
                (IEnumerable<QuotaCellCounter>)result.OrderBy(x => x.Descriptor).ToArray(),
                new[]
                    {
                        new QuotaCellCounter { Descriptor = "1,1", Value = 4 },
                        new QuotaCellCounter { Descriptor = "1,2", Value = 1 },
                        new QuotaCellCounter { Descriptor = "2,1", Value = 1 }
                    }.OrderBy(x => x.Descriptor).ToArray());

            CheckIts(quota, calculator, new[] { "1", "1" }, new Dictionary<int, int> { { (int)CallOutcome.Completed, 2 }, { 31, 1 }, {32,1} });
            CheckIts(quota, calculator, new[] { "1", "2" }, new Dictionary<int, int> { { (int)CallOutcome.Completed, 1 } });
            CheckIts(quota, calculator, new[] { "2", "1" }, new Dictionary<int, int> { { (int)CallOutcome.Completed, 1 } });
            CheckIts(quota, calculator, new[] { "2", "2" }, new Dictionary<int, int>());
        }



        [TestMethod, Owner(@"FIRM\LiubovK")]
        public void GetCellCounter_CreateQuotaWithInterviewsWithDifferentCallStatesAndEnabledIncludeDisableCallParameter_ResultCorrect()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData { Tag="S1", IsUseDb = true,
                        Forms = new []
                        {
                            new SingleFormData{Name="q1", Precodes = new []{"1", "2"}},
                        },
                        Quotas = new[]{
                            new QuotaData(){ Id = 1, Name="Q1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=10},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=10},
                                }
                            }},
                        Interviews = new []
                        {
                            new InterviewData(){Tag="S1.I1", Data="q1=1",  Call = new CallData(){CallState = (int)Common.CallState.DisabledByFCD}},
                            new InterviewData(){Tag="S1.I2", Data="q1=1", Call = new CallData(){CallState = (int)Common.CallState.Scheduled}},
                            new InterviewData(){Tag="S1.I3", Data="q1=1", Call = new CallData(){CallState = (int)Common.CallState.DisabledByUser}},
                            new InterviewData(){Tag="S1.I4", Data="q1=2",  Call = new CallData(){CallState = (int)Common.CallState.DisabledByFCD}},
                            new InterviewData(){Tag="S1.I5", Data="q1=2", Call = new CallData(){CallState = (int)Common.CallState.Scheduled}},
                            new InterviewData(){Tag="S1.I6", Data="q1=2", Call = new CallData(){CallState = (int)Common.CallState.DisabledByUser}}
                        }
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var quotaId = 1;

            var fields = _quotaInfoService.GetQuotaFields(survey.Id, quotaId);

            var parameters = new CallsCounterParameter(survey.Id, quotaId, true, null, fields);
            var callCounterCalculator = new CallsCounterCalculator(parameters);
            var cellCounter = callCounterCalculator.GetCellCounter();

            Assert.AreEqual(BackendTools.Format(cellCounter), @"
 Descriptor Value
          1     3
          2     3");

            parameters = new CallsCounterParameter(survey.Id, quotaId, false, null, fields);
            callCounterCalculator = new CallsCounterCalculator(parameters);
            cellCounter = callCounterCalculator.GetCellCounter();

            Assert.AreEqual(BackendTools.Format(cellCounter), @"
 Descriptor Value
          1     1
          2     1");
        }


        private void CheckIts(TestQuota quota, IExtraQuotaCounterCalculator calculator, string[] cellValue, Dictionary<int, int> expected)
        {
            var cellId = quota.GetQuotaCell(cellValue);
            var actual = calculator.GetItsCountersForCell(cellId);

            Assert.AreEqual(CountersToString(expected), CountersToString(actual), String.Format("Wrong counters of cell [{0}]", String.Join(",", cellValue)));
        }

        private static string CountersToString(IEnumerable<KeyValuePair<int, int>> counters)
        {
            return String.Join( ",", counters.OrderBy(x => x.Key).Select(y => String.Format("{0}={1}", y.Key, y.Value)));
        }
    }
}
