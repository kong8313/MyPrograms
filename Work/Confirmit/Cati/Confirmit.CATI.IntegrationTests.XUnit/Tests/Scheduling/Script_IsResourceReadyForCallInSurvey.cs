using System;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public sealed class ScriptIsResourceReadyForCallInSurvey : BaseMockedIntegrationTest
    {
        [Theory, Owner(@"FIRM\grigoryk")]
        [ClassData(typeof(TestDataGenerator))]
        public void IsResourceReadyForCallInSurvey_PersonNotLoggedIn_SubruleIsNotExecuted(SecurityMode mode)
        {
            SetSecurityMode(mode);

            new DateTimeMocker("2018-03-27T14:15:00");
            var notExistedPersonOrGroupId = 123;
            
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
                            new SubRule(new Action(Action.Operation.SetNewITS, "31"), (int)CallOutcome.Appointment, 0, 0, $"IsResourceReadyForCallInSurvey({notExistedPersonOrGroupId})", true),
                            new SubRule(new Action(Action.Operation.SetNewITS, "32"))
                        },
                        new Shift(1, 1, "0.00:00:00", "6.23:59:59"))
                    }
                }

            }.Create();

            ServiceLocator.Resolve<ICallQueueService>().ExpireAllCalls();
            
            var interview = context.GetInterview("S1.I1");
            interview.Assert.IsTrue(x => x.TransientState == 32);
        }

        [Theory, Owner(@"FIRM\grigoryk")]
        [ClassData(typeof(TestDataGenerator))]
        public void IsResourceReadyForCallInSurvey_PersonIsLoggedInOrNoCalls_SubruleIsExecuted(SecurityMode mode)
        {
            SetSecurityMode(mode);

            new DateTimeMocker("2018-03-27T14:15:00");
            var personId = 35;
                
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
                            new SubRule(new Action(Action.Operation.SetNewITS, "31"), (int)CallOutcome.Appointment, 0, 0, $"IsResourceReadyForCallInSurvey({personId})", true),
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
            
            ServiceLocator.Resolve<ICallQueueService>().ExpireAllCalls();
            
            var interview = context.GetInterview("S1.I1");
            interview.Assert.IsTrue(x => x.TransientState == 31);
            
            console.StartInterview();
            
            ServiceLocator.Resolve<ICallQueueService>().ExpireAllCalls();
            
            interview = context.GetInterview("S1.I1");
            interview.Assert.IsTrue(x => x.TransientState == 31);
        }
        
        [Theory, Owner(@"FIRM\grigoryk")]
        [ClassData(typeof(TestDataGenerator))]
        public void IsResourceReadyForCallInSurvey_PersonIsInterviewing_SubruleIsNotExecuted(SecurityMode mode)
        {
            SetSecurityMode(mode);

            new DateTimeMocker("2018-03-27T14:15:00");
            var personId = 35;
                
            var context = new TestData
            {
                Surveys = new[]{
                    new SurveyData {Tag = "S1", Assigns = new [] {"P1"}, SchedulingScript = "SS1", IsUseDb = true,
                        Interviews = new [] {
                            new InterviewData { Tag="S1.I1", Call = new CallData { Priority = 10} },
                            new InterviewData { Tag="S1.I2", ITS = CallOutcome.Appointment, Call = new CallData {Resource = "P1", TimeToExpire = DateTime.Parse("2018-03-27T14:10:00")}}
                        }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] {
                    new ScriptData { Tag="SS1", Script = new TestScript(new [] {
                            new SubRule(new Action(Action.Operation.SetNewITS, "31"), (int)CallOutcome.Appointment, 0, 0, $"IsResourceReadyForCallInSurvey({personId})", true),
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
            
            interview = context.GetInterview("S1.I2");
            interview.Assert.IsTrue(x => x.TransientState == 32);
        }
        
        [Theory, Owner(@"FIRM\grigoryk")]
        [ClassData(typeof(TestDataGenerator))]
        public void IsResourceReadyForCallInSurvey_NoLoggedPersonsInPersonGroup_SubruleIsNotExecuted(SecurityMode mode)
        {
            SetSecurityMode(mode);

            new DateTimeMocker("2018-03-27T14:15:00");
            var groupId = 37;
            
            var context = new TestData
            {
                Surveys = new[]{
                    new SurveyData {Tag = "S1", Assigns = new [] {"P1"}, SchedulingScript = "SS1", IsUseDb = true,
                        Interviews = new [] {
                            new InterviewData { Tag="S1.I1", ITS = CallOutcome.Appointment, Call = new CallData {Resource = "P1", TimeToExpire = DateTime.Parse("2018-03-27T14:10:00")}}
                        }
                    }
                },
                PersonGroups = new [] { new PersonGroupData { Tag = "PG1" } },
                Persons = new[] { new PersonData { Tag = "P1" }, new PersonData { Tag = "P2" } },
                Scripts = new[] {
                    new ScriptData { Tag="SS1", Script = new TestScript(new [] {
                            new SubRule(new Action(Action.Operation.SetNewITS, "31"), (int)CallOutcome.Appointment, 0, 0, $"IsResourceReadyForCallInSurvey({groupId})", true),
                            new SubRule(new Action(Action.Operation.SetNewITS, "32"))
                        },
                        new Shift(1, 1, "0.00:00:00", "6.23:59:59"))
                    }
                }

            }.Create();

            ServiceLocator.Resolve<ICallQueueService>().ExpireAllCalls();
            
            var interview = context.GetInterview("S1.I1");
            interview.Assert.IsTrue(x => x.TransientState == 32);
        }
        
        [Theory, Owner(@"FIRM\grigoryk")]
        [ClassData(typeof(TestDataGenerator))]
        public void IsResourceReadyForCallInSurvey_OneLoggedPersonInPersonGroup_SubruleIsExecuted(SecurityMode mode)
        {
            SetSecurityMode(mode);

            new DateTimeMocker("2018-03-27T14:15:00");
            var groupId = 38;
            
            var context = new TestData
            {
                Surveys = new[] {
                    new SurveyData {Tag = "S1", Assigns = new [] {"P1"}, SchedulingScript = "SS1", IsUseDb = true,
                        Interviews = new [] {
                            new InterviewData { Tag="S1.I1", ITS = CallOutcome.Appointment, Call = new CallData {Resource = "P1", TimeToExpire = DateTime.Parse("2018-03-27T14:10:00")}}
                        }
                    }
                },
                PersonGroups = new [] { new PersonGroupData { Tag = "PG1" } },
                Persons = new[] { 
                    new PersonData { Tag = "P1", Memberships = "PG1" }, 
                    new PersonData { Tag = "P2", Memberships = "PG1" }, 
                    new PersonData { Tag = "P3", Memberships = "PG1" } 
                },
                Scripts = new[] {
                    new ScriptData { Tag="SS1", Script = new TestScript(new [] {
                            new SubRule(new Action(Action.Operation.SetNewITS, "31"), (int)CallOutcome.Appointment, 0, 0, $"IsResourceReadyForCallInSurvey({groupId})", true),
                            new SubRule(new Action(Action.Operation.SetNewITS, "32"))
                        },
                        new Shift(1, 1, "0.00:00:00", "6.23:59:59"))
                    }
                }

            }.Create();

            var person = context.GetPerson("P3");
            var survey = context.GetSurvey("S1");
            var console = new AutomaticConsoleController(context, person, survey);
            console.Login();
            
            ServiceLocator.Resolve<ICallQueueService>().ExpireAllCalls();
            
            var interview = context.GetInterview("S1.I1");
            interview.Assert.IsTrue(x => x.TransientState == 31);
        }
        
        [Theory, Owner(@"FIRM\grigoryk")]
        [ClassData(typeof(TestDataGenerator))]
        public void IsResourceReadyForCallInSurvey_OneLoggedPersonInDifferentPersonGroup_SubruleIsNotExecuted(SecurityMode mode)
        {
            SetSecurityMode(mode);

            new DateTimeMocker("2018-03-27T14:15:00");
            var group2Id = 40;
            
            var context = new TestData
            {
                Surveys = new[] {
                    new SurveyData {Tag = "S1", Assigns = new [] {"P1"}, SchedulingScript = "SS1", IsUseDb = true,
                        Interviews = new [] {
                            new InterviewData { Tag="S1.I1", ITS = CallOutcome.Appointment, Call = new CallData {Resource = "P1", TimeToExpire = DateTime.Parse("2018-03-27T14:10:00")}}
                        }
                    }
                },
                PersonGroups = new [] { new PersonGroupData { Tag = "PG1" }, new PersonGroupData { Tag = "PG2" } },
                Persons = new[] { 
                    new PersonData { Tag = "P1", Memberships = "PG1" }, 
                    new PersonData { Tag = "P2", Memberships = "PG1" }, 
                    new PersonData { Tag = "P3", Memberships = "PG1" },
                    new PersonData { Tag = "P4", Memberships = "PG2" }
                },
                Scripts = new[] {
                    new ScriptData { Tag="SS1", Script = new TestScript(new [] {
                            new SubRule(new Action(Action.Operation.SetNewITS, "31"), (int)CallOutcome.Appointment, 0, 0, $"IsResourceReadyForCallInSurvey({group2Id})", true),
                            new SubRule(new Action(Action.Operation.SetNewITS, "32"))
                        },
                        new Shift(1, 1, "0.00:00:00", "6.23:59:59"))
                    }
                }

            }.Create();

            var person = context.GetPerson("P3");
            var survey = context.GetSurvey("S1");
            var console = new AutomaticConsoleController(context, person, survey);
            console.Login();
            
            ServiceLocator.Resolve<ICallQueueService>().ExpireAllCalls();
            
            var interview = context.GetInterview("S1.I1");
            interview.Assert.IsTrue(x => x.TransientState == 32);
        }
        
        [Theory, Owner(@"FIRM\grigoryk")]
        [ClassData(typeof(TestDataGenerator))]
        public void IsResourceReadyForCallInSurvey_SurveySelectionPersonIsLoggedIntoDifferentSurvey_SubruleIsNotExecuted(SecurityMode mode)
        {
            SetSecurityMode(mode);

            new DateTimeMocker("2018-03-27T14:15:00");
            var person2Id = 36;
                
            var context = new TestData
            {
                Surveys = new[]{
                    new SurveyData {Tag = "S1", Assigns = new [] {"P1"}, SchedulingScript = "SS1", IsUseDb = true,
                        Interviews = new [] {
                            new InterviewData { Tag="S1.I1", ITS = CallOutcome.Appointment, Call = new CallData {Resource = "P1", TimeToExpire = DateTime.Parse("2018-03-27T14:10:00")}}
                        }
                    },
                    new SurveyData {Tag = "S2", Assigns = new [] {"P2"}, SchedulingScript = "SS1", IsUseDb = true,
                        Interviews = new [] { new InterviewData { Tag="S2.I1" } }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1" }, new PersonData { Tag = "P2", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] {
                    new ScriptData { Tag="SS1", Script = new TestScript(new [] {
                            new SubRule(new Action(Action.Operation.SetNewITS, "31"), (int)CallOutcome.Appointment, 0, 0, $"IsResourceReadyForCallInSurvey({person2Id})", true),
                            new SubRule(new Action(Action.Operation.SetNewITS, "32"))
                        },
                        new Shift(1, 1, "0.00:00:00", "6.23:59:59"))
                    }
                }

            }.Create();

            var person = context.GetPerson("P2");
            var survey = context.GetSurvey("S2");
            var console = new AutomaticConsoleController(context, person, survey);
            console.Login();
            
            ServiceLocator.Resolve<ICallQueueService>().ExpireAllCalls();
            
            var interview = context.GetInterview("S1.I1");
            interview.Assert.IsTrue(x => x.TransientState == 32);
        }
    }
}
