using System;
using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.PriorityGroups;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class AddCallGroupInterviewerAssignment : BaseForm
    {
        [StoreInViewState]
        protected int CallGroupId;

        private readonly IPriorityGroupsManager _priorityGroupsManager = ServiceLocator.Resolve<IPriorityGroupsManager>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["CallGroupId"] != null)
                    CallGroupId = Convert.ToInt32(Request["CallGroupId"]);
            }

            grid.GetPage += (out int totalCount) => PriorityGroupsManager.GetPersonsPageNotInGroup(CallGroupId, grid.PageArguments, out  totalCount);
        }

        protected void OKButtonClick(object sender, EventArgs e)
        {
            try
            {
                List<int> selectedInterviewers = grid.SelectedKeysInt;
                if (selectedInterviewers.Count == 0)
                {
                    CloseOverlay();
                    return;
                }

                using (var transaction = new DatabaseTransactionScope("AssignInterviewersToCallGroup", DeadlockPriority.Supervisor))
                {
                    _priorityGroupsManager.AddInterviewerAssignment(CallGroupId, selectedInterviewers);     
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