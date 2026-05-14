using System;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Timezone;

namespace Confirmit.CATI.Supervisor.CallManagement
{    
    [CheckSurveyPermission(RequestParameterName = "ID")]
    public partial class ActiveCalls : BaseForm
    {
        private readonly ICachedLocalTimezoneManager _timezoneProvider = ServiceLocator.Resolve<ICachedLocalTimezoneManager>();

        [StoreInViewState]
        public int SurveyId;
      
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                SurveyId = Convert.ToInt32(Request["ID"]);
            }

            m_grid.GetPage += (out int totalCount) =>
                              BvSpGetActiveCallsForSurveyAdapter.ExecuteEntityList(SurveyId, DateTime.UtcNow, out totalCount);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            m_grid.RefreshData();

            lblTime.Text = _timezoneProvider.GetCurrentLocalTime().ToString("g");

            RegisterScriptBlock(String.Format("var statusPanelId = \"{0}\";", statusBarUpdatePanel.ClientID));            
        }

        protected void gridSurveys_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var info = (BvSpGetActiveCallsForSurveyEntity)e.Row.DataItem;

                var lbCount = (Label)e.Row.FindControl("lblCount");

                lbCount.Text = (info.ResultCount >= info.RequestCount) ? String.Format("{0}+", info.ResultCount) :
                                                                         info.ResultCount.ToString();
            }
        }        
    }
}
