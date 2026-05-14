using System;
using System.Web.UI.WebControls;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Activity;
using Confirmit.CATI.Supervisor.Classes.Activity;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Common;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.ActivityViews.Controls
{
	public partial class SurveyAlertsList : BaseWUC
	{
		/// <summary>
		/// Occurs when alerts have been changed.
		/// </summary>
		public event EventHandler AlertsChanged;

        /// <summary>
        /// Gets or sets a value indicating whether data bind should be done on each postback.
        /// </summary>
        public bool AutoBindOnPostback { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
                foreach (BvThresholdType thresholdType in ((BaseActivityView)Page).GetThresholdsList())
                {
                    ddlAlert.Items.Add(new ListItem(GetResString(thresholdType.ToString()), ((int)thresholdType).ToString()));
                }
				RefreshData();
			}
		}

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (IsPostBack && AutoBindOnPostback)
            {
                RefreshData();
            }
        }

        protected void innerGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                using (var transactionScope = new DatabaseTransactionScope("SetActivityAlert", DeadlockPriority.Supervisor))
                {
                    int alertTypeId = (int)innerGrid.DataKeys[e.RowIndex]["ThresholdsTypeId"];

                    ActivityManager.DeleteAlert((int)innerGrid.DataKeys[e.RowIndex]["ObjectSID"], alertTypeId);

                    transactionScope.Commit();
                }

                RefreshData();
                if (AlertsChanged != null) AlertsChanged(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

	    protected void btnSetAlert_Click(object sender, EventArgs e)
        {
            try
            {
                int amber;
                int red;
                int alertTypeId = Int32.Parse(ddlAlert.SelectedValue);
                var thresholdType = (BvThresholdType) alertTypeId;

                if (!Int32.TryParse(tbxAmberThreshold.Text, out amber) || !Int32.TryParse(tbxRedThreshold.Text, out red) || (!thresholdType.IsNegativeAllowed() && (amber < 0 || red < 0)))
                {
                    ShowClientMessage(Strings.Err_IntegerThresholds);
                }
                else
                {
                    using (var transactionScope = new DatabaseTransactionScope("SetActivityAlert", DeadlockPriority.Supervisor))
                    {
                        ActivityManager.SetAlert(new SurveyAlertInfo(0, amber, red, alertTypeId));

                        transactionScope.Commit();
                    }

                    RefreshData();
                    if (AlertsChanged != null) AlertsChanged(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

	    private void RefreshData()
		{
            innerGrid.DataSource = ((BaseActivityView)Page).GetAlertsList();
			innerGrid.DataBind();
		}
	}
}