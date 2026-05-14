using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class SchedulingRepositoryTest : BaseMockedIntegrationTest
    {
        [Theory]
        [ClassData(typeof(TestDataGenerator))]
        public void Delete_ScriptAreAssignedToSurvey_UserMessageExceptionAreThrown(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var script = new TestScript(
                new[]
                {
                    new Action(Action.Operation.IncrementPriority, "2"),
                },
                new Shift(1, 1, "0.00:00:00", "0.00:00:00"));

            BackendToolsObject.CreateSurvey(script, "p10000001");

            Xunit.Assert.Throws<UserMessageException>(() => ScheduleRepository.Delete(script.ScheduleID));
        }

        [Theory]
        [ClassData(typeof(TestDataGenerator))]
        public void Delete_ScriptAreAssignedToMarkedToDeleteSurvey_ScriptAreDeletedAndAssignmentAreReset(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var script = new TestScript(
                new[]
                {
                    new Action(Action.Operation.IncrementPriority, "2"),
                },
                new Shift(1, 1, "0.00:00:00", "0.00:00:00"));

            int surveyId = BackendToolsObject.CreateSurvey(script, "p10000001");

            new ManagementService().SoftDeleteSurvey("p10000001");

            ScheduleRepository.Delete(script.ScheduleID);

            var survey = SurveyRepository.GetById(surveyId);

            Assert.IsNull(ScheduleRepository.GetById(script.ScheduleID));
            Assert.AreEqual(BackendTools.GetDefaultScheduleID(), survey.ScheduleID);
        }
    }
}
