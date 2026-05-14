using System;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Resources;
using Infragistics.Web.UI.GridControls;

namespace Confirmit.CATI.Supervisor.Surveys
{
    public partial class CallListHistory: BaseForm
    {
        [StoreInViewState]
        protected int InterviewId;

        [StoreInViewState]
        protected int SurveyId;

        private readonly ICallCenterProvider _callCenterProvider = ServiceLocator.Resolve<ICallCenterProvider>();
        private readonly IHistoryRepository _historyRepository = ServiceLocator.Resolve<IHistoryRepository>();
        private readonly ICallCenterService _callCenterService = ServiceLocator.Resolve<ICallCenterService>();

        protected void Page_PreRender(object sender, EventArgs e)
        {
            var command = grid.GetCommand("LinkedInterviews");
            command.OnClientClick = $"ShowLinkedInterviews({grid.ClientControllerName})";

            command = grid.GetCommand("Edit");
            command.OnClientClick = $"ShowCallHistoryProperties({grid.ClientControllerName})";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!User.IsCatiAdministratorOrPros)
            {
                grid.HideCommand("Edit");
                grid.HideCommand("Delete");
            }

            grid.GetPage =
                delegate(out int totalCount)
                    {
                        var list =
                            from record in CallQueueService.GetInterviewHistoryList(SurveyId, InterviewId, _callCenterProvider.GetCurrentId())
                            select new CallHistoryItemProvider(record, _callCenterService.IsNeedToHidePii());

                        totalCount = list.Count();
                        return list;
                    };

            if (!IsPostBack)
            {
                SurveyId = Convert.ToInt32(Request["ID"]);
                InterviewId = Convert.ToInt32(Request["InterviewID"]);
            }

            grid.GridName = String.Format(Strings.InterviewHistory, InterviewId);
            grid.InitializeRow += InitializeHistoryRow;
        }

        private void InitializeHistoryRow(object sender, RowEventArgs e)
        {
            var historyItem = (CallHistoryItemProvider)e.Row.DataItem;
            if (historyItem.LinkedInterviewSessionId > 0)
            {
                e.Row.CssClass += " IsLinkedInterview";
            }
        }

        protected void Delete(object sender, EventArgs e)
        {
            if (!User.IsCatiAdministratorOrPros)
            {
                AddUserMessage(Strings.PermissionDenied);
                return;
            }

            var historyId = Convert.ToInt32(grid.SelectedKeys[0]);

            if (historyId == 0)
            {
                AddUserMessage(Strings.RemovingOfCallHistoryRowIsImpossible);
                return;
            }

            _historyRepository.Delete(historyId);
        }
    }
}