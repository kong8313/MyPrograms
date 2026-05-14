using System.Linq;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.FCDSpecificTests
{
    [TestClass]
    public class FcdFilteringTests : BaseMockedIntegrationTest
    {

        [TestMethod]
        public void CloseCell_ClassWithDifferentItses_CallsWithFilteredItsesAreDeleted()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", IsUseDb = true,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData(){ Tag="S1.I1", Data="q1=1", ITS = CallOutcome.FreshSample, Call = new CallData()},
                            new InterviewData(){ Tag="S1.I2", Data="q1=1", ITS = CallOutcome.Appointment, Call = new CallData()},
                            new InterviewData(){ Tag="S1.I3", Data="q1=1", ITS = CallOutcome.Fax, Call = new CallData()},
                            new InterviewData(){ Tag="S1.I4", Data="q1=1", ITS = CallOutcome.NoReply, Call = new CallData()},
                            new InterviewData(){ Tag="S1.I5", Data="q1=2", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I6", ITS = CallOutcome.FreshSample, Call = new CallData()},
                        },
                        Assigns = new[]{"P1"}}
                   },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.Automatic } }
            }.Create();

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("quota1");

            quota.CloseCellById(1);

            context.GetCalls("S1.I1", "S1.I3", "S1.I4").Assert.IsNull();
            context.GetCalls("S1.I2", "S1.I5", "S1.I6").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }

        [TestMethod]
        public void CloseCell_ClassWithDifferentItsesAndIgnoredFreshSample_CallsWithFilteredItsesAreDeleted()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", IsUseDb = true,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData(){ Tag="S1.I1", Data="q1=1", ITS = CallOutcome.FreshSample, Call = new CallData()},
                            new InterviewData(){ Tag="S1.I2", Data="q1=1", ITS = CallOutcome.Appointment, Call = new CallData()},
                            new InterviewData(){ Tag="S1.I3", Data="q1=1", ITS = CallOutcome.Fax, Call = new CallData()},
                            new InterviewData(){ Tag="S1.I4", Data="q1=1", ITS = CallOutcome.NoReply, Call = new CallData()},
                            new InterviewData(){ Tag="S1.I5", Data="q1=2", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I6", ITS = CallOutcome.FreshSample, Call = new CallData()},
                        },
                        Assigns = new[]{"P1"}}
                   },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.Automatic } }
            }.Create();

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("quota1");

            var state = StateRepository.GetById(survey.Model.StateGroupID, (int)CallOutcome.FreshSample);
            state.FcdAction = true;
            StateRepository.Update(state);


            quota.CloseCellById(1);

            context.GetCalls("S1.I3", "S1.I4").Assert.IsNull();
            context.GetCalls("S1.I1", "S1.I2", "S1.I5", "S1.I6").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }

        [TestMethod]
        public void OnQuotaChanged_ClassWithDifferentItses_CallsWithFilteredItsesAreDeleted()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", IsUseDb = true,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData(){ Tag="S1.I1", Data="q1=1", ITS = CallOutcome.FreshSample , Call = new CallData()},
                            new InterviewData(){ Tag="S1.I2", Data="q1=1", ITS = CallOutcome.Appointment,  Call = new CallData()},
                            new InterviewData(){ Tag="S1.I3", Data="q1=1", ITS = CallOutcome.Fax,  Call = new CallData() },
                            new InterviewData(){ Tag="S1.I4", Data="q1=1", ITS = CallOutcome.NoReply,  Call = new CallData()},
                            new InterviewData(){ Tag="S1.I5", Data="q1=2", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I6", ITS = CallOutcome.FreshSample, Call = new CallData()},
                            new InterviewData(){ Tag="S1.I7", Data="q1=3", ITS = CallOutcome.FreshSample,  Call = new CallData()},
                            new InterviewData(){ Tag="S1.I8", Data="q1=3", ITS = CallOutcome.Appointment, Call = new CallData()},
                        },
                        Assigns = new[]{"P1"}}
                   },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.Automatic } }
            }.Create();

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("quota1");

            survey.Database.CloseCell(quota.Data.Id, 1);
            quota.OnQuotaChanged();

            context.GetCalls("S1.I1", "S1.I3", "S1.I4").Assert.IsNull();
            context.GetCalls("S1.I2", "S1.I5", "S1.I6", "S1.I8", "S1.I7").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }

        [TestMethod]
        public void OnQuotaChanged_ClassWithDifferentItsesAndIgnoredFreshSample_CallsWithFilteredItsesAreDeleted()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", IsUseDb = true,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData(){ Tag="S1.I1", Data="q1=1", ITS = CallOutcome.FreshSample, Call = new CallData()},
                            new InterviewData(){ Tag="S1.I2", Data="q1=1", ITS = CallOutcome.Appointment, Call = new CallData()},
                            new InterviewData(){ Tag="S1.I3", Data="q1=1", ITS = CallOutcome.Fax, Call = new CallData()},
                            new InterviewData(){ Tag="S1.I4", Data="q1=1", ITS = CallOutcome.NoReply, Call = new CallData()},
                            new InterviewData(){ Tag="S1.I5", Data="q1=2", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I6", ITS = CallOutcome.FreshSample, Call = new CallData()},
                        },
                        Assigns = new[]{"P1"}}
                   },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.Automatic } }
            }.Create();

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("quota1");

            var state = StateRepository.GetById(survey.Model.StateGroupID, (int)CallOutcome.FreshSample);
            state.FcdAction = true;
            StateRepository.Update(state);


            survey.Database.CloseCell(quota.Data.Id, 1);
            quota.OnQuotaChanged();

            context.GetCalls("S1.I3", "S1.I4").Assert.IsNull();
            context.GetCalls("S1.I1", "S1.I2", "S1.I5", "S1.I6").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }

        [TestMethod]
        public void ActivateCalls_ClassWithDifferentItses_CallsWithFilteredItsesAreDeleted()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", IsUseDb = true,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData(){ Tag="S1.I1", Data="q1=1", ITS = CallOutcome.FreshSample},
                            new InterviewData(){ Tag="S1.I2", Data="q1=1", ITS = CallOutcome.Appointment},
                            new InterviewData(){ Tag="S1.I3", Data="q1=1", ITS = CallOutcome.Fax},
                            new InterviewData(){ Tag="S1.I4", Data="q1=1", ITS = CallOutcome.NoReply},
                            new InterviewData(){ Tag="S1.I5", Data="q1=2"},
                            new InterviewData(){ Tag="S1.I6", ITS = CallOutcome.FreshSample },
                        },
                        Assigns = new[]{"P1"}}
                   },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.Automatic } }
            }.Create();

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("quota1");

            quota.CloseCellById(1);

            CallTools.ActivateCalls(survey.Id, 1, CallStates.All, 0, (int)CallShiftType.None, null, true,
                context.GetInterviews("S1.I1", "S1.I2", "S1.I3", "S1.I4", "S1.I5", "S1.I6").Select(x => x.Id));

            context.GetCalls("S1.I1", "S1.I3", "S1.I4").Assert.IsNull();
            context.GetCalls("S1.I2", "S1.I5", "S1.I6").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }

        [TestMethod]
        public void ActivateCalls_ClassWithDifferentItsesAndIgnoredFreshSample_CallsWithFilteredItsesAreDeleted()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", IsUseDb = true,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData(){ Tag="S1.I1", Data="q1=1", ITS = CallOutcome.FreshSample},
                            new InterviewData(){ Tag="S1.I2", Data="q1=1", ITS = CallOutcome.Appointment},
                            new InterviewData(){ Tag="S1.I3", Data="q1=1", ITS = CallOutcome.Fax},
                            new InterviewData(){ Tag="S1.I4", Data="q1=1", ITS = CallOutcome.NoReply},
                            new InterviewData(){ Tag="S1.I5", Data="q1=2"},
                            new InterviewData(){ Tag="S1.I6", ITS = CallOutcome.FreshSample },
                            new InterviewData(){ Tag="S1.I7", Data="q1=3", ITS = CallOutcome.FreshSample},
                        },
                        Assigns = new[]{"P1"}}
                   },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.Automatic } }
            }.Create();

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("quota1");

            quota.CloseCellById(1);

            var state = StateRepository.GetById(survey.Model.StateGroupID, (int)CallOutcome.FreshSample);
            state.FcdAction = true;
            StateRepository.Update(state);


            CallTools.ActivateCalls(survey.Id, 1, CallStates.All, 0, (int)CallShiftType.None, null, true,
                context.GetInterviews("S1.I1", "S1.I2", "S1.I3", "S1.I4", "S1.I5", "S1.I6", "S1.I7").Select(x => x.Id));

            context.GetCalls("S1.I1", "S1.I2", "S1.I5", "S1.I6", "S1.I7").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
            context.GetCalls("S1.I3", "S1.I4").Assert.IsNull();
        }

        [TestMethod]
        public void MoveAndReschedule_ClassWithDifferentItses_CallsWithFilteredItsesAreDeleted()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", IsUseDb = true,
                        SchedulingScript = AllHoursSchedule.Name,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData(){ Tag="S1.I1", Data="q1=1", ITS = CallOutcome.FreshSample},
                            new InterviewData(){ Tag="S1.I2", Data="q1=1", ITS = CallOutcome.Appointment},
                            new InterviewData(){ Tag="S1.I3", Data="q1=1", ITS = CallOutcome.Fax},
                            new InterviewData(){ Tag="S1.I4", Data="q1=1", ITS = CallOutcome.NoReply},
                            new InterviewData(){ Tag="S1.I5", Data="q1=2"},
                            new InterviewData(){ Tag="S1.I6", ITS = CallOutcome.FreshSample },
                            new InterviewData(){ Tag="S1.I7", Data="q1=3", ITS = CallOutcome.FreshSample },
                        },
                        Assigns = new[]{"P1"}}
                   },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.Automatic } },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("quota1");

            quota.CloseCellById(1);

            CallTools.MoveAndRescheduleCalls(survey.Id,
                context.GetInterviews("S1.I1", "S1.I2", "S1.I3", "S1.I4", "S1.I5", "S1.I6", "S1.I7").Select(x => x.Id),
                (int)CallOutcome.FreshSample);

            context.GetCalls("S1.I1", "S1.I2", "S1.I3", "S1.I4").Assert.IsNull();
            context.GetCalls("S1.I5", "S1.I6", "S1.I7").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }

        [TestMethod]
        public void MoveAndReschedule_ClassWithDifferentItsesAndIgnoredFreshSample_CallsWithFilteredItsesAreDeleted()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", IsUseDb = true, SchedulingScript = AllHoursSchedule.Name,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData(){ Tag="S1.I1", Data="q1=1", ITS = CallOutcome.FreshSample},
                            new InterviewData(){ Tag="S1.I2", Data="q1=1", ITS = CallOutcome.Appointment},
                            new InterviewData(){ Tag="S1.I3", Data="q1=1", ITS = CallOutcome.Fax},
                            new InterviewData(){ Tag="S1.I4", Data="q1=1", ITS = CallOutcome.NoReply},
                            new InterviewData(){ Tag="S1.I5", Data="q1=2"},
                            new InterviewData(){ Tag="S1.I6", ITS = CallOutcome.FreshSample },
                        },
                        Assigns = new[]{"P1"}}
                   },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.Automatic }, },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("quota1");

            quota.CloseCellById(1);

            var state = StateRepository.GetById(survey.Model.StateGroupID, (int)CallOutcome.FreshSample);
            state.FcdAction = true;
            StateRepository.Update(state);

            CallTools.MoveAndRescheduleCalls(survey.Id,
                context.GetInterviews("S1.I1", "S1.I2", "S1.I3", "S1.I4", "S1.I5", "S1.I6").Select(x => x.Id),
                (int)CallOutcome.FreshSample);

            context.GetCalls("S1.I1", "S1.I2", "S1.I3", "S1.I4", "S1.I5", "S1.I6").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }
    }
}
