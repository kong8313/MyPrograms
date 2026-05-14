using System;
using System.Linq;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.Surveys
{
    [CheckSurveyPermission(RequestParameterName = "ID")]
    public partial class SchedulingLog : BaseForm
    {
        [StoreInViewState] 
        protected int InterviewId;

        [StoreInViewState] 
        private int SurveySid;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InterviewId = Convert.ToInt32(Request["InterviewID"]);
                SurveySid = Convert.ToInt32(Convert.ToInt32(Request["ID"]));
            }

            var logs = ServiceLocator.Resolve<ISchedulingScriptLogRepository>().GetByInterviewId(SurveySid, InterviewId);
            var formatter = new SchedulingLogMessageFormatter();

            foreach (var formattedLine in formatter.FormatLogMessages(logs.Select(x => x.LogMessages)))
            {
                var label = new Label { Text = formattedLine };
                txtlogDiv.Controls.Add(label);
            }

            var totalDays = (int)ServiceLocator.Resolve<ISchedulingScriptLogTableCleanupSettings>().ExpirationPeriod.TotalDays;
            gridHint.Text = $@"All times are in UTC.
                             Logs are deleted after {totalDays} days";
        }
    }
}
