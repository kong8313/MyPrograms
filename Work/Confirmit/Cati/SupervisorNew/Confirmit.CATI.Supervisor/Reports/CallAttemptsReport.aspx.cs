using System;
using System.Linq;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.Reports;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Timezone;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.ServerControls;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Supervisor.Core.ITSs;

namespace Confirmit.CATI.Supervisor.Reports
{
    [CheckSurveyPermission(RequestParameterName = "ID", IsRequired = false)]
    public partial class CallAttemptsReport : SurveyFormBase
    {
        private readonly ICachedLocalTimezoneManager _timezoneProvider = ServiceLocator.Resolve<ICachedLocalTimezoneManager>();
        private readonly IHistoryRepository _historyRepository = ServiceLocator.Resolve<IHistoryRepository>();
        private readonly ICallCenterService _callCenterService = ServiceLocator.Resolve<ICallCenterService>();

        public override string TopTitle => Strings.CallAttempts;

        /// <summary>
        /// Gets/sets survey identifier
        /// </summary>
        private int? SurveyId
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!User.IsCatiAdministratorOrPros)
            {
                m_Grid.HideCommand("Edit");
                m_Grid.HideCommand("Delete");
            }

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

            m_Grid.GetPage = GetPage;

            FillStatesList();
        }

        /// <summary>
        /// Returns call attempts report page.
        /// </summary>
        /// <param name="totalCount">Returns total count of records.</param>
        /// <returns>List of CallAttemptsReportRecord items.</returns>
        private object GetPage(out int totalCount)
        {
            var searchParameterCollection = PrepareSearchParameters(m_Grid.PageArguments.SearchParameters);

            var args = new PagingArgs(
                            m_Grid.PageIndex,
                            m_Grid.PageSize,
                            m_Grid.SortedColumnKey,
                            m_Grid.SortIndicatorAsc,
                            searchParameterCollection);

            return ReportManager.GetCallAttemptsPage(
                User.Name,
                _timezoneProvider.GetLocalTimezoneId(),
                args,
                btnIncludeDisposedByDialerAttempts.ToggleButtonPressed,
                _callCenterService.IsNeedToHidePii(),
                out totalCount);
        }

        /// <summary>
        /// Fills state list in searching toolbar with states from default state group.
        /// </summary>
        private void FillStatesList()
        {
            GeneralGridColumn column = m_Grid.Columns.FromKey("ExtendedStatusName") as GeneralGridColumn;

            StateGroupsManager.GetDefaultITSList().ForEach(
                state => column.Items.Add(new ListItem(state.Name, state.StateID.ToString()))
            );
        }

        /// <summary>
        /// Fills search controls first time. For current page in search header
        /// date should set to "Today" by default. Also if survey identifier is
        /// given, proper project name should be selected in search header.
        /// </summary>
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

        protected void Delete(object sender, EventArgs e)
        {
            if (!User.IsCatiAdministratorOrPros)
            {
                AddUserMessage(Strings.PermissionDenied);
                return;
            }

            var historyId = Convert.ToInt32(m_Grid.SelectedKeys[0]);

            _historyRepository.Delete(historyId);
        }

        private SearchParameterCollection PrepareSearchParameters(SearchParameterCollection searchParameterCollection)
        {
            var parameter = searchParameterCollection.FirstOrDefault(x => x.ColumnName == "DisplayTime" && x.ColumnType == SearchColumnType.Number);

            if (parameter != null)
            {
                parameter.Value = ((int)(parameter.Value)) * 1000;
            }

            return searchParameterCollection;
        }
    }
}
