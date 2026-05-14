using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using Confirmit.CATI.Core.Services.InterviewServiceImplementation;
using Confirmit.CATI.Core.Services.InterviewServiceImplementation.Fakes;
using Confirmit.CATI.Core.Services.SampleServiceImplementation;
using Confirmit.CATI.Core.Services.SampleServiceImplementation.Fakes;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Confirmit.CATI.IntegrationTests.Tests.SampleTest
{
    [TestClass]
    public class SampleWithFCDTests
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;

        [TestInitialize]
        public void Init()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);

            //emulate CF table where data for sample is stored
            ConfirmitTools.CreateRespondentTable(_framework.DbEngine);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _framework.TestCleanup();
        }

        const int BatchId = 1;

        public BvInterviewEntity GetExpectedInterview(int number, int surveySid)
        {
            return new BvInterviewEntity
            {
                ID = number,
                SurveySID = surveySid,
                BatchID = BatchId,
                TransientState = (int)CallOutcome.FreshSample,
                Duration = number,
                ExtensionNumber = number.ToString(CultureInfo.InvariantCulture),
                TelephoneNumber = number.ToString(CultureInfo.InvariantCulture),
                TimezoneID = number,
                RespondentName = "resp" + number.ToString(CultureInfo.InvariantCulture)
            };
        }

        internal static void AsyncCloseCell(TestQuota quota, int cellId)
        {
            System.Action asyncAction = () =>
            {
                try
                {
                    quota.CloseCell(cellId);
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning(ex.ToString());
                }
            };
            var asyncResult = asyncAction.BeginInvoke(null, null);
            asyncAction.EndInvoke(asyncResult);
        }

        

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void BvFMWS_AddSampleWithFullSchedulingWithSomeInterviewInClosedCell_CallsShouldNotBeAddedForClosedCells()
        {
            _backendTools.LaunchAllHoursScript();
            var context = new TestData
            {
                Surveys = new[] {
                    new SurveyData
                    {
                        Tag="S1", IsUseDb = true, SchedulingScript = AllHoursSchedule.Name,
                        Forms = new[] {
                            new SingleFormData { Name="q1", Precodes = new []{"1", "2", "3"} },
                            new SingleFormData { Name="q2", Precodes = new []{"1", "2", "3"} }
                        },
                        Quotas = new [] {
                            new QuotaData
                            {
                                Id = 1, Name="quota1", Fields = new[] {"q1", "q2"},
                                Cells = new[]
                                {
                                    new CellData { Id = 1, Values="q1=1,q2=1", Counter=6, Limit=10 }, 
                                    new CellData { Id = 2, Values="q1=2,q2=1", Counter=10, Limit=10 },//closed cells
                                    new CellData { Id = 3, Values="q1=1,q2=2", Counter=6, Limit=10 }, 
                                    new CellData { Id = 4, Values="q1=2,q2=2", Counter=6, Limit=10 },
                                    
                                }
                            }
                        }
                    }
                },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var survey = context.GetSurvey("S1");
            int surveySid = survey.Id;
   
            survey.AddSample(SchedulingMode.Full,
                new InterviewData { Tag = "S1.I1", Data = "q1=2,q2=1", ITS = CallOutcome.FreshSample },//in closed cell
                new InterviewData { Tag = "S1.I2", Data = "q1=1,q2=2", ITS = CallOutcome.FreshSample },
                new InterviewData { Tag = "S1.I3", Data = "q1=2,q2=1", ITS = CallOutcome.FreshSample },//in closed cell
                new InterviewData { Tag = "S1.I4", Data = "q1=1,q2=2", ITS = CallOutcome.FreshSample }
                );

            var interview1 = InterviewRepository.GetById(surveySid, 1);
            var interview2 = InterviewRepository.GetById(surveySid, 2);
            var interview3 = InterviewRepository.GetById(surveySid, 3);
            var interview4 = InterviewRepository.GetById(surveySid, 4);

            Assert.AreEqual( (int)CallOutcome.FilteredByCallDelivery,interview1.TransientState);
            Assert.AreEqual( (int)CallOutcome.FreshSample,interview2.TransientState);
            Assert.AreEqual( (int)CallOutcome.FilteredByCallDelivery,interview3.TransientState);
            Assert.AreEqual( (int)CallOutcome.FreshSample,interview4.TransientState);
            
            Assert.IsNull(CallQueueService.GetCallAndNoLock(surveySid, 1),
                String.Format("Call for interview with id {0} should not be added during sample addition", 1));
            Assert.IsNull(CallQueueService.GetCallAndNoLock(surveySid, 3),
                String.Format("Call for interview with id {0} should not be added during sample addition", 3));
            Assert.AreEqual(2, CallQueueService.GetCallAndNoLock(surveySid, 2).CallState,
                String.Format("Call for interview with id {0} should be loaded with phase 2", 2));
            Assert.AreEqual(2, CallQueueService.GetCallAndNoLock(surveySid, 4).CallState,
                String.Format("Call for interview with id {0} should be loaded with phase 2", 4));

            BackendTools.AssertAggregateData(surveySid, 4, 2/*call count*/);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddFullSchedulingSampleWithDeletingByFCD_AddInterviewsWithNotIgnoredITSFromClossedCells_CallAreFiltered()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", IsUseDb = true, SchedulingScript = AllHoursSchedule.Name,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2", "3"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=10, Limit=10},//closed cells
                                    new CellData(){Id = 2, Values="q1=2", Counter=6, Limit=10},
                                }
                            }
                        }
                    }
                },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var survey = context.GetSurvey("S1");

            survey.AddSample(SchedulingMode.Full,
                new InterviewData() { Tag = "S1.I1", Data = "q1=1", ITS = CallOutcome.FreshSample },
                new InterviewData() { Tag = "S1.I2", Data = "q1=1", ITS = CallOutcome.Appointment },
                new InterviewData() { Tag = "S1.I3", Data = "q1=2", ITS = CallOutcome.FreshSample },
                new InterviewData() { Tag = "S1.I4", Data = "q1=2", ITS = CallOutcome.Appointment });

            context.GetCalls("S1.I1", "S1.I2").Assert.IsNull();
            context.GetCalls("S1.I3", "S1.I4").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);

            var history = BvCallHistoryExAdapter.GetByCondition("SurveyId = @SurveyId", new SqlParameter("@SurveyId", survey.Id));

            var first = history.First();
            Assert.AreEqual(2, history.Count);
            Assert.AreEqual((int)OperationType.DeleteByFcdDuringSample, first.OperationType);
            Assert.AreEqual((byte)CallOutcome.FilteredByCallDelivery, first.ITS);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddFullSchedulingSampleWithDeletingByFCD_AddInterviewsWithIgnoredITSFromClossedCells_CallAreCreated()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", IsUseDb = true, SchedulingScript = AllHoursSchedule.Name,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2", "3"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=10, Limit=10},//closed cells
                                    new CellData(){Id = 2, Values="q1=2", Counter=6, Limit=10},
                                }
                            }
                        }
                    }
                },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var survey = context.GetSurvey("S1");

            new StateGroupController(null, survey.Model.StateGroupID, context).ChangeState((int)CallOutcome.FreshSample, x => x.FcdAction = true);

            survey.AddSample(SchedulingMode.Full,
                new InterviewData() { Tag = "S1.I1", Data = "q1=1", ITS = CallOutcome.FreshSample },
                new InterviewData() { Tag = "S1.I2", Data = "q1=1", ITS = CallOutcome.Appointment },
                new InterviewData() { Tag = "S1.I3", Data = "q1=2", ITS = CallOutcome.FreshSample },
                new InterviewData() { Tag = "S1.I4", Data = "q1=2", ITS = CallOutcome.Appointment });

            context.GetCalls("S1.I1", "S1.I2", "S1.I3", "S1.I4").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddFullSchedulingSampleWithDisablingByFCD_AddInterviewsWithNotIgnoredITSFromClossedCells_CallAreFiltered()
        {
            ServiceLocator.Resolve<ISystemSettings>().FCD.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;

            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", IsUseDb = true, SchedulingScript = AllHoursSchedule.Name,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2", "3"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=10, Limit=10},//closed cells
                                    new CellData(){Id = 2, Values="q1=2", Counter=6, Limit=10},
                                }
                            }
                        }
                    }
                },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var survey = context.GetSurvey("S1");

            survey.AddSample(SchedulingMode.Full,
                new InterviewData() { Tag = "S1.I1", Data = "q1=1", ITS = CallOutcome.FreshSample },
                new InterviewData() { Tag = "S1.I2", Data = "q1=1", ITS = CallOutcome.Appointment },
                new InterviewData() { Tag = "S1.I3", Data = "q1=2", ITS = CallOutcome.FreshSample },
                new InterviewData() { Tag = "S1.I4", Data = "q1=2", ITS = CallOutcome.Appointment });

            context.GetCalls("S1.I1", "S1.I2").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD);
            context.GetCalls("S1.I3", "S1.I4").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);

            var history = BvCallHistoryExAdapter.GetByCondition("SurveyId = @SurveyId", new SqlParameter("@SurveyId", survey.Id));

            var first = history.First();
            Assert.AreEqual(2, history.Count);
            Assert.AreEqual((int)OperationType.DisableByFcdDuringSample, first.OperationType);
            Assert.AreEqual((short)CallState.DisabledByFCD, first.CallState);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddFullSchedulingSampleWithDisablingByFCD_AddInterviewsWithIgnoredITSFromClossedCells_CallAreCreated()
        {
            ServiceLocator.Resolve<ISystemSettings>().FCD.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;

            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", IsUseDb = true, SchedulingScript = AllHoursSchedule.Name,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2", "3"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=10, Limit=10},//closed cells
                                    new CellData(){Id = 2, Values="q1=2", Counter=6, Limit=10},
                                }
                            }
                        }
                    }
                },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var survey = context.GetSurvey("S1");

            new StateGroupController(null, survey.Model.StateGroupID, context).ChangeState((int)CallOutcome.FreshSample, x => x.FcdAction = true);

            survey.AddSample(SchedulingMode.Full,
                new InterviewData() { Tag = "S1.I1", Data = "q1=1", ITS = CallOutcome.FreshSample },
                new InterviewData() { Tag = "S1.I2", Data = "q1=1", ITS = CallOutcome.Appointment },
                new InterviewData() { Tag = "S1.I3", Data = "q1=2", ITS = CallOutcome.FreshSample },
                new InterviewData() { Tag = "S1.I4", Data = "q1=2", ITS = CallOutcome.Appointment });

            context.GetCalls("S1.I1", "S1.I2", "S1.I3", "S1.I4").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }


        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddSimpleSchedulingSampleWithDeletingByFCD_AddInterviewsWithNotIgnoredITSFromClossedCells_CallAreFiltered()
        {
            var context = new TestData
            {
                Surveys = new[] {
                    new SurveyData
                    {
                        Tag="S1", IsUseDb = true, SchedulingScript = AllHoursSchedule.Name,
                        Forms = new[] {
                            new SingleFormData { Name="q1", Precodes = new [] {"1", "2", "3"} }
                        },
                        Quotas = new [] {
                            new QuotaData
                            {
                                Id = 1, Name="quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData { Id = 1, Values="q1=1", Counter=10, Limit=10 },//closed cells
                                    new CellData { Id = 2, Values="q1=2", Counter=6, Limit=10 },
                                }
                            }
                        }
                    }
                },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var survey = context.GetSurvey("S1");

            survey.AddSample(SchedulingMode.Simple,
                new InterviewData { Tag = "S1.I1", Data = "q1=1", ITS = CallOutcome.FreshSample },
                new InterviewData { Tag = "S1.I2", Data = "q1=1", ITS = CallOutcome.Appointment },
                new InterviewData { Tag = "S1.I3", Data = "q1=2", ITS = CallOutcome.FreshSample },
                new InterviewData { Tag = "S1.I4", Data = "q1=2", ITS = CallOutcome.Appointment });

            context.GetCalls("S1.I1", "S1.I2", "S1.I4").Assert.IsNull();
            context.GetCalls("S1.I3").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddSimpleSchedulingSampleWithDeletingByFCD_AddInterviewsWithIgnoredITSFromClossedCells_CallAreCreated()
        {
            var context = new TestData
            {
                Surveys = new[] {
                    new SurveyData
                    {
                        Tag="S1", IsUseDb = true, SchedulingScript = AllHoursSchedule.Name,
                        Forms = new[] {
                            new SingleFormData {Name="q1", Precodes = new []{"1", "2", "3"}}
                        },
                        Quotas = new [] {
                            new QuotaData
                            {
                                Id = 1, Name="quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData {Id = 1, Values="q1=1", Counter=10, Limit=10},//closed cells
                                    new CellData {Id = 2, Values="q1=2", Counter=6, Limit=10},
                                }
                            }
                        }
                    }
                },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var survey = context.GetSurvey("S1");

            new StateGroupController(null, survey.Model.StateGroupID, context).ChangeState((int)CallOutcome.FreshSample, x => x.FcdAction = true);

            survey.AddSample(SchedulingMode.Simple,
                new InterviewData { Tag = "S1.I1", Data = "q1=1", ITS = CallOutcome.FreshSample },
                new InterviewData { Tag = "S1.I2", Data = "q1=1", ITS = CallOutcome.Appointment },
                new InterviewData { Tag = "S1.I3", Data = "q1=2", ITS = CallOutcome.FreshSample },
                new InterviewData { Tag = "S1.I4", Data = "q1=2", ITS = CallOutcome.Appointment });

            context.GetCalls("S1.I2", "S1.I4").Assert.IsNull();
            context.GetCalls("S1.I1", "S1.I3").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddSimpleSchedulingSampleWithDisablingByFCD_AddInterviewsWithNotIgnoredITSFromClossedCells_CallAreFiltered()
        {
            ServiceLocator.Resolve<ISystemSettings>().FCD.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;

            var context = new TestData
            {
                Surveys = new[] {
                    new SurveyData
                    {
                        Tag="S1", IsUseDb = true, SchedulingScript = AllHoursSchedule.Name,
                        Forms = new[] {
                            new SingleFormData { Name="q1", Precodes = new []{"1", "2", "3"} }
                        },
                        Quotas = new [] {
                            new QuotaData
                            {
                                Id = 1, Name="quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData { Id = 1, Values="q1=1", Counter=10, Limit=10 }, //closed cells
                                    new CellData { Id = 2, Values="q1=2", Counter=6, Limit=10 },
                                }
                            }
                        }
                    }
                },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var survey = context.GetSurvey("S1");

            survey.AddSample(SchedulingMode.Simple,
                new InterviewData { Tag = "S1.I1", Data = "q1=1", ITS = CallOutcome.FreshSample },
                new InterviewData { Tag = "S1.I2", Data = "q1=1", ITS = CallOutcome.Appointment },
                new InterviewData { Tag = "S1.I3", Data = "q1=2", ITS = CallOutcome.FreshSample },
                new InterviewData { Tag = "S1.I4", Data = "q1=2", ITS = CallOutcome.Appointment });

            context.GetCalls("S1.I1").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD);
            context.GetCalls("S1.I2", "S1.I4").Assert.IsNull();
            context.GetCalls("S1.I3").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddSimpleSchedulingSampleWithDisablingByFCD_AddInterviewsWithIgnoredITSFromClossedCells_CallAreCreated()
        {
            ServiceLocator.Resolve<ISystemSettings>().FCD.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;

            var context = new TestData
            {
                Surveys = new[] {
                    new SurveyData
                    {
                        Tag="S1", IsUseDb = true, SchedulingScript = AllHoursSchedule.Name,
                        Forms = new[] {
                            new SingleFormData { Name="q1", Precodes = new []{"1", "2", "3"} }
                        },
                        Quotas = new [] {
                            new QuotaData
                            {
                                Id = 1, Name="quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData { Id = 1, Values="q1=1", Counter=10, Limit=10 }, //closed cells
                                    new CellData { Id = 2, Values="q1=2", Counter=6, Limit=10 },
                                }
                            }
                        }
                    }
                },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var survey = context.GetSurvey("S1");

            new StateGroupController(null, survey.Model.StateGroupID, context).ChangeState((int)CallOutcome.FreshSample, x => x.FcdAction = true);

            survey.AddSample(SchedulingMode.Simple,
                new InterviewData { Tag = "S1.I1", Data = "q1=1", ITS = CallOutcome.FreshSample },
                new InterviewData { Tag = "S1.I2", Data = "q1=1", ITS = CallOutcome.Appointment },
                new InterviewData { Tag = "S1.I3", Data = "q1=2", ITS = CallOutcome.FreshSample },
                new InterviewData { Tag = "S1.I4", Data = "q1=2", ITS = CallOutcome.Appointment });

            context.GetCalls("S1.I2", "S1.I4").Assert.IsNull();
            context.GetCalls("S1.I1", "S1.I3").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void BvFMWS_AddSampleWithSimpleSchedulingWithSomeInterviewInClosedCell_CallsShouldNotBeAddedForClosedCells()
        {
          _backendTools.LaunchAllHoursScript();
            var context = new TestData
            {
                Surveys = new[] {
                    new SurveyData
                    {
                        Tag="S1", IsUseDb = true, SchedulingScript = AllHoursSchedule.Name,
                        Forms = new[] {
                            new SingleFormData { Name="q1", Precodes = new []{"1", "2", "3"} },
                            new SingleFormData { Name="q2", Precodes = new []{"1", "2", "3"} }
                        },
                        Quotas = new [] {
                            new QuotaData
                            {
                                Id = 1, Name="quota1", Fields = new[] {"q1", "q2"},
                                Cells = new[]
                                {
                                    new CellData { Id = 1, Values="q1=1,q2=1", Counter=6, Limit=10 }, 
                                    new CellData { Id = 2, Values="q1=2,q2=1", Counter=6, Limit=10 },
                                    new CellData { Id = 3, Values="q1=1,q2=2", Counter=10, Limit=10 }, //closed cells
                                    new CellData { Id = 4, Values="q1=2,q2=2", Counter=6, Limit=10 },
                                    
                                }
                            }
                        }
                    }
                },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var survey = context.GetSurvey("S1");
            int surveySid = survey.Id;
   
            survey.AddSample(SchedulingMode.Simple,
                new InterviewData { Tag = "S1.I1", Data = "q1=2,q2=1", ITS = CallOutcome.FreshSample },
                new InterviewData { Tag = "S1.I2", Data = "q1=1,q2=2", ITS = CallOutcome.FreshSample },//in closed cell
                new InterviewData { Tag = "S1.I3", Data = "q1=2,q2=1", ITS = CallOutcome.FreshSample },
                new InterviewData { Tag = "S1.I4", Data = "q1=1,q2=2", ITS = CallOutcome.FreshSample }//in closed cell
                );

            var interview1 = InterviewRepository.GetById(surveySid, 1);
            var interview2 = InterviewRepository.GetById(surveySid, 2);
            var interview3 = InterviewRepository.GetById(surveySid, 3);
            var interview4 = InterviewRepository.GetById(surveySid, 4);

            Assert.AreEqual( (int)CallOutcome.FreshSample,interview1.TransientState);
            Assert.AreEqual( (int)CallOutcome.FilteredByCallDelivery,interview2.TransientState);
            Assert.AreEqual( (int)CallOutcome.FreshSample,interview3.TransientState);
            Assert.AreEqual( (int)CallOutcome.FilteredByCallDelivery,interview4.TransientState);

            Assert.IsNull(CallQueueService.GetCallAndNoLock(surveySid, 2),
                String.Format("Call for interview with id {0} should not be added during sample addition", 2));
            Assert.IsNull(CallQueueService.GetCallAndNoLock(surveySid, 4),
                String.Format("Call for interview with id {0} should not be added during sample addition", 4));
            Assert.AreEqual(2, CallQueueService.GetCallAndNoLock(surveySid, 1).CallState,
                String.Format("Call for interview with id {0} should be loaded with phase 2", 1));
            Assert.AreEqual(2, CallQueueService.GetCallAndNoLock(surveySid, 3).CallState,
                String.Format("Call for interview with id {0} should be loaded with phase 2", 3));

            BackendTools.AssertAggregateData(surveySid, 4, 2/*call count*/);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void BvFMWS_AddSampleSimpleWithAssignmentSchedulingWithSomeInterviewInClosedCell_CallsShouldNotBeAddedForClosedCells()
        {
            var context = new TestData
            {
                Surveys = new[] {
                    new SurveyData
                    {
                        Tag="S1", IsUseDb = true, SchedulingScript = AllHoursSchedule.Name,
                        Forms = new[] {
                            new SingleFormData { Name="q1", Precodes = new []{"1", "2", "3"} }
                        },
                        Quotas = new [] {
                            new QuotaData
                            {
                                Id = 1, Name="quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData { Id = 1, Values="q1=1", Counter=10, Limit=10 }, //closed cells
                                    new CellData { Id = 2, Values="q1=2", Counter=6, Limit=10 },
                                }
                            }
                        }
                    }
                },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var survey = context.GetSurvey("S1");
            int surveySid = survey.Id;
            
            survey.AddSample(SchedulingMode.Simple,
                new InterviewData { Tag = "S1.I1", Data = "q1=1", ITS = CallOutcome.FreshSample },//in closed cell
                new InterviewData { Tag = "S1.I2", Data = "q1=2", ITS = CallOutcome.FreshSample });//in open cell

            var interview1 = InterviewRepository.GetById(surveySid, 1);
            var interview2 = InterviewRepository.GetById(surveySid, 2);


            Assert.AreEqual((int)CallOutcome.FilteredByCallDelivery, interview1.TransientState); 
            Assert.AreEqual((int)CallOutcome.FreshSample, interview2.TransientState);

            Assert.IsNull(CallQueueService.GetCallAndNoLock(surveySid, 1),
                String.Format("Call for interview with id {0} should not be added during sample addition", 1));
            Assert.IsNotNull(CallQueueService.GetCallAndNoLock(surveySid, 2),
                String.Format("Call for interview with id {0} should be added during sample addition", 2));

            BackendTools.AssertAggregateData(surveySid, 2, 1/*call count*/);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void BvFMWS_AddSample_FCDLockIsHoldedDuringGetRespondentBatch()
        {
            const string projectId = "p000123";
            const int recordsCount = 4;
            bool isLockHolded = false;

            _backendTools.LaunchAllHoursScript();
            int surveySid = _backendTools.CreateSurvey(projectId);

            var createdRecords = ConfirmitTools.FillRespondentTable(_framework.DbEngine, BatchId, 1, recordsCount, Enumerable.Range(1, recordsCount));

            var respondentDataObtainer = _framework.RegistryStub<IRespondentBatchObtainer, StubIRespondentBatchObtainer>();
            respondentDataObtainer.GetRespondentBatchPartitionBvSurveyEntityInt32Int32Int32Boolean =
                (survey, batchId, startRangeOfInterviewId, partitionSize, isUpdateMode) =>
                {
                    isLockHolded =
                        _framework.DbEngine.ExecuteScalar<bool>(
                            "SELECT IsLockHeld FROM BvAppLocks WHERE ResourceName = @ResourceName",
                            CommandType.Text,
                            new SqlParameter("@ResourceName",
                                DatabaseLockTimeoutsAndRecourceNames.GetFcdResourceName(surveySid)));

                    return createdRecords.ToArray();
                };

            _backendTools.AddSample(projectId,
                BatchId,
                (int)SchedulingMode.Full);

            Assert.IsTrue(isLockHolded, "FCD Lock wasn't holded during AddSample operation");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void BvFMWS_CloseCellDuringAddSample_CallsShouldBeDeletedDuringCellClosing()
        {
            _backendTools.LaunchAllHoursScript();
            var testContext = new TestData
            {
                Surveys = new[] {
                    new SurveyData
                    {
                        Tag="S1", IsUseDb = true, SchedulingScript = AllHoursSchedule.Name,
                        Forms = new[] {
                            new SingleFormData { Name="q1", Precodes = new []{"1", "2", "3"} },
                            new SingleFormData { Name="q2", Precodes = new []{"1", "2", "3"} }
                        },
                        Quotas = new [] {
                            new QuotaData
                            {
                                Id = 1, Name="quota1", Fields = new[] {"q1", "q2"},
                                Cells = new[]
                                {
                                    new CellData { Id = 1, Values="q1=1,q2=1", Counter=6, Limit=10 }, 
                                    new CellData { Id = 2, Values="q1=1,q2=2", Counter=6, Limit=10 },
                                    new CellData { Id = 3, Values="q1=1,q2=3", Counter=6, Limit=10 },
                                    new CellData { Id = 4, Values="q1=2,q2=1", Counter=6, Limit=10 },
                                    new CellData { Id = 5, Values="q1=2,q2=2", Counter=6, Limit=10 },
                                    new CellData { Id = 6, Values="q1=2,q2=3", Counter=6, Limit=10 },
                                    new CellData { Id = 7, Values="q1=3,q2=1", Counter=6, Limit=10 },
                                    new CellData { Id = 8, Values="q1=3,q2=2", Counter=6, Limit=10 },
                                    new CellData { Id = 9, Values="q1=3,q2=3", Counter=6, Limit=10 },
                                }
                            }
                        }
                    }
                },
                Scripts = new[] { ScriptData.AllHours },
            }.Create();

            var survey = testContext.GetSurvey("S1");
            int surveySid = survey.Id;
            var quota = survey.GetQuota("quota1");
            
            const int cellId2 = 5;

            int callNumber = 0;

            ServiceLocator.Resolve<ISystemSettings>().AsyncOperation.AddSamplePortionSize = 1;

            var p = _framework.RegistryStub<ISampleBatchProcessor, StubISampleBatchProcessor>();
            p.ProcessSampleContextInt32 = (context, id) =>
            {
                if (callNumber++ == 1)
                    quota.CloseCellById(cellId2);

                var data = new SampleBatchProcessor(
                    ServiceLocator.Resolve<ISampleRecordProcessorFactory>(),
                    ServiceLocator.Resolve<IFCDSettings>(),
                    ServiceLocator.Resolve<IDatabaseLockTimeouts>());
                data.Process(context, id);
                p.RecordsGet = () => data.Records;
            };

            survey.AddSample(SchedulingMode.Full,
                new InterviewData { Tag = "S1.I1", Data = "q1=2,q2=2", ITS = CallOutcome.FreshSample },
                new InterviewData { Tag = "S1.I2", Data = "q1=1,q2=3", ITS = CallOutcome.FreshSample },
                new InterviewData { Tag = "S1.I3", Data = "q1=1,q2=3", ITS = CallOutcome.FreshSample },
                new InterviewData { Tag = "S1.I4", Data = "q1=2,q2=2", ITS = CallOutcome.FreshSample }
            );
            
            var interview1 = InterviewRepository.GetById(surveySid, 1);
            var interview2 = InterviewRepository.GetById(surveySid, 2);
            var interview3 = InterviewRepository.GetById(surveySid, 3);
            var interview4 = InterviewRepository.GetById(surveySid, 4);

            Assert.AreEqual((int)CallOutcome.FilteredByCallDelivery, interview1.TransientState );
            Assert.AreEqual((int)CallOutcome.FreshSample, interview2.TransientState );
            Assert.AreEqual((int)CallOutcome.FreshSample, interview3.TransientState );
            Assert.AreEqual((int)CallOutcome.FilteredByCallDelivery, interview4.TransientState );

            Assert.IsNull(CallQueueService.GetCallAndNoLock(surveySid, 1),
                String.Format("Call for interview with id {0} should not be added during sample addition", 1));
            Assert.IsNull(CallQueueService.GetCallAndNoLock(surveySid, 4),
                String.Format("Call for interview with id {0} should not be added during sample addition", 4));

            Assert.IsNotNull(CallQueueService.GetCallAndNoLock(surveySid, 2),
                String.Format("Call for interview {0} should exists", 3));
            Assert.IsNotNull(CallQueueService.GetCallAndNoLock(surveySid, 3),
                String.Format("Call for interview {0} should exists", 2));
        }

        private QuotaData GenerateQuotaData(int id, string[] names, int[] answersCount)
        {
            var quota = new QuotaData() { Id = id, Name = $"quota{id}", Fields = names};
            var cellsCount = answersCount.Aggregate((x, y) => x * y);
            quota.Cells = new CellData[cellsCount];
            var cellIndex = 0;
            for (int i0 = 1; i0 <=answersCount[0]; i0++)
            {
                for (int i1 = 1; i1 <=answersCount[1]; i1++)
                {
                    for (int i2 = 1; i2 <=answersCount[2]; i2++)
                    {
                        for (int i3 = 1; i3 <=answersCount[3]; i3++)
                        {
                            for (int i4 = 1; i4 <=answersCount[4]; i4++)
                            {
                                quota.Cells[cellIndex] = new CellData() {
                                    Id = cellIndex + 1,
                                    Counter = 0, 
                                    Limit = 10,
                                    Values = $"{names[0]}={i0},{names[1]}={i1},{names[2]}={i2},{names[3]}={i3},{names[4]}={i4}"
                                };
                                cellIndex++;
                            }
                        }
                    }
                }
            }

            return quota;
        }
        
        private List<SingleFormData> GenerateFormsData(string[] names, int[] answersCount)
        {
            var formData = new List<SingleFormData>();
            for (int i = 0; i < names.Length; i++)
            {
                var precodes = new List<string>();
                for (int j = 1; j <= answersCount[i]; j++)
                {
                    precodes.Add(j.ToString());
                }

                formData.Add(new SingleFormData()
                {
                    Name =  names[i],
                    Precodes = precodes.ToArray()
                });
            }

            return formData;
        }
        
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void BvFMWS_ManyQuotasHasManyLongColumns_AllDynamicQueryShouldBePerformedCorrectly()
        {
            var columnsForQuota1 = new[]
            {
                "A1234567890123456789012345678901A",
                "A1234567890123456789012345678901B",
                "A1234567890123456789012345678901C",
                "A1234567890123456789012345678901D",
                "A1234567890123456789012345678901E"
            };
            var columnsForQuota2 = new[]
            {
                "A234567890123456789012345678901F",
                "A234567890123456789012345678901G",
                "A234567890123456789012345678901H",
                "A234567890123456789012345678901J",
                "A234567890123456789012345678901K"
            };
            var columnsForQuota3 = new[]
            {
                "A234567890123456789012345678901L",
                "A234567890123456789012345678901M",
                "A234567890123456789012345678901N",
                "A234567890123456789012345678901O",
                "A234567890123456789012345678901P"
            };
            var columnsForQuota4 = new[]
            {
                "A234567890123456789012345678901Q",
                "A234567890123456789012345678901R",
                "A234567890123456789012345678901S",
                "A234567890123456789012345678901T",
                "A234567890123456789012345678901U"
            };

            var answerCountsForQuota1 = new[] { 2, 2, 2, 3, 2 };
            var answerCountsForQuota2 = new[] { 2, 3, 4, 2, 2 };
            var answerCountsForQuota3 = new[] { 2, 2, 2, 2, 2 };
            var answerCountsForQuota4 = new[] { 2, 2, 3, 3, 2 };

            _backendTools.LaunchAllHoursScript();
            var forms = GenerateFormsData(columnsForQuota1, answerCountsForQuota1);
            forms.AddRange(GenerateFormsData(columnsForQuota2, answerCountsForQuota2));
            forms.AddRange(GenerateFormsData(columnsForQuota3, answerCountsForQuota3));
            forms.AddRange(GenerateFormsData(columnsForQuota4, answerCountsForQuota4));

            var quota1Data = GenerateQuotaData(1, columnsForQuota1, answerCountsForQuota1);
            var quota2Data = GenerateQuotaData(2,columnsForQuota2, answerCountsForQuota2);
            var quota3Data = GenerateQuotaData(3,columnsForQuota3, answerCountsForQuota3);
            var quota4Data = GenerateQuotaData(4,columnsForQuota4, answerCountsForQuota4);
            
            var testContext = new TestData
                {
                    Surveys = new[] {
                        new SurveyData
                        {
                            Tag="S1", IsUseDb = true, SchedulingScript = AllHoursSchedule.Name,
                            Forms = forms.ToArray(),
                            Quotas = new [] {
                                quota1Data,
                                quota2Data,
                                quota3Data,
                                quota4Data,
                            }
                        }
                    },
                    Scripts = new[] { ScriptData.AllHours },
                }.Create();

            var survey = testContext.GetSurvey("S1");
            int surveySid = survey.Id;
            var quota1 = survey.GetQuota("quota1");
            var quota2 = survey.GetQuota("quota2");
            var quota3 = survey.GetQuota("quota3");
            var quota4 = survey.GetQuota("quota4");

            const int openCellIdForQuota1 = 2;
            const int closeCellIdForQuota1 = 4;
            const int openCellIdForQuota2 = 3;
            const int closeCellIdForQuota2 = 5;
            const int openCellIdForQuota3 = 1;
            const int closeCellIdForQuota3 = 6;

            quota1.CloseCellById(closeCellIdForQuota1);
            quota2.CloseCellById(closeCellIdForQuota2);
            quota3.CloseCellById(closeCellIdForQuota3);

            var interviewer1Answers = quota1Data.Cells[openCellIdForQuota1 - 1].Values + "," + quota2Data.Cells[openCellIdForQuota2 - 1].Values + "," + quota3Data.Cells[closeCellIdForQuota3 - 1].Values;
            var interviewer2Answers = quota1Data.Cells[closeCellIdForQuota1 - 1].Values + "," + quota2Data.Cells[openCellIdForQuota2 - 1].Values + "," + quota3Data.Cells[openCellIdForQuota3 - 1].Values;
            var interviewer3Answers = quota2Data.Cells[openCellIdForQuota2 - 1].Values + "," + quota3Data.Cells[openCellIdForQuota3 - 1].Values;

            survey.AddSample(SchedulingMode.Full,
                new InterviewData { Tag = "S1.I1", Data = interviewer1Answers, ITS = CallOutcome.FreshSample },
                new InterviewData { Tag = "S1.I2", Data = interviewer2Answers, ITS = CallOutcome.FreshSample },
                new InterviewData { Tag = "S1.I3", Data = interviewer3Answers, ITS = CallOutcome.FreshSample },
                new InterviewData { Tag = "S1.I4", ITS = CallOutcome.FreshSample }
            );

            Assert.IsNull(CallQueueService.GetCallAndNoLock(surveySid, 1),
                String.Format("Call for Interview with id {0} should not be added", 1));
            Assert.IsNull(CallQueueService.GetCallAndNoLock(surveySid, 2),
                String.Format("Call for Interview with id {0} should not be added", 2));

            Assert.IsNotNull(CallQueueService.GetCallAndNoLock(surveySid, 3),
                String.Format("Call for Interview with id {0} should be added", 3));
            Assert.IsNotNull(CallQueueService.GetCallAndNoLock(surveySid, 4),
                String.Format("Call for Interview with id {0} should be added", 4));
        }
    }
}
