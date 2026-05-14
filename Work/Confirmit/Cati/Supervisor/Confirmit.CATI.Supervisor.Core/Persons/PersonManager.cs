using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.CATI.Core.Validators.Interfaces;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Core.Exceptions;
using Confirmit.CATI.Supervisor.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Confirmit.CATI.Supervisor.Core.Persons
{
    /// <summary>
    /// Class responsible for common operations with CATI interviewers
    /// </summary>
    public static class PersonManager
    {
        public const int BasePersonGroupParentId = 0;

        /// <summary>
        /// Deletes person with specified SID
        /// </summary>
        /// <param name="personIds"></param>
        public static void DeletePersons(List<int> personIds)
        {
            foreach (var personId in personIds)
            {
                if (PersonService.IsPersonLoggedIn(personId))
                {
                    throw new PersonLoggedInException(personId);
                } 
            }

            var supervisorServiceClient = ServiceLocator.Resolve<ISupervisorServiceClient>();
            supervisorServiceClient.DeletePersons(personIds);
        }

        /// <summary>
        /// Creates Cati person group with the parameter specified.
        /// </summary>
        /// <param name="group">Person group entity to insert/create</param>
        /// <param name="parentSids">Group's parent SIDs</param>
        /// <returns>Created group's SID</returns>
        public static int CreatePersonGroup(BvPersonGroupEntity group, int[] parentSids)
        {
            int sid = ServiceLocator.Resolve<IPersonGroupRepository>().Insert(group);
            PersonGroupService.SetParentGroups(sid, parentSids);

            return sid;
        }

        /// <summary>
        /// Deletes persons group with specified SID
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public static void DeletePersonGroup(int sid)
        {
            List<BvSpPerson_ListByParentEntity> list = PersonGroupService.GetChildPersonsForAllCallCenters(sid);

            if (list.Count > 0)
            {
                throw new GroupNotEmptyException(sid);
            }

            PersonGroupRepository.Delete(sid);
        }

        /// <summary>
        /// Returns SID of CATI Interviewers group.
        /// </summary>
        /// <returns></returns>
        public static int GetCatiRootID()
        {
            return PersonGroupService.RootGroupId;
        }

        /// <summary>
        /// Determins if group with the specified name is root group of the specified type.
        /// </summary>
        public static bool IsRootGroup(int groupId)
        {
            return groupId == PersonGroupService.RootGroupId;
        }

        // TODO: remove and use GetPersonsLevel instaed.
        /// <summary>
        /// Returns a level of person's hierarchy with specified parent filtered by specified survey
        /// </summary>
        /// <param name="parentId">SID of the parent element, -1 if root</param>
        /// <param name="surveyId">Survey to filter by, -1 if no filtering</param>
        /// <param name="filter">Part of the person's or group's name to filter by (empty string if no filter).</param>
        /// <returns></returns>		
        public static List<PersonGroupInfo> GetPersonsHierarchyLevel(
            int parentId,
            int surveyId,
            string filter)
        {
            filter = SearchManager.FormatLikeValueForSql(filter);

            var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();

            return (from c in BvSpPersonAndGroups_ListAdapter.ExecuteEntityList(
                        parentId,
                        surveyId != -1 ? surveyId : (int?)null,
                        String.IsNullOrEmpty(filter) ? null : filter,
                        callCenterId)
                    select new PersonGroupInfo(
                        c.isGroup.Value == 1,
                        c.SID.Value,
                        c.UserName,
                        c.MembersCount.Value,
                        c.CurSurvAssign.Value,
                        c.AllSurvAssign.Value,
                        c.TotalAssignedSurveys.Value,
                        c.IsAssignedOnCurrentSurvey.Value > 0
                    )
                ).ToList();
        }

        /// <summary>
        /// Returns immediate child persons of parent group.
        /// </summary>
        /// <param name="parentSID">SID of parent group.</param>
        /// <param name="filter">Person name filter.</param>
        /// <returns></returns>
        public static List<PersonInfoItem> GetPersonsLevel(int parentSID, string filter)
        {
            filter = SearchManager.FormatLikeValueForSql(filter);

            var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();

            return (from c in BvSpGetPersonsLevelAdapter.ExecuteEntityList(parentSID, String.IsNullOrEmpty(filter) ? null : filter, callCenterId)
                    orderby c.Name
                    select new PersonInfoItem(c.SID.Value, c.Name)).ToList();
        }

        /// <summary>
        /// Returns immediate child person groups of parent group.
        /// </summary>
        /// <param name="parentSid">SID of parent group.</param>
        /// <param name="filter">Person name filter (used to count number of child items).</param>
        /// <returns></returns>
        public static List<PersonGroupInfoItem> GetPersonGroupsLevel(int parentSid, string filter)
        {
            filter = SearchManager.FormatLikeValueForSql(filter);

            var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();

            return (from c in BvSpGetPersonGroupsLevelAdapter.ExecuteEntityList(parentSid, String.IsNullOrEmpty(filter) ? null : filter, callCenterId)
                    orderby c.Name
                    select new PersonGroupInfoItem(c.SID.Value, c.Name, c.Description, (InboundGroupBehavior)c.InboundCallBehavior, (TransferGroupBehavior)c.CallTransferBehavior, c.Count.Value)).ToList();
        }

        /// <summary>
        /// Returns immediate all person groups.
        /// </summary>
        /// <param name="filter">Person name filter (used to count number of child items).</param>
        /// <returns></returns>
        public static List<PersonGroupInfoItem> GetAllPersonGroups(string filter)
        {
            filter = SearchManager.FormatLikeValueForSql(filter);

            var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();

            return (from c in BvSpGetPersonGroupsAdapter.ExecuteEntityList(string.IsNullOrEmpty(filter) ? null : filter, callCenterId)
                orderby c.Name
                select new PersonGroupInfoItem(c.SID.Value, c.Name, c.Description, (InboundGroupBehavior)c.InboundCallBehavior,
                    (TransferGroupBehavior)c.CallTransferBehavior, c.Count.Value, c.IsAdministrative.Value)).ToList();
        }

        public static List<CatiUserItem> GetPersonList()
        {
            var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();

            return PersonRepository.GetAll(callCenterId)
                                   .Select(x => new CatiUserItem(x.SID, x.Name, x.Description)).ToList();
        }

        /// <summary>
        /// Returns single page of Persons list.
        /// </summary>
        /// <param name="args">Paging args</param>
        /// <param name="totalCount">Returns persons total count.</param>
        /// <returns></returns>
        public static List<BvSpGetPersonsListPageEntity> GetPersonsListPage(PagingArgs args, out int totalCount)
        {
            var groupIds = PersonGroupRepository.GetAll();
            var groupList = string.Join(",", groupIds.Select(g => g.SID.ToString()).ToList());

            return GetPersonsListPage(groupList, args, out totalCount);
        }

        /// <summary>
        /// Returns single page of Persons list.
        /// </summary>
        /// <param name="ParentGroupsIDs">String of comma delimited IDs of folders</param>
        /// <param name="args">Paging args</param>
        /// <param name="totalCount">Returns persons total count.</param>
        /// <returns></returns>
        public static List<BvSpGetPersonsListPageEntity> GetPersonsListPage(string ParentGroupsIDs, PagingArgs args, out int totalCount)
        {
            var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();
            return PersonRepository.GetPage(ParentGroupsIDs, args, callCenterId, out totalCount);
        }

        public static List<PersonAndGroupInfoItem> GetAllPersonsAndGroups(bool includeAdministrativeGroups = true)
        {
            var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();

            return BvSpGetAllPersonsAndGroupsAdapter.ExecuteEntityList(callCenterId, null, includeAdministrativeGroups)
                                                    .Select(x => new PersonAndGroupInfoItem(x.Id.Value, x.Name, x.Description, x.IsGroup.Value)).ToList();
        }

        public static List<PersonAndGroupInfoItem> GetAllNotAssignedPersonsAndGroups(int surveyId)
        {
            var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();

            return BvSpGetAllPersonsAndGroupsAdapter.ExecuteEntityList(callCenterId, surveyId, true)
                                                    .Select(x => new PersonAndGroupInfoItem(x.Id.Value, x.Name, x.Description, x.IsGroup.Value)).ToList();
        }

        /// <summary>
        /// Returns list of groups under group with passed sid.
        /// </summary>
        public static List<CatiGroupItem> GetPersonGroups(int ParentID)
        {
            var groups = new List<CatiGroupItem>();
            var group = new CatiGroupItem(ParentID);

            if (ParentID == GetCatiRootID())
            {
                group.Name = Strings.CatiInterviewersGroupName;
            }

            groups.Add(group);
            FillListWithPersonGroups(ref groups, ParentID);
            return groups;
        }

        /// <summary>
        /// Recursive fills list with groups.
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="ParentID"></param>
        private static void FillListWithPersonGroups(ref List<CatiGroupItem> groups, int ParentID)
        {
            List<BvSpPersonGroup_ListEntity> list = PersonGroupService.GetChildGroups(ParentID);

            foreach (BvSpPersonGroup_ListEntity group in list)
            {
                int sid = group.SID.Value;
                string name = group.Name;

                if (!groups.Contains(new CatiGroupItem(sid, name)))
                    groups.Add(new CatiGroupItem(sid, name));
                FillListWithPersonGroups(ref groups, sid);
            }
        }

        /// <summary>
        /// Looks up if person name is used for any person.
        /// </summary>
        /// <param name="name">Name to look up.</param>
        /// <returns>True if person with such name found; false otherwise.</returns>
        public static bool IsPersonNameUsed(string name)
        {
            int foundSid;

            return PersonService.IsNameUsed(name, out foundSid);
        }

        /// <summary>
        /// Looks up for person with specified name.
        /// </summary>
        /// <param name="name">Name to look up.</param>
        /// <returns>Person ID if person found; 0 otherwise.</returns>
        public static int LookupPersonName(string name)
        {
            int foundSid;

            if (PersonService.IsNameUsed(name, out foundSid))
            {
                return foundSid;
            }

            return 0;
        }

        /// <summary>
        /// Looks up if persons group name is used for group with another id (differs from specified).
        /// </summary>
        /// <param name="name">Name to look up.</param>
        /// <param name="sid">Current group sid.</param>
        /// <returns>True if another group with such name found; false otherwise.</returns>
        public static bool IsPersonGroupNameUsed(string name, int sid)
        {
            int foundSid;
            bool result = PersonGroupService.IsNameUsed(name, out foundSid);

            return result && foundSid != sid;
        }

        /// <summary>
        /// Looks up if persons group name is used for any group.
        /// </summary>
        /// <param name="name">Name to look up.</param>
        /// <returns>True if group with such name found; false otherwise.</returns>
        public static bool IsPersonGroupNameUsed(string name)
        {
            int foundSid;
            return PersonGroupService.IsNameUsed(name, out foundSid);
        }

        public static bool IsPersonGroupNameValid(string name)
        {
            return ServiceLocator.Resolve<IPersonGroupValidator>().IsNameValid(name);
        }

        public static void FillArrayWithUsers(int gID, ref List<CatiUserItem> users, ref Hashtable ht, int callCenterId)
        {
            List<BvSpPerson_ListByParentEntity> personsList = PersonGroupService.GetChildPersons(gID, callCenterId);

            foreach (BvSpPerson_ListByParentEntity person in personsList)
            {
                int sid = person.SID.Value;
                string name = person.Name;
                string description = person.Description;

                if (ht[sid] == null)
                {
                    var user = new CatiUserItem(sid, name, description);
                    users.Add(user);
                    ht[sid] = user;
                }
            }

            List<BvSpPersonGroup_ListEntity> groupsList = PersonGroupService.GetChildGroups(gID);

            foreach (BvSpPersonGroup_ListEntity group in groupsList)
            {
                int sid = group.SID.Value;
                FillArrayWithUsers(sid, ref users, ref ht, callCenterId);
            }
        }

        /// <summary>
        /// Recursive. Returns list of all persons under group with passed sid and its subgroups. All persons exist in the list only once.
        /// </summary>
        public static List<CatiUserItem> GetAllPersons(int parentId, int callCenterId)
        {
            Hashtable ht = new Hashtable();
            List<CatiUserItem> users = new List<CatiUserItem>();
            FillArrayWithUsers(parentId, ref users, ref ht, callCenterId);
            return users;
        }
        
        public static List<CatiUserItem> GetAllPersons(IEnumerable<int> parentIds, int callCenterId)
        {
            Hashtable ht = new Hashtable();
            var users = new List<CatiUserItem>();
            foreach (var parentId in parentIds)
            {
                FillArrayWithUsers(parentId, ref users, ref ht, callCenterId);
            }
            
            return users;
        }
    }
}
