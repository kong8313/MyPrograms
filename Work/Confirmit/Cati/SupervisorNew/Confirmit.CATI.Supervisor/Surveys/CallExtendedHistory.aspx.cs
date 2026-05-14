using System;
using System.Linq;
using System.Web.Services;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.Resources;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Supervisor.Surveys
{
    public partial class CallExtendedHistory : BaseForm
    {
        [StoreInViewState]
        protected int InterviewId;

        [StoreInViewState]
        protected int SurveyId;

        private readonly ICallCenterProvider _callCenterProvider = ServiceLocator.Resolve<ICallCenterProvider>();
        private readonly ICallOperationsProvider _callOperationProvider = ServiceLocator.Resolve<ICallOperationsProvider>();
        private readonly IDiallingModeNameProvider _dialingModeNameProvider = ServiceLocator.Resolve<IDiallingModeNameProvider>();
        private readonly IToggleSettings _toggleSettings = ServiceLocator.Resolve<IToggleSettings>();

        protected void Page_PreRender(object sender, EventArgs e)
        {
            var command = gridCallHistory.GetCommand("OperationDetails");
            if (_toggleSettings.ShowDialType)
            {
                gridCallHistory.Columns.FromKey("DialType").Hidden = false;
            }
            command.OnClientClick = $"ShowOperationDetails({gridCallHistory.ClientControllerName})";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            gridCallHistory.GetPage =
                delegate(out int totalCount)
                {
                    var list =
                        from record in CallQueueService.GetExtendedCallHistoryList(SurveyId, InterviewId, _callCenterProvider.GetCurrentId())
                        join operation in _callOperationProvider.GetAll()
                            on (OperationType) record.OperationType equals operation.Id
                        join dialingMode in _dialingModeNameProvider.GetAll()
                            on (DialingMode) record.DialingMode equals dialingMode.Id into dm
                        from dialModeWithDefault in dm.DefaultIfEmpty()
                        select new CallEntendedHistoryItemProvider(record, operation.Title, dialModeWithDefault != null ? dialModeWithDefault.Title : String.Empty);

                    totalCount = list.Count();
                    return list;
                };

            if (!IsPostBack)
            {
                SurveyId = Convert.ToInt32(Request["ID"]);
                InterviewId = Convert.ToInt32(Request["InterviewID"]);
            }

            gridCallHistory.GridName = String.Format(Strings.InterviewHistory, InterviewId);
        }

        [WebMethod]
        public static bool CheckOperation(int operationId)
        {
            var operation = ServiceLocator.Resolve<IAsyncOperationRepository>().Get(operationId);
            return operation != null;
        }
    }
}