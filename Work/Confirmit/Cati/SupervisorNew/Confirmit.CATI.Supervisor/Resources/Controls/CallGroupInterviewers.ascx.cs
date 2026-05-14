using System;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Common;
using Confirmit.CATI.Supervisor.Core.PriorityGroups;

namespace Confirmit.CATI.Supervisor.Resources.Controls
{
    public partial class CallGroupInterviewers : BaseWUC
    {
        [StoreInViewState]
        protected int CallGroupId;

        private readonly IPriorityGroupsManager _priorityGroupsManager = ServiceLocator.Resolve<IPriorityGroupsManager>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["CallGroupId"] != null)
                CallGroupId = Convert.ToInt32(Request["CallGroupId"]);

            var group = _priorityGroupsManager.GetGroup(CallGroupId);
            
            grid.GridName = String.Format(Strings.CallGroupStatuses, group.Name);

            grid.GetPage += GetPage;
            
        }

        protected void DeassignInterviewers(object sender, EventArgs e)
        {
            try
            {
                using (var transaction = new DatabaseTransactionScope("DeletePriorityGroupInterviewers", DeadlockPriority.Supervisor))
                {
                    _priorityGroupsManager.DeleteInterviewerAssignment(grid.SelectedKeysInt);
                    
                    transaction.Commit();
                }
                grid.BindData();                    
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

      /// <summary>
      /// Returns page of information to show in grid.
      /// </summary>
      protected object GetPage(out int totalCount)
      {
          var usersList = PersonRepository.GetAllAssignedOnCallGroup(CallGroupId);

          return BaseMethods.GetPage(usersList, grid.PageArguments, out totalCount);
      }
    }
}