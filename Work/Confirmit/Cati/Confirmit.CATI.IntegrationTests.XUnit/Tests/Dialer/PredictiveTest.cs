using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Dialer
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait2)]
    public class PredictiveTest: BaseMockedIntegrationTest
    {
        private const string UserName = "user";
        private const string Password = "pass";
        
        /// <summary>
        /// a.	Prerequisites
        ///     i.	    Add user
        ///     ii.	    Add group 1 and get group ID
        ///     iii.	Add group 2 and get group ID
        ///     iv.	    Add group 3 and get group ID
        ///     v.	    Make user a member of all these 3 groups
        ///     vi.	    Add survey  and get an implicit group number for this survey
        ///     vii.	Assign user this survey 
        /// b.	Execute GetGroups method for a user
        /// c.	Test should return 3 groups ( 1,2,3 but not implicit survey group ), group ids should be equal to the ones we have created.
        /// </summary>
        [Theory, Owner(@"FIRM\AlexanderM")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void GetGroups_3GroupsAnd1Survey_Get3Groups(DialType dialType)
        {
            int groupSid1 = PersonTools.CreatePersonGroup("Group1");
            int groupSid2 = PersonTools.CreatePersonGroup("Group2");
            int groupSid3 = PersonTools.CreatePersonGroup("Group3");
            int personSid = PersonTools.CreatePerson(
                "Person1",
                "pass1",
                AgentTaskChoiceMode.CampaignAssignment,
                new[] { groupSid1, groupSid2, groupSid3 },
                CallCenterTools.DefaultId,
                dialType: dialType);
            int surveySID = PredictiveToolsObject.CreatePredictiveSurvey();
            BackendTools.AssignCatiPersonToSurvey(surveySID, personSid);

            List<int> result = PredictiveTools.GetGroups(personSid);

            PredictiveTools.CheckGetGroupsResult(new[] { groupSid1, groupSid2, groupSid3}, result);
        }

        /// <summary>
        /// a.	Prerequisites
        ///     i.	    Add user
        ///     ii.	    Add group 1 and get group ID
        ///     iii.	Add group 2 and get group ID
        ///     iv.	    Add group 3 and get group ID
        ///     v.	    Make user a member of all these 3 groups
        ///     vi.	    Add survey  one and get an implicit group number for this survey
        ///     vii.	Add survey  two  survey and get an implicit group number for this survey
        ///     viii.	Assign user to  survey one
        /// b.	Execute GetGroups method for a user
        /// c.	Test should return 3 groups ( 1,2,3 but not the implicit survey group ), group ids should be equal to the ones we have created.
        /// </summary>
        [Theory, Owner(@"FIRM\AlexanderM")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void GetGroups_3GroupsAnd2SurveysWith1Assigned_Get3Groups(DialType dialType)
        {
            int groupSid1 = PersonTools.CreatePersonGroup("Group1");
            int groupSid2 = PersonTools.CreatePersonGroup("Group2");
            int groupSid3 = PersonTools.CreatePersonGroup("Group3");
            int personSid = PersonTools.CreatePerson(
                "Person1",
                "pass1",
                AgentTaskChoiceMode.CampaignAssignment,
                new[] { groupSid1, groupSid2, groupSid3 },
                dialType: dialType);
            int surveySid = PredictiveToolsObject.CreatePredictiveSurvey();
            BackendTools.AssignCatiPersonToSurvey(surveySid, personSid);

            List<int> result = PredictiveTools.GetGroups(personSid);

            PredictiveTools.CheckGetGroupsResult(new[] { groupSid1, groupSid2, groupSid3}, result);
        }

        /// <summary>
        /// a.	Prerequisites
        ///     i.	    Add user
        ///     ii.	    Add group 1 and get group ID
        ///     iii.	Add group 2 and get group ID
        ///     iv.	    Add group 3 and get group ID
        ///     v.	    Make user a member of all these 3 groups
        ///     vi.	    Add survey one and get an implicit group number for this survey
        ///     vii.	Add survey two and get an implicit group number for this survey
        ///     viii.	Assign user to survey one
        ///     ix.     Assign group 1 to survey 2
        /// b.	Execute GetGroups method for a user
        /// c.	Test should return 3 groups ( 1,2,3 but not the implicit survey group ), group ids should be equal to the ones we have created.
        /// </summary>
        [Theory, Owner(@"FIRM\AlexanderM")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void GetGroups_3GroupsAnd2SurveysWith2Assigned_Get3GroupsFor1Survey(DialType dialType)
        {
            int groupSid1 = PersonTools.CreatePersonGroup("Group1");
            int groupSid2 = PersonTools.CreatePersonGroup("Group2");
            int groupSid3 = PersonTools.CreatePersonGroup("Group3");
            int personSid = PersonTools.CreatePerson(
                "Person1",
                "pass1",
                AgentTaskChoiceMode.CampaignAssignment,
                new[] { groupSid1, groupSid2, groupSid3 },
                dialType: dialType);
            int surveySid = PredictiveToolsObject.CreatePredictiveSurvey();
            int surveySid2 = PredictiveToolsObject.CreatePredictiveSurvey();

            BackendTools.AssignCatiPersonToSurvey(surveySid, personSid);
            BackendTools.AssignCatiPersonToSurvey(surveySid2, groupSid1);

            List<int> result = PredictiveTools.GetGroups(personSid);

            PredictiveTools.CheckGetGroupsResult(new[] {groupSid1, groupSid2, groupSid3}, result);
        }

        /// <summary>
        /// a.	Prerequisites
        ///     i.	    Add user
        ///     ii.	    Add group 1
        ///     iii.	Add group 2
        ///     iv.	    Add group 3
        ///     v.	    Make user a member of all groups
        ///     vi.	    Add survey 
        ///     vii.	Assign groups 1,2,3 to survey
        /// b.	Execute GetGroups method for a user
        /// c.	Test should return 3 groups ( 1,2,3 but not the implicit survey group ), group ids should be equal to the ones we have created.
        /// </summary>
        [Theory, Owner(@"FIRM\AlexanderM")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void GetGroups_3GroupsAssignedToSurvey_Get3Groups(DialType dialType)
        {
            int groupSid1 = PersonTools.CreatePersonGroup("Group1");
            int groupSid2 = PersonTools.CreatePersonGroup("Group2");
            int groupSid3 = PersonTools.CreatePersonGroup("Group3");
            int personSid = PersonTools.CreatePerson(
                "Person1",
                "pass1",
                AgentTaskChoiceMode.CampaignAssignment,
                new[] { groupSid1, groupSid2, groupSid3 },
                dialType: dialType);
            int surveySid = PredictiveToolsObject.CreatePredictiveSurvey();

            BackendTools.AssignCatiPersonToSurvey(surveySid, groupSid1);
            BackendTools.AssignCatiPersonToSurvey(surveySid, groupSid2);
            BackendTools.AssignCatiPersonToSurvey(surveySid, groupSid3);

            List<int> result = PredictiveTools.GetGroups(personSid);

            PredictiveTools.CheckGetGroupsResult(new[] { groupSid1, groupSid2, groupSid3 }, result);
        }

        /// <summary>
        /// a.	Prerequisites
        ///     i.	    Add user
        ///     ii.	    Add group 1 
        ///     iii.	Add group 2 to group 1 � i.e. group1\group2
        ///     iv.	    Make user a member of group2 only
        ///     v.	    Add survey
        /// b.	Execute GetGroups method for a user
        /// c.	Test should return one group � group 2
        /// </summary>
        [Theory, Owner(@"FIRM\AlexanderM")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void GetGroups_GroupWithSubgroupWithPerson_GetSubgroupOnly(DialType dialType)
        {
            int groupSid1 = PersonTools.CreatePersonGroup("Group1");
            int groupSid2 = PersonTools.CreatePersonGroup("Group2", new[] { groupSid1 });
            int personSid = PersonTools.CreatePerson(
                "Person1",
                "pass1",
                AgentTaskChoiceMode.CampaignAssignment,
                new[] { groupSid2 },
                dialType: dialType);
            PredictiveToolsObject.CreatePredictiveSurvey();

            List<int> result = PredictiveTools.GetGroups(personSid);

            PredictiveTools.CheckGetGroupsResult(new[] { groupSid2 }, result);
        }

        /// <summary>
        /// a.	Prerequisites
        ///     i.	    Add user
        ///     ii.	    Add group 1
        ///     iii.	Add group 2 to group 1 � i.e. group1\group2
        ///     iv.	    Make user a member of group1 and group 2 
        ///     v.	    Add survey
        /// b.	Execute GetGroups method for a user
        /// c.	Test should return 2 groups � group 1 and 2
        /// </summary>
        [Theory, Owner(@"FIRM\AlexanderM")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void GetGroups_GroupWithSubgroupBothWithPerson_GetBothGroups(DialType dialType)
        {
            int groupSid1 = PersonTools.CreatePersonGroup("Group1");
            int groupSid2 = PersonTools.CreatePersonGroup("Group2", new[] { groupSid1 });
            int personSid = PersonTools.CreatePerson(
                "Person1",
                "pass1",
                AgentTaskChoiceMode.CampaignAssignment,
                new[] { groupSid1, groupSid2 },
                dialType: dialType);
            PredictiveToolsObject.CreatePredictiveSurvey();

            List<int> result = PredictiveTools.GetGroups(personSid);

            PredictiveTools.CheckGetGroupsResult(new[] { groupSid1, groupSid2 }, result);
        }

        /// <summary>
        /// a.	Prerequisites
        ///     i.	Add user1
        ///     ii.	Add group 1
        ///     iii.	Add group 2
        ///     iv.	Add group 3
        ///     v.	Make user a member of all groups
        ///     vi.	Add survey 
        ///     vii.	Assign groups to survey
        ///     viii.	Assign user to survey  
        ///     ix.	Add 10 calls
        ///     x.	Assign  call 1 to group 1
        ///     xi.	Assign call 2 to group 2
        ///     xii.	Assign  call 3 to group 3
        ///     xiii.	Assign  calls 4, 5 to user1
        ///     xiv.	Set Priority 100 to call 10
        ///     xv.	Login user to survey ( survey assignment mode)
        ///     xvi.	Run scheduling procedure  ( make sure that setting maxCallby� has value more then 10 )
        /// b.	Execute GetCallsPerGroup ( group 1 )  - in all requests use count of requested calls more then expected.
        ///     i.	test should return 1 call assigned to group 1 � we should check call ids
        ///         Only first call assigned to group 1 ( explicit group ) so only call 1 should be returned
        /// c.	Execute GetCallsPerGroup ( group 2 ) 
        ///     i.	test should return 2 call assigned to group 2 � we should check call ids
        ///         Only  call 2  assigned to group 2 ( explicit group ) so only call 2 should be returned
        /// d.	Execute GetCallsPerGroup ( group 3 ) 
        ///     i.	test should return 3 call assigned to group 3 � we should check call ids
        ///         Only  call 3  assigned to group 3 ( explicit group ) so only call 3 should be returned
        /// e.	Execute GetCallsPerGroup ( implicit survey  group )  request 10 calls
        ///     i.	test should return 7  calls � first 2 returned  calls ( 4,5)  in the list should be assigned to user1  
        ///         and other calls to implicit  survey group and the order should be 10,6,7,8,9.
        ///         Here we request calls for survey ( implicit group ). This request returns calls which are 
        ///         explicitly assigned to user(s) ( these calls should be returned first ) and 
        ///         calls assigned to a survey ( implicit group ).  
        ///         So 4,5 assigned to user1 so they are returned first, call 10 � has the higher priority then 6,7,8,9 
        ///         (all these calls are assigned to survey ( implicit group )).
        /// </summary>
        [Theory, Owner(@"FIRM\SergeyC")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void GetCallsPerGroup_GetListOfCallsForAllGroups_ListsAreCorrect(DialType dialType)
        {
            int group1SID = PersonTools.CreatePersonGroup("group1");
            int group2SID = PersonTools.CreatePersonGroup("group2");
            int group3SID = PersonTools.CreatePersonGroup("group3");
            int personSID = PersonTools.CreatePerson(
                UserName,
                Password,
                AgentTaskChoiceMode.CampaignAssignment,
                new[] { group1SID, group2SID, group3SID },
                dialType: dialType
            );

            int surveySID = PredictiveToolsObject.CreatePredictiveSurvey();
            PredictiveToolsObject.OpenSurvey(surveySID);

            BackendTools.AssignCatiPersonToSurvey(surveySID, group1SID);
            BackendTools.AssignCatiPersonToSurvey(surveySID, group2SID);
            BackendTools.AssignCatiPersonToSurvey(surveySID, group2SID);
            BackendTools.AssignCatiPersonToSurvey(surveySID, personSID);

            BvCallEntity[] calls = PredictiveTools.CreateCalls(surveySID, 10, dialType);

            // a.x
            PredictiveTools.AssignCallsToPerson(surveySID, calls.Where((c, index) => index == 0), group1SID);
            // a.xi
            PredictiveTools.AssignCallsToPerson(surveySID, calls.Where((c, index) => index == 1), group2SID);
            // a.xii
            PredictiveTools.AssignCallsToPerson(surveySID, calls.Where((c, index) => index == 2), group3SID);
            // a.xiii
            PredictiveTools.AssignCallsToPerson(surveySID, calls.Where((c, index) => index == 3 || index == 4), personSID);
            // a.xiv
            PredictiveTools.SetTimePriorityToCall(surveySID, calls.Where((c, index) => index == 9), 100, null, false);

            BackendTools.LoginPerson(personSID, "");
            PersonService.LoginPersonOnSurveyForSurveySelectionMode(personSID, surveySID);

            // check b point
            PredictiveTools.CheckCalls(
                new[] { calls[0].CallID },
                PredictiveTools.GetCallsPerGroup(surveySID, group1SID, 10, dialType)
            );

            // check c point
            PredictiveTools.CheckCalls(
                new[] { calls[1].CallID },
                PredictiveTools.GetCallsPerGroup(surveySID, group2SID, 10, dialType)
            );

            // check d point
            PredictiveTools.CheckCalls(
                new[] { calls[2].CallID },
                PredictiveTools.GetCallsPerGroup(surveySID, group3SID, 10, dialType)
            );

            // check e point
            IEnumerable<PredictiveCall> result = PredictiveTools.GetCallsPerGroup(surveySID, surveySID, 10, dialType);
            PredictiveTools.CheckCalls(
                new[] { calls[9].CallID, calls[5].CallID, calls[6].CallID, calls[7].CallID, calls[8].CallID },
                result
            );
        }

        /// <summary>
        /// a.	Prerequisites
        ///     i.	Add user1
        ///     ii.	Add group 1
        ///     iii.	Make user a member of this group
        ///     iv.	Add survey 
        ///     v.	Assign user and a group to survey
        ///     vi.	Add  10 calls, set call time to now, priority 1
        ///     vii.	Set priority for call 2 � 100
        ///     viii.	Set priority for call  6 � 100
        ///     ix.	Assign  calls 5,6,7 to user 1
        ///     x.	Login user to survey ( survey assignment mode )
        ///     xi.	Run scheduling procedure  ( make sure that setting maxCallby� has value more then 10 )
        /// b.	Execute GetCallsPerGroup ( implicit survey  group )  request 10 calls
        ///     i.	Test should return calls in the following sequence 2,1,3,4,8,9,10
        ///         2,1,3,4,8,9,10 are assigned to implicit group so returned after calls assigned to a user 
        ///         and call 2 has higher priority then others
        /// c.	Execute GetCallsPerGroup (  group 1 )  - request 10 calls
        ///     i.	Test should return 0 calls
        ///         there are no calls assigned to group 1
        /// </summary>
        [Theory, Owner(@"FIRM\SergeyC")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void GetCallsPerGroup_GetListOfCallsForSingleGroupWithDifferentTimePriorityProperties_ListIsCorrect(DialType dialType)
        {
            int group1SID = PersonTools.CreatePersonGroup("group1");
            int personSID = PersonTools.CreatePerson(
                UserName,
                Password,
                AgentTaskChoiceMode.CampaignAssignment,
                new[] { group1SID },
                dialType: dialType
            );

            int surveySID = PredictiveToolsObject.CreatePredictiveSurvey();
            PredictiveToolsObject.OpenSurvey(surveySID);

            BackendTools.AssignCatiPersonToSurvey(surveySID, group1SID);
            BackendTools.AssignCatiPersonToSurvey(surveySID, personSID);

            BvCallEntity[] calls = PredictiveTools.CreateCalls(surveySID, 10, dialType);
            // a.vi
            PredictiveTools.SetTimePriorityToCall(surveySID, calls, 1, null, true);
            // a.vii, a.viii
            PredictiveTools.SetTimePriorityToCall(surveySID, calls.Where((c, index) => index == 1 || index == 5), 100, null, false);
            // a.ix
            PredictiveTools.AssignCallsToPerson(surveySID, calls.Where((c, index) => index >= 4 && index <= 6), personSID);

            BackendTools.LoginPerson(personSID, "");
            PersonService.LoginPersonOnSurveyForSurveySelectionMode(personSID, surveySID);

            // check point b
            PredictiveTools.CheckCalls(
                new[] { calls[1].CallID, calls[0].CallID, calls[2].CallID, calls[3].CallID, calls[7].CallID, calls[8].CallID, calls[9].CallID },
                PredictiveTools.GetCallsPerGroup(surveySID, surveySID, 10, dialType)
            );

            // check point c
            PredictiveTools.CheckCalls(
                new int[0],
                PredictiveTools.GetCallsPerGroup(surveySID, group1SID, 10, dialType)
            );
        }

        /// <summary>
        /// a.	Prerequisites
        ///     i.	Add user1
        ///     ii.	Add group 1
        ///     iii.	Make user a member of this group
        ///     iv.	Add survey 
        ///     v.	Assign user and a group to survey  
        ///     vi.	Add 10 calls
        ///     vii.	Assign calls 1-3, 9 to group 1
        ///     viii.	Set priority 100 to call 9
        ///     ix.	Assign  6,7,8  calls to a user1 
        ///     x.	Login user to survey ( survey assignment mode)
        ///     xi.	Run scheduling procedure  ( make sure that setting maxCallby� has value more then 10 )
        /// b.	Execute GetCallsPerGroup ( implicit group )  - request   3 calls.
        ///     i.	test should return 6,7,8 calls
        ///         We request 3 calls so only 3 calls are returned and these calls are assigned to a user 
        ///         so they should be returned first 
        /// c.	Execute GetCallsPerGroup ( implicit group )  - request   10 calls.
        ///     i.	test should return 6,7,8,4,5,10 calls
        ///         We have only 6 to return ( 3 assigned to a user, 3 to implicit group ) so only 6 calls are returned and 
        ///         calls assigned to a user returned first
        /// d.	Execute GetCallsPerGroup ( group 1 )  request 10 calls.
        ///     i.	test should return  calls  9,1,2,3
        ///         Four calls are assigned to group 1 so 4 calls should be returned and call 9 has higher priority
        /// </summary>
        [Theory, Owner(@"FIRM\SergeyC")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void GetCallsPerGroup_GetListOfCallsForSinglePerson_ListIsCorrect(DialType dialType)
        {
            int group1SID = PersonTools.CreatePersonGroup("group1");
            int personSID = PersonTools.CreatePerson(
                UserName,
                Password,
                AgentTaskChoiceMode.CampaignAssignment,
                new[] { group1SID },
                dialType: dialType
            );

            int surveySID = PredictiveToolsObject.CreatePredictiveSurvey();
            PredictiveToolsObject.OpenSurvey(surveySID);

            BackendTools.AssignCatiPersonToSurvey(surveySID, group1SID);
            BackendTools.AssignCatiPersonToSurvey(surveySID, personSID);

            BvCallEntity[] calls = PredictiveTools.CreateCalls(surveySID, 10, dialType);

            // a.vii
            PredictiveTools.AssignCallsToPerson(surveySID, calls.Where((c, index) => (index >= 0 && index <= 2) || index == 8), group1SID);
            // a.viii
            PredictiveTools.SetTimePriorityToCall(surveySID, calls.Where((c, index) => index == 8), 100, null, false);
            // a.ix
            PredictiveTools.AssignCallsToPerson(surveySID, calls.Where((c, index) => index >= 5 && index <= 7), personSID);

            BackendTools.LoginPerson(personSID, "");
            PersonService.LoginPersonOnSurveyForSurveySelectionMode(personSID, surveySID);

            // check b point //Just to check that so called default group id not in force any more 
            const int defaultGroupId = 1;
            PredictiveTools.CheckCalls(
                new int [] {},
                PredictiveTools.GetCallsPerGroup(surveySID, defaultGroupId, 3, dialType)
            );

            // check c point
            PredictiveTools.CheckCalls(
                new[] { calls[3].CallID, calls[4].CallID, calls[9].CallID },
                PredictiveTools.GetCallsPerGroup(surveySID, surveySID, 10, dialType)
            );
            //Release calls in order to use them again
            PredictiveTools.ReleaseCalls(new[] { calls[5], calls[6], calls[7], calls[3], calls[4], calls[9] });

            // check d point
            PredictiveTools.CheckCalls(
                new[] { calls[8].CallID, calls[0].CallID, calls[1].CallID, calls[2].CallID },
                PredictiveTools.GetCallsPerGroup(surveySID, group1SID, 10, dialType)
            );
        }

        /// <summary>
        /// a.	Prerequisites
        ///     1.	Add user 1
        ///     2.  Add user 2
        ///     3.	Add group 1
        ///     4.	Add group 2
        ///     5.	Make user 1 a member of group 1
        ///     6.  Make user 2 a member of group 2
        ///     7.	Add survey 
        ///     8.	Assign group 1 to survey
        ///     9.	Add 5 calls
        ///     10.	Assign  call 1 to group 2
        ///     11.	Login user 1 to survey ( survey assignment mode)
        ///     12. Login user 2
        ///     13. Run scheduling procedure  ( make sure that setting maxCallby� has value more then 10 )
        ///     14. Log out user 2
        /// b.  Execute GetCallsPerGroup ( group 1 )  request 5 calls
        ///     i.  should return all calls except call 1.
        /// </summary>
        [Theory, Owner(@"FIRM\SergeyC")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void GetCallsPerGroup_DoNotReturnCallsOfGroupsWhichAreNotLoggedIn_ListIsCorrect(DialType dialType)
        {
            int group1SID = PersonTools.CreatePersonGroup("group1");
            int group2SID = PersonTools.CreatePersonGroup("group2");
            // a.5
            int personSID = PersonTools.CreatePerson("user", "pass", AgentTaskChoiceMode.CampaignAssignment, new[] { group1SID }, dialType);
            // a.6
            int person2SID = PersonTools.CreatePerson("user2", "pass2", AgentTaskChoiceMode.CampaignAssignment, new[] { group2SID }, dialType);
            // a.7
            int surveySID = PredictiveToolsObject.CreatePredictiveSurvey();
            PredictiveToolsObject.OpenSurvey(surveySID);
            // a.8
            BackendTools.AssignCatiPersonToSurvey(surveySID, group1SID);

            BvCallEntity[] calls = PredictiveTools.CreateCalls(surveySID, 5, dialType);
            // a.10
            PredictiveTools.AssignCallsToPerson(surveySID, calls.Where((c, index) => index == 0), group2SID);

            BackendTools.LoginPerson(personSID, "");
            BackendTools.LoginPerson(person2SID, "");
            PersonService.LoginPersonOnSurveyForSurveySelectionMode(personSID, surveySID);
            PersonService.LoginPersonOnSurveyForSurveySelectionMode(person2SID, surveySID);

            TaskService.RemoveTaskAndLogoutPerson(person2SID);

            PredictiveTools.CheckCalls(
                new[] { calls[1].CallID, calls[2].CallID, calls[3].CallID, calls[4].CallID },
                PredictiveTools.GetCallsPerGroup(surveySID, surveySID, 5, dialType)
            );
        }
    }
}
