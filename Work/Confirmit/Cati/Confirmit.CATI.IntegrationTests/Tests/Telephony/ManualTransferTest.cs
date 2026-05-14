using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.Telephony
{
    [TestClass]
    public class ManualTransferTest : BaseMockedIntegrationTest
    {
        [TestMethod, Owner(@"Firm\MaximL")]
        public void GetInternalTransferTargets_SeveralPersonGroupsWithDifferentTransferConfiguration_ResultIsCorrect()
        {
            var context = new TestData()
            {
                Surveys = new[] {new SurveyData{Tag = "S1", AssignsS = "P1",
                    Interviews = new [] { new InterviewData(){ Tag = "S1.I1", Call = new CallData() }
                }}},
                Persons = new[] {new PersonData() { Tag="P1", TaskChoice = TaskChoiceMode.SurveyAssignment} },
                PersonGroups = new[]
                {
                    new PersonGroupData() { Tag = "PG.No", Name ="PG.No.Name", Description="PG.No.Description"},
                    new PersonGroupData() { Tag = "PG.Same", Name ="PG.Same.Name", Description="PG.Same.Description", TransferBehavior = TransferGroupBehavior.DeliverCallsFromTheSameSurvey},
                    new PersonGroupData() { Tag = "PG.Cross", Name ="PG.Cross.Name", Description="PG.Cross.Description", TransferBehavior = TransferGroupBehavior.DeliverCallsFromOtherSurvey}
                }
            }.Create();

            var actual = context.GetPerson("P1").Login("S1").Start().Wait().GetInternalTransferTargets();

            Assert.AreEqual(
                BackendTools.Format(actual), @"
          Name          Description CountOfTotalInterviewersLoggedIn CountOfFreeInterviewersLoggedIn
  PG.Same.Name  PG.Same.Description                                0                               0
 PG.Cross.Name PG.Cross.Description                                0                               0");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void GetInternalTransferTargets_SeveralPersonGroupsWithDifferentLoggedPersons_ResultIsCorrect()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData{ Tag = "S1", AssignsS = "P.Initiator,PG.None,PG.Same,PG.Cross",
                        Interviews = new [] { new InterviewData(){ Tag = "S1.I1", Call = new CallData() }}},
                    new SurveyData{ Tag = "S2", AssignsS = "PG.None,PG.Same,PG.Cross",
                        Interviews = new [] { new InterviewData(){ Tag = "S2.I1", Call = new CallData() }}},
                },
                Persons = new[]
                {
                    new PersonData() { Tag = "P.Initiator", TaskChoice = TaskChoiceMode.SurveyAssignment },

                    new PersonData() { Tag = "P.Auto.None", TaskChoice = TaskChoiceMode.Automatic, Memberships = "PG.None" },
                    new PersonData() { Tag = "P.Auto.Same", TaskChoice = TaskChoiceMode.Automatic, Memberships = "PG.Same" },// +1(PG.Same)
                    new PersonData() { Tag = "P.Auto.Cross", TaskChoice = TaskChoiceMode.Automatic, Memberships = "PG.Cross" }, // +1(PG.Cross)
                    new PersonData() { Tag = "P.Auto.Both", TaskChoice = TaskChoiceMode.Automatic, Memberships = "PG.Same,PG.Cross" },// +1(PG.Same)  +1(PG.Cross)

                    new PersonData() { Tag = "P.SA.None.S1", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.None"},
                    new PersonData() { Tag = "P.SA.Same.S1", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.Same"},// +1(PG.Same)
                    new PersonData() { Tag = "P.SA.Cross.S1", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.Cross"},// +1(PG.Cross)
                    new PersonData() { Tag = "P.SA.Both.S1", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.Same,PG.Cross"},// +1(PG.Same)  +1(PG.Cross)

                    new PersonData() { Tag = "P.SA.None.S2", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.None"},
                    new PersonData() { Tag = "P.SA.Same.S2", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.Same"},
                    new PersonData() { Tag = "P.SA.Cross.S2", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.Cross"},// +1(PG.Cross)
                    new PersonData() { Tag = "P.SA.Both.S2", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.Same,PG.Cross"},// +1(PG.Cross)
                },
                PersonGroups = new[]
                {
                    new PersonGroupData() { Tag = "PG.None", Name ="PG.None.Name", Description="PG.None.Description"},
                    new PersonGroupData() { Tag = "PG.Same", Name ="PG.Same.Name", Description="PG.Same.Description", TransferBehavior = TransferGroupBehavior.DeliverCallsFromTheSameSurvey},
                    new PersonGroupData() { Tag = "PG.Cross", Name ="PG.Cross.Name", Description="PG.Cross.Description", TransferBehavior = TransferGroupBehavior.DeliverCallsFromOtherSurvey}
                }
            }.Create();


            var console = context.GetPerson("P.Initiator").Login("S1").Start().Wait();

            context.GetPersons("P.Auto.None", "P.Auto.Same", "P.Auto.Cross", "P.Auto.Both").Login().Start().Wait();
            context.GetPersons("P.SA.None.S1", "P.SA.Same.S1","P.SA.Cross.S1","P.SA.Both.S1").Login("S1").Start().Wait();
            context.GetPersons("P.SA.None.S2", "P.SA.Same.S2", "P.SA.Cross.S2", "P.SA.Both.S2").Login("S2").Start().Wait();

            var actual = console.GetInternalTransferTargets();
            Assert.AreEqual(
                BackendTools.Format(actual), @"
          Name          Description CountOfTotalInterviewersLoggedIn CountOfFreeInterviewersLoggedIn
  PG.Same.Name  PG.Same.Description                                4                               4
 PG.Cross.Name PG.Cross.Description                                6                               6");
        }

        [TestMethod, Owner(@"Firm\GrigoryK")]
        public void GetInternalTransferTargets_SeveralPersonGroupsWithDifferentLoggedPersonsAndState_ResultIsCorrect()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData{ Tag = "S1", AssignsS = "P.Initiator,PG.None,PG.Same,PG.Cross",
                        Interviews = new []
                        {
                            new InterviewData(){ Tag = "S1.I1", Call = new CallData() },
                            new InterviewData(){ Tag = "S1.I2", Call = new CallData(){Resource = "P.Auto.Same"} },
                            new InterviewData(){ Tag = "S1.I2", Call = new CallData(){Resource = "P.Auto.Both"} }
                        }},
                    new SurveyData{ Tag = "S2", AssignsS = "PG.None,PG.Same,PG.Cross",
                        Interviews = new [] { new InterviewData(){ Tag = "S2.I1", Call = new CallData() }}},
                },
                Persons = new[]
                {
                    new PersonData() { Tag = "P.Initiator", TaskChoice = TaskChoiceMode.SurveyAssignment },

                    new PersonData() { Tag = "P.Auto.None", TaskChoice = TaskChoiceMode.Automatic, Memberships = "PG.None" },
                    new PersonData() { Tag = "P.Auto.Same", TaskChoice = TaskChoiceMode.Automatic, Memberships = "PG.Same" },// +1(PG.Same)
                    new PersonData() { Tag = "P.Auto.Cross", TaskChoice = TaskChoiceMode.Automatic, Memberships = "PG.Cross" }, // +1(PG.Cross)
                    new PersonData() { Tag = "P.Auto.Both", TaskChoice = TaskChoiceMode.Automatic, Memberships = "PG.Same,PG.Cross" },// +1(PG.Same)  +1(PG.Cross)

                    new PersonData() { Tag = "P.SA.None.S1", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.None"},
                    new PersonData() { Tag = "P.SA.Same.S1", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.Same"},// +1(PG.Same)
                    new PersonData() { Tag = "P.SA.Cross.S1", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.Cross"},// +1(PG.Cross)
                    new PersonData() { Tag = "P.SA.Both.S1", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.Same,PG.Cross"},// +1(PG.Same)  +1(PG.Cross)

                    new PersonData() { Tag = "P.SA.None.S2", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.None"},
                    new PersonData() { Tag = "P.SA.Same.S2", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.Same"},
                    new PersonData() { Tag = "P.SA.Cross.S2", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.Cross"},// +1(PG.Cross)
                    new PersonData() { Tag = "P.SA.Both.S2", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.Same,PG.Cross"},// +1(PG.Cross)
                },
                PersonGroups = new[]
                {
                    new PersonGroupData() { Tag = "PG.None", Name ="PG.None.Name", Description="PG.None.Description"},
                    new PersonGroupData() { Tag = "PG.Same", Name ="PG.Same.Name", Description="PG.Same.Description", TransferBehavior = TransferGroupBehavior.DeliverCallsFromTheSameSurvey},
                    new PersonGroupData() { Tag = "PG.Cross", Name ="PG.Cross.Name", Description="PG.Cross.Description", TransferBehavior = TransferGroupBehavior.DeliverCallsFromOtherSurvey}
                }
            }.Create();

            var console = context.GetPerson("P.Initiator").Login("S1").Start().Wait();

            context.GetPersons("P.Auto.None", "P.Auto.Same", "P.Auto.Cross", "P.Auto.Both").Login().Start().Wait();
            context.GetPersons("P.SA.None.S1", "P.SA.Same.S1", "P.SA.Cross.S1", "P.SA.Both.S1").Login("S1").Start().Wait();
            context.GetPersons("P.SA.None.S2", "P.SA.Same.S2", "P.SA.Cross.S2", "P.SA.Both.S2").Login("S2").Start().Wait();

            var actual = console.GetInternalTransferTargets();
            Assert.AreEqual(
                BackendTools.Format(actual), @"
          Name          Description CountOfTotalInterviewersLoggedIn CountOfFreeInterviewersLoggedIn
  PG.Same.Name  PG.Same.Description                                4                               2
 PG.Cross.Name PG.Cross.Description                                6                               5");
        }

        [TestMethod, Owner(@"Firm\GrigoryK")]
        public void GetInternalTransferTargets_SeveralPersonGroupsWithDifferentLoggedPersonsAndStateAndIncludeIntitor_ResultAreCorrectAndCounterExcludeInitiator()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData{ Tag = "S1", AssignsS = "P.Initiator,PG.None,PG.Same,PG.Cross",
                        Interviews = new []
                        {
                            new InterviewData(){ Tag = "S1.I1", Call = new CallData() },
                            new InterviewData(){ Tag = "S1.I2", Call = new CallData(){Resource = "P.Auto.Same"} },
                            new InterviewData(){ Tag = "S1.I2", Call = new CallData(){Resource = "P.Auto.Both"} }
                        }},
                    new SurveyData{ Tag = "S2", AssignsS = "PG.None,PG.Same,PG.Cross",
                        Interviews = new [] { new InterviewData(){ Tag = "S2.I1", Call = new CallData() }}},
                },
                Persons = new[]
                {
                    new PersonData() { Tag = "P.Initiator", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.None,PG.Same,PG.Cross", },

                    new PersonData() { Tag = "P.Auto.None", TaskChoice = TaskChoiceMode.Automatic, Memberships = "PG.None" },
                    new PersonData() { Tag = "P.Auto.Same", TaskChoice = TaskChoiceMode.Automatic, Memberships = "PG.Same" },// +1(PG.Same)
                    new PersonData() { Tag = "P.Auto.Cross", TaskChoice = TaskChoiceMode.Automatic, Memberships = "PG.Cross" }, // +1(PG.Cross)
                    new PersonData() { Tag = "P.Auto.Both", TaskChoice = TaskChoiceMode.Automatic, Memberships = "PG.Same,PG.Cross" },// +1(PG.Same)  +1(PG.Cross)

                    new PersonData() { Tag = "P.SA.None.S1", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.None"},
                    new PersonData() { Tag = "P.SA.Same.S1", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.Same"},// +1(PG.Same)
                    new PersonData() { Tag = "P.SA.Cross.S1", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.Cross"},// +1(PG.Cross)
                    new PersonData() { Tag = "P.SA.Both.S1", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.Same,PG.Cross"},// +1(PG.Same)  +1(PG.Cross)

                    new PersonData() { Tag = "P.SA.None.S2", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.None"},
                    new PersonData() { Tag = "P.SA.Same.S2", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.Same"},
                    new PersonData() { Tag = "P.SA.Cross.S2", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.Cross"},// +1(PG.Cross)
                    new PersonData() { Tag = "P.SA.Both.S2", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.Same,PG.Cross"},// +1(PG.Cross)
                },
                PersonGroups = new[]
                {
                    new PersonGroupData() { Tag = "PG.None", Name ="PG.None.Name", Description="PG.None.Description"},
                    new PersonGroupData() { Tag = "PG.Same", Name ="PG.Same.Name", Description="PG.Same.Description", TransferBehavior = TransferGroupBehavior.DeliverCallsFromTheSameSurvey},
                    new PersonGroupData() { Tag = "PG.Cross", Name ="PG.Cross.Name", Description="PG.Cross.Description", TransferBehavior = TransferGroupBehavior.DeliverCallsFromOtherSurvey}
                }
            }.Create();

            var console = context.GetPerson("P.Initiator").Login("S1").Start().Wait();

            context.GetPersons("P.Auto.None", "P.Auto.Same", "P.Auto.Cross", "P.Auto.Both").Login().Start().Wait();
            context.GetPersons("P.SA.None.S1", "P.SA.Same.S1", "P.SA.Cross.S1", "P.SA.Both.S1").Login("S1").Start().Wait();
            context.GetPersons("P.SA.None.S2", "P.SA.Same.S2", "P.SA.Cross.S2", "P.SA.Both.S2").Login("S2").Start().Wait();

            var actual = console.GetInternalTransferTargets();
            Assert.AreEqual(
                BackendTools.Format(actual), @"
          Name          Description CountOfTotalInterviewersLoggedIn CountOfFreeInterviewersLoggedIn
  PG.Same.Name  PG.Same.Description                                4                               2
 PG.Cross.Name PG.Cross.Description                                6                               5");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void GetInternalTransferTargets_TargetPersonOnABreak_ResultAreCorrect()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData{ Tag = "S1", AssignsS = "P.Initiator,PG.None,PG.Same,PG.Cross",
                        Interviews = new []
                        {
                            new InterviewData(){ Tag = "S1.I1", Call = new CallData() }
                        }},
                    new SurveyData{ Tag = "S2", AssignsS = "PG.None,PG.Same,PG.Cross"},
                },
                Persons = new[]
                {
                    new PersonData() { Tag = "P.Initiator", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.None,PG.Same,PG.Cross", },

                    new PersonData() { Tag = "P.Auto.None", TaskChoice = TaskChoiceMode.Automatic, Memberships = "PG.None" },
                    new PersonData() { Tag = "P.Auto.Same", TaskChoice = TaskChoiceMode.Automatic, Memberships = "PG.Same" },// +1(PG.Same)
                    new PersonData() { Tag = "P.Auto.Cross", TaskChoice = TaskChoiceMode.Automatic, Memberships = "PG.Cross" }, // +1(PG.Cross)
                    new PersonData() { Tag = "P.Auto.Both", TaskChoice = TaskChoiceMode.Automatic, Memberships = "PG.Same,PG.Cross" },// +1(PG.Same)  +1(PG.Cross)

                    new PersonData() { Tag = "P.SA.None.S1", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.None"},
                    new PersonData() { Tag = "P.SA.Same.S1", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.Same"},// +1(PG.Same)
                    new PersonData() { Tag = "P.SA.Cross.S1", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.Cross"},// +1(PG.Cross)
                    new PersonData() { Tag = "P.SA.Both.S1", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.Same,PG.Cross"},// +1(PG.Same)  +1(PG.Cross)

                    new PersonData() { Tag = "P.SA.None.S2", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.None"},
                    new PersonData() { Tag = "P.SA.Same.S2", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.Same"},
                    new PersonData() { Tag = "P.SA.Cross.S2", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.Cross"},// +1(PG.Cross)
                    new PersonData() { Tag = "P.SA.Both.S2", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG.Same,PG.Cross"},// +1(PG.Cross)
                },
                PersonGroups = new[]
                {
                    new PersonGroupData() { Tag = "PG.None", Name ="PG.None.Name", Description="PG.None.Description"},
                    new PersonGroupData() { Tag = "PG.Same", Name ="PG.Same.Name", Description="PG.Same.Description", TransferBehavior = TransferGroupBehavior.DeliverCallsFromTheSameSurvey},
                    new PersonGroupData() { Tag = "PG.Cross", Name ="PG.Cross.Name", Description="PG.Cross.Description", TransferBehavior = TransferGroupBehavior.DeliverCallsFromOtherSurvey}
                }
            }.Create();

            var console = context.GetPerson("P.Initiator").Login("S1").Start().Wait();

            context.GetPersons("P.Auto.None", "P.Auto.Same", "P.Auto.Cross", "P.Auto.Both").Login().Start().Break().Wait();
            context.GetPersons("P.SA.None.S1", "P.SA.Same.S1", "P.SA.Cross.S1", "P.SA.Both.S1").Login("S1").Start().Break().Wait();
            context.GetPersons("P.SA.None.S2", "P.SA.Same.S2", "P.SA.Cross.S2", "P.SA.Both.S2").Login("S2").Start().Break().Wait();

            var actual = console.GetInternalTransferTargets();
            Assert.AreEqual(
                BackendTools.Format(actual), @"
          Name          Description CountOfTotalInterviewersLoggedIn CountOfFreeInterviewersLoggedIn
  PG.Same.Name  PG.Same.Description                                4                               0
 PG.Cross.Name PG.Cross.Description                                6                               0");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void GetExternalTransferTargets_TwoOfThreeNumbersAssignedToSurvey_ResultIsCorrect()
        {
            var context = new TestData()
            {
                Surveys = new[] {new SurveyData{Tag = "S1", AssignsS = "P1",
                    Interviews = new [] { new InterviewData(){ Tag = "S1.I1", Call = new CallData() }
                }}},
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                ExternalNumbers = new[]
                {
                    new ExternalNumberData(){Phone="111111", Description= "Desc for 111111", Assigns =  "S1"},
                    new ExternalNumberData(){Phone="222222", Description= "Desc for 222222", Assigns =  "S1"},
                    new ExternalNumberData(){Phone="333333", Description= "Desc for 333333"}
                }
            }.Create();

            var actual = context.GetPerson("P1").Login("S1").Start().Wait().GetExternalTransferTargets();

            Assert.AreEqual(
                BackendTools.Format(actual), @"
 TelephoneNumber     Description
          111111 Desc for 111111
          222222 Desc for 222222");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void GetExternalTransferList_NumbersWithDifferentCountOfAssign_ResultIsCorrect()
        {
            var context = new TestData()
            {
                Surveys = new[] {new SurveyData{Tag = "S1"}, new SurveyData{Tag = "S2"}, new SurveyData{Tag = "S3"} },
                ExternalNumbers = new[]
                {
                    new ExternalNumberData(){Tag="EN1", Phone="111111", Description= "Desc for 111111", Hidden = true, Assigns =  "S1,S2,S3"},
                    new ExternalNumberData(){Tag="EN2", Phone="222222", Description= "Desc for 222222", Assigns =  "S1,S2"},
                    new ExternalNumberData(){Tag="EN3", Phone="333333", Description= "Desc for 333333"},
                    new ExternalNumberData(){Tag="EN4", Phone="444444", Description= "Desc for 444444", Assigns =  "S3"}
                }
            }.Create();

            var actual = BvSpTransfer_GetExternalListAdapter.ExecuteEntityList();

            Assert.AreEqual(
                BackendTools.Format(actual), @"
 Id TelephoneNumber     Description Hidden Count
  1          111111 Desc for 111111   True     3
  2          222222 Desc for 222222  False     2
  3          333333 Desc for 333333  False     0
  4          444444 Desc for 444444  False     1");
        }
    }
}
