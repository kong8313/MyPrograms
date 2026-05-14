using System;
using System.Linq;
using System.Web.Script.Services;
using System.Web.Services;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.Surveys
{
    [CheckSurveyPermission(RequestParameterName = "ID")]
    public partial class CallListHistoryTabs : BaseForm
    {
        private readonly ICallCenterService _callCenterService = ServiceLocator.Resolve<ICallCenterService>();

        protected void Page_Init(object sender, EventArgs e)
        {
            dialog1.CancelButton.InnerText = "Close";
            DisableControlsOnPostback = false;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (IsPostBack == false)
            {

                tabs.GetTabByKey("tabCallAttempts").ContentUrl += $"?ID={Request["ID"]}&InterviewID={Request["InterviewID"]}";
                tabs.GetTabByKey("tabCallExtendedHistory").ContentUrl += $"?ID={Request["ID"]}&InterviewID={Request["InterviewID"]}";
                tabs.GetTabByKey("tabCallHistoryLoop").ContentUrl += $"?ID={Request["ID"]}&InterviewID={Request["InterviewID"]}";
                tabs.GetTabByKey("tabSchedulingLog").ContentUrl += $"?ID={Request["ID"]}&InterviewID={Request["InterviewID"]}";

                if (_callCenterService.IsNeedToHidePii())
                {
                    tabs.GetTabByKey("tabCallHistoryLoop").Hidden = true;
                    tabs.GetTabByKey("tabSchedulingLog").Hidden = true;
                }

                /* must be done after visibility of some tabs was changed*/
                string tabKey = MaintainTabHelper.GetTabKey(ViewWithTabs.CallListHistoryTabs);

                if (string.IsNullOrEmpty(tabKey) == false)
                {
                    tabs.SelectTabByKey(tabKey);
                }
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public static void SetSelectedTab(string tabKey)
        {
            MaintainTabHelper.SetTabKey(ViewWithTabs.CallListHistoryTabs, tabKey);
        }
    }
}