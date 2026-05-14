using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Survey.Quota;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Controls;
using Infragistics.Web.UI.GridControls;

namespace Confirmit.CATI.Supervisor.CallManagement
{
    public partial class InterviewQuotaStatus : BaseForm
    {
        [StoreInViewState] protected int InterviewId;

        [StoreInViewState] protected int SurveyId;

        private List<InterviewQuotaStatusItem> _quotaStatusData;

        private readonly InterviewQuotaStatusProvider _interviewQuotaStatusProvider =
            ServiceLocator.Resolve<InterviewQuotaStatusProvider>();
        
        private readonly ISurveyRepository _surveyRepository =
            ServiceLocator.Resolve<ISurveyRepository>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SurveyId = Convert.ToInt32(Request["ID"]);
                InterviewId = Convert.ToInt32(Request["InterviewID"]);
            }

            grid.GridName = $"Status of quotas for interview {InterviewId}";
            grid.InitializeRow += InitializeHistoryRow;

            _quotaStatusData = _interviewQuotaStatusProvider
                .GetQuotaStatus(SurveyId, InterviewId).ToList();

            (hint.Text, hint.HintType) = GetHint();

            lblCallStateValue.Text = GetCallStateText();

            grid.GetPage =
                delegate(out int totalCount)
                {
                    totalCount = _quotaStatusData.Count;
                    return _quotaStatusData;
                };
        }

        private string GetCallStateText()
        {
            var call = CallQueueService.GetCallAndNoLock(SurveyId, InterviewId);

            if (call == null)
            {
                return "Not Scheduled";
            }

            switch ((CallState)call.CallState)
            {
                case CallState.DisabledByFCD:
                    return "Disabled by Quota";
                case CallState.Scheduled:
                    return "Scheduled";
                case CallState.DisabledByUser:
                    return "Disabled by User";
                case CallState.LoadedToDialerPredictively:
                    return "Sent to Dialer";
                case CallState.InterviewInProgress:
                    return "In Progress";
                case CallState.ToBeDeleted:
                    return "Deleted";
                default:
                    return "";
            }
        }

        private (string, HintType) GetHint()
        {
            var hintType = HintType.Info;
            var hintText = "";
            var fcdQuotas = _quotaStatusData.Where(x => x.IsFcdQuota).ToList();
            if (fcdQuotas.Any())
            {
                hintText = fcdQuotas.All(x => x.IsOpen)
                    ? "All quota cells are open. "
                    : "Some quota cells are <strong>closed</strong>. It is not possible to schedule a call for this interview. ";
                
                if (fcdQuotas.Any(x => !x.IsNormalCell && x.HasEmptyAnswers))
                {
                    hintType = HintType.Warning;
                    hintText +=
                        "<br>The interview does not contain responses for some of the questions used in the quotas. It is associated with multiple quota cells and will be disabled only when all cells or entire quota is full. ";
                }

                if (fcdQuotas.Any(x => !x.IsNormalCell && !x.HasEmptyAnswers))
                {
                    hintType = HintType.Warning;
                    hintText +=
                        "<br>The interview contains responses that do not match the expected quota cell values. It is associated with multiple quota cells and will be disabled only when all cells or entire quota is full. ";
                }

                if (fcdQuotas.Any(x => x.IsZeroLimit))
                {
                    hintType = HintType.Warning;
                    hintText +=
                        "<br>Some quota cells have limits set to 0. In this event the cells will be closed and so the interview will always be disabled. ";
                }
            }

            return (hintText, hintType);
        }

        private void InitializeHistoryRow(object sender, RowEventArgs rowEventArgs)
        {
            var item = (InterviewQuotaStatusItem)rowEventArgs.Row.DataItem;

            var statusCell = rowEventArgs.Row.Items.FindItemByKey("Status");
            if (statusCell != null)
            {
                if (item.IsFcdQuota)
                {
                    if (item.IsOpen)
                    {
                        statusCell.Text = "Open";
                    }
                    else
                    {
                        statusCell.Text = "Closed";
                        statusCell.CssClass += " closedCell";
                    }
                }
                else
                {
                    statusCell.Text = "Not filtered";
                }
            }

            var cellCell = rowEventArgs.Row.Items.FindItemByKey("Cell");

            if (item.Fields != null)
            {
                cellCell.Text = string.Join("; ", item.Fields.Select(x => $"{x.Key}={x.Value ?? "(empty)"}"));
            }

            if (!item.IsNormalCell)
            {
                cellCell.CssClass += " redFont";
            }
        }
    }
}