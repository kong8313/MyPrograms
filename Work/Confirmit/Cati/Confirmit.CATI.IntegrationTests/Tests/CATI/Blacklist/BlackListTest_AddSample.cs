using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Common;
using Confirmit.CATI.IntegrationTests.Framework.ControllerExtensions;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.CATI.Blacklist
{
    [TestClass]
    public class BlacklistAddSampleTest : BaseMockedIntegrationTest
    {
        [TestMethod, Owner(@"Firm\MaximL")]
        public void AddSample_SimpleMode_TwoInterviewMovedToBlacklist()
        {
            var context = new TestData()
            {
                Surveys = new[] {new SurveyData()
                {
                    Tag = "S1", IsUseDb = true, IsSupportBlackList = true,
                    Forms = new[] {
                        new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}}
                    },
                    Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=0},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                }
                            }
                        }
                }},
                TelephoneBlacklist = new[] { "88001001010" }
            }.Create();

            var survey = context.GetSurvey("S1");

            var interviews = new[]
            {
                new InterviewData() {Tag = "S1.I1", TelephoneNumber = "88001001010", Data="q1="},
                //should be also closed by FCD
                new InterviewData() {Tag = "S1.I2", TelephoneNumber = "8 (800) 100-10-10", Data="q1=1"},
                new InterviewData() {Tag = "S1.I3", TelephoneNumber = "88001001011", Data="q1="}
            };

            survey.AddSample(SchedulingMode.Simple, interviews);

            context.GetInterview("S1.I1").Assert.IsTrue(x => x.TransientState == (int)CallOutcome.Blacklist);
            context.GetInterview("S1.I2").Assert.IsTrue(x => x.TransientState == (int)CallOutcome.Blacklist);
            context.GetInterview("S1.I3").Assert.IsTrue(x => x.TransientState == (int)CallOutcome.FreshSample);

            context.GetCall("S1.I1").Assert.IsNull();
            context.GetCall("S1.I2").Assert.IsNull();
            context.GetCall("S1.I3").Assert.IsTrue(x => x != null);

            Assert.AreEqual((byte)OperationType.DeleteCallByBlacklistInAddSample, context.GetInterview("S1.I1").GetCallHistory().Last().OperationType);
            Assert.AreEqual((byte)OperationType.DeleteCallByBlacklistInAddSample, context.GetInterview("S1.I2").GetCallHistory().Last().OperationType);
            Assert.AreEqual(0, context.GetInterview("S1.I3").GetCallHistory().Count);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void AddSample_FullSchedulingMode_TwoInterviewMovedToBlacklist()
        {
            var context = new TestData()
            {
                Surveys = new[] {new SurveyData()
                {
                    SchedulingScript = AllHoursSchedule.Name,
                    Tag = "S1", IsUseDb = true, IsSupportBlackList = true,
                    Forms = new[] {
                        new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}}
                    },
                    Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=0},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                }
                            }
                        }
                }},
                TelephoneBlacklist = new[] { "88001001010" },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var survey = context.GetSurvey("S1");

            var interviews = new[]
            {
                new InterviewData() {Tag = "S1.I1", TelephoneNumber = "88001001010", Data="q1="},
                //should be also closed by FCD
                new InterviewData() {Tag = "S1.I2", TelephoneNumber = "8 (800) 100-10-10", Data="q1=1"},
                new InterviewData() {Tag = "S1.I3", TelephoneNumber = "88001001011", Data="q1="}
            };

            survey.AddSample(SchedulingMode.Full, interviews);

            context.GetInterview("S1.I1").Assert.IsTrue(x => x.TransientState == (int)CallOutcome.Blacklist);
            context.GetInterview("S1.I2").Assert.IsTrue(x => x.TransientState == (int)CallOutcome.Blacklist);
            context.GetInterview("S1.I3").Assert.IsTrue(x => x.TransientState == (int)CallOutcome.FreshSample);

            context.GetCall("S1.I1").Assert.IsNull();
            context.GetCall("S1.I2").Assert.IsNull();
            context.GetCall("S1.I3").Assert.IsTrue(x => x != null);

            Assert.AreEqual((byte)OperationType.DeleteCallByBlacklistInAddSample, context.GetInterview("S1.I1").GetCallHistory().Last().OperationType);
            Assert.AreEqual((byte)OperationType.DeleteCallByBlacklistInAddSample, context.GetInterview("S1.I2").GetCallHistory().Last().OperationType);
            Assert.AreEqual(0, context.GetInterview("S1.I3").GetCallHistory().Count);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void AddSample_BlacklistWithStartWithPattern_TwoInterviewMovedToBlacklist()
        {
            var context = new TestData()
            {
                Surveys = new[] {new SurveyData()
                {
                    SchedulingScript = AllHoursSchedule.Name,
                    Tag = "S1", IsUseDb = true, IsSupportBlackList = true,
                    Forms = new[] {
                        new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}}
                    },
                    Quotas = new [] {
                        new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"},
                            Cells = new[]
                            {
                                new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=0},
                                new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                            }
                        }
                    }
                }},
                TelephoneBlacklist = new[] { "111*", "1112*", "11112" },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var survey = context.GetSurvey("S1");

            var interviews = new[]
            {
                new InterviewData() {Tag = "S1.I1", TelephoneNumber = "1-11-12", Data="q1="},
                //should be also closed by FCD
                new InterviewData() {Tag = "S1.I2", TelephoneNumber = "1 11 13", Data="q1=1"},
                new InterviewData() {Tag = "S1.I3", TelephoneNumber = "21114", Data="q1="}
            };

            survey.AddSample(SchedulingMode.Full, interviews);

            context.GetInterview("S1.I1").Assert.IsTrue(x => x.TransientState == (int)CallOutcome.Blacklist);
            context.GetInterview("S1.I2").Assert.IsTrue(x => x.TransientState == (int)CallOutcome.Blacklist);
            context.GetInterview("S1.I3").Assert.IsTrue(x => x.TransientState == (int)CallOutcome.FreshSample);

            context.GetCall("S1.I1").Assert.IsNull();
            context.GetCall("S1.I2").Assert.IsNull();
            context.GetCall("S1.I3").Assert.IsTrue(x => x != null);

            Assert.AreEqual((byte)OperationType.DeleteCallByBlacklistInAddSample, context.GetInterview("S1.I1").GetCallHistory().Last().OperationType);
            Assert.AreEqual((byte)OperationType.DeleteCallByBlacklistInAddSample, context.GetInterview("S1.I2").GetCallHistory().Last().OperationType);
            Assert.AreEqual(0, context.GetInterview("S1.I3").GetCallHistory().Count);
        }
    }
}
