using System;
using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Core.PriorityGroups;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class AddCallGroupStatuses : BaseForm
    {
        [StoreInViewState]
        protected int CallGroupId;

        private readonly IPriorityGroupsManager _priorityGroupsManager = ServiceLocator.Resolve<IPriorityGroupsManager>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["CallGroupId"] != null)
                CallGroupId = Convert.ToInt32(Request["CallGroupId"]);

            grid.GetPage = delegate(out int totalCount)
               {
                   var list = _priorityGroupsManager.GetNotIncludedStatuses(CallGroupId);
                   return BaseMethods.GetPage(list, grid.PageArguments, out totalCount);
              };
        }

        protected void SaveHandler(object sender, EventArgs e)
        {
            try
            {
                List<int> selectedItses = grid.SelectedKeysInt;

                if (selectedItses.Count == 0)
                {
                    CloseOverlay();
                    return;
                }

                using (var transaction = new DatabaseTransactionScope("AddPriorityGroupStatuses", DeadlockPriority.Supervisor))
                {
                    _priorityGroupsManager.AddStatuses(CallGroupId, selectedItses);

                    transaction.Commit();
                }                

                CloseOverlay(true);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }
    }
}