using System;
using System.Web.UI.WebControls;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Activity;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.ActivityViews.Controls
{
	public partial class StatusAlertsList : BaseWUC
	{
	    private readonly IActivityManager _activityManager;

	    /// <summary>
		/// Occurs when alerts have been changed.
		/// </summary>
		public event EventHandler AlertsChanged;

        /// <summary>
        /// Gets or sets a value indicating whether data bind should be done on each postback.
        /// </summary>
        public bool AutoBindOnPostback { get; set; }

	    public StatusAlertsList()
	    {
            _activityManager = ServiceLocator.Resolve<IActivityManager>();
        }

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				//fill dropdownlist
				ddlAlert.DataSource = _activityManager.GetStatusAlertsList(true);
				ddlAlert.DataValueField = "StatusId";
				ddlAlert.DataTextField = "StatusName";
				ddlAlert.DataBind();
				//bind grid
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
            int alertTypeId = (int)innerGrid.DataKeys[e.RowIndex]["StatusId"];

            using ( var transactionScope = new DatabaseTransactionScope( "DeleteStatusAlert", DeadlockPriority.Supervisor ) )
            {
                ActivityManager.DeleteStatusAlert((int)innerGrid.DataKeys[e.RowIndex]["ObjectSID"], alertTypeId);

                transactionScope.Commit();
            }
			RefreshData();
			if (AlertsChanged != null)
				AlertsChanged(this, EventArgs.Empty);
		}

		protected void btnSetAlert_Click(object sender, EventArgs e)
		{
			int amber;
			int red;
            if (!Int32.TryParse(tbxAmberThreshold.Text, out amber) || !Int32.TryParse(tbxRedThreshold.Text, out red) || amber < 0 || red < 0)
                ShowClientMessage(Strings.Err_IntegerThresholds);
			else
			{
                int alertStatusId = Int32.Parse(ddlAlert.SelectedValue);
                string alertStatusName = ddlAlert.SelectedItem.Text;

                using ( var transactionScope = new DatabaseTransactionScope( "SetStatusAlert", DeadlockPriority.Supervisor ) )
                {
                    ActivityManager.SetStatusAlert(new StatusAlertInfo(0, amber, red, alertStatusId, alertStatusName));

                    transactionScope.Commit();
                }
				
				RefreshData();
				if (AlertsChanged != null)
					AlertsChanged(this, EventArgs.Empty);
			}
		}

		private void RefreshData()
		{
			innerGrid.DataSource = _activityManager.GetStatusAlertsList(false);
			innerGrid.DataBind();
		}
	}
}