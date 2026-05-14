using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework.Controllers;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Dialer;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using DialType = Confirmit.CATI.Common.DialType;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Dialer
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait2)]
    public class HybridTest: BaseMockedIntegrationTest
    {
        const string ProjectId = "p999999";
        const int ItsToSetPreviewDialingMode = 100;
        const int ItsToSetSpecialDialDialingMode = 75;
        const int ItsToResetDialingMode = 50;
        
        private readonly int _surveyId;
        private readonly TestScript _script;
        
        public HybridTest()
        {
            var surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();

            _surveyId = BackendToolsObject.CreateSurvey(ProjectId);
            surveyStateService.Open(_surveyId);
            BackendToolsObject.LaunchAllHoursScript();
            SurveyService.SetDialingMode(_surveyId, DialingMode.Predictive);

            _script = new TestScript(
                new[]
                {
                    new SubRule
                    {
                        Actions = new List<Action>
                        {
                            new Action(Action.Operation.SetDialingMode, ((int)DialingMode.SpecialDial).ToString(CultureInfo.InvariantCulture)),
                            new Action(Action.Operation.SetNewCallPriority, "10" /*doesn't matter*/),
                        },
                        ItsId = ItsToSetSpecialDialDialingMode,
                        ShiftTypeId = 0,
                        Phase = 2,
                        Filter = null,
                        FilterEnabled = false,
                    },
                    new SubRule
                    {
                        Actions = new List<Action>
                        {
                            new Action(Action.Operation.SetDialingMode, ((int)DialingMode.Preview).ToString(CultureInfo.InvariantCulture)),
                            new Action(Action.Operation.SetNewCallPriority, "10" /*doesn't matter*/),
                        },
                        ItsId = ItsToSetPreviewDialingMode,
                        ShiftTypeId = 0,
                        Phase = 2,
                        Filter = null,
                        FilterEnabled = false,
                    },

                    new SubRule
                    {
                        Actions = new List<Action>
                        {
                            new Action(Action.Operation.SetDialingMode, "0" /*reset mode*/),
                            new Action(Action.Operation.SetNewCallPriority, "10" /*doesn't matter*/),
                        },
                        ItsId = ItsToResetDialingMode,
                        ShiftTypeId = 0,
                        Phase = 2,
                        Filter = null,
                        FilterEnabled = false
                    }
                },
                new Shift(1, 1, "0.00:00:00", "0.23:59:00"),
                new Shift(2, 1, "1.00:00:00", "1.23:59:00"),
                new Shift(3, 1, "2.00:00:00", "2.23:59:00"),
                new Shift(4, 1, "3.00:00:00", "3.23:59:00"),
                new Shift(5, 1, "4.00:00:00", "4.23:59:00"),
                new Shift(6, 1, "5.00:00:00", "5.23:59:00"),
                new Shift(7, 1, "6.00:00:00", "6.23:59:00"));

            BackendToolsObject.LaunchScript(_surveyId, _script);
        }
        
        //
        // 1. Create survey with specific script for possibility to change call dialing mode
        // 2. Create 3 calls. Change dialing mode via script of second and first calls. Reset dialing mode of third
        //      call over scheduling script.
        // 3. Check dialing mode of calls with WS.
        //
        [Fact, Owner(@"FIRM\AlexanderL")]
        public void Hybrid_SetDialingModeIsCorrect_ActionIsCorrect()
        {
            var test = new TestCati2(true, BackendToolsObject)
            {
                SurveySID = _surveyId
            };

            BvInterviewEntity[] interviews = test.CreateInterviewsWithCalls(4);

            CallTools.MoveAndRescheduleCalls(_surveyId, interviews.Skip(1).Select(x => x.ID).ToArray(), ItsToSetPreviewDialingMode);
            
            CallTools.MoveAndRescheduleCalls(_surveyId, interviews.Skip(2).Select(x => x.ID).ToArray(), ItsToSetPreviewDialingMode);
            CallTools.MoveAndRescheduleCalls(_surveyId, interviews.Skip(2).Select(x => x.ID).ToArray(), ItsToResetDialingMode);
            
            CallTools.MoveAndRescheduleCalls(_surveyId, interviews.Skip(3).Select(x => x.ID).ToArray(), ItsToSetSpecialDialDialingMode);

            TestAssert.AreEqual(
                new [] { DialingMode.Predictive, DialingMode.Preview, DialingMode.Predictive, DialingMode.SpecialDial },
                interviews.Select(x => (DialingMode)new ManagementService().GetDialingMode(ProjectId, x.ID)));
        }

        [Fact, Owner(@"FIRM\AlexanderL")]
        public void Hybrid_AddSample_InterviewIsCorrect()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData()
                    {
                        Tag="S1", DialMode = DialingMode.Predictive, IsUseDb = true,
                        Forms = new []{new FormData(){Name="DialMode", TableName = "respondent", IsReplicated = false}}
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");

            var interviews = new[]
            {
                new InterviewData() {Tag = "S1.I1", Data = "DialMode="},
                new InterviewData() {Tag = "S1.I2", Data = "DialMode=0"},
                new InterviewData() {Tag = "S1.I3", Data = "DialMode=1"},
                new InterviewData() {Tag = "S1.I4", Data = "DialMode=2"},
                new InterviewData() {Tag = "S1.I5", Data = "DialMode=3"},
                new InterviewData() {Tag = "S1.I6", Data = "DialMode=4"},
                new InterviewData() {Tag = "S1.I7", Data = "DialMode=5"}

            };
            survey.AddSample(SchedulingMode.Simple, interviews);

            context.GetInterviews("S1.I1", "S1.I2", "S1.I3", "S1.I5", "S1.I6").Assert.IsTrue(x => x.DialingMode == (int)0);
            context.GetInterview("S1.I4").Assert.IsTrue(x => x.DialingMode == (int)DialingMode.Preview);
            context.GetInterview("S1.I7").Assert.IsTrue(x => x.DialingMode == (int)DialingMode.SpecialDial);

        }

        //
        // 1. Create survey with manual dialing mode
        // 2. reschedule call with set dialing mode to preview
        // 3. action would not take affect( dialing mode wasn't changed )
        //
        [Fact, Owner(@"FIRM\AlexanderL")]
        public void Hybrid_SetDialingModeForManualAndPreviewSurvey_ExceptionDoesntThrow()
        {
            SurveyService.SetDialingMode(_surveyId, DialingMode.Manual);

            var interview = BackendTools.NewInterview(_surveyId);
            BackendTools.CreateInterview(interview);
            var call = BackendTools.NewCall(interview);
            BackendTools.CreateCall(call);

            CallTools.MoveAndRescheduleCalls(_surveyId, new[] { interview.ID }, ItsToSetPreviewDialingMode);

            Assert.AreEqual(0, InterviewRepository.GetById(_surveyId, interview.ID).DialingMode, 
                "Interview dialing mode was updatet for survey in manual mode");

            SurveyService.SetDialingMode(_surveyId, DialingMode.Preview);

            CallTools.MoveAndRescheduleCalls(_surveyId, new[] { interview.ID }, ItsToSetPreviewDialingMode);

            Assert.AreEqual(0, InterviewRepository.GetById(_surveyId, interview.ID).DialingMode,
                "Interview dialing mode was updatet for survey in preview mode");
        }

        [Fact]
        public void PredictiveHybrid_ReceiveNotificationAboutLoggedAgentState_DialingModeIsCorrect()
        {
            var context = new TestData(){
                Surveys = new[] {new SurveyData() { Tag="S1", DialMode = DialingMode.Predictive, AssignsS = "P1", 
                    Interviews = new []{new InterviewData(){Tag="S1.I1", DialMode = "2", Call = new CallData()}}
                }},
                Persons = new[] {new PersonData() { Tag="P1", TaskChoice = TaskChoiceMode.SurveyAssignment} },
                Dialers = new[] {new DialerData() { Tag="D1"} }
            }.Create();

            var predicitve = context.GetDialer("D1").Predictive("S1");
            
            var console = context.GetPerson("P1").Console.Login("S1").LoginToDialer().Start()
                    .Do(x => predicitve.Request().Connect("S1.I1", x)).Wait();

            Assert.AreEqual("S1.I1", console.Interview?.Tag, "Wrong interview");
            Assert.AreEqual(DialingMode.Preview, BvTasksAdapter.GetAll().Single().DialingMode, "Wrong dialing mode in BvTask");

            DialerMethodBehaviors.SendAgentState(predicitve.Dialer.Behavior, console, AgentStateMsgs.LOGGEDIN);

            Assert.AreEqual("S1.I1", console.Interview?.Tag, "Wrong interview");
            Assert.AreEqual(DialingMode.Preview, BvTasksAdapter.GetAll().Single().DialingMode, "Wrong dialing mode in BvTask");
        }

        // 1. create survey with predictive dialing mode
        // 2. first call is delivered with predictive dialing mode
        //    second call is delivered with predictive dialing mode. During interviewing hungup command is called
        //    third call is delivered with predictive dialing mode
        // 3. Interviewing is correct
        [Theory, Owner(@"FIRM\AlexanderL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void PredictiveHybrid_ProcessingThreePredictiveCall_InterviewingCorrect(DialType dialType)
        {
            PredictiveHybrid_TestBase(new[]{
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    Mode = DialingMode.Predictive
                },
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = true,
                    Mode = DialingMode.Predictive
                },
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    Mode = DialingMode.Predictive
                }
            }, dialType);
        }

        // 1. create survey with predictive dialing mode
        // 2. first call is delivered with predictive dialing mode
        //    second call is delivered with preview dialing mode. During interviewing Dial and hungup commands are called
        //    third call is delivered with predictive dialing mode
        // 3. Interviewing is correct
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void PredictiveHybridWithPreview_ProcessingPreviewCallWithSuccessDialing_InterviewingCorrect(DialType dialType)
        {
            var context = PredictiveHybrid_TestBase(new[]{
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    Mode = DialingMode.Predictive
                },
                new InterviewInfo
                {
                    IsUseDial = true,
                    IsDialSucceed = true,
                    IsUseHangUp = true,
                    Mode = DialingMode.Preview
                },
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    Mode = DialingMode.Predictive
                }
            }, dialType);

            var history = BvCallHistoryExAdapter.GetByCondition("SurveyId = @SurveyId ORDER BY ID", new SqlParameter("@SurveyId", context.SurveySID));
            Assert.AreEqual(4, history.Count);
     
            var historyRecord = history.First();
            Assert.AreEqual((byte) DialingMode.Preview, historyRecord.DialingMode);
            Assert.AreEqual((int)OperationType.MovedAndReschedule, (int)historyRecord.OperationType);
            Assert.AreEqual(1, historyRecord.CallCenterId);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void PredictiveHybridWithSpecialDial_ProcessingPreviewCallWithSuccessDialing_InterviewingCorrect(DialType dialType)
        {
            PredictiveHybrid_TestBase(new[]{
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    Mode = DialingMode.Predictive
                },
                new InterviewInfo
                {
                    IsUseDial = true,
                    IsDialSucceed = true,
                    IsUseHangUp = true,
                    Mode = DialingMode.SpecialDial
                },
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    Mode = DialingMode.Predictive
                }
            }, dialType);
        }

        // 1. create survey with predictive dialing mode
        // 2. first call is delivered with predictive dialing mode
        //    second call is delivered with preview dialing mode. During interviewing Dial command is called
        //    third call is delivered with predictive dialing mode
        // 3. Interviewing is correct        
        [Theory, Owner(@"FIRM\AlexanderL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void PredictiveHybridWithPreview_ProcessingPreviewCallWithFailedDialing_InterviewingCorrect(DialType dialType)
        {
            PredictiveHybrid_TestBase(new[]{
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    Mode = DialingMode.Predictive
                },
                new InterviewInfo
                {
                    IsUseDial = true,
                    IsDialSucceed = true,
                    IsUseHangUp = false,
                    Mode = DialingMode.Preview
                },
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    Mode = DialingMode.Predictive
                }
            }, dialType);
        }

        [Theory, Owner(@"FIRM\AlexanderL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void PredictiveHybridWithSpecificDial_ProcessingPreviewCallWithFailedDialing_InterviewingCorrect(DialType dialType)
        {
            PredictiveHybrid_TestBase(new[]{
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    Mode = DialingMode.Predictive
                },
                new InterviewInfo
                {
                    IsUseDial = true,
                    IsDialSucceed = true,
                    IsUseHangUp = false,
                    Mode = DialingMode.SpecialDial
                },
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    Mode = DialingMode.Predictive
                }
            }, dialType);
        }

        // 1. create survey with predictive dialing mode
        // 2. first call is delivered with predictive dialing mode
        //    second call is delivered with preview dialing mode. During interviewing Dial and Hungup commands aren't called
        //    third call is delivered with predictive dialing mode
        // 3. Interviewing is correct  
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void PredictiveHybridWithPreview_ProcessingPreviewCallWithoutDialing_InterviewingCorrect(DialType dialType)
        {
            PredictiveHybrid_TestBase(new[]{
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    Mode = DialingMode.Predictive
                },
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    Mode = DialingMode.Preview
                },
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    Mode = DialingMode.Predictive
                }
            }, dialType);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void PredictiveHybridWithSpecialDial_ProcessingPreviewCallWithoutDialing_InterviewingCorrect(DialType dialType)
        {
            PredictiveHybrid_TestBase(new[]{
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    Mode = DialingMode.Predictive
                },
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    Mode = DialingMode.SpecialDial
                },
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    Mode = DialingMode.Predictive
                }
            }, dialType);
        }

        // 1. create survey with predictive dialing mode
        // 2. first call is delivered with predictive dialing mode
        //    second call is delivered with preview dialing mode. During interviewing Hungup command is called
        //    third call is delivered with predictive dialing mode
        // 3. Interviewing is correct  
        [Theory, Owner(@"FIRM\AlexanderL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void PredictiveHybridWithPreview_ProcessingPreviewCallWithoutDialingButWithHangup_InterviewingCorrect(DialType dialType)
        {
            PredictiveHybrid_TestBase(new[]{
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    Mode = DialingMode.Predictive
                },
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = true,
                    Mode = DialingMode.Preview
                },
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    Mode = DialingMode.Predictive
                }
            }, dialType);
        }

        [Theory, Owner(@"FIRM\AlexanderL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void PredictiveHybridWithSpecialDial_ProcessingPreviewCallWithoutDialingButWithHangup_InterviewingCorrect(DialType dialType)
        {
            PredictiveHybrid_TestBase(new[]{
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    Mode = DialingMode.Predictive
                },
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = true,
                    Mode = DialingMode.SpecialDial
                },
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    Mode = DialingMode.Predictive
                }
            }, dialType);
        }

        // 1. create survey with predictive dialing mode
        // 2. First call is delivered with preview dialing mode. During interviewing Dial and Hungup commands aren't called
        //    Second call is delivered with preview dialing mode. During interviewing Hungup command is called after failed Dial command
        //    Third call is delivered with preview dialing mode. During interviewing Dial command is called with failed result( not connect )
        //    Fourth call is delivered with preview dialing mode. During interviewing Hungup command is called after failed Dial command
        //    Fifth call is delivered with preview dialing mode. During interviewing Hungup command is called without call of Dial command
        // 3. Interviewing is correct  
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void PredictiveHybridWithPreview_ProcessingOnlyPreviewCallWithFailedDialing_InterviewingCorrect(DialType dialType)
        {
            PredictiveHybrid_TestBase(new[]{
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    IsDialSucceed = false,
                    Mode = DialingMode.Preview
                },
                new InterviewInfo
                {
                    IsUseDial = true,
                    IsUseHangUp = true,
                    IsDialSucceed = false,
                    Mode = DialingMode.Preview
                },
                new InterviewInfo
                {
                    IsUseDial = true,
                    IsUseHangUp = false,
                    IsDialSucceed = false,
                    Mode = DialingMode.Preview
                },
                new InterviewInfo
                {
                    IsUseDial = true,
                    IsUseHangUp = true,
                    IsDialSucceed = false,
                    Mode = DialingMode.Preview
                },
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = true,
                    IsDialSucceed = false,
                    Mode = DialingMode.Preview
                }
            }, dialType);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void PredictiveHybridWithSpecialDial_ProcessingOnlyPreviewCallWithFailedDialing_InterviewingCorrect(DialType dialType)
        {
            PredictiveHybrid_TestBase(new[]{
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    IsDialSucceed = false,
                    Mode = DialingMode.SpecialDial
                },
                new InterviewInfo
                {
                    IsUseDial = true,
                    IsUseHangUp = true,
                    IsDialSucceed = false,
                    Mode = DialingMode.SpecialDial
                },
                new InterviewInfo
                {
                    IsUseDial = true,
                    IsUseHangUp = false,
                    IsDialSucceed = false,
                    Mode = DialingMode.SpecialDial
                },
                new InterviewInfo
                {
                    IsUseDial = true,
                    IsUseHangUp = true,
                    IsDialSucceed = false,
                    Mode = DialingMode.SpecialDial
                },
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = true,
                    IsDialSucceed = false,
                    Mode = DialingMode.SpecialDial
                }
            }, dialType);
        }

        // 1. create survey with predictive dialing mode
        // 2. First call is delivered with preview dialing mode. During interviewing Dial and Hungup commands aren't called
        //    Second call is delivered with preview dialing mode. During interviewing Hungup command is called after success Dial command
        //    Third call is delivered with preview dialing mode. During interviewing Dial command is called with success result( connect )
        //    Fourth call is delivered with preview dialing mode. During interviewing Hungup command is called after success Dial command
        //    Fifth call is delivered with preview dialing mode. During interviewing Hungup command is called without call of Dial command
        // 3. Interviewing is correct  
        [Theory, Owner(@"FIRM\AlexanderL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void PredictiveHybridWithPreview_ProcessingOnlyPreviewCallWithSuccessDialing_InterviewingCorrect(DialType dialType)
        {
            PredictiveHybrid_TestBase(new[]{
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    IsDialSucceed = true,
                    Mode = DialingMode.Preview
                },
                new InterviewInfo
                {
                    IsUseDial = true,
                    IsUseHangUp = true,
                    IsDialSucceed = true,
                    Mode = DialingMode.Preview
                },
                new InterviewInfo
                {
                    IsUseDial = true,
                    IsUseHangUp = false,
                    IsDialSucceed = true,
                    Mode = DialingMode.Preview
                },
                new InterviewInfo
                {
                    IsUseDial = true,
                    IsUseHangUp = true,
                    IsDialSucceed = true,
                    Mode = DialingMode.Preview
                },
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = true,
                    IsDialSucceed = true,
                    Mode = DialingMode.Preview
                }
            }, dialType);
        }

        [Theory, Owner(@"FIRM\AlexanderL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void PredictiveHybridWithSpecialDial_ProcessingOnlyPreviewCallWithSuccessDialing_InterviewingCorrect(DialType dialType)
        {
            PredictiveHybrid_TestBase(new[]{
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    IsDialSucceed = true,
                    Mode = DialingMode.SpecialDial
                },
                new InterviewInfo
                {
                    IsUseDial = true,
                    IsUseHangUp = true,
                    IsDialSucceed = true,
                    Mode = DialingMode.SpecialDial
                },
                new InterviewInfo
                {
                    IsUseDial = true,
                    IsUseHangUp = false,
                    IsDialSucceed = true,
                    Mode = DialingMode.SpecialDial
                },
                new InterviewInfo
                {
                    IsUseDial = true,
                    IsUseHangUp = true,
                    IsDialSucceed = true,
                    Mode = DialingMode.SpecialDial
                },
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = true,
                    IsDialSucceed = true,
                    Mode = DialingMode.SpecialDial
                }
            }, dialType);
        }

        // 1. create survey with progressive dialing mode
        // 2. First call is delivered with progressive dialing mode. During interviewing Dial and Hungup commands aren't used
        //    Second call is delivered with progressive dialing mode. During interviewing Hungup command is called without call of Dial command
        //    Third call is delivered with progressive dialing mode. During interviewing Hungup command is called after Dial command( dial command isn't take effect )
        //    Fourth call is delivered with progressive dialing mode. During interviewing Dial command is called.( It isn't take effect )
        //    Fifth call is delivered with progressive dialing mode. During interviewing Dial and Hungup commands aren't used
        // 3. Interviewing is correct  
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void ProgressiveHybrid_ProcessingOnlyProgressiveCallWithFailedDialing_InterviewingCorrect(DialType dialType)
        {
            PassProgressiveHybridInterview(new[]{
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    IsDialSucceed = false,
                    Mode = DialingMode.Automatic
                },
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = true,
                    IsDialSucceed = false,
                    Mode = DialingMode.Automatic
                },
                new InterviewInfo
                {
                    IsUseDial = true,
                    IsUseHangUp = true,
                    IsDialSucceed = false,
                    Mode = DialingMode.Automatic
                },
                new InterviewInfo
                {
                    IsUseDial = true,
                    IsUseHangUp = false,
                    IsDialSucceed = false,
                    Mode = DialingMode.Automatic
                },
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    IsDialSucceed = false,
                    Mode = DialingMode.Automatic
                }
            }, AgentTaskChoiceMode.Automatic, dialType);
        }

        [Theory, Owner(@"FIRM\AlexanderL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void ProgressiveHybrid_ProcessingOnlyProgressiveCallWithSuccessDialing_InterviewingCorrect(DialType dialType)
        {
            PassProgressiveHybridInterview(new[]{
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    IsDialSucceed = true,
                    Mode = DialingMode.Automatic
                },
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = true,
                    IsDialSucceed = true,
                    Mode = DialingMode.Automatic
                },
                new InterviewInfo
                {
                    IsUseDial = true,
                    IsUseHangUp = true,
                    IsDialSucceed = true,
                    Mode = DialingMode.Automatic
                },
                new InterviewInfo
                {
                    IsUseDial = true,
                    IsUseHangUp = false,
                    IsDialSucceed = true,
                    Mode = DialingMode.Automatic
                },
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    IsDialSucceed = true,
                    Mode = DialingMode.Automatic
                }
            }, AgentTaskChoiceMode.CampaignAssignment, dialType);
        }

        // 1. create survey with progressive dialing mode
        // 2. First call is delivered with preview dialing mode. During interviewing Dial and Hungup commands aren't used
        //    Second call is delivered with preview dialing mode. During interviewing Hungup command is called without call of Dial command
        //    Third call is delivered with preview dialing mode. During interviewing Hungup command is called after Dial command
        //    Fourth call is delivered with preview dialing mode. During interviewing Dial command is called.
        //    Fifth call is delivered with preview dialing mode. During interviewing Dial and Hungup commands aren't used
        // 3. Interviewing is correct  
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void ProgressiveHybridWithPreview_ProcessingOnlyPreviewCallWithSuccessDialing_InterviewingCorrect(DialType dialType)
        {
            PassProgressiveHybridInterview(new[]{
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    IsDialSucceed = true,
                    Mode = DialingMode.Preview
                },
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = true,
                    IsDialSucceed = true,
                    Mode = DialingMode.Preview
                },
                new InterviewInfo
                {
                    IsUseDial = true,
                    IsUseHangUp = true,
                    IsDialSucceed = true,
                    Mode = DialingMode.Preview
                },
                new InterviewInfo
                {
                    IsUseDial = true,
                    IsUseHangUp = false,
                    IsDialSucceed = true,
                    Mode = DialingMode.Preview
                },
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    IsDialSucceed = true,
                    Mode = DialingMode.Preview
                }
            }, AgentTaskChoiceMode.Automatic, dialType);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void ProgressiveHybridWithSpecialDial_ProcessingOnlyPreviewCallWithSuccessDialing_InterviewingCorrect(DialType dialType)
        {
            PassProgressiveHybridInterview(new[]{
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    IsDialSucceed = true,
                    Mode = DialingMode.SpecialDial
                },
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = true,
                    IsDialSucceed = true,
                    Mode = DialingMode.SpecialDial
                },
                new InterviewInfo
                {
                    IsUseDial = true,
                    IsUseHangUp = true,
                    IsDialSucceed = true,
                    Mode = DialingMode.SpecialDial
                },
                new InterviewInfo
                {
                    IsUseDial = true,
                    IsUseHangUp = false,
                    IsDialSucceed = true,
                    Mode = DialingMode.SpecialDial
                },
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    IsDialSucceed = true,
                    Mode = DialingMode.SpecialDial
                }
            }, AgentTaskChoiceMode.Automatic, dialType);
        }

        // 1. create survey with progressive dialing mode
        // 2. First call is delivered with preview dialing mode. During interviewing Dial and Hungup commands aren't used
        //    Second call is delivered with preview dialing mode. During interviewing Hungup command is called without call of Dial command
        //    Third call is delivered with preview dialing mode. During interviewing Hungup command is called after failed Dial command
        //    Fourth call is delivered with preview dialing mode. During interviewing Dial command is called without connect.
        //    Fifth call is delivered with preview dialing mode. During interviewing Dial and Hungup commands aren't used
        // 3. Interviewing is correct  
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void ProgressiveHybridWithPreview_ProcessingOnlyPreviewCallWithFailedDialing_InterviewingCorrect(DialType dialType)
        {
            PassProgressiveHybridInterview(new[]{
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    IsDialSucceed = false,
                    Mode = DialingMode.Preview
                },
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = true,
                    IsDialSucceed = false,
                    Mode = DialingMode.Preview
                },
                new InterviewInfo
                {
                    IsUseDial = true,
                    IsUseHangUp = true,
                    IsDialSucceed = false,
                    Mode = DialingMode.Preview
                },
                new InterviewInfo
                {
                    IsUseDial = true,
                    IsUseHangUp = false,
                    IsDialSucceed = false,
                    Mode = DialingMode.Preview
                },
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    IsDialSucceed = false,
                    Mode = DialingMode.Preview
                }
            }, AgentTaskChoiceMode.Automatic, dialType);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void ProgressiveHybridWithSpecialDial_ProcessingOnlyPreviewCallWithFailedDialing_InterviewingCorrect(DialType dialType)
        {
            PassProgressiveHybridInterview(new[]{
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    IsDialSucceed = false,
                    Mode = DialingMode.SpecialDial
                },
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = true,
                    IsDialSucceed = false,
                    Mode = DialingMode.SpecialDial
                },
                new InterviewInfo
                {
                    IsUseDial = true,
                    IsUseHangUp = true,
                    IsDialSucceed = false,
                    Mode = DialingMode.SpecialDial
                },
                new InterviewInfo
                {
                    IsUseDial = true,
                    IsUseHangUp = false,
                    IsDialSucceed = false,
                    Mode = DialingMode.SpecialDial
                },
                new InterviewInfo
                {
                    IsUseDial = false,
                    IsUseHangUp = false,
                    IsDialSucceed = false,
                    Mode = DialingMode.SpecialDial
                }
            }, AgentTaskChoiceMode.Automatic, dialType);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void ProgressiveHybridWithSaTaskChouse_LoginToSurveyWithoutCalls_DialingModeIsCorrect(DialType dialType)
        {
            var test = CreateSurveyLoggedInPersonAndInterviews(DialingMode.Automatic,
                AgentTaskChoiceMode.CampaignAssignment,
                new InterviewInfo[0], dialType);

            test.WS.StartInterview(test.SurveyName, 0);
            test.WaitInterviewState(InterviewState.NO_CALLS);
            test.CheckActivityView(x=> Assert.AreEqual(x.DiallingMode, (int)DialingMode.Automatic) );
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void ProgressiveHybridWithAutoTaskChoise_LoginToSurveyWithoutCalls_DialingModeIsCorrect(DialType dialType)
        {
            var test = CreateSurveyLoggedInPersonAndInterviews(DialingMode.Automatic,
                AgentTaskChoiceMode.Automatic,
                new InterviewInfo[0], dialType);

            test.WS.StartInterview(null, 0);
            test.WaitInterviewState(InterviewState.NO_CALLS);
            test.CheckActivityView(x => Assert.AreEqual(x.DiallingMode, (int)DialingMode.Manual));
        }

        [Theory, Owner(@"FIRM\alm")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void PredictiveHybrid_ProcessingHybridCall_DialerIdIsBondToInterview(DialType dialType)
        {
            const string extensionNumber = "101010";

            var interviewInfo = new InterviewInfo
            {
                IsUseDial = true,
                IsUseHangUp = true,
                IsDialSucceed = true,
                Mode = DialingMode.Preview
            };

            var test = CreateSurveyLoggedInPersonAndInterviews(DialingMode.Predictive,
                AgentTaskChoiceMode.CampaignAssignment,
                new[] { interviewInfo }, dialType);

            test.LoginToDialer_Predictive(extensionNumber, false, null);

            test.StartInterview_Predictive(test.Interviews.Count());

            var interview = InterviewRepository.GetById(test.SurveySID, 1);
            
            // deliver interview to CATI console
            test.PreviewScreenPopToInterview_Predictive(interview);

            test.Dial_Predictive(interview, DialingMode.Preview, true);

            interview = InterviewRepository.GetById(test.SurveySID, 1);

            Assert.AreEqual(1, interview.DialerId, "DialerId is not as expected");

            test.Hangup(interview, 1);
            test.CompleteInterview_Predictive(interview);
        }
        
        private class InterviewInfo
        {
            public DialingMode Mode;
            public bool IsUseDial;
            public bool IsDialSucceed;
            public bool IsUseHangUp;
        }

        private TestCati2 PredictiveHybrid_TestBase(InterviewInfo[] interviewsInfo, DialType dialType)
        {
            const string extensionNumber = "101010";

            var test = CreateSurveyLoggedInPersonAndInterviews(DialingMode.Predictive,
                AgentTaskChoiceMode.CampaignAssignment,
                interviewsInfo, dialType);

            test.LoginToDialer_Predictive(extensionNumber, false, null);

            test.StartInterview_Predictive(test.Interviews.Count());

            for (int i = 1; i <= interviewsInfo.Length; ++i)
            {
                var info = interviewsInfo[i-1];
                var interview = InterviewRepository.GetById(test.SurveySID, i);

                test.CheckActivityView(view => Assert.AreEqual(view.DiallingMode, (int)DialingMode.Predictive));

                //delivere interview to CATI console
                switch(info.Mode)
                {
                    case DialingMode.Predictive:
                        test.ConnectToInterview_Predictive(interview);
                        break;
                    case DialingMode.Preview:
                    case DialingMode.SpecialDial:
                        test.PreviewScreenPopToInterview_Predictive(interview);
                        break;
                }

                test.CheckActivityView(view => Assert.AreEqual(view.DiallingMode, (int)info.Mode));

                if (info.IsUseDial)
                {
                    test.Dial_Predictive(interview, info.Mode, info.IsDialSucceed);
                }

                test.CheckActivityView(view => Assert.AreEqual(view.DiallingMode, (int)info.Mode));

                if (info.IsUseHangUp)
                {
                    const int initiator = 1;
                    test.Hangup(interview, initiator);
                }

                test.CheckActivityView(view => Assert.AreEqual(view.DiallingMode, (int)info.Mode));
                
                test.CompleteInterview_Predictive(interview);

                test.CheckActivityView(view => Assert.AreEqual(view.DiallingMode, (int)DialingMode.Predictive));
            }
            test.CheckActivityView(view => Assert.AreEqual(view.DiallingMode, (int)DialingMode.Predictive));

            return test;
        }
        
        private TestCati2 CreateSurveyLoggedInPersonAndInterviews(DialingMode surveyDialingMode,
            AgentTaskChoiceMode personAssignmentMode, InterviewInfo[] interviewsInfo, DialType dialType)
        {
            const string user = "user";
            const string pwd = "pwd";

            var test = new TestCati2(true, BackendToolsObject, dialType);

            test.CreateSurveyWithPerson(surveyDialingMode, user, pwd, personAssignmentMode, dialType: dialType);


            BackendToolsObject.LaunchScript(test.SurveySID, _script);

            BvInterviewEntity[] interviews = test.CreateInterviewsWithCalls(interviewsInfo.Length, dialType);
            //Reschedule all intereviews with dialing mode preview
            int[] previewInterviewIds = interviews.Where(x => interviewsInfo[x.ID - interviews[0].ID].Mode == DialingMode.Preview).Select(x => x.ID).ToArray();
            if (previewInterviewIds.Length > 0)
            {
                CallTools.MoveAndRescheduleCalls(
                    test.SurveySID,
                    previewInterviewIds,
                    ItsToSetPreviewDialingMode);
            }

            int[] specialDialInterviewIds = interviews.Where(x => interviewsInfo[x.ID - interviews[0].ID].Mode == DialingMode.SpecialDial).Select(x => x.ID).ToArray();
            if (specialDialInterviewIds.Length > 0)
            {
                CallTools.MoveAndRescheduleCalls(
                    test.SurveySID,
                    specialDialInterviewIds,
                    ItsToSetSpecialDialDialingMode);
            }

            test.Login(user, pwd, personAssignmentMode, true);

            return test;
        }
        
        private void PassProgressiveHybridInterview(InterviewInfo[] interviewsInfo, AgentTaskChoiceMode personMode, DialType dialType)
        {
            const string extensionNumber = "101010";
            const int initiator = 0;

            var test = CreateSurveyLoggedInPersonAndInterviews(DialingMode.Automatic,
                personMode,
                interviewsInfo.ToArray(), dialType);

            test.LoginToDialer(extensionNumber);

            if (personMode == AgentTaskChoiceMode.Automatic)
            {
                test.CheckActivityView(view => Assert.AreEqual(view.DiallingMode, (int) DialingMode.Preview));
            }
            else
            {
                test.CheckActivityView(view => Assert.AreEqual(view.DiallingMode, (int) DialingMode.Automatic));
            }

            test.StartInterview_HybridProgressive(InterviewRepository.GetById(test.SurveySID, 1));

            for(int i = 1; i <= interviewsInfo.Length; ++i)
            {
                var info = interviewsInfo[i-1];
                var interview = InterviewRepository.GetById(test.SurveySID, i);

                if(info.Mode == DialingMode.Automatic)
                {
                    test.ReplyOnInterview_Progressive(interview);
                }

                if (info.IsUseDial)
                {
                    test.Dial_HybridProgressive(interview, info.IsDialSucceed);
                }

                if (info.IsUseHangUp)
                {
                    test.Hangup(interview, initiator);
                }

                test.DialerHelper.AddRequestCompleteCall();

                if (i != interviewsInfo.Length && interviewsInfo[i].Mode == DialingMode.Automatic)
                    test.DialerHelper.AddRequestSendNumber();

                test.CompleteInterview_Progressive(interview, false);
            }

            if (personMode == AgentTaskChoiceMode.Automatic)
                test.CheckActivityView(view => Assert.AreEqual(view.DiallingMode, (int)DialingMode.Manual));
            else
                test.CheckActivityView(view => Assert.AreEqual(view.DiallingMode, (int)DialingMode.Automatic));
        }
    }
}