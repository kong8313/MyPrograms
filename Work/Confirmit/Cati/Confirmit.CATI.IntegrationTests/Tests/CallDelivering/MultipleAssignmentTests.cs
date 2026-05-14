using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Validators.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework.Controllers;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using PersonTools = BvCallHandlerLibrary.Tools.PersonTools;

namespace Confirmit.CATI.IntegrationTests.Tests.CallDelivering
{
    [TestClass]
    public class MultipleAssignmentTests : BaseMockedIntegrationTest
    {
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void CallDelivery_CallWithMultipleAssignment_CallIsnotDeliveredToPerson()
        {
            var testContext = new TestData
            {
                Surveys = new []
                {
                    new SurveyData
                    {
                        Tag = "S1", IsOpen = true,
                        Interviews = new []
                        {
                            new InterviewData
                            {
                                Tag = "S1.I1",
                                Call = new CallData { Resource = "PG1,PG2" }
                            } 
                        }
                    }
                },
                PersonGroups = new []
                {
                    new PersonGroupData {Tag="PG1", Name = "PersonGroup1"},
                    new PersonGroupData {Tag="PG2", Name = "PersonGroup2"}
                },
                Persons = new[]
                {
                    new PersonData { Tag = "P1", Memberships = "PG1"}
                }
            }.Create();

            var interviewer = testContext.GetPerson("P1");

            var console = new AutomaticConsoleController(testContext, interviewer, null);
            console.Login();

            InterviewController interview = console.StartInterview();
            Assert.IsNull(interview);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void CallDelivery_CallWithMultipleAssignment_CallIsDeliveredToPerson()
        {
            var testContext = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1", IsOpen = true,
                        Interviews = new []
                        {
                            new InterviewData
                            {
                                Tag = "S1.I1",
                                Call = new CallData { Resource = "PG1,PG2" }
                            } 
                        }
                    }
                },
                PersonGroups = new[]
                {
                    new PersonGroupData {Tag="PG1", Name = "PersonGroup1"},
                    new PersonGroupData {Tag="PG2", Name = "PersonGroup2"}
                },
                Persons = new[]
                {
                    new PersonData { Tag = "P1", Memberships = "PG1,PG2"}
                }
            }.Create();

            var interviewer = testContext.GetPerson("P1");

            var console = new AutomaticConsoleController(testContext, interviewer, null);
            console.Login();

            InterviewController interview = console.StartInterview();
            Assert.IsNotNull(interview);
        }


        [TestMethod, Owner(@"FIRM\MaximL")]
        public void CallDelivery_AssignCallToGroups_CallIsDeliveredToPerson()
        {
            var testContext = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        IsOpen = true,
                        Interviews = new []
                        {
                            new InterviewData
                            {
                                Tag = "S1.I1",
                                Call = new CallData { Resource = "PG1" }
                            }
                        }
                    }
                },
                PersonGroups = new[]
                {
                    new PersonGroupData {Tag="PG1", Name = "PersonGroup1"},
                    new PersonGroupData {Tag="PG2", Name = "PersonGroup2"}
                },
                Persons = new[]
                {
                    new PersonData { Tag = "P1", Memberships = "PG1,PG2"}
                }
            }.Create();

            var interviewer = testContext.GetPerson("P1");
            var survey = testContext.GetSurvey("S1");
            var firstGroup = testContext.GetResource("PG1");
            var secondGroup = testContext.GetResource("PG2");
            var call = testContext.GetCall("S1.I1");

            CallTools.AssignCalls(survey.Id, new[] { call.Id }, new[] { firstGroup.Id, secondGroup.Id });

            var console = new AutomaticConsoleController(testContext, interviewer, null);
            console.Login();

            InterviewController interview = console.StartInterview();
            Assert.IsNotNull(interview);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void CallDelivery_DeassignCallFromGroupsSingleAssignment_CallIsDeliveredToPerson()
        {
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        IsOpen = true,
                        Interviews = new[]
                        {
                            new InterviewData
                            {
                                Tag = "S1.I1",
                                Call = new CallData { Resource = "PG1,PG2" }
                            }
                        }
                    }
                },
                PersonGroups = new[]
                {
                    new PersonGroupData {Tag = "PG1", Name = "PersonGroup1"},
                    new PersonGroupData {Tag = "PG2", Name = "PersonGroup2"}
                },
                Persons = new[]
                {
                    new PersonData { Tag = "P1", Memberships = "PG1"}
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var call = context.GetCall("S1.I1");

            var group1 = context.GetResource("PG1");

            CallTools.AssignCalls(survey.Id, new[] { call.Id }, new[] { group1.Id });

            var interviewer = context.GetPerson("P1");

            var console = new AutomaticConsoleController(context, interviewer, null);
            console.Login();

            InterviewController interview = console.StartInterview();
            Assert.IsNotNull(interview);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void CallDelivery_DeassignCallFromGroupsMultipleAssignment_CallIsDeliveredToPerson()
        {
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        IsOpen = true,
                        Interviews = new[]
                        {
                            new InterviewData
                            {
                                Tag = "S1.I1",
                                Call = new CallData { Resource = "PG1,PG2,PG3" }
                            }
                        }
                    }
                },
                PersonGroups = new[]
                {
                    new PersonGroupData {Tag = "PG1", Name = "PersonGroup1"},
                    new PersonGroupData {Tag = "PG2", Name = "PersonGroup2"},
                    new PersonGroupData {Tag = "PG3", Name = "PersonGroup3"}
                },
                Persons = new[]
                {
                    new PersonData { Tag = "P1", Memberships = "PG1,PG2"}
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var call = context.GetInterview("S1.I1");

            var group1 = context.GetResource("PG1");
            var group2 = context.GetResource("PG2");

            CallTools.AssignCalls(survey.Id, new[] { call.Id }, new[] { group1.Id, group2.Id });

            var interviewer = context.GetPerson("P1");

            var console = new AutomaticConsoleController(context, interviewer, null);
            console.Login();

            InterviewController interview = console.StartInterview();
            Assert.IsNotNull(interview);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetUserGroups_AssignUserOnGroup_MultiAssignmentsIsSend()
        {
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        IsOpen = true,
                        Interviews = new[]
                        {
                            new InterviewData
                            {
                                Tag = "S1.I1",
                                Call = new CallData { Resource = "PG1,PG2,PG3" }
                            }
                        }
                    }
                },
                PersonGroups = new[]
                {
                    new PersonGroupData {Tag = "PG1", Name = "PersonGroup1"},
                    new PersonGroupData {Tag = "PG2", Name = "PersonGroup2"},
                    new PersonGroupData {Tag = "PG3", Name = "PersonGroup3"}
                },
                Persons = new[]
                {
                    new PersonData { Tag = "P1", Memberships = "PG1,PG2"}
                }
            }.Create();

            var person = context.GetPerson("P1");

            var expected= context.GetResources("PG1", "PG2").Select(x => x.Id).OrderBy(o => o).ToArray();
            var actual = PersonTools.GetUserGroups(person.Id).OrderBy(o => o).ToArray();
            
            CollectionAssert.AreEquivalent(expected, actual);

            var groups = context.GetResources("PG1", "PG2", "PG3").Select(x => x.Id).OrderBy(o => o).ToArray();
            PersonService.SetParentGroups(person.Id, groups);

            var multipleAssignment = BvAssignmentResourceAdapter.GetAll().Select(x => x.ID).Single();

            expected = groups.Union(new []{multipleAssignment}).OrderBy(o => o).ToArray();
            actual = PersonTools.GetUserGroups(person.Id).OrderBy(o => o).ToArray();
            
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetUserGroups_DeassignUserOnGroup_MultiAssignmentsIsSend()
        {
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        IsOpen = true,
                        Interviews = new[]
                        {
                            new InterviewData
                            {
                                Tag = "S1.I1",
                                Call = new CallData { Resource = "PG1,PG2,PG3" }
                            }
                        }
                    }
                },
                PersonGroups = new[]
                {
                    new PersonGroupData {Tag = "PG1", Name = "PersonGroup1"},
                    new PersonGroupData {Tag = "PG2", Name = "PersonGroup2"},
                    new PersonGroupData {Tag = "PG3", Name = "PersonGroup3"}
                },
                Persons = new[]
                {
                    new PersonData { Tag = "P1", Memberships = "PG1,PG2,PG3"}
                }
            }.Create();

            var person = context.GetPerson("P1");

            var groups = context.GetResources("PG1", "PG2", "PG3").Select(x => x.Id).OrderBy(o => o).ToArray();
            var multipleAssignment = BvAssignmentResourceAdapter.GetAll().Select(x => x.ID).Single();

            var expected = groups.Union(new[] { multipleAssignment }).OrderBy(o => o).ToArray();
            var actual = PersonTools.GetUserGroups(person.Id).OrderBy(o => o).ToArray();

            CollectionAssert.AreEquivalent(expected, actual);

            expected = context.GetResources("PG1", "PG2").Select(x => x.Id).OrderBy(o => o).ToArray();
            
            PersonService.SetParentGroups(person.Id, expected);

            actual = PersonTools.GetUserGroups(person.Id).OrderBy(o => o).ToArray();


            CollectionAssert.AreEquivalent(expected, actual);

        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetUserGroups_ActivateCall_MultiAssignmentsIsSend()
        {
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        IsOpen = true,
                        Interviews = new[]
                        {
                            new InterviewData
                            {
                                Tag = "S1.I1",
                                Call = new CallData()
                            }
                        }
                    }
                },
                PersonGroups = new[]
                {
                    new PersonGroupData {Tag = "PG1", Name = "PersonGroup1"},
                    new PersonGroupData {Tag = "PG2", Name = "PersonGroup2"},
                    new PersonGroupData {Tag = "PG3", Name = "PersonGroup3"}
                },
                Persons = new[]
                {
                    new PersonData { Tag = "P1", Memberships = "PG1,PG2,PG3"}
                }
            }.Create();

            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");
            var interview = context.GetInterview("S1.I1");

            var groups = context.GetResources("PG1", "PG2", "PG3").Select(x => x.Id).OrderBy(o => o).ToArray();
            var actual = PersonTools.GetUserGroups(person.Id).OrderBy(o => o).ToArray();

            CollectionAssert.AreEquivalent(groups, actual);

            CallTools.ActivateCalls(survey.Id, 1, CallStates.All, groups, (int) CallShiftType.None, null, true,
                new[] {interview.Id});

            var multipleAssignment = BvAssignmentResourceAdapter.GetAll().Select(x => x.ID).Single();

            var expected = groups.Union(new[] { multipleAssignment }).OrderBy(o => o).ToArray();
            actual = PersonTools.GetUserGroups(person.Id).OrderBy(o => o).ToArray();

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod, Owner(@"FIRM\vyacheslavb")]
        public void ValidationGroupForMultipleAssignment_GroupInMultipleAssignment_ValidationSuccesed()
        {
            var testContext = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        IsOpen = true,
                        Interviews = new[]
                        {
                            new InterviewData
                            {
                                Tag = "S1.I1",
                                Call = new CallData {Resource = "PG1,PG2"}
                            }
                        }
                    }
                },
                PersonGroups = new[]
                {
                    new PersonGroupData {Tag = "PG1", Name = "PersonGroup1"},
                    new PersonGroupData {Tag = "PG2", Name = "PersonGroup2"}
                }
            }.Create();

            var firstGroup = testContext.GetResource("PG1");
            var secondGroup = testContext.GetResource("PG2");

            Assert.IsTrue(ServiceLocator.Resolve<IMultipleAssignmentValidator>().IsMultipleAssignmentGroup(firstGroup.Id));
            Assert.IsTrue(ServiceLocator.Resolve<IMultipleAssignmentValidator>().IsMultipleAssignmentGroup(secondGroup.Id));
        }

    }
}
