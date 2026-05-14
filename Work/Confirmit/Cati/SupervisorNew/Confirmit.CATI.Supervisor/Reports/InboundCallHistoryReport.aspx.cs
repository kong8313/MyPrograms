using System;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Reports;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Timezone;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.ServerControls;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Supervisor.Reports
{
    [CheckSurveyPermission(RequestParameterName = "ID", IsRequired = false)]
    public partial class InboundCallHistoryReport : SurveyFormBase
    {
        private readonly ICachedLocalTimezoneManager _timezoneProvider = ServiceLocator.Resolve<ICachedLocalTimezoneManager>();
        private readonly ICallCenterService _callCenterService = ServiceLocator.Resolve<ICallCenterService>();

        private readonly IInboundHandlerOperationsProvider _inboundHandlerOperationsProvider = ServiceLocator.Resolve<IInboundHandlerOperationsProvider>();

        public override string TopTitle
        {
            get
            {
                return Strings.InboundCallHistoryReport;
            }
        }

        private int? SurveyId
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            m_Grid.GridName = TopTitle;
            if (!IsPostBack)
            {
                string tmp = Request["ID"];
                if (!String.IsNullOrEmpty(tmp))
                {
                    // survey id has been passed in url
                    SurveyId = Int32.Parse(tmp);
                }
            }

            FillDefaultSearchControls();

            FillOperationTypeSearchFilter();

            m_Grid.GetPage = GetPage;
        }

        private object GetPage(out int totalCount)
        {
            var localTz = _timezoneProvider.GetLocalTimezoneId();

            var results = ReportManager.GetInboundCallsReportPage(
                User.Name,
                localTz,
                m_Grid.PageArguments,
                out totalCount);

            bool hidePii = _callCenterService.IsNeedToHidePii();
            return from record in results
                    join operation in _inboundHandlerOperationsProvider.GetAll()
                    on (InboundHandlerOperationType)record.OperationType equals operation.Id
                    select new InboundCallsReportRecord(record, hidePii, operation.Title, localTz);
        }

        private void FillDefaultSearchControls()
        {
            GeneralGridColumn date = m_Grid.Columns.FromKey("EventDate") as GeneralGridColumn;
            if (date != null)
            {
                // setting default value "Today" for date column
                date.SearchDefaultValue = SearchPredefinedDate.Today.ToString();
            }

            if (SurveyId.HasValue)
            {
                GeneralGridColumn projectName = m_Grid.Columns.FromKey("ProjectID") as GeneralGridColumn;
                if (projectName != null)
                {
                    // setting default survey from survey list
                    var survey = SurveyRepository.GetById(SurveyId.Value);
                    projectName.SearchDefaultValue = survey.ProjectId;
                }
            }
        }

        private void FillOperationTypeSearchFilter()
        {
            GeneralGridColumn column = m_Grid.Columns.FromKey("OperationTitle") as GeneralGridColumn;

            _inboundHandlerOperationsProvider.GetAll().ForEach(
                operationType => column.Items.Add(new ListItem(operationType.Title, ((int)operationType.Id).ToString()))
            );
        }
    }
}