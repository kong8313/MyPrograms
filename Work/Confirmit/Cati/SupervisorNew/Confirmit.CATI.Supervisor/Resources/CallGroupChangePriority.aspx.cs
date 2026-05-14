using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.PriorityGroups;

namespace Confirmit.CATI.Supervisor.Resources
{
    /// <summary>
    /// Pop up window used to change priority of its-statuses of priority group.
    /// </summary>
    public partial class CallGroupChangePriority : BaseForm
    {
        private List<int> _Itses;

        #region Properties

        [StoreInViewState]
        protected int PriorityGroupId;


        private List<int> Itses
        {
            get
            {
                if (_Itses == null)
                {
                    var requestIds = (string)ViewState["IDS"];

                    var ids = requestIds.Split(',');

                    _Itses = ids.Select(Int32.Parse).ToList();
                }
                return _Itses;
            }
        }

        private int Priority
        {
            get
            {
                if (nePriority.ValueInt != nePriority.ValueLong)
                {
                    throw new UserMessageException(string.Format(Strings.ValueYouHaveEnteredIsTooLarge, Int32.MaxValue));
                }

                return nePriority.ValueInt;
            }
            set
            {
                nePriority.ValueInt = value;
            }
        }       
      
        #endregion

        private readonly IPriorityGroupsManager _priorityGroupsManager = ServiceLocator.Resolve<IPriorityGroupsManager>();

        #region Event Handlers

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {            
            if (!IsPostBack)
            {
                if (Request["CallGroupId"] != null)
                    PriorityGroupId = Convert.ToInt32(Request["CallGroupId"]);

                ViewState["IDS"] = Request.Params["IDS"];

                var statuses = _priorityGroupsManager.GetStatusesByGroupId(PriorityGroupId);

                Priority = statuses.First(x => Itses.Contains(x.Id)).Priority;
            }
        }

        /// <summary>
        /// Handles the OK button click event.
        /// Updates the quota cells limit with the specified value via the web service.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void OKButtonClick(object sender, EventArgs e)
        {
            try
            {
                _priorityGroupsManager.UpdatePriority(PriorityGroupId, Itses, Priority);                
                
                CloseOverlay(true);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        #endregion        
    }
}
