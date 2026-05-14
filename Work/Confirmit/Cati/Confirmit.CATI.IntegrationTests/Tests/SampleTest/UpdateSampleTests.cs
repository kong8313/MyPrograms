using System;
using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.Tests.SampleTest
{
    [TestClass]
    public class UpdateSampleTests : BaseMockedIntegrationTest
    {
        [TestMethod]
        public void UpdateSample_CallExists_CallShouldBeDeleted()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() { Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Interviews = new [] {
                            new InterviewData(){Tag="S1.I1", ITS = CallOutcome.FreshSample, Call = new CallData(){CallState = (int)CallState.Scheduled}}, 
                        }
                    }
                },
                Scripts = new [] {
                    new ScriptData(){Tag="SS1", Script = new TestScript(
                        new Rule(new SubRule(new Action(Action.Operation.SetNewITS, "1")),
                            true), 
                        new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var interview = context.GetInterview("S1.I1");
            survey.ProcessSample(SchedulingMode.Full, SampleMode.Update, interview.Data);

            interview.Assert.IsTrue(x => x.TransientState == 1);
            context.GetCall("S1.I1").Assert.IsNull();
        }

        [TestMethod]
        public void UpdateSample_CallNotExists_CallShouldBeCreated()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() { Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Interviews = new [] {
                            new InterviewData(){Tag="S1.I1", ITS = CallOutcome.FreshSample}, 
                        }
                    }
                },
                Scripts = new[] {
                    new ScriptData(){Tag="SS1", Script = new TestScript(
                        new Rule(new SubRule(new []{ 
                            new Action(Action.Operation.SetNewITS, "1"),
                            new Action(Action.Operation.SetNewCallPriority, "10")
                        }),
                            true), 
                        new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var interview = context.GetInterview("S1.I1");
            survey.ProcessSample(SchedulingMode.Full, SampleMode.Update, interview.Data);

            interview.Assert.IsTrue(x => x.TransientState == 1);
            context.GetCall("S1.I1").Assert.IsTrue(x => x.Priority == 10);
        }

        [TestMethod]
        public void UpdateSample_CallExists_CallShouldBeUpdated()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() { Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Interviews = new [] {
                            new InterviewData(){Tag="S1.I1", ITS = CallOutcome.FreshSample, Call = new CallData(){CallState = (int)CallState.Scheduled}}, 
                        }
                    }
                },
                Scripts = new[] {
                    new ScriptData(){Tag="SS1", Script = new TestScript(
                        new Rule(new SubRule(new []{ 
                            new Action(Action.Operation.SetNewITS, "1"),
                            new Action(Action.Operation.SetNewCallPriority, "10")
                        }),
                            true), 
                        new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var interview = context.GetInterview("S1.I1");
            survey.ProcessSample(SchedulingMode.Full, SampleMode.Update, interview.Data);

            interview.Assert.IsTrue(x => x.TransientState == 1);
            context.GetCall("S1.I1").Assert.IsTrue(x => x.Priority == 10);
        }

        [TestMethod]
        public void UpdateSample_CallExistsAndInterviewIsMovedToClosedCell_CallShouldBeDeletedByFCD()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() { Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"}, 
                                Cells = new[] 
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=0},
                                }
                            }
                        },
                        Interviews = new [] {
                            new InterviewData(){Tag="S1.I1", Data="q1=1", ITS = CallOutcome.FreshSample, Call = new CallData(){CallState = (int)CallState.Scheduled}}, 
                        }
                    }
                },
                Scripts = new[] {
                    new ScriptData(){Tag="SS1", Script = new TestScript(
                        new Rule(new SubRule(new []{ 
                            new Action(Action.Operation.SetNewITS, "1"),
                            new Action(Action.Operation.SetNewCallPriority, "10")
                        }),
                            true), 
                        new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var interview = context.GetInterview("S1.I1");
            
            interview.Data.Data = "q1=2";
            survey.ProcessSample(SchedulingMode.Full, SampleMode.Update, interview.Data);

            interview.Assert.IsTrue(x => x.TransientState == (int)CallOutcome.FilteredByCallDelivery);
            context.GetCall("S1.I1").Assert.IsNull();
        }

        [TestMethod]
        public void UpdateSample_CallExistsAndInterviewIsMovedToClosedCell_CallShouldNOTBeDisabledByFCDBecauseITSIsInTheIgnoredByFCDList()
        {
            ServiceLocator.Resolve<IFCDSettings>().AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() { Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"}, 
                                Cells = new[] 
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=0},
                                }
                            }
                        },
                        Interviews = new [] {
                            new InterviewData(){Tag="S1.I1", Data="q1=1", ITS = CallOutcome.FreshSample, Call = new CallData(){CallState = (int)CallState.Scheduled}}, 
                        }
                    }
                },
                Scripts = new[] {
                    new ScriptData(){Tag="SS1", Script = new TestScript(
                        new Rule(new SubRule(new []{ 
                            new Action(Action.Operation.SetNewITS, "1"),
                            new Action(Action.Operation.SetNewCallPriority, "10")
                        }),
                            true), 
                        new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                    }
                },
            }.Create();

            var survey = context.GetSurvey("S1");
            var interview = context.GetInterview("S1.I1");

            interview.Data.Data = "q1=2";
            survey.ProcessSample(SchedulingMode.Full, SampleMode.Update, interview.Data);

            interview.Assert.IsTrue(x => x.TransientState == 1);
            context.GetCall("S1.I1").Assert.IsTrue(x => x.Priority == 10 && x.CallState == (int)CallState.Scheduled);
        }

        [TestMethod]
        public void UpdateSample_CallExistsAndInterviewIsMovedToClosedCell_CallShouldBeDisabledByFCD()
        {
            ServiceLocator.Resolve<IFCDSettings>().AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() { Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"}, 
                                Cells = new[] 
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=0},
                                }
                            }
                        },
                        Interviews = new [] {
                            new InterviewData(){Tag="S1.I1", Data="q1=1", ITS = CallOutcome.FreshSample, Call = new CallData(){CallState = (int)CallState.Scheduled}}, 
                        }
                    }
                },
                Scripts = new[] {
                    new ScriptData(){Tag="SS1", Script = new TestScript(
                        new Rule(new SubRule(new []{ 
                            new Action(Action.Operation.SetNewITS, "2"),
                            new Action(Action.Operation.SetNewCallPriority, "10")
                        }),
                            true), 
                        new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                    }
                },
            }.Create();

            var survey = context.GetSurvey("S1");
            var interview = context.GetInterview("S1.I1");

            interview.Data.Data = "q1=2";
            survey.ProcessSample(SchedulingMode.Full, SampleMode.Update, interview.Data);

            interview.Assert.IsTrue(x => x.TransientState == 2);
            context.GetCall("S1.I1").Assert.IsTrue(x => x.Priority == 10 && x.CallState == (int)CallState.DisabledByFCD);
        }

        [TestMethod]
        public void UpdateSample_CallExistsAndDisabledByFCDandUser_CallShouldBeDisabledByFCD()
        {
            ServiceLocator.Resolve<IFCDSettings>().AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() { Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"}, 
                                Cells = new[] 
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=0},
                                }
                            }
                        },
                        Interviews = new [] {
                            new InterviewData(){Tag="S1.I1", Data="q1=1", ITS = CallOutcome.FreshSample, Call = new CallData(){CallState = (int)CallState.DisabledByUser}}, 
                        }
                    }
                },
                Scripts = new[] {
                    new ScriptData(){Tag="SS1", Script = new TestScript(
                        new Rule(new SubRule(new []{ 
                            new Action(Action.Operation.SetNewITS, "2"),
                            new Action(Action.Operation.SetNewCallPriority, "10")
                        }),
                            true), 
                        new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                    }
                },
            }.Create();

            var survey = context.GetSurvey("S1");
            var interview = context.GetInterview("S1.I1");

            interview.Data.Data = "q1=2";
            survey.ProcessSample(SchedulingMode.Full, SampleMode.Update, interview.Data);

            interview.Assert.IsTrue(x => x.TransientState == 2);
            context.GetCall("S1.I1").Assert.IsTrue(x => x.Priority == 10 && x.CallState == (int)CallState.DisabledByFCD);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void UpdateSample_InterviewInBlackList_CallIsNotCreated()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() { Tag = "S1", IsUseDb = true, SchedulingScript = "SS1", IsSupportBlackList = true,
                        Interviews = new [] {
                            new InterviewData(){Tag="S1.I1", TelephoneNumber = "88001001010", ITS = CallOutcome.FreshSample}, 
                        }
                    }
                },
                Scripts = new [] {
                    new ScriptData(){Tag="SS1", Script = new TestScript(
                        new Rule(new SubRule(new []{ 
                            new Action(Action.Operation.SetNewITS, "1"),
                            new Action(Action.Operation.SetNewCallPriority, "10")
                        }),
                            true), 
                        new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                    }
                },
                TelephoneBlacklist = new[] { "88001001010" }
            }.Create();

            var survey = context.GetSurvey("S1");
            var interview = context.GetInterview("S1.I1");

            survey.ProcessSample(SchedulingMode.Full, SampleMode.Update, interview.Data);

            interview.Assert.IsTrue(x => x.TransientState == (int)CallOutcome.Blacklist);
            context.GetCall("S1.I1").Assert.IsNull();
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void UpdateSample_InterviewInBlackList_InterviewShouldBeUpdated()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() { Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Interviews = new [] {
                            new InterviewData(){Tag="S1.I1", TelephoneNumber = "88001001010", ITS = CallOutcome.FreshSample}, 
                        }
                    }
                },
                Scripts = new[] {
                    new ScriptData(){Tag="SS1", Script = new TestScript(
                        new Rule(new SubRule(new []{ 
                            new Action(Action.Operation.SetNewITS, "1")
                        }),
                            true), 
                        new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                    }
                },
                TelephoneBlacklist = new[] { "88001001010" }
            }.Create();

            var survey = context.GetSurvey("S1");
            var interview = context.GetInterview("S1.I1");

            survey.ProcessSample(SchedulingMode.Full, SampleMode.Update, interview.Data);

            interview.Assert.IsTrue(x => x.TransientState == (int)CallOutcome.Appointment);
            context.GetCall("S1.I1").Assert.IsNull();
        }

        [TestMethod]
        public void UpdateSample_CallNotExistsAndInterviewIsMovedToClosedCellWithoutCreateNewCall_InterviewItsShouldBeChangedToFilteredByFcd()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() { Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"}, 
                                Cells = new[] 
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=0},
                                }
                            }
                        },
                        Interviews = new [] {
                            new InterviewData(){Tag="S1.I1", Data="q1=1", ITS = CallOutcome.FreshSample}, 
                        }
                    }
                },
                Scripts = new[] {
                    new ScriptData(){Tag="SS1", Script = new TestScript(
                        new Rule(new SubRule(new []{ 
                            new Action(Action.Operation.SetNewITS, "1")
                        }),
                            true), 
                        new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var interview = context.GetInterview("S1.I1");

            interview.Data.Data = "q1=2";
            survey.ProcessSample(SchedulingMode.Full, SampleMode.Update, interview.Data);

            interview.Assert.IsTrue(x => x.TransientState == (int)CallOutcome.FilteredByCallDelivery);
            context.GetCall("S1.I1").Assert.IsNull();
        }

        [TestMethod]
        public void UpdateSample_TwoCallExistsAndWeUpdateOnlyOne_OneCallShouldBeDeletedAndOneShouldNotBeDeleted()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() { Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Interviews = new [] {
                            new InterviewData(){Tag="S1.I1", ITS = CallOutcome.FreshSample, Call = new CallData(){CallState = (int)CallState.Scheduled}}, 
                            new InterviewData(){Tag="S1.I2", ITS = CallOutcome.FreshSample, Call = new CallData(){CallState = (int)CallState.Scheduled}}, 
                        }
                    }
                },
                Scripts = new[] {
                    new ScriptData(){Tag="SS1", Script = new TestScript(
                        new Rule(new SubRule(new []{ 
                            new Action(Action.Operation.SetNewITS, "1"),
                            new Action(Action.Operation.SetNewCallPriority, "10")
                        }),
                            true), 
                        new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var interview = context.GetInterview("S1.I1");

            survey.ProcessSample(SchedulingMode.Full, SampleMode.Update, interview.Data);

            interview.Assert.IsTrue(x => x.TransientState == 1);
            context.GetCall("S1.I1").Assert.IsTrue(x => x.Priority == 10);

            context.GetInterview("S1.I2").Assert.IsTrue(x => x.TransientState == 16);
            context.GetCall("S1.I2").Assert.IsTrue(x => x.Priority == 1);
        }

        [TestMethod]
        public void UpdateSample_CallExistsAndActive_CallShouldNotBeDeleted()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() { Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Interviews = new [] {
                            new InterviewData(){Tag="S1.I1", ITS = CallOutcome.FreshSample, Call = new CallData(){CallState = (int)CallState.InterviewInProgress}}, 
                        }
                    }
                },
                Scripts = new[] {
                    new ScriptData(){Tag="SS1", Script = new TestScript(
                        new Rule(new SubRule(new Action(Action.Operation.SetNewITS, "1")),
                            true), 
                        new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var interview = context.GetInterview("S1.I1");
            survey.ProcessSample(SchedulingMode.Full, SampleMode.Update, interview.Data);

            interview.Assert.IsTrue(x => x.TransientState == 16);
            context.GetCall("S1.I1").Assert.IsTrue(x => x.CallState == (int)CallState.InterviewInProgress);
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public void UpdateSample_SimpleScheduling_DoesNotDoAnythingAndFinishedSuccess()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() { Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Interviews = new [] {
                            new InterviewData(){Tag="S1.I1", ITS = CallOutcome.FreshSample}, 
                        }
                    }
                },
                Scripts = new[] {
                    new ScriptData(){Tag="SS1", Script = new TestScript(
                        new Rule(new SubRule(new []{ 
                            new Action(Action.Operation.SetNewITS, "1"),
                            new Action(Action.Operation.SetNewCallPriority, "10")
                        }),
                            true), 
                        new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var interview = context.GetInterview("S1.I1");
            survey.ProcessSample(SchedulingMode.Simple, SampleMode.Update, interview.Data);

            interview.Assert.IsTrue(x => x.TransientState == 16);
            context.GetCall("S1.I1").Assert.IsNull();
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public void UpdateSample_FullSchedulingButNoSampleUpdateRule_DoesNotDoAnythingAndFinishedSuccess()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() { Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Interviews = new [] {
                            new InterviewData(){Tag="S1.I1", ITS = CallOutcome.FreshSample}, 
                        }
                    }
                },
                Scripts = new[] {
                    new ScriptData(){Tag="SS1", Script = new TestScript(
                        new Rule(new SubRule(new []{ 
                            new Action(Action.Operation.SetNewITS, "1"),
                            new Action(Action.Operation.SetNewCallPriority, "10")
                        })), 
                        new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var interview = context.GetInterview("S1.I1");
            survey.ProcessSample(SchedulingMode.Full, SampleMode.Update, interview.Data);

            interview.Assert.IsTrue(x => x.TransientState == 16);
            context.GetCall("S1.I1").Assert.IsNull();
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public void MergeSample_FullScheduling_Success()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() { Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Interviews = new [] {
                            new InterviewData(){Tag="S1.I1", ITS = CallOutcome.FreshSample}, 
                            new InterviewData(){Tag="S1.I2", ITS = CallOutcome.FreshSample}, 
                            new InterviewData(){Tag="S1.I3", ITS = CallOutcome.FreshSample}, 
                            }
                        }
                },
                Scripts = new[] {
                    new ScriptData(){Tag="SS1", Script = new TestScript(new Rule[]
                        {
                            new Rule(new SubRule(new []{ 
                                new Action(Action.Operation.SetNewITS, "1"),
                                new Action(Action.Operation.SetNewCallPriority, "10")
                            }),
                                true), 
                            new Rule(new SubRule(new []{ 
                                new Action(Action.Operation.SetNewITS, "2"),
                                new Action(Action.Operation.SetNewCallPriority, "20")
                            }))
                        }, 
                        new Shift(1, 1, "0.00:00:00", "1.00:00:00"))}
                }
            }.Create();

            var survey = context.GetSurvey("S1");

            /* same bath id for add and update sample additions emulates how Confirmit call's CATI in merge mode*/
            int batchId = 123; 
            BvSamplesAdapter.Insert(new BvSamplesEntity{ BatchID = batchId, SurveySID = survey.Id, SampleType = (int)SampleMode.Add, State = 2, FinishedTime = DateTime.Now, StartedTime = DateTime.Now});
            survey.ProcessSample(SchedulingMode.Full, SampleMode.Update, batchId, new InterviewData { Tag = "S1.I2", ITS = CallOutcome.FreshSample });

            context.GetCall("S1.I2").Assert.IsTrue(x => x.Priority == 10);
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public void ProcessSample_UpdateMode_EnableDisabledByFCDCallsDuringUpdate_CallsEnabled()
        {
            var timeToExpire = DateTime.UtcNow.AddYears(2).CutMilliseconds();

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", SchedulingScript = "SS1", IsUseDb = true,
                        Interviews = new[] 
                        {
                            new InterviewData() {Tag = "S1.I1", Call = new CallData(){ CallState = (int)CallState.DisabledByFCD, Priority = 999, TimeToExpire = timeToExpire}}
                        }
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new Rule(new SubRule(
                                new []{
                                    new Action(Action.Operation.RestorePreviousCallState),
                                    new Action(Action.Operation.EnableCall)}), true),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var interview = context.GetInterview("S1.I1");
            survey.ProcessSample(SchedulingMode.Full, SampleMode.Update, interview.Data);

            context.GetCall("S1.I1").Assert.IsTrue(entity => entity.Priority == 999);
            context.GetCall("S1.I1").Assert.IsTrue(entity => entity.CallState == (int)CallState.Scheduled);
            context.GetCall("S1.I1").Assert.IsTrue(entity => entity.TimeToExpire == timeToExpire);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ProcessSample_UpdateMode_EnableDisabledByUserCallsDuringUpdate_CallsEnabled()
        {
            var timeToExpire = DateTime.UtcNow.AddYears(2).CutMilliseconds();

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", SchedulingScript = "SS1", IsUseDb = true,
                        Interviews = new[] 
                        {
                            new InterviewData() {Tag = "S1.I1", Call = new CallData(){ CallState = (int)CallState.DisabledByUser, Priority = 999, TimeToExpire = timeToExpire}}
                        }
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new Rule(new SubRule(
                                new []{
                                    new Action(Action.Operation.RestorePreviousCallState),
                                    new Action(Action.Operation.EnableCall)}), true),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var interview = context.GetInterview("S1.I1");
            survey.ProcessSample(SchedulingMode.Full, SampleMode.Update, interview.Data);

            context.GetCall("S1.I1").Assert.IsTrue(entity => entity.Priority == 999);
            context.GetCall("S1.I1").Assert.IsTrue(entity => entity.CallState == (int)CallState.Scheduled);
            context.GetCall("S1.I1").Assert.IsTrue(entity => entity.TimeToExpire == timeToExpire);
        }
    }
}
