using System;
using System.Globalization;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;
using Assert = Xunit.Assert;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class ActionAddAdditionalAssignmentOnGroup : BaseMockedIntegrationTest
    {
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void AddAdditionalAssignmentOnGroup_AssignTwoGroups_Successed(SecurityMode mode)
        {
            SetSecurityMode(mode);

            int firstGroupId = PersonGroupService.RootGroupId;
            int secondGroupId = PersonTools.CreatePersonGroup("PG1");

            var script = new TestScript(
                    new SubRule(new[]{
                            new Action(Action.Operation.AssignResource, firstGroupId.ToString(CultureInfo.InvariantCulture)),
                            new Action(Action.Operation.AddAdditionalAssignmentOnGroup, secondGroupId.ToString(CultureInfo.InvariantCulture))
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
            var assignment = BvAssignmentResourceAdapter.GetAll().Single();
            Assert.Equal(String.Format("{0},{1}", firstGroupId, secondGroupId), assignment.Qualifier);

            BackendTools.CheckInterview(interview);
            call.Resource = assignment.ID;
            BackendTools.CheckCall(call);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void AddAdditionalAssignmentOnGroup_ReassignWithSameGroup_Successed(SecurityMode mode)
        {
            SetSecurityMode(mode);

            int firstGroupId = PersonGroupService.RootGroupId;
            int secondGroupId = PersonTools.CreatePersonGroup("PG1");

            var script = new TestScript(
                    new SubRule(new[]{
                            new Action(Action.Operation.AssignResource, firstGroupId.ToString(CultureInfo.InvariantCulture)),
                            new Action(Action.Operation.AddAdditionalAssignmentOnGroup, firstGroupId.ToString(CultureInfo.InvariantCulture))
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
            var assignment = BvAssignmentResourceAdapter.GetAll().Count();
            Assert.Equal(0, assignment);

            BackendTools.CheckInterview(interview);
            call.Resource = firstGroupId;
            BackendTools.CheckCall(call);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void AddAdditionalAssignmentOnGroup_AssignTwoGroupsAndOneGroupIsAlreadyAssigned_Successed(SecurityMode mode)
        {
            SetSecurityMode(mode);

            int firstGroupId = PersonGroupService.RootGroupId;
            int secondGroupId = PersonTools.CreatePersonGroup("PG1");

            var script = new TestScript(
                    new SubRule(new[]{
                            new Action(Action.Operation.AssignResource, firstGroupId.ToString(CultureInfo.InvariantCulture)),
                            new Action(Action.Operation.AddAdditionalAssignmentOnGroup, String.Format("{0},{1}", firstGroupId, secondGroupId))
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
            var assignment = BvAssignmentResourceAdapter.GetAll().Single();
            Assert.Equal(String.Format("{0},{1}", firstGroupId, secondGroupId), assignment.Qualifier);

            BackendTools.CheckInterview(interview);
            call.Resource = assignment.ID;
            BackendTools.CheckCall(call);
        }
        
        [Theory, Owner(@"FIRM\EgorK")]
        [ClassData(typeof(TestDataGenerator))]
        public void AddAdditionalAssignmentOnGroup_AssignNotExistingGroup_LaunchScript_ErrorExpected(SecurityMode mode)
        {
            SetSecurityMode(mode);

            int firstGroupId = PersonGroupService.RootGroupId;
            int secondGroupId = PersonTools.CreatePersonGroup("PG1");

            var script = new TestScript(
                new SubRule(new[]{
                    new Action(Action.Operation.AssignResource, firstGroupId.ToString(CultureInfo.InvariantCulture)),
                    new Action(Action.Operation.AddAdditionalAssignmentOnGroup, String.Format("{0},{1}, -53", firstGroupId, secondGroupId))
                }),
                new Shift(1, 1, "0.00:00:00", "1.00:00:00"));

            try
            {
                BackendToolsObject.CreateSurvey(script);
            }
            catch (Exception ex)
            {
                Assert.EndsWith("Add a group to a multiple assignment'. The specified group either does not exist or is an administrative group.", ex.Message);
                return;
            }
            
            Assert.True(false, "No exception thrown");
        }
        
        [Theory, Owner(@"FIRM\EgorK")]
        [ClassData(typeof(TestDataGenerator))]
        public void AddAdditionalAssignmentOnGroup_AssignAdministrativeGroup_LaunchScript_ErrorExpected(SecurityMode mode)
        {
            SetSecurityMode(mode);

            int firstGroupId = PersonGroupService.RootGroupId;
            int secondGroupId = PersonTools.CreatePersonGroup("PG1", true);

            var script = new TestScript(
                new SubRule(new[]{
                    new Action(Action.Operation.AssignResource, firstGroupId.ToString(CultureInfo.InvariantCulture)),
                    new Action(Action.Operation.AddAdditionalAssignmentOnGroup, String.Format("{0},{1}", firstGroupId, secondGroupId))
                }),
                new Shift(1, 1, "0.00:00:00", "1.00:00:00"));

            try
            {
                BackendToolsObject.CreateSurvey(script);
            }
            catch (Exception ex)
            {
                Assert.EndsWith("Add a group to a multiple assignment'. The specified group either does not exist or is an administrative group.", ex.Message);
                return;
            }
          
            Assert.True(false, "No exception thrown");
        }
        
        [Theory, Owner(@"FIRM\EgorK")]
        [ClassData(typeof(TestDataGenerator))]
        public void AddAdditionalAssignmentOnGroup_LaunchScript_DeleteGroup_RunScript_ErrorExpected(SecurityMode mode)
        {
            SetSecurityMode(mode);

            int firstGroupId = PersonGroupService.RootGroupId;
            int secondGroupId = PersonTools.CreatePersonGroup("PG1");

            var script = new TestScript(
                new SubRule(new[]{
                    new Action(Action.Operation.AssignResource, firstGroupId.ToString(CultureInfo.InvariantCulture)),
                    new Action(Action.Operation.AddAdditionalAssignmentOnGroup, String.Format("{0},{1}", firstGroupId, secondGroupId))
                }),
                new Shift(1, 1, "0.00:00:00", "1.00:00:00"));

            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            PersonTools.RemovePersonGroup(secondGroupId);
            //
            // Execute scheduling script
            // 
            BackendTools.FireEvent(interview);

            var logs = ServiceLocator.Resolve<ISchedulingScriptLogRepository>().GetByInterviewId(surveySID, interview.ID);
            
            Assert.Contains($"One or more specified groups do not exist or are administrative. Administrative groups cannot be assigned to calls.", logs[0].LogMessages);
            
        }
        
        [Theory, Owner(@"FIRM\EgorK")]
        [ClassData(typeof(TestDataGenerator))]
        public void AddAdditionalAssignmentOnGroup_LaunchScript_MakeGroupAdministrative_RunScript_ErrorExpected(SecurityMode mode)
        {
            SetSecurityMode(mode);

            int firstGroupId = PersonGroupService.RootGroupId;
            int secondGroupId = PersonTools.CreatePersonGroup("PG1");

            var script = new TestScript(
                new SubRule(new[]{
                    new Action(Action.Operation.AssignResource, firstGroupId.ToString(CultureInfo.InvariantCulture)),
                    new Action(Action.Operation.AddAdditionalAssignmentOnGroup, String.Format("{0},{1}", firstGroupId, secondGroupId))
                }),
                new Shift(1, 1, "0.00:00:00", "1.00:00:00"));

            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            
            var personGroupRepo = ServiceLocator.Resolve<IPersonGroupRepository>();
            var group = personGroupRepo.GetById(secondGroupId);
            group.IsAdministrative = true;
            personGroupRepo.Update(group);
            
            //
            // Execute scheduling script
            // 
            BackendTools.FireEvent(interview);

            var logs = ServiceLocator.Resolve<ISchedulingScriptLogRepository>().GetByInterviewId(surveySID, interview.ID);
            
            Assert.Contains($"One or more specified groups do not exist or are administrative. Administrative groups cannot be assigned to calls.", logs[0].LogMessages);
        }
    }
}
