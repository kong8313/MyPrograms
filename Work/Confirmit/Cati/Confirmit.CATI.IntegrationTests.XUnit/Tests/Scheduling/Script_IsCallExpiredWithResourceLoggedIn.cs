using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class ScriptIsCallExpiredWithResourceLoggedIn : BaseMockedIntegrationTest
    {
        [Theory, Owner(@"FIRM\grigoryk")]
        [ClassData(typeof(TestDataGenerator))]
        public void IsCallExpiredWithResourceLoggedIn_ResourceNotLoggedIn_CallExpired(SecurityMode mode)
        {
            SetSecurityMode(mode);

            new DateTimeMocker("2018-03-27T14:15:00");

            var context = new TestData
            {
                Surveys = new[]{
                    new SurveyData {Tag = "S1", Assigns = new [] {"P1"}, SchedulingScript = "SS1", IsUseDb = true,
                        Interviews = new [] {
                            new InterviewData { Tag="S1.I1", ITS = CallOutcome.Appointment, Call = new CallData {Resource = "P1", TimeToExpire = DateTime.Parse("2018-03-27T14:10:00")}}
                        }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1" } },
                Scripts = new[] {
                    new ScriptData { Tag="SS1", Script = new TestScript(new [] {
                            new SubRule(new Action(Action.Operation.RestorePreviousCallState), (int)CallOutcome.Appointment, 0, 0, "IsCallExpiredWithResourceLoggedIn(10)", true),
                            new SubRule(new Action(Action.Operation.SetNewITS, "31"), (int)CallOutcome.Appointment, 0, 0, "IsCallExpired()", true),
                            new SubRule(new Action(Action.Operation.SetNewITS, "32"))
                        },
                        new Shift(1, 1, "0.00:00:00", "6.23:59:59"))
                    }
                }

            }.Create();

            ServiceLocator.Resolve<ICallQueueService>().ExpireAllCalls();

            var call = context.GetCall("S1.I1");
            var interview = context.GetInterview("S1.I1");

            Assert.IsNull(call.Model);
            interview.Assert.IsTrue(x => x.TransientState == 31);
            Assert.AreEqual(1, BvCallHistoryExAdapter.GetAll().Where(x => x.SurveyId == interview.Survey.Id && x.InterviewID == interview.Id).Count());
        }

        [Theory, Owner(@"FIRM\grigoryk")]
        [ClassData(typeof(TestDataGenerator))]
        public void IsCallExpiredWithResourceLoggedIn_ResourceIsLoggedIn_CallNotExpired(SecurityMode mode)
        {
            SetSecurityMode(mode);

            new DateTimeMocker("2018-03-27T14:15:00");

            var context = new TestData
            {
                Surveys = new[]{
                    new SurveyData {Tag = "S1", Assigns = new [] {"P1"}, SchedulingScript = "SS1", IsUseDb = true,
                        Interviews = new [] {
                            new InterviewData { Tag="S1.I1", Call = new CallData { Priority = 10} },
                            new InterviewData { Tag="S1.I2", ITS = CallOutcome.Appointment, Call = new CallData { Resource = "P1", TimeToExpire = DateTime.Parse("2018-03-27T14:10:00") }}
                        }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] {
                    new ScriptData() { Tag="SS1", Script = new TestScript(new [] {
                            new SubRule(new Action(Action.Operation.RestorePreviousCallState), (int)CallOutcome.Appointment, 0, 0, "IsCallExpiredWithResourceLoggedIn(10)", true),
                            new SubRule(new Action(Action.Operation.SetNewITS, "31"), (int)CallOutcome.Appointment, 0, 0, "IsCallExpired()", true),
                            new SubRule(new Action(Action.Operation.SetNewITS, "32"))
                        },
                        new Shift(1, 1, "0.00:00:00", "6.23:59:59"))
                    }
                }

            }.Create();

            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var console = new AutomaticConsoleController(context, person, survey);
            console.Login();
            var interview = console.StartInterview();
            
            Assert.AreEqual("S1.I1", interview.Tag);

            ServiceLocator.Resolve<ICallQueueService>().ExpireAllCalls();

            var call = context.GetCall("S1.I2");
            interview = context.GetInterview("S1.I2");

            call.Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled && x.Resource == person.Id && x.TimeToExpire == DateTime.Parse("2018-03-27T14:10:00"));
            interview.Assert.IsTrue(x => x.TransientState == (int)CallOutcome.Appointment);

            Assert.AreEqual(0, BvCallHistoryExAdapter.GetAll().Where(x => x.SurveyId == interview.Survey.Id && x.InterviewID == interview.Id).Count());
        }

        [Theory, Owner(@"FIRM\grigoryk")]
        [ClassData(typeof(TestDataGenerator))]
        public void IsCallExpiredWithResourceLoggedIn_CallNotExpiredAndHistoryRecorIsLogged(SecurityMode mode)
        {
            SetSecurityMode(mode);

            new DateTimeMocker("2018-03-27T14:15:00");

            var context = new TestData
            {
                Surveys = new[]{
                    new SurveyData {Tag = "S1", Assigns = new [] {"P1"}, SchedulingScript = "SS1", IsUseDb = true,
                        Interviews = new [] {
                            new InterviewData { Tag="S1.I1", Call = new CallData { Priority = 10} },
                            new InterviewData { Tag="S1.I2", ITS = CallOutcome.Appointment, Call = new CallData { Resource = "P1", TimeToExpire = DateTime.Parse("2018-03-27T14:10:00") }}
                        }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] {
                    new ScriptData() { Tag="SS1", Script = new TestScript(new [] {
                            new SubRule(new []{ new Action(Action.Operation.RestorePreviousCallState), 
                                                new Action(Action.Operation.IncrementPriority, "1")})
                            {
                                ItsId = (int)CallOutcome.Appointment,  Filter = "IsCallExpiredWithResourceLoggedIn(10)", FilterEnabled = true
                            },
                            new SubRule(new Action(Action.Operation.SetNewITS, "31"), (int)CallOutcome.Appointment, 0, 0, "IsCallExpired()", true),
                            new SubRule(new Action(Action.Operation.SetNewITS, "32"))
                        },
                        new Shift(1, 1, "0.00:00:00", "6.23:59:59"))
                    }
                }
            }.Create();

            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var console = new AutomaticConsoleController(context, person, survey);
            console.Login();
            var interview = console.StartInterview();

            Assert.AreEqual("S1.I1", interview.Tag);

            ServiceLocator.Resolve<ICallQueueService>().ExpireAllCalls();

            var call = context.GetCall("S1.I2");
            interview = context.GetInterview("S1.I2");

            call.Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled && x.Resource == person.Id && x.Priority == 2 && x.TimeToExpire == DateTime.Parse("2018-03-27T14:10:00"));
            interview.Assert.IsTrue(x => x.TransientState == (int)CallOutcome.Appointment);

            Assert.AreEqual(1, BvCallHistoryExAdapter.GetAll().Where(x => x.SurveyId == interview.Survey.Id && x.InterviewID == interview.Id).Count());
        }

        [Theory, Owner(@"FIRM\grigoryk")]
        [ClassData(typeof(TestDataGenerator))]
        public void IsCallExpiredWithResourceLoggedIn_ResourceIsLoggedIn_CallIsDeliveredAfterAttemptToExpire(SecurityMode mode)
        {
            SetSecurityMode(mode);

            new DateTimeMocker("2018-03-27T14:15:00");

            var context = new TestData
            {
                Surveys = new[]{
                    new SurveyData {Tag = "S1", Assigns = new [] {"P1"}, SchedulingScript = "SS1", IsUseDb = true,
                        Interviews = new [] {
                            new InterviewData { Tag="S1.I1", Call = new CallData { Priority = 10} },
                            new InterviewData { Tag="S1.I2", Call = new CallData() },
                            new InterviewData { Tag="S1.I3", ITS = CallOutcome.Appointment, Call = new CallData {Resource = "P1", TimeToExpire = DateTime.Parse("2018-03-27T14:10:00")}}
                        }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] {
                    new ScriptData { Tag="SS1", Script = new TestScript(new [] {
                            new SubRule(new Action(Action.Operation.RestorePreviousCallState), (int)CallOutcome.Appointment, 0, 0, "IsCallExpiredWithResourceLoggedIn(10)", true),
                            new SubRule(new Action(Action.Operation.SetNewITS, "31"), (int)CallOutcome.Appointment, 0, 0, "IsCallExpired()", true),
                            new SubRule(new Action(Action.Operation.SetNewITS, "32"))
                        },
                        new Shift(1, 1, "0.00:00:00", "6.23:59:59"))
                    }
                }

            }.Create();

            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var console = new AutomaticConsoleController(context, person, survey);
            console.Login();
            var interview = console.StartInterview();

            Assert.AreEqual("S1.I1", interview.Tag);

            ServiceLocator.Resolve<ICallQueueService>().ExpireAllCalls();

            interview = console.NextInterview(interview, null);

            Assert.AreEqual("S1.I3", interview.Tag);
        }

        [Theory, Owner(@"FIRM\grigoryk")]
        [ClassData(typeof(TestDataGenerator))]
        public void IsCallExpiredWithResourceLoggedIn_ResourceIsLoggedIn_TwoCallsAreDeliveredAfterAttemptToExpire(SecurityMode mode)
        {
            SetSecurityMode(mode);

            new DateTimeMocker("2018-03-27T14:15:00");

            var context = new TestData
            {
                Surveys = new[]{
                    new SurveyData {Tag = "S1", Assigns = new [] {"P1"}, SchedulingScript = "SS1", IsUseDb = true,
                        Interviews = new [] {
                            new InterviewData { Tag="S1.I1", Call = new CallData { Priority = 10} },
                            new InterviewData { Tag="S1.I2", Call = new CallData() },
                            new InterviewData { Tag="S1.I3", ITS = CallOutcome.Appointment, Call = new CallData {Resource = "P1", TimeToExpire = DateTime.Parse("2018-03-27T14:12:00")}},
                            new InterviewData { Tag="S1.I4", ITS = CallOutcome.Appointment, Call = new CallData {Resource = "P1", TimeToExpire = DateTime.Parse("2018-03-27T14:10:00")}}
                        }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] {
                    new ScriptData { Tag="SS1", Script = new TestScript(new [] {
                            new SubRule(new List<Action> { new Action(Action.Operation.RestorePreviousCallState), new Action(Action.Operation.IncrementPriority, "1") }, (int)CallOutcome.Appointment, 0, 0, "IsCallExpiredWithResourceLoggedIn(10)", true),
                            new SubRule(new Action(Action.Operation.SetNewITS, "31"), (int)CallOutcome.Appointment, 0, 0, "IsCallExpired()", true),
                            new SubRule(new Action(Action.Operation.SetNewITS, "32"))
                        },
                        new Shift(1, 1, "0.00:00:00", "6.23:59:59"))
                    }
                }

            }.Create();

            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var console = new AutomaticConsoleController(context, person, survey);
            console.Login();
            var interview = console.StartInterview();

            Assert.AreEqual("S1.I1", interview.Tag);

            ServiceLocator.Resolve<ICallQueueService>().ExpireAllCalls();

            var calls = context.GetCalls("S1.I3", "S1.I4");

            Assert.AreEqual(2, calls.Count());

            calls.Assert.IsTrue(x => x.Priority == 2);
        }


        [Theory, Owner(@"FIRM\grigoryk")]
        [ClassData(typeof(TestDataGenerator))]
        public void IsCallExpiredWithResourceLoggedIn_TwoResourcesAreLoggedIn_TheSecondResourceGetNotAssignedInterview(SecurityMode mode)
        {
            SetSecurityMode(mode);

            new DateTimeMocker("2018-03-27T14:15:00");

            var context = new TestData
            {
                Surveys = new[]{
                    new SurveyData {Tag = "S1", Assigns = new [] {"P1", "P2"}, SchedulingScript = "SS1", IsUseDb = true,
                        Interviews = new [] {
                            new InterviewData { Tag="S1.I1", Call = new CallData { Priority = 10} },
                            new InterviewData { Tag="S1.I2", ITS = CallOutcome.Appointment, Call = new CallData {Resource = "P1", TimeToExpire = DateTime.Parse("2018-03-27T14:10:00")},},
                            new InterviewData { Tag="S1.I3", Call = new CallData() }
                        }
                    }
                },
                Persons = new[] {
                    new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment },
                    new PersonData { Tag = "P2", TaskChoice = TaskChoiceMode.SurveyAssignment }
                },
                Scripts = new[] {
                    new ScriptData { Tag="SS1", Script = new TestScript(new [] {
                            new SubRule(new Action(Action.Operation.RestorePreviousCallState), (int)CallOutcome.Appointment, 0, 0, "IsCallExpiredWithResourceLoggedIn(10)", true),
                            new SubRule(new Action(Action.Operation.SetNewITS, "31"), (int)CallOutcome.Appointment, 0, 0, "IsCallExpired()", true),
                            new SubRule(new Action(Action.Operation.SetNewITS, "32"))
                        },
                        new Shift(1, 1, "0.00:00:00", "6.23:59:59"))
                    }
                }

            }.Create();

            var person1 = context.GetPerson("P1");
            var person2 = context.GetPerson("P2");
            var survey = context.GetSurvey("S1");

            var console1 = new AutomaticConsoleController(context, person1, survey);
            console1.Login();
            var interview1 = console1.StartInterview();

            Assert.AreEqual("S1.I1", interview1.Tag);

            ServiceLocator.Resolve<ICallQueueService>().ExpireAllCalls();

            var console2 = new AutomaticConsoleController(context, person2, survey);
            console2.Login();
            var interview2 = console2.StartInterview();

            Assert.AreEqual("S1.I3", interview2.Tag);
        }

        [Theory, Owner(@"FIRM\grigoryk")]
        [ClassData(typeof(TestDataGenerator))]
        public void IsCallExpiredWithResourceLoggedIn_TwoResourcesAreLoggedIn_TheSecondResourceDoNotGetExpiredInterviewAssignedToTheFirstRecource(SecurityMode mode)
        {
            SetSecurityMode(mode);

            new DateTimeMocker("2018-03-27T14:15:00");

            var context = new TestData
            {
                Surveys = new[]{
                    new SurveyData {Tag = "S1", Assigns = new [] {"P1", "P2"}, SchedulingScript = "SS1", IsUseDb = true,
                        Interviews = new [] {
                            new InterviewData { Tag="S1.I1", Call = new CallData { Priority = 10} },
                            new InterviewData { Tag="S1.I2", ITS = CallOutcome.Appointment, Call = new CallData {Resource = "P1", TimeToExpire = DateTime.Parse("2018-03-27T14:10:00")},}
                        }
                    }
                },
                Persons = new[] {
                    new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment },
                    new PersonData { Tag = "P2", TaskChoice = TaskChoiceMode.SurveyAssignment }
                },
                Scripts = new[] {
                    new ScriptData { Tag="SS1", Script = new TestScript(new [] {
                            new SubRule(new Action(Action.Operation.RestorePreviousCallState), (int)CallOutcome.Appointment, 0, 0, "IsCallExpiredWithResourceLoggedIn(10)", true),
                            new SubRule(new Action(Action.Operation.SetNewITS, "31"), (int)CallOutcome.Appointment, 0, 0, "IsCallExpired()", true),
                            new SubRule(new Action(Action.Operation.SetNewITS, "32"))
                        },
                        new Shift(1, 1, "0.00:00:00", "6.23:59:59"))
                    }
                }

            }.Create();

            var person1 = context.GetPerson("P1");
            var person2 = context.GetPerson("P2");
            var survey = context.GetSurvey("S1");

            var console1 = new AutomaticConsoleController(context, person1, survey);
            console1.Login();
            var interview1 = console1.StartInterview();

            Assert.AreEqual("S1.I1", interview1.Tag);

            ServiceLocator.Resolve<ICallQueueService>().ExpireAllCalls();

            var console2 = new AutomaticConsoleController(context, person2, survey);
            console2.Login();
            var interview2 = console2.StartInterview();

            Assert.IsNull(interview2);
        }
    }
}
