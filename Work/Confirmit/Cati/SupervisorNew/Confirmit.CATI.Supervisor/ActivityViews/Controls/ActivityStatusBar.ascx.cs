using System;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Core.Activity;
using Confirmit.CATI.Supervisor.Classes.Activity;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Timezone;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.ActivityViews.Controls
{
    public partial class ActivityStatusBar : BaseWUC
    {
        public bool HideSystemWideInfo { get; set; }

        private readonly ICachedLocalTimezoneManager _timezoneProvider =
            ServiceLocator.Resolve<ICachedLocalTimezoneManager>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (HideSystemWideInfo)
            {
                divLoggedInterviewers.Visible = false;
                divLoggedIvrAgents.Visible = false;
                divCalls.Visible = false;
                divOpenSurveys.Visible = false;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            RefreshData();
        }

        public void SetLoggedIvrAgentsCountVisibility(bool isVisible)
        {
            spnLoggedIvrAgents.Visible = isVisible && !HideSystemWideInfo;
            lblLoggedIvrAgents.Visible = isVisible &&  !HideSystemWideInfo;
        }

        public void SetOpenSurveysCountVisibility(bool isVisible)
        {
            divOpenSurveys.Visible = isVisible && !HideSystemWideInfo;
            lblOpenSurveys.Visible = isVisible && !HideSystemWideInfo;
        }

        public void SetActivityListExceededWarningVisibility(bool isVisible, int maxLimit)
        {
            activityListExceededWarning.Visible = isVisible;
            activityListExceededWarningMessage.Attributes["title"] = string.Format(Strings.ActivityInterviewersExceededWarning, maxLimit);
        }

        /// <summary>
        /// Refresh status bar data from DB.
        /// </summary>
        public void RefreshData()
        {
            if (!HideSystemWideInfo)
            {
                var info = ActivityManager.GetSystemWideInfo(((BaseActivityView)Page).SelectedSurveys);

                lblLoggedInterviewers.Text = info.LoggedInterviewersCount.ToString();
                lblLoggedIvrAgents.Text = info.LoggedIvrAgentsCount.ToString();
                lblOpenSurveys.Text = info.OpenSurveysCount.ToString();
                lblCalls.Text = info.CallsCount.ToString();
            }

            lblTime.Text = _timezoneProvider.GetCurrentLocalTime().ToString("g");
        }
    }
}