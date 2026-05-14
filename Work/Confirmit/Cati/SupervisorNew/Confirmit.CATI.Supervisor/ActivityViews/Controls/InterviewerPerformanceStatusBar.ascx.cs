using System;
using System.Collections.Generic;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Core.Activity;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Timezone;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.ActivityViews.Controls
{
    public partial class InterviewerPerformanceStatusBar: BaseWUC
    {
        private readonly ICachedLocalTimezoneManager _timezoneProvider =
            ServiceLocator.Resolve<ICachedLocalTimezoneManager>();

        protected void Page_PreRender(object sender, EventArgs e)
        {
            RefreshData();
        }

        public void SetActivityListExceededWarningVisibility(bool isVisible, int maxLimit)
        {
            totalInterviewsExceededWarning.Visible = isVisible;
            totalInterviewsExceededWarningMessage.Attributes["title"] = string.Format(Strings.ActivityInterviewersExceededWarning, maxLimit);
        }

        /// <summary>
        /// Refresh status bar data from DB.
        /// </summary>
        public void RefreshData()
        {
            var info = ActivityManager.GetSystemWideInfo(new List<int>());

            lblLoggedInterviewers.Text = info.LoggedInterviewersCount.ToString();
            lblTotalInterviewersWorkedToday.Text = info.TotalInterviewersWorkedTodayCount.ToString();            

            lblTime.Text = _timezoneProvider.GetCurrentLocalTime().ToString("g");
        }
    }
}