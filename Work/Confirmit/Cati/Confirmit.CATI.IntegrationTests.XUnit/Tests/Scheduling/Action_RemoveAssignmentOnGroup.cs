using System;
using System.Globalization;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class ActionRemoveAssignmentOnGroup : BaseMockedIntegrationTest
    {
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void RemoveAssignmentOnGroup_DeassignFromSecondGroup_Success(SecurityMode mode)
        {
            SetSecurityMode(mode);

            int firstGroupId = PersonGroupService.RootGroupId;
            int secondGroupId = PersonTools.CreatePersonGroup("PG1");

            var script = new TestScript(
                    new SubRule(new []{
                            new Action(Action.Operation.AssignResource, String.Format("{0},{1}", firstGroupId, secondGroupId)),
                            new Action(Action.Operation.RemoveAssignmentOnGroup, secondGroupId.ToString(CultureInfo.InvariantCulture))
                        }),
                        new Shift(1, 1, "0.00:00:00", "1.00:00:00"));

            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);

            //
            // Execute scheduling script
            // 
            BackendTools.FireEvent(interview);

            //
            // Action executed, so lets check execution results
            //
            BackendTools.CheckInterview(interview);
            call.Resource = firstGroupId;
            BackendTools.CheckCall(call);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void RemoveAssignmentOnGroup_DeassignAllGroups_Successed(SecurityMode mode)
        {
            SetSecurityMode(mode);

            int firstGroupId = PersonGroupService.RootGroupId;
            int secondGroupId = PersonTools.CreatePersonGroup("PG1");

            var script = new TestScript(
                    new SubRule(new[]{
                            new Action(Action.Operation.AssignResource, String.Format("{0},{1}", firstGroupId, secondGroupId)),
                            new Action(Action.Operation.RemoveAssignmentOnGroup, String.Format("{0},{1}", firstGroupId, secondGroupId))
                        }),
                        new Shift(1, 1, "0.00:00:00", "1.00:00:00"));

            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);

            //
            // Execute scheduling script
            // 
            BackendTools.FireEvent(interview);

            //
            // Action executed, so lets check execution results
            //
            BackendTools.CheckInterview(interview);
            call.Resource = 0;
            BackendTools.CheckCall(call);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void RemoveAssignmentOnGroup_DeassignAllGroupsWithNotAssignmedGroup_Successed(SecurityMode mode)
        {
            SetSecurityMode(mode);

            int firstGroupId = PersonGroupService.RootGroupId;
            int secondGroupId = PersonTools.CreatePersonGroup("PG1");

            var script = new TestScript(
                    new SubRule(new[]{
                            new Action(Action.Operation.AssignResource, firstGroupId.ToString(CultureInfo.InvariantCulture)),
                            new Action(Action.Operation.RemoveAssignmentOnGroup, String.Format("{0},{1}", firstGroupId, secondGroupId))
                        }),
                        new Shift(1, 1, "0.00:00:00", "1.00:00:00"));

            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);

            //
            // Execute scheduling script
            // 
            BackendTools.FireEvent(interview);

            //
            // Action executed, so lets check execution results
            //
            BackendTools.CheckInterview(interview);
            call.Resource = 0;
            BackendTools.CheckCall(call);
        }
    }
}
