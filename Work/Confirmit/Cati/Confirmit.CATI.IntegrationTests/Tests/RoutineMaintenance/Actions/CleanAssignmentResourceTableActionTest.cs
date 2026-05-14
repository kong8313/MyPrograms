using System;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Tests.CallDelivering.CallDeliveringTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.RoutineMaintenance.Actions
{
    [TestClass]
    public class CleanAssignmentResourceTableActionTest : BaseMockedIntegrationTest
    {
        [TestMethod]
        public void CleanUnusedAssignment_OneUsedAndTwoUnusedAssignmnet_UnusedAssignmentIsDeleted()
        {
            var context = new TestData(){
                Surveys = new[]{
                    new SurveyData(){ Tag="S1", IsUseDb = false, 
                        Interviews = new[] {
                            new InterviewData() {Tag = "S1.I1", Call = new CallData {Resource = "PG1,PG2"}},
                            new InterviewData() {Tag = "S1.I2", Call = new CallData {Resource = "PG1,PG3"}}}
                    }
                },
                PersonGroups = new[]{
                    new PersonGroupData() { Tag = "PG1", Name="PG1"},
                    new PersonGroupData() { Tag = "PG2", Name="PG2"},
                    new PersonGroupData() { Tag = "PG3", Name="PG3"}},
                Persons = new[] { new PersonData() { Tag = "P1", Memberships = "PG1,PG2,PG3" } }
            }.Create();
            
            var survey = context.GetSurvey("S1");
            var interviews = context.GetInterviews("S1.I1", "S1.I2");
            var groupIds = context.GetResources("PG2", "PG3").Select(x => x.Id).ToArray();
            var person = context.GetResource("P1");
            
            CollectionAssert.AreEqual(
                BvAssignmentResourceAdapter.GetAll().Select(x => x.ID).Union(
                    context.GetResources("PG1", "PG2", "PG3").Select(y => y.Id)
                    ).OrderBy(z => z).ToArray(),
                BvCallHandlerLibrary.Tools.PersonTools.GetUserGroups(person.Id).OrderBy(x => x).ToArray(),
                "Whrong list of person groups which should be sent to dialer.");

            CallTools.ActivateCalls(survey.Id, 1, CallStates.All, groupIds, (int) CallShiftType.None, null, false, interviews.Select(x => x.Id));


            var now = DateTime.Parse("2015.02.06 14:00:00");
            ServiceLocator.RegisterInstance<ITimeService>(new TestTimeService(now));
            var settings = ServiceLocator.Resolve<IRoutineMaintenanceSettings>();
            
            settings.Duration = TimeSpan.FromHours(4);
            settings.DailyShiftStartTime = TimeSpan.FromHours(12);
            settings.Actions.AssignmentResourceTableCleanup.ShiftType = 1;/*Daily*/

            BackendToolsObject.ExecuteRoutineMaintenance();

            var assignments = BvAssignmentResourceAdapter.GetAll();
            var qualifier = StringService.Join(",", x => x.ToString(), groupIds.OrderBy(x => x).ToArray());
            
            Assert.AreEqual(1, assignments.Count, "Wrong count of assignments after cleanup.");
            var assignment = assignments.Single();
            Assert.AreEqual(qualifier, assignment.Qualifier);
            Assert.AreEqual("PG2,PG3", assignment.Name);

            CollectionAssert.AreEqual(
                assignments.Select(x => x.ID).Union(
                    context.GetResources("PG1", "PG2", "PG3").Select(y => y.Id)
                    ).OrderBy(z => z).ToArray(),
                BvCallHandlerLibrary.Tools.PersonTools.GetUserGroups(person.Id).OrderBy(x => x).ToArray(),
                "Whrong list of person groups which should be sent to dialer.");

            var items = BvAssignmentResourceItemAdapter.GetAll();
            Assert.AreEqual(2, items.Count);
            Assert.AreEqual(assignment.ID, items[0].AssignmentID);
            Assert.AreEqual(assignment.ID, items[1].AssignmentID);
        }
    }
}
