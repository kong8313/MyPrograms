using System;
using System.Globalization;
using System.Linq;
using Confirmit.CATI.Common.Exceptions;
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
    public class ActionAssignResource : BaseMockedIntegrationTest
    {
        private void Interview_AssignPerson_PersonAssigned(bool withCall)
        {
            int personSID = PersonTools.CreatePerson(null);

            var script = new TestScript(
                    new Action(Action.Operation.AssignResource, personSID.ToString(CultureInfo.InvariantCulture)),
                    @"Scheduling2007\Schedule.xml");

            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            if (withCall)
                BackendTools.CreateCall(call);

            //
            // Execute scheduling script
            // 
            BackendTools.FireEvent(interview);

            //
            // Action executed, so lets check execution results
            //
            BackendTools.CheckInterview(interview);

            call.Resource = personSID;
            BackendTools.CheckCall(call);
        }
        internal void Interview_AssignGroup_GroupAssigned(bool withCall)
        {
            int groupSID = PersonGroupService.RootGroupId;

            var script = new TestScript(
                    new Action(Action.Operation.AssignResource, groupSID.ToString(CultureInfo.InvariantCulture)),
                    @"Scheduling2007\Schedule.xml");

            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            if (withCall)
                BackendTools.CreateCall(call);

            //
            // Execute scheduling script
            // 
            BackendTools.FireEvent(interview);

            //
            // Action executed, so lets check execution results
            //
            BackendTools.CheckInterview(interview);
            call.Resource = groupSID;
            BackendTools.CheckCall(call);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewWithCall_AssignPerson_PersonAssignedAndCallChanged(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Interview_AssignPerson_PersonAssigned(true);
        }
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewWithoutCall_AssignPerson_PersonAssignedAndCallCreated(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Interview_AssignPerson_PersonAssigned(false);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewWithCall_AssignGroup_GroupAssignedAndCallChanged(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Interview_AssignGroup_GroupAssigned(true);
        }
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewWithoutCall_AssignGroup_GroupAssignedAndCallCreated(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Interview_AssignGroup_GroupAssigned(false);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewWithResourceGroup_UnchangeResource_ResourceUnchanged(SecurityMode mode)
        {
            SetSecurityMode(mode);

            int groupSID = PersonGroupService.RootGroupId;

            var script = new TestScript(
                    new Action(Action.Operation.AssignResource, "-1"),
                    @"Scheduling2007\Schedule.xml");

            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            call.Resource = groupSID;
            BackendTools.CreateCall(call);

            BackendTools.FireEvent(interview);

            BackendTools.CheckInterview(interview);
            //call.Resource = Convert.ToInt32(groupSID);
            BackendTools.CheckCall(call);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewWithResourcePerson_UnchangeResource_ResourceUnchanged(SecurityMode mode)
        {
            SetSecurityMode(mode);

            int personSID = PersonTools.CreatePerson(null);

            var script = new TestScript(
                    new Action(Action.Operation.AssignResource, "-1"),
                    @"Scheduling2007\Schedule.xml");

            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            call.Resource = personSID;
            BackendTools.CreateCall(call);

            BackendTools.FireEvent(interview);

            BackendTools.CheckInterview(interview);
            //call.Resource = Convert.ToInt32(groupSID);
            BackendTools.CheckCall(call);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewWithoutResource_UnchangeResource_ResourceUnchanged(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var script = new TestScript(
                    new Action(Action.Operation.AssignResource, "-1"),
                    @"Scheduling2007\Schedule.xml");

            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview);
            var call = BackendTools.NewCall(interview);

            BackendTools.FireEvent(interview);

            BackendTools.CheckInterview(interview);
            //call.Resource = Convert.ToInt32(groupSID);
            BackendTools.CheckCall(call);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewWithoutResource_AssignLastPerson_ResourceAssigned(SecurityMode mode)
        {
            SetSecurityMode(mode);

            int personSID = PersonTools.CreatePerson(null);

            var script = new TestScript(
                    new Action(Action.Operation.AssignResource, "-2"),
                    @"Scheduling2007\Schedule.xml");

            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            interview.LastCallPersonSID = personSID;
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);

            BackendTools.FireEvent(interview);

            BackendTools.CheckInterview(interview);
            //call.Resource = Convert.ToInt32(groupSID);
            call.Resource = personSID;
            BackendTools.CheckCall(call);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewWithResourceGroup_AssignLastPerson_ResourceAssigned(SecurityMode mode)
        {
            SetSecurityMode(mode);

            int personSID = PersonTools.CreatePerson(null);

            var script = new TestScript(
                    new Action(Action.Operation.AssignResource, "-2"),
                    @"Scheduling2007\Schedule.xml");

            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            interview.LastCallPersonSID = personSID;
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            call.Resource = PersonGroupService.RootGroupId;
            BackendTools.CreateCall(call);

            BackendTools.FireEvent(interview);

            BackendTools.CheckInterview(interview);
            //call.Resource = Convert.ToInt32(groupSID);
            call.Resource = personSID;
            BackendTools.CheckCall(call);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewWithResourceGroup_AssignSurveyinterviewers_ResourceAssigned(SecurityMode mode)
        {
            SetSecurityMode(mode);

            int personSID = PersonTools.CreatePerson(null);

            var script = new TestScript(
                    new Action(Action.Operation.AssignResource, "-3"),
                    @"Scheduling2007\Schedule.xml");

            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            interview.LastCallPersonSID = personSID;
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            call.Resource = PersonGroupService.RootGroupId;
            BackendTools.CreateCall(call);

            BackendTools.FireEvent(interview);

            BackendTools.CheckInterview(interview);
            //call.Resource = Convert.ToInt32(groupSID);
            call.Resource = 0;
            BackendTools.CheckCall(call);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void InterviewWithResourceGroup_AssignSeveralGroups_ResourceAssigned(SecurityMode mode)
        {
            SetSecurityMode(mode);

            int personSID1 = PersonTools.CreatePersonGroup(null);
            int personSID2 = PersonTools.CreatePersonGroup(null);

            var script = new TestScript(
                    new Action(Action.Operation.AssignResource, string.Format("{0},{1}", personSID1, personSID2)),
                    @"Scheduling2007\Schedule.xml");

            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            call.Resource = PersonGroupService.RootGroupId;
            BackendTools.CreateCall(call);

            BackendTools.FireEvent(interview);

            var assignmentId = BvAssignmentResourceAdapter.GetAll().Single().ID;

            BackendTools.CheckInterview(interview);
            call.Resource = assignmentId;
            BackendTools.CheckCall(call);
        }
        
        [Theory, Owner(@"FIRM\EgorK")]
        [ClassData(typeof(TestDataGenerator))]
        public void AssignGroups_OneGroupIsAdministrative_LaunchScript_ErrorExpected(SecurityMode mode)
        {
            SetSecurityMode(mode);

            int group1 = PersonTools.CreatePersonGroup(null, false);
            int group2 = PersonTools.CreatePersonGroup(null, true);

            var script = new TestScript(
                new Action(Action.Operation.AssignResource, string.Format("{0},{1}", group1, group2)),
                @"Scheduling2007\Schedule.xml");
            try
            {
                BackendToolsObject.CreateSurvey(script);
            }
            catch (UserMessageException e)
            {
               Assert.EndsWith("Assign user/group(s)'. The specified group either does not exist or is an administrative group.", e.Message);
               return;
            }

            Assert.True(false, "No exception thrown");
        }
        
        [Theory, Owner(@"FIRM\EgorK")]
        [ClassData(typeof(TestDataGenerator))]
        public void AssignSingleAdministrativeGroup_LaunchScript_ErrorExpected(SecurityMode mode)
        {
            SetSecurityMode(mode);
            
            int group1 = PersonTools.CreatePersonGroup(null, true);

            var script = new TestScript(
                new Action(Action.Operation.AssignResource, string.Format("{0}", group1)),
                @"Scheduling2007\Schedule.xml");
            try
            {
                BackendToolsObject.CreateSurvey(script);
            }
            catch (UserMessageException e)
            {
                Assert.EndsWith("Assign user/group(s)'. The specified group either does not exist or is an administrative group.", e.Message);
                return;
            }

            Assert.True(false, "No exception thrown");
        }
        
        [Theory, Owner(@"FIRM\EgorK")]
        [ClassData(typeof(TestDataGenerator))]
        public void AssignSingleNotExistingGroup_LaunchScript_ErrorExpected(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var script = new TestScript(
                new Action(Action.Operation.AssignResource, string.Format("{0}", -53)),
                @"Scheduling2007\Schedule.xml");
            try
            {
                BackendToolsObject.CreateSurvey(script);
            }
            catch (UserMessageException e)
            {
                Assert.EndsWith("Assign user/group(s)'. Specified user or group doesn't exist.", e.Message);
                return;
            }

            Assert.True(false, "No exception thrown");
        }
        
        [Theory, Owner(@"FIRM\EgorK")]
        [ClassData(typeof(TestDataGenerator))]
        public void AssignGroups_OneGroupDoesNotExist_LaunchScript_ErrorExpected(SecurityMode mode)
        {
            SetSecurityMode(mode);

            int group1 = PersonTools.CreatePersonGroup(null, false);
            int group2 = PersonTools.CreatePersonGroup(null, false);

            var script = new TestScript(
                new Action(Action.Operation.AssignResource, string.Format("{0},{1},-53", group1, group2)),
                @"Scheduling2007\Schedule.xml");
            try
            {
                BackendToolsObject.CreateSurvey(script);
            }
            catch (UserMessageException e)
            {
                Assert.EndsWith("Assign user/group(s)'. The specified group either does not exist or is an administrative group.", e.Message);
                return;
            }

            Assert.True(false, "No exception thrown");
        }
        
        [Theory, Owner(@"FIRM\EgorK")]
        [ClassData(typeof(TestDataGenerator))]
        public void AssignGroups_LaunchScript_DeleteGroup_RunScript_ErrorExpected(SecurityMode mode)
        {
            SetSecurityMode(mode);

            int group1 = PersonTools.CreatePersonGroup(null, false);
            int group2 = PersonTools.CreatePersonGroup(null, false);

            var script = new TestScript(
                new Action(Action.Operation.AssignResource, string.Format("{0},{1}", group1, group2)),
                @"Scheduling2007\Schedule.xml");
           
            var surveySID = BackendToolsObject.CreateSurvey(script);
            
            var interview = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            call.Resource = PersonGroupService.RootGroupId;
            BackendTools.CreateCall(call);

            PersonTools.RemovePersonGroup(group1);
            
            BackendTools.FireEvent(interview);

            var logs = ServiceLocator.Resolve<ISchedulingScriptLogRepository>().GetByInterviewId(surveySID, interview.ID);
            
            Assert.Contains("One or more specified groups do not exist or are administrative. Administrative groups cannot be assigned to calls.", logs[0].LogMessages);
        }
        
        [Theory, Owner(@"FIRM\EgorK")]
        [ClassData(typeof(TestDataGenerator))]
        public void AssignGroups_LaunchScript_MakeGroupAdministrative_RunScript_ErrorExpected(SecurityMode mode)
        {
            SetSecurityMode(mode);

            int group1 = PersonTools.CreatePersonGroup(null, false);
            int group2 = PersonTools.CreatePersonGroup(null, false);

            var script = new TestScript(
                new Action(Action.Operation.AssignResource, string.Format("{0},{1}", group1, group2)),
                @"Scheduling2007\Schedule.xml");
           
            var surveySID = BackendToolsObject.CreateSurvey(script);
            
            var interview = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            call.Resource = PersonGroupService.RootGroupId;
            BackendTools.CreateCall(call);

            var personGroupRepo = ServiceLocator.Resolve<IPersonGroupRepository>();
            var group = personGroupRepo.GetById(group1);
            group.IsAdministrative = true;
            personGroupRepo.Update(group);
            
            BackendTools.FireEvent(interview);

            var logs = ServiceLocator.Resolve<ISchedulingScriptLogRepository>().GetByInterviewId(surveySID, interview.ID);
            
            Assert.Contains("One or more specified groups do not exist or are administrative. Administrative groups cannot be assigned to calls.", logs[0].LogMessages);
        }
        
        [Theory, Owner(@"FIRM\EgorK")]
        [ClassData(typeof(TestDataGenerator))]
        public void AssignSingleGroup_LaunchScript_DeleteGroup_RunScript_ErrorExpected(SecurityMode mode)
        {
            SetSecurityMode(mode);

            int group1 = PersonTools.CreatePersonGroup(null, false);

            var script = new TestScript(
                new Action(Action.Operation.AssignResource, string.Format("{0}", group1)),
                @"Scheduling2007\Schedule.xml");
           
            var surveySID = BackendToolsObject.CreateSurvey(script);
            
            var interview = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            call.Resource = PersonGroupService.RootGroupId;
            BackendTools.CreateCall(call);

            PersonTools.RemovePersonGroup(group1);
            
            BackendTools.FireEvent(interview);

            var logs = ServiceLocator.Resolve<ISchedulingScriptLogRepository>().GetByInterviewId(surveySID, interview.ID);
            
            Assert.Contains($"The specified resource (\"{group1}\") was not found. A resource must be a valid interviewer or group ID", logs[0].LogMessages);
        }
        
        [Theory, Owner(@"FIRM\EgorK")]
        [ClassData(typeof(TestDataGenerator))]
        public void AssignSingleGroup_LaunchScript_MakeGroupAdministrative_RunScript_ErrorExpected(SecurityMode mode)
        {
            SetSecurityMode(mode);

            int group1 = PersonTools.CreatePersonGroup(null, false);

            var script = new TestScript(
                new Action(Action.Operation.AssignResource, string.Format("{0}", group1)),
                @"Scheduling2007\Schedule.xml");
           
            var surveySID = BackendToolsObject.CreateSurvey(script);
            
            var interview = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            call.Resource = PersonGroupService.RootGroupId;
            BackendTools.CreateCall(call);

            var personGroupRepo = ServiceLocator.Resolve<IPersonGroupRepository>();
            var group = personGroupRepo.GetById(group1);
            group.IsAdministrative = true;
            personGroupRepo.Update(group);
            
            BackendTools.FireEvent(interview);

            var logs = ServiceLocator.Resolve<ISchedulingScriptLogRepository>().GetByInterviewId(surveySID, interview.ID);
            
            Assert.Contains($"The group with ID \"{group1}\" is administrative. Administrative groups cannot be assigned to calls.\n", logs[0].LogMessages);
        }
    }
}
