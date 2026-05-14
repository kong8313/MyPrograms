using System;
using System.Collections.Generic;
using System.Globalization;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Supervisor.Core.Resources;
using Confirmit.CATI.Supervisor.Core.Exceptions;
using Confirmit.CATI.Supervisor.Core.Timezone;

namespace Confirmit.CATI.Supervisor.Core.ITSs
{
    public class StateGroupsManager
    {
        /// <summary>
        /// Gets all states for the group.
        /// </summary>
        /// <param name="stateGroupID">StateGroup ID</param>
        /// <returns>Collection of states</returns>
        public static List<BvSpState_ListEntity> GetITSList(int stateGroupID)
        {
            return ServiceLocator.Resolve<IStateGroupService>().GetStates(stateGroupID);
        }

        /// <summary>
        /// Gets default group ITS list.
        /// </summary>
        /// <returns>Default group ITS list</returns>
        public static List<BvSpState_ListEntity> GetDefaultITSList()
        {
            return GetITSList(0);
        }

        public static List<BvSpState_ListEntity> GetITSListForScript(int schedulingScriptId)
        {
            var script = ScheduleRepository.GetById(schedulingScriptId);
            return script.DesignStateGroupID.HasValue ? GetITSList(script.DesignStateGroupID.Value) : GetDefaultITSList();
        }

        public static BvStateGroupEntity GetStateGroupForScript(BvScheduleEntity script)
        {
            return script.DesignStateGroupID.HasValue ? StateGroupRepository.GetById(script.DesignStateGroupID.Value) : StateGroupRepository.GetDefault();
        }

        /// <summary>
        /// Creates new state group base on Default group.
        /// </summary>
        /// <param name="name">New group name</param>
        public static int AddStateGroup(string name)
        {
            return CopyStateGroup(name, 0);
        }

        /// <summary>
        /// Creates new state group based on group specified by copyID.
        /// </summary>
        /// <param name="name">New group name</param>
        /// <param name="copyID">Base group ID</param>
        /// <returns>Created state group SID</returns>
        public static int CopyStateGroup(string name, int copyID)
        {
            if (CheckGroupNameExists(name))
            {
                throw new ArgumentException(Strings.GroupNameExistMessage);
            }

            var stateGroup = new BvStateGroupEntity {Name = name};

            return StateGroupRepository.Insert(copyID, stateGroup);
        }

        public static void CopyToDefaultGroup(int customStateGroupId, DateTime clientTime)
        {
            var defaultStateGroup = StateGroupRepository.GetDefault();

            if (customStateGroupId == defaultStateGroup.ID)
            {
                throw new UserMessageException(string.Format(Strings.Error_TryToCopyDefaultStatusGroupIntoItself, defaultStateGroup.Name));
            }

            CopyStateGroup($"{defaultStateGroup.Name} backup ({clientTime.ToString("dd/MMM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)})", defaultStateGroup.ID);

            var customStateGroup = StateGroupRepository.GetById(customStateGroupId);

            var evt = new CopyToDefaultStateGroupEvent(defaultStateGroup.ID, defaultStateGroup.Name, customStateGroupId, customStateGroup.Name);

            BvSpStateGroup_CopyToDefaultAdapter.ExecuteNonQuery(defaultStateGroup.ID, customStateGroup.ID);

            evt.Finish();
        }

        /// <summary>
        /// Returns ITS object with specified ID
        /// </summary>
        /// <param name="its_id"></param>
        /// <returns></returns>
        public static BvSpState_ListEntity GetITSByID(int its_id)
        {
            BvSpState_ListEntity result = GetDefaultITSList().Find(x => x.StateID == its_id);
            if (result == null)
            {
                throw new ITSNotFoundException(its_id);
            }

            return result;
        }

        /// <summary>
        /// Checks if specified group name already exists.
        /// </summary>
        /// <param name="name">Name for check</param>
        /// <returns>True if name exists. False otherwise.</returns>
        public static bool CheckGroupNameExists(string name)
        {
            var stateGroup = StateGroupRepository.GetByName(name);

            return stateGroup != null;
        }
    }
}
