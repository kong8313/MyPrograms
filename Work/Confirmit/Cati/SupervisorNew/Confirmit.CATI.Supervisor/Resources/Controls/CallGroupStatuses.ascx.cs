using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Classes;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Common;
using Confirmit.CATI.Supervisor.Core.PriorityGroups;

namespace Confirmit.CATI.Supervisor.Resources.Controls
{
    public partial class CallGroupStatuses : BaseWUC
    {
        [StoreInViewState]
        protected int PriorityGroupId;

        private readonly IPriorityGroupsManager _priorityGroupsManager = ServiceLocator.Resolve<IPriorityGroupsManager>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["CallGroupId"] != null)
                PriorityGroupId = Convert.ToInt32(Request["CallGroupId"]);

            var group = _priorityGroupsManager.GetGroup(PriorityGroupId);
            
            grid.GridName = String.Format(Strings.CallGroupStatuses, group.Name);
        
            grid.GetPage += delegate(out int totalCount)
            {
                List<PriorityGroupStatus> list = _priorityGroupsManager.GetStatusesByGroupId(PriorityGroupId);

                return BaseMethods.GetPage(list, grid.PageArguments, out totalCount);
            };
        }

      protected void Delete(object sender, EventArgs e)
        {
            try
            {
                using (var transaction = new DatabaseTransactionScope("DeletePriorityGroupStatuses", DeadlockPriority.Supervisor))
                {
                    foreach (int its in grid.SelectedKeysInt)
                    {
                        _priorityGroupsManager.DeleteStatus(PriorityGroupId, its);
                    }

                    grid.BindData();                    

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }
    }
}