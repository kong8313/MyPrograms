using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Supervisor.Core.PriorityGroups;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Persons
{
    public partial class ChangeCallGroup : BaseForm
    {
        #region Fields

        private List<int> m_IDs;
        
        private readonly IPriorityGroupsManager _priorityGroupsManager = ServiceLocator.Resolve<IPriorityGroupsManager>();
        private readonly ICallGroupRepository _callGroupRepository = ServiceLocator.Resolve<ICallGroupRepository>();

        #endregion

        #region Properties

        protected List<int> InterviewersIDs
        {
            get
            {
                if (m_IDs == null)
                {
                    string requestIDS = (String)ViewState["IDS"];
                    string[] ids = requestIDS.Split(',');
                    m_IDs = ids.Select(x => Int32.Parse(x)).ToList();
                }
                return m_IDs;
            }
        }

        protected int? SelectedCallGroupId
        {
            get
            {
                var selectedGroupId = int.Parse(ddlCallGroup.SelectedValue);
                return selectedGroupId == 0 ? null : (int?)selectedGroupId;                
            }
        }

        #endregion

        #region Life Cycle

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ViewState["IDS"] = Request.Params["IDS"];
                InitCallGroupDropdown();
            }            
        }        

        #endregion

        #region Event Handlers
        
        protected void OKButtonClick(object sender, EventArgs e)
        {
            try
            {
                using (var transaction = new DatabaseTransactionScope("Supervisor.ChangeCallGroup", DeadlockPriority.Supervisor))
                {
                    if(SelectedCallGroupId.HasValue)
                    {
                        _priorityGroupsManager.AddInterviewerAssignment(SelectedCallGroupId.Value, InterviewersIDs);                           
                    }
                    else
                    {
                        _priorityGroupsManager.DeleteInterviewerAssignment(InterviewersIDs);                           
                    }

                    transaction.Commit();
                }

                CloseOverlay(true);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        private void InitCallGroupDropdown()
        {
            ddlCallGroup.Items.Add(new ListItem { Text = Strings.None, Value = "0" });

            ddlCallGroup.Items.AddRange(_callGroupRepository.
                                        GetAllGroups().Select(x => new ListItem
                                        {
                                            Text = x.Name,
                                            Value = x.Id.ToString(CultureInfo.InvariantCulture)
                                        }).ToArray());

        }
      
        #endregion
    }
}
