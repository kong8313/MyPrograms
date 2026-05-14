using System.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;

using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services
{
    public class PersonGroupService : IPersonGroupService
    {
        private readonly IPersonGroupRepository _personGroupRepository;

        public PersonGroupService( IPersonGroupRepository personGroupRepository)
        {
            _personGroupRepository = personGroupRepository;
        }


        private enum GroupAction
        {
            Delete = 0,
            Skip = 1,
            Insert = 2
        }

        public static int[] GetParentGroups(int sid)
        {
            var list = BvMembershipAdapter.GetByCondition("ObjectSID = @ObjectSID", new SqlParameter("@ObjectSID", sid));
            return list.Select(x => x.ContainerSID).ToArray();
        }

        /// <summary>
        /// Sets parent groups for the GROUP.
        /// Not for the PERSON.
        /// To set parent groups for the person see PersonService.SetParentGroups
        /// </summary>
        /// <param name="sid">Group SID</param>
        /// <param name="parentGroupsSid">Group parent groups</param>
        public static void SetParentGroups(int sid, int[] parentGroupsSid)
        {
            var groupsToAssign = new Dictionary<int, GroupAction>();
            var newGroups = new List<int>(parentGroupsSid);

            foreach (int oldGroupId in GetParentGroups(sid))
            {
                groupsToAssign[oldGroupId] = newGroups.Contains(oldGroupId) ? GroupAction.Skip : GroupAction.Delete;
            }

            //
            // new groups that do not exist in old groups list should be inserted
            foreach (int newGroupId in newGroups)
            {
                if (!groupsToAssign.ContainsKey(newGroupId))
                {
                    groupsToAssign[newGroupId] = GroupAction.Insert;
                }
            }

            //
            // process groups
            foreach (int groupId in groupsToAssign.Keys)
            {
                GroupAction action = groupsToAssign[groupId];

                if (action == GroupAction.Insert)
                {
                    BvSpMembership_InsertAdapter.ExecuteNonQuery(
                        groupId,
                        sid);
                }
                else if (action == GroupAction.Delete)
                {
                    BvSpMembership_DeleteAdapter.ExecuteNonQuery(
                        groupId,
                        sid);
                }
            }
        }


        public static List<BvSpPersonGroup_ListEntity> GetChildGroups(int sid)
        {
            return BvSpPersonGroup_ListAdapter.ExecuteEntityList(sid);
        }


        /// <summary>
        /// Recursively gets IDs of all child groups of the current group.
        /// </summary>
        public static int[] GetAllChildNotAdministrativeGroupSids(int sid)
        {
            var childGroupSids = GetChildGroups(sid).Where(x => x.IsAdministrative == false).Select(x => x.SID.Value);

            return childGroupSids
                .Union(childGroupSids.SelectMany(x => GetAllChildNotAdministrativeGroupSids(x)))
                .Distinct()
                .ToArray();
        }


        /// <summary>
        /// Recursively gets IDs of all parent groups of the current group.
        /// </summary>
        public static int[] GetAllParentGroupSids(int sid)
        {
            return GetParentGroups(sid)
                .Union(GetParentGroups(sid).SelectMany(x => GetAllParentGroupSids(x)))
                .Distinct()
                .ToArray();
        }
        
        public static List<BvSpPerson_ListByParentEntity> GetChildPersons(int sid, int callCenterId)
        {
            return BvSpPerson_ListByParentAdapter.ExecuteEntityList(sid, callCenterId);
        }

        public static bool IsNameUsed(
            string name,
            out int personGroupId)
        {
            personGroupId = 0;

            var personGroup = PersonGroupRepository.TryGetByName(name);

            if (personGroup == null)
                return false;

            personGroupId = personGroup.SID;

            return true;
        }

        /// <summary>
        /// Gets CATI root group id.
        /// </summary>
        public static int RootGroupId
        {
            get
            {
                return BvSpPersonGroup_GetRootGroupAdapter.ExecuteScalar<int>();
            }
        }

        /// <summary>
        /// Creates Cati person group with the parameter specified.
        /// </summary>
        /// <param name="name">Group's name</param>
        /// <param name="description">Group's description</param>
        /// <param name="parentSids">Group's parent SIDs</param>
        /// <returns>Created group's SID</returns>
        public static int CreatePersonGroup(string name, string description, int[] parentSids, bool isAdministrative = false)
        {
            var group = new BvPersonGroupEntity { Name = name, Description = description, IsAdministrative = isAdministrative };
            int sid = ServiceLocator.Resolve<IPersonGroupRepository>().Insert(group);
            SetParentGroups(sid, parentSids);

            return sid;
        }

        public static List<BvSpPerson_ListByParentEntity> GetChildPersonsForAllCallCenters(int sid)
        {
            return BvSpPerson_ListByParentAdapter.ExecuteEntityList(sid, 0);
        }

        public bool IsExistsAndNotAdministrative(int[] groupIds)
        {
            foreach (var groupId in groupIds)
            {
                var group = _personGroupRepository.TryGetById(groupId);
                if (group == null || group.IsAdministrative)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsGroupContainsInterviewer(int interviewerId, string groupName)
        {
            var groupItem = _personGroupRepository.TryGetByName(groupName);

            if (groupItem == null)
                return false;

            var list = BvMembershipAdapter.GetByCondition("ObjectSID = @ObjectSID AND ContainerSID = @ContainerSID", 
                new SqlParameter("@ObjectSID", interviewerId),
                new SqlParameter("@ContainerSID", groupItem.SID));

            return list.Count > 0;
        }
    }
}
