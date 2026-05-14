using System.Web.UI;
using System.Web.UI.WebControls;

using Confirmit.CATI.Common;
using Confirmit.CATI.Supervisor.Core.Activity;

namespace Confirmit.CATI.Supervisor.ActivityViews.Controls
{
    public partial class StatusBreakdown : UserControl
    {
        /// <summary>
        /// Binds breakdown data to control for selected survey (with passed sid).
        /// </summary>
        /// <param name="sid">BvFEE survey sid.</param>
        public void Bind(int sid, bool onlyCatiInterviews = false)
        {
            repeater.DataSource = ActivityManager.GetStatusBreakdown(sid, onlyCatiInterviews);
            repeater.DataBind();
        }

        /// <summary>
        /// Here we fill text labels with data values and also highlight alerting cells.
        /// </summary>
        protected void repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                StatusInfo item = (StatusInfo)e.Item.DataItem;
                Label lblHeader = (Label)e.Item.FindControl("lblHeader");
                lblHeader.Text = item.Name;
                Label lblValue = (Label)e.Item.FindControl("lblValue");
                lblValue.Text = item.Value.ToString();
                switch (item.Alert)
                {
                    case AlertStatus.Error:
                        lblValue.BackColor = System.Drawing.Color.FromArgb(255, 150, 125);
                        break;
                    case AlertStatus.Warning:
                        lblValue.BackColor = System.Drawing.Color.FromArgb(255, 255, 125);
                        break;
                }
            }

        }
    }
}